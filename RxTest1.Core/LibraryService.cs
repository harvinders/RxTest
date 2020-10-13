using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;


namespace RxTest.Core.Services
{
    public class LibraryService 
    {
        private static readonly Lazy<LibraryService> Lazy = new Lazy<LibraryService> (() => new LibraryService());

        public static LibraryService Instance => Lazy.Value;


        private LibraryService()
        {
        }


        public IObservable<IContent> GetContents(string feedUrl, IScheduler scheduler)
        {
            scheduler = scheduler ?? TaskPoolScheduler.Default;

            var fetchObject =
                Observable.Using(
                    () => new ParserService(new HttpClient()),
                    service => service.GetContents(feedUrl, scheduler)
                        .Do(x =>
                        {
                            Trace.WriteLine($"Parsing => {x.Title}");
                        }))
                .Do(x =>
                {
                    using (var session = DataService.GetSession())
                    {
                        Trace.WriteLine($"Attempting add/save {Thread.CurrentThread.ManagedThreadId} {x.Title} ");

                        switch (x)
                        {
                            case Article article:
                                if (null == session.Articles.Find(article.Url))
                                {
                                    session.Save(article);
                                    Trace.WriteLine($"\t- Added article {Thread.CurrentThread.ManagedThreadId} {article.Url} ");
                                }

                                break;
                            case NewsSource source:
                                var newsSource = session.NewsSources.Find(source.SourceUrl);
                                if (null == newsSource)
                                {
                                    session.Save(source);
                                    Trace.WriteLine($"\t- Added newsSource {Thread.CurrentThread.ManagedThreadId} {source.SourceUrl} ");
                                }
                                else
                                {
                                    newsSource.Author = source.Author;
                                    newsSource.Description = source.Description;
                                    newsSource.HomeUrl = source.HomeUrl;
                                    newsSource.ImageUrl = source.ImageUrl;
                                    newsSource.PublicationDate = source.PublicationDate;
                                    newsSource.Title = source.Title;
                                    session.Save(newsSource);
                                    Trace.WriteLine($"\t- Updated newsSource {Thread.CurrentThread.ManagedThreadId} {newsSource.SourceUrl} ");
                                }

                                break;
                        }
                    }
                });

                var dbEpisodeObs = Observable.Using(
                                    () => DataService.GetSession(),
                                    session =>
                                        (from newsSource in Observable.FromAsync(ct => session.GetNewsSourceAsync(feedUrl, ct))
                                        select newsSource).Cast<IContent>().Where(x=> null!=x)
                                        .Concat(
                                        from articles in Observable.FromAsync(ct => session.GetArticlesAsync(feedUrl, ct))
                                        from article in articles
                                        select article));


                return dbEpisodeObs
                            .Concat(
                                    //Observable.Timer(TimeSpan.FromSeconds(1))
                                    //                    .SelectMany(x => fetchObject.Catch(Observable.Empty<Episode>()))
                                    fetchObject.Catch(Observable.Empty<Article>())
                            )
                            .Distinct(x =>
                            {
                                if (x is Article article)
                                    return article.Url;
                                return x.SourceUrl;
                            });

        }

    }
}
