using System;
using Windows.UI.Xaml.Controls;
using RxTest.ViewModels;

namespace RxTest.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
