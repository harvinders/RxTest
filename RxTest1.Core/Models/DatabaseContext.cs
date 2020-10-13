using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RxTest.Core
{
    public class DatabaseContext:DbContext
    {
        public DbSet<NewsSource> NewsSources { get; set; }
        public DbSet<Article> Articles { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            :base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new NewsSourceConfig());
            modelBuilder.ApplyConfiguration(new ArticleConfig());

            base.OnModelCreating(modelBuilder);
        }
    }

    internal class NewsSourceConfig : IEntityTypeConfiguration<NewsSource>
    {
        public void Configure(EntityTypeBuilder<NewsSource> entity)
        {

            entity
                .HasMany(x => x.Articles)
                .WithOne()
                .HasForeignKey(x => x.SourceUrl)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasKey(x => x.SourceUrl);


            entity.Property(p => p.PublicationDate)
                .HasColumnType("date");
        }
    }

    internal class ArticleConfig : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> entity)
        {
            entity
                .HasKey(x => x.Url);
            entity.Property(p => p.PublicationDate)
                .HasColumnType("date");
        }
    }
}
