using System.Collections.ObjectModel;
using WDWPennyFinder.Models;
using Microsoft.Maui;
using CommunityToolkit.Maui.Views;

namespace WDWPennyFinder;

public partial class CheckBoxContentPage : Popup
{


    public CheckBoxContentPage(ObservableCollection<Item> items)
    {
        InitializeComponent();
        listView.ItemsSource = items;


    }
    private void OnDismissButtonClicked(object sender, EventArgs e)
    {
        this.Close();
    }

    private async void CheckedDone(object sender, CheckedChangedEventArgs e)
    {
        HapticFeedback.Perform(HapticFeedbackType.Click);
        // get the sender
        var cb = sender as CheckBox;

        // use the BindingContext to get the bound Item
        var item = cb.BindingContext as Item;

        // update the Collected value
        item.Collected = e.Value;

        // save the item to the database
        await App.Database.SaveItemAsync(item);


    }


}

