namespace RxTest.ViewModels
{
    public class ArticleViewModel {
        public Article Article { get; set; }
        public ArticleViewModel(Article article)
        {
            Article = article;
        }
    }
}
