using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RxTest.Core
{
    public static class DataService
    {
        public static DatabaseContext GetSession()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");


            return new DatabaseContext(optionsBuilder.Options);
        }

        public static void Save(this DatabaseContext context, NewsSource newsSource)
        {
            var storedItem = context.NewsSources.AsNoTracking().SingleOrDefault(x => x.SourceUrl == newsSource.SourceUrl);
            if (null == storedItem)
            {
                context.NewsSources.Add(newsSource);
            }
            else
            {
                context.NewsSources.Update(newsSource);
            }

            context.SaveChanges();
        }


        public static  void Save(this DatabaseContext context, Article episode)
        {
            try
            {
                var storedEpisode = context.Articles.AsNoTracking().SingleOrDefault(x => x.Url == episode.Url);
                if (null == storedEpisode)
                {
                    //Trace.WriteLine($"Adding {Thread.CurrentThread.ManagedThreadId} {episode.EpisodeUrl}");
                    context.Set<Article>().Add(episode);
                }
                else
                {
                    //Trace.WriteLine($"Updating {Thread.CurrentThread.ManagedThreadId} {episode.EpisodeUrl}");
                    context.Set<Article>().Update(episode);
                }
                context.SaveChanges();
            }
            catch (Exception e)
            {
            }
        }

        public static async Task<NewsSource> GetNewsSourceAsync(this DatabaseContext context, string feedId, CancellationToken ct)
        {
            return await context.NewsSources.AsNoTracking().SingleOrDefaultAsync(x => x.SourceUrl == feedId, cancellationToken: ct);
        }

        public static async Task<IEnumerable<Article>> GetArticlesAsync(this DatabaseContext context, string feedUrl, CancellationToken ct)
        {
            return await context.Articles.AsNoTracking().Where(x=>x.SourceUrl == feedUrl).ToListAsync(cancellationToken: ct);
        }

    }
}