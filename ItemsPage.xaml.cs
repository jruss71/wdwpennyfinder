using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using WDWPennyFinder.ViewModels;
using WDWPennyFinder.Models;
using System.Globalization;
using WDWPennyFinder.Converters;
using System.Windows.Input;
using WDWPennyFinder;
using CommunityToolkit.Maui.Views;

namespace WDWPennyFinder
{
    public partial class ItemsPage : ContentPage
    {
        public ICommand FilterItemCommand { get; }
        public ICommand SortItemCommand { get; }
        public ItemsViewModel _viewModel;
        // Public property to store the selected filter
        public string SelectedFilter { get; set; }

        public ItemsPage()
        {
            InitializeComponent();

            _viewModel = new ItemsViewModel();
            _viewModel.Navigation = Navigation;
            BindingContext = _viewModel;
            Resources.Add("LocationParkConverter", new LocationParkConverter());
            FilterItemCommand = new Command<ItemDetail>(OnFilterItem);
        }

        private async void OnFilterItem(object obj)
        {
            Console.WriteLine("itemfilterbuttonclicked");
            FilterPage filterPage = new FilterPage(this);

            // Get the current top-level page to display the popup
            var currentTopPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault()
                ?? Application.Current.MainPage;

            currentTopPage.ShowPopup(filterPage);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
            LoadItems();
        }

        public async void CallAppearing()
        {
            this.OnAppearing();
        }

        private async Task LoadItems()
        {
            await _viewModel.ExecuteLoadItemsCommand();
        }

        private async void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_viewModel.SelectedItem.Item.Collected))
            {
                await _viewModel.ExecuteLoadItemsCommand();
            }
        }

        void OnStackLayoutTapped(object sender, EventArgs e)
        {
            // Scroll to the top of the CollectionView
            ItemsListView.ScrollTo(0, 0, ScrollToPosition.Start, true);
        }

        void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
        }
    }
}

namespace WDWPennyFinder.Converters
{
    public class LocationParkConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var park = values[0] as string;
            var location = values[1] as string; if (string.IsNullOrEmpty(park))
            {
                return location ?? string.Empty;
            }

            if (string.IsNullOrEmpty(location))
            {
                return park ?? string.Empty;
            }

            return $"{park} - {location}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}