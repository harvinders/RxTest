using System;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Linq;


namespace RxTest.Core.Services
{
    public class LibraryService 
    {
        private static readonly Lazy<LibraryService> Lazy = new Lazy<LibraryService> (() => new LibraryService());

        public static LibraryService Instance => Lazy.Value;


        private LibraryService()
        {
        }


        public IObservable<IContent> GetContents(string sourceUrl, IScheduler scheduler)
        {
            scheduler = scheduler ?? TaskPoolScheduler.Default;

            var parserService = new ParserService(new HttpClient());

            var fetchObject = parserService
                .GetContents(sourceUrl, scheduler);

            return fetchObject.Publish(fetchSubject =>
            {
                var updateObs = fetchSubject
                    .Select(x => Observable.FromAsync(async () =>
                    {
                        using (var session = DataService.GetSession())
                        {

                            switch (x)
                            {
                                case Article article:
                                    if (null == await session.Articles.FindAsync(article.Url))
                                    {
                                            session.Save(article);
                                    }

                                    break;
                                case NewsSource newsSource:
                                    var source = await session.NewsSources.FindAsync(newsSource.SourceUrl);
                                    if (null == source)
                                    {
                                        session.Save(newsSource);
                                    }
                                    else
                                    {
                                        source.Author = newsSource.Author;
                                        source.Description = newsSource.Description;
                                        source.HomeUrl = newsSource.HomeUrl;
                                        source.ImageUrl = newsSource.ImageUrl;
                                        source.PublicationDate = newsSource.PublicationDate;
                                        source.Title = newsSource.Title;
                                        session.Save(source);
                                    }

                                    break;
                            }
                        }
                        return x;
                    }))
                    .Concat()
                    .Where(x => false)
                    .Catch(Observable.Empty<NewsSource>());

                var dbEpisodeObs = Observable.Create<IContent>(o =>
                {
                    return scheduler.ScheduleAsync(async (ctrl, ct) =>
                    {
                        using (var session = DataService.GetSession())
                        {
                            var newsSource = await session.GetNewsSourceAsync(sourceUrl, ct);
                            if (null != newsSource)
                                o.OnNext(newsSource);

                            var articles = await session.GetArticlesAsync(sourceUrl, ct);
                            foreach (var article in articles)
                            {
                                o.OnNext(article);
                            }
                        }


                        o.OnCompleted();
                    });
                });

                return
                    dbEpisodeObs
                        .Concat(fetchSubject
                            .Catch(Observable.Empty<Article>())
                            .Merge(updateObs))
                        .Distinct(x =>
                        {
                            if (x is Article article)
                                return article.Url;
                            return x.SourceUrl;
                        });
            });

        }

    }
}
