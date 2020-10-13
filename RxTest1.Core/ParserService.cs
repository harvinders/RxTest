using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Xml;

namespace RxTest.Core.Services
{
    public class ParserService : IDisposable
    {
        public HttpClient Client { get; }

        public ParserService(HttpClient client)
        {
            Client = client;
        }


        public IObservable<IContent> GetContents(string feedUrl, IScheduler scheduler)
        {
            return Observable.Create<IContent>(o =>
            {
                scheduler = scheduler ?? DefaultScheduler.Instance;
                scheduler.ScheduleAsync(async (ctrl, ct) =>
                {
                    try
                    {
                        Trace.WriteLine($"***************    Doing things");
                        using (var inputStream = await Client.GetStreamAsync(feedUrl))
                        {
                            var settings = new XmlReaderSettings
                            {
                                IgnoreComments = true,
                                IgnoreProcessingInstructions = true,
                                IgnoreWhitespace = true,
                                Async = true
                            };

                            //var parsingState = ParsingState.Channel;
                            Article article = null;
                            NewsSource newsSource = null;

                            using (var reader = XmlReader.Create(inputStream, settings))
                            {
                                while (await reader.ReadAsync())
                                {
                                    ct.ThrowIfCancellationRequested();
                                    if (reader.IsStartElement())
                                    {
                                        switch (reader.LocalName)
                                        {
                                            case "rss":
                                                break;
                                            case "channel":
                                                newsSource = new NewsSource() {SourceUrl = feedUrl};
                                                break;
                                            case "title":
                                                reader.Read();
                                                if (null == article && string.IsNullOrEmpty(newsSource.Title))
                                                    newsSource.Title = await reader.ReadContentAsStringAsync();
                                                else if (null != article)
                                                    article.Title = await reader.ReadContentAsStringAsync();

                                                break;
                                            case "description":
                                                if (string.IsNullOrEmpty(reader.NamespaceURI))
                                                {
                                                    reader.Read();

                                                    var description = await reader.ReadContentAsStringAsync();

                                                    if (null == article)
                                                        newsSource.Description = description;
                                                    else
                                                        article.Description = description;
                                                }

                                                break;

                                            case "item":
                                                if (null == article)
                                                    o.OnNext(newsSource);

                                                article = new Article()
                                                {
                                                    SourceUrl = feedUrl,
                                                    ImageUrl = newsSource.ImageUrl,
                                                    Author = newsSource.Author,
                                                    LastActiveDate = DateTime.UtcNow
                                                };

                                                break;
                                            case "pubDate":
                                                reader.Read();
                                                if (DateTime.TryParse(reader.Value, out var episodeDate) &&
                                                    null != article)
                                                {
                                                    article.PublicationDate = episodeDate.ToUniversalTime();
                                                }

                                                break;

                                            case "guid":
                                                reader.Read();
                                                article.Url = reader.Value;
                                                break;
                                        }
                                    }
                                    else if (reader.LocalName == "item" &&
                                             reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        o.OnNext(article);
                                    }
                                }
                            }

                            o.OnCompleted();
                        }
                    }
                    catch (Exception e)
                    {
                        o.OnError(e);
                    }

                });
                return Disposable.Empty;
            });
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Client.Dispose();
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
