
using System;
using System.Collections.Generic;
using System.Linq;
using WDWPennyFinder.Models;
using WDWPennyFinder.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using CommunityToolkit.Maui.Views;
using System.Drawing;
using Microsoft.Maui.Graphics;

namespace WDWPennyFinder
{
    public partial class FilterPage : Popup
    {
        private Button _previouslyClickedFilterButton;
        private Button _previouslyClickedSortButton;
        private string _lastSelectedFilter;
        private string _lastSelectedSort;
        // Public property to store the selected filter
        public string SelectedFilter { get; set; }
        public string SelectedSort { get; set; }
        private ItemsPage _itemsPage;
        private ItemsViewModel _itemsViewModel;

        public FilterPage()
        {
            InitializeComponent();
        }

        public FilterPage(ItemsPage itemsPage)
        {
            _itemsPage = itemsPage;
            InitializeComponent();
        }

        public FilterPage(ItemsViewModel itemsViewModel)
        {
            _itemsViewModel = itemsViewModel;
            InitializeComponent();
        }



        private async void OnFilterButtonClicked(object sender, EventArgs e)
        {
            if (_previouslyClickedFilterButton != null)
            {
                _previouslyClickedFilterButton.BackgroundColor = Colors.Transparent;
            }

            SelectedFilter = ((Button)sender).Text;
            _itemsViewModel.FilterItems(SelectedFilter);

            // Set last clicked filter button
            _previouslyClickedFilterButton = (Button)sender;
            _previouslyClickedFilterButton.BackgroundColor = Colors.LightGray;

            // Update last selected filter
            _lastSelectedFilter = SelectedFilter;

        }

        private async void OnSortButtonClicked(System.Object sender, System.EventArgs e)
        {
            if (_previouslyClickedSortButton != null)
            {
                _previouslyClickedSortButton.BackgroundColor = Colors.Transparent;
            }

            SelectedSort = ((Button)sender).Text;
            _itemsViewModel.SortItems(SelectedSort);

            // Set last clicked sort button
            _previouslyClickedSortButton = (Button)sender;
            _previouslyClickedSortButton.BackgroundColor = Colors.LightGray;

            // Update last selected sort
            _lastSelectedSort = SelectedSort;

        }

        void OnDismissButtonClicked(System.Object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}