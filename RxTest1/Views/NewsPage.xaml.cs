using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using ReactiveUI;
using RxTest.ViewModels;

namespace RxTest.Views
{
    public class NewsReactivePage : ReactivePage<NewsViewModel>
    {
    }

    public sealed partial class NewsPage : NewsReactivePage
    {

        public NewsPage()
        {
            InitializeComponent();
            ViewModel = new NewsViewModel(new NewsSource(){ SourceUrl = "http://feeds.bbci.co.uk/news/technology/rss.xml#", Title="BBC News", Author = "Author" });
            //https://www.abc.net.au/radionational/programs/latenightlive/feed/2890652/podcast.xml
            //http://feeds.bbci.co.uk/news/technology/rss.xml#
            this.WhenActivated((disposables) =>
            {


                this.WhenAnyValue(x => x.ViewModel)
                    .Where(x => null != x)
                    .Do(Initialise)
                    .Subscribe()
                    .DisposeWith(disposables);


            });

        }
        private void Initialise(NewsViewModel viewModel)
        {
            Title.Text = viewModel.Source.Title;
            Author.Text = viewModel.Source.Author;
            ListView.ItemsSource = viewModel.Articles;
        }
    }
}
