using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using WDWPennyFinder.Models;
using System;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DWPennyFinder.ViewModels;
using CommunityToolkit.Maui.Views;

namespace WDWPennyFinder.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private ItemDetail _selectedItem;

        private Collection<ItemDetail> SourceItems { get; }
        public ObservableCollection<ItemDetail> Items { get; }

        public ICommand AddItemCommand { get; }
        public ICommand ItemTapped { get; }
        public ICommand ItemCollected { get; }
        public ICommand ItemRemoved { get; }
        public ICommand RefreshCommand { get; }
        public ICommand AltFilterCommand { get; }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }
        private string _selectedFilter;
        private string _selectedSort;

        // Store the last selected filter and sort button information
        private string _lastSelectedFilter;
        private string _lastSelectedSort;

        public string SelectedFilter
        {
            get { return _selectedFilter; }
            set
            {
                if (_selectedFilter != value)
                {
                    _selectedFilter = value;
                    OnPropertyChanged(nameof(SelectedFilter));
                    UpdateLastSelectedFilter(value);
                }
            }
        }

        public string SelectedSort
        {
            get { return _selectedSort; }
            set
            {
                if (_selectedSort != value)
                {
                    _selectedSort = value;
                    OnPropertyChanged(nameof(SelectedSort));
                    UpdateLastSelectedSort(value);
                }
            }
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Park { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Filter = string.Empty;

        public string Sort = string.Empty;
        public ItemDetail ItemDetail { get; set; }
        public INavigation Navigation { get; set; }
        IEnumerable<ItemDetail> itemsByFilter;

        IEnumerable<ItemDetail> itemsBySort;

        public ObservableCollection<Item> CheckBoxItems { get; set; }

        public ItemsViewModel()
        {
            Items = new ObservableCollection<ItemDetail>();
            SourceItems = new Collection<ItemDetail>();
            CheckBoxItems = new ObservableCollection<Item>();
            ItemTapped = new Command<ItemDetail>(OnItemSelected);
            AddItemCommand = new Command(OnAddItem);
            AltFilterCommand = new Command(OnFilterCommand);
            ItemCollected = new Command<ItemDetail>(OnItemCollected);
            ItemRemoved = new Command<ItemDetail>(OnItemRemoved);
            RefreshCommand = new Command(async () =>
            {
                IsRefreshing = true;
                await ExecuteLoadItemsCommand();
                IsRefreshing = false;
            });

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ItemDetail))
                {
                    if (_selectedItem != null)
                    {
                        _selectedItem.PropertyChanged -= SelectedItem_PropertyChanged;
                    }
                    if (ItemDetail != null)
                    {
                        ItemDetail.PropertyChanged += SelectedItem_PropertyChanged;
                    }
                }
            };

            //MessagingCenter.Subscribe<CheckBoxContentPage>(this, "RefreshNeeded", (sender) =>
            //{
            //    Console.WriteLine("Refresh Needed message received");
            //});
        }
        public async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                if (Filter != string.Empty)
                {
                    FilterItems(Filter);
                }

                else
                {
                    Items.Clear();
                    var items = await App.Database.GetItemsAsync();
                    foreach (var item in items)
                    {
                        var machine = await App.Database.GetMachineByIdAsync(item.MachineId);
                        var location = await App.Database.GetLocationAsync(machine.locationId);

                        ItemDetail itemDetail = new ItemDetail
                        {
                            Item = item,
                            Machine = machine,
                            Location = location
                        };
                        SourceItems.Add(itemDetail);
                        Items.Add(itemDetail);
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;

            }
        }

        public void OnAppearing()
        {
            SelectedItem = null;
        }

        protected override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Your custom implementation here

            // Call the base implementation to raise the PropertyChanged event
            base.RaisePropertyChanged(propertyName);
        }

        private void SelectedItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Handle the PropertyChanged event for the ItemDetail object here
            // You can perform any necessary updates or actions based on the changes in the ItemDetail object
            if (e.PropertyName == nameof(ItemDetail.Item))
            {
                // Perform updates based on the changes in the Item object
                Console.Write("We need to reload from the DB here");
            }
        }


        public ItemDetail SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }
        // Update the last selected sort information
        private void UpdateLastSelectedSort(string sort)
        {
            _lastSelectedSort = sort;
        }

        // Update the last selected sort information
        private void UpdateLastSelectedFilter(string filter)
        {
            _lastSelectedSort = filter;
        }

        // Get the last selected filter
        public string GetLastSelectedFilter()
        {
            return _lastSelectedFilter;
        }

        // Get the last selected sort
        public string GetLastSelectedSort()
        {
            return _lastSelectedSort;
        }
        private async void OnAddItem(object obj)
        {
            //await Navigation.PushAsync(new NewItemPage());
        }

        private async void OnFilterCommand(object obj)
        {
            // FilterPage filterPage = new FilterPage(this);
            // await PopupNavigation.Instance.PushAsync(filterPage);
            FilterPage filterPage = new FilterPage(this);

            // Get the current top-level page to display the popup
            var currentTopPage = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault()
                ?? Application.Current.MainPage;

            currentTopPage.ShowPopup(filterPage);
        }


        private async void OnItemCollected(ItemDetail itemDetail)
        {
            if (itemDetail == null)
                return;
            // update the Collected value
            itemDetail.Item.Collected = true;

            // save the item to the database
            await App.Database.SaveItemAsync(itemDetail.Item);
            OnPropertyChanged(nameof(Item.Collected));

        }
        private async void OnItemRemoved(ItemDetail itemDetail)
        {
            if (itemDetail == null)
                return;

            // update the Collected value
            itemDetail.Item.Collected = false;

            // save the item to the database
            await App.Database.SaveItemAsync(itemDetail.Item);
            OnPropertyChanged(nameof(Item.Collected));

        }
        async void OnItemSelected(ItemDetail item)
        {
            if (item == null)
                return;


            var mapPage = new MapPage();
            mapPage.BindingContext = item;
            await Navigation.PushAsync(mapPage);

        }
        public async Task CheckBoxItemsForMachine(int machine)
        {
            CheckBoxItems.Clear();

            var items = await App.Database.GetItemsByMachineAsync(machine);
            foreach (var item in items)
            {
                CheckBoxItems.Add(item);
            }
        }

        public void FilterItems(string filterSelected)
        {

            Filter = filterSelected;
            if (filterSelected == "Resorts")
            {
                itemsByFilter = SourceItems.Where(itemDetail =>
                !new List<string> {
                    "Disney Springs",
                    "Animal Kingdom",
                    "Magic Kingdom",
                    "Epcot",
                    "Hollywood Studios" }
                .Contains(itemDetail.Location.name));
            }
            else if (filterSelected == "Clear Filter")
            {
                itemsByFilter = SourceItems;
            }
            else if (filterSelected == "Collected")
            {
                itemsByFilter = SourceItems.Where(itemDetail => itemDetail.Item.Collected == true);

            }
            else if (filterSelected == "Uncollected")
            {

                itemsByFilter = SourceItems.Where(itemDetail => itemDetail.Item.Collected == false);
            }
            else
            {

                itemsByFilter = SourceItems
                        .Where(itemDetail =>
                        itemDetail.Location.name == filterSelected);
            }
            Items.Clear();
            foreach (var item in SourceItems)
            {
                if (!itemsByFilter.Contains(item))
                {
                    Items.Remove(item);
                }
                else if (!Items.Contains(item))
                {
                    Items.Add(item);
                }
            }
        }
        public void SortItems(string sortOrder)
        {
            switch (sortOrder)
            {
                case "Name A-Z":
                    itemsBySort = SourceItems.OrderBy(itemDetail => itemDetail.Item.Name);
                    break;
                case "Name Z-A":
                    itemsBySort = SourceItems.OrderByDescending(itemDetail => itemDetail.Item.Name);
                    break;
                case "Location A-Z":
                    itemsBySort = SourceItems.OrderBy(itemDetail => itemDetail.Location.name);
                    break;
                case "Location Z-A":
                    itemsBySort = SourceItems.OrderByDescending(itemDetail => itemDetail.Location.name);
                    break;
                case "Collected - Uncollected":
                    itemsBySort = SourceItems.OrderByDescending(itemDetail => itemDetail.Item.Collected);
                    break;
                case "Uncollected - Collected":
                    itemsBySort = SourceItems.OrderBy(itemDetail => itemDetail.Item.Collected);
                    break;
                default:
                    itemsBySort = SourceItems;
                    break;
            }

            // Clear the Items collection and add the sorted items
            Items.Clear();
            foreach (var item in itemsBySort)
            {
                Items.Add(item);
            }
        }

    }
}
