using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace WDWPennyFinder.Models
{
    public class ItemDetail : INotifyPropertyChanged
    {
        private Item _item;
        private Machine _machine;
        private Location _location;

        public ItemDetail()
        {
            // Subscribe to the Item's PropertyChanged event when it's set
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Item))
                {
                    if (_item != null)
                    {
                        _item.PropertyChanged -= Item_PropertyChanged;
                    }
                    if (Item != null)
                    {
                        Item.PropertyChanged += Item_PropertyChanged;
                    }
                }
            };
        }

        public Item Item
        {
            get => _item;
            set
            {
                if (_item != value)
                {
                    _item = value;
                    OnPropertyChanged(nameof(Item));
                }
            }
        }

        public Machine Machine
        {
            get => _machine;
            set
            {
                if (_machine != value)
                {
                    _machine = value;
                    OnPropertyChanged(nameof(Machine));
                }
            }
        }

        public Location Location
        {
            get => _location;
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged(nameof(Location));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Raise the PropertyChanged event for the ItemDetail object
            // when a property value in the associated Item object changes
            OnPropertyChanged(nameof(Item));
        }

        // This method returns a "formatted string" that includes newline character
        // between each Penny found at a location for display in our Map Annotation
        public async Task<String> GetOtherPenniesAtLocation()
        {
            var items = await App.Database.GetItemsByMachineAsync(Machine.Id);
            StringBuilder pennies = new StringBuilder();

            foreach (var item in items)
            {
                if (pennies.Length != 0)
                {
                    pennies.Append("\n");
                }
                pennies.Append(item.Name);

            }

            return pennies.ToString();
        }
    }



    public class Location
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string name { get; set; }
    }

    public class Machine
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int locationId { get; set; }
        public string name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public Microsoft.Maui.Devices.Sensors.Location pinPosition
        {
            get
            {
                return new Microsoft.Maui.Devices.Sensors.Location(latitude, longitude);
            }
        }
    }

    public class Item : INotifyPropertyChanged
    {
        private int _id;
        private int _machineId;
        private string _name;
        private bool _collected;

        [PrimaryKey, AutoIncrement]
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public int MachineId
        {
            get => _machineId;
            set
            {
                if (_machineId != value)
                {
                    _machineId = value;
                    OnPropertyChanged(nameof(MachineId));
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public bool Collected
        {
            get => _collected;
            set
            {
                if (_collected != value)
                {
                    _collected = value;
                    OnPropertyChanged(nameof(Collected));
                }
            }
        }

        // Computed property PinPosition

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
