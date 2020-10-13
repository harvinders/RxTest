using System;

//using RxTest1.Helpers;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System.Reactive;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using DynamicData.Binding;
using DynamicData;
using RxTest.Core.Services;

namespace RxTest.ViewModels
{
    public class NewsViewModel : ReactiveObject, IActivatableViewModel
    {
        [Reactive] public string SearchQuery { get; set; } = string.Empty;

        [Reactive] public NewsSource Source { get; set; }
        public ReactiveCommand<string, Unit> OpenWebLinkCommand { get; set; }
        public ReactiveCommand<bool, Unit> SubscribeCommand { get; set; }

        private ReadOnlyObservableCollection<ArticleViewModel> _articles;
        public ReadOnlyObservableCollection<ArticleViewModel> Articles => _articles;

        public ViewModelActivator Activator { get; }

        public NewsViewModel(NewsSource source)
        {
            Activator = new ViewModelActivator();
            Source = source;

            this.WhenActivated(disposables =>
            {
                var content = LibraryService.Instance?.GetContents(source.SourceUrl, null).Publish();

                var episodes = content.OfType<Article>();

                var filter = this.WhenAnyValue(x => x.SearchQuery)
                    .Throttle(TimeSpan.FromMilliseconds(250))
                    .Select(BuildFilter);

                Observable.Return(source)
                    .Concat(content.OfType<NewsSource>()
                        .Do(x => Trace.WriteLine(x.Title)))
                    .SubscribeOn(RxApp.TaskpoolScheduler)
                    .ObserveOnDispatcher()
                    .Subscribe(x =>
                    {
                        Source = x;
                    })
                    .DisposeWith(disposables);

                Func<ArticleViewModel, bool> BuildFilter(string query)
                {
                    if (string.IsNullOrEmpty(query))
                        return vm => true;

                    return vm => vm.Article.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                 vm.Article.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                 vm.Article.Author.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                 vm.Article.SourceUrl.Contains(query, StringComparison.OrdinalIgnoreCase);
                }

                var newsSorter = SortExpressionComparer<ArticleViewModel>
                    .Descending(vm => vm.Article.PublicationDate.Date)
                    .ThenByAscending(vm => vm.Article.PublicationDate);



                episodes
                    .Select(x => new ArticleViewModel(x))
                    .ToObservableChangeSet(vm => vm.Article.Url)
                    .Do(x => Trace.WriteLine($"{Thread.CurrentThread.ManagedThreadId} {DateTime.Now:mm:ss:fff} {x.Updates} {x.Adds} {x.Removes}"))
                    .Filter(filter)
                    .Sort(newsSorter)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .SubscribeOn(RxApp.TaskpoolScheduler)
                    .Bind(out _articles)
                    .DisposeMany()
                    .Subscribe()
                    .DisposeWith(disposables);

                OpenWebLinkCommand = ReactiveCommand.CreateFromTask<string, Unit>(async x =>
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri(x));
                    return Unit.Default;
                });


                content.Connect()
                    .DisposeWith(disposables);
            });
        }
    }

}
