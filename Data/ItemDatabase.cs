using System.Collections.Generic;
using System.Threading.Tasks;
using WDWPennyFinder.Models;
using SQLite;

namespace WDWPennyFinder.Data
{
    public class ItemDatabase
    {
        readonly SQLiteAsyncConnection _database;

        public ItemDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<WDWPennyFinder.Models.Location>().Wait();
            _database.CreateTableAsync<Machine>().Wait();
            _database.CreateTableAsync<Item>().Wait();
        }

        // Location methods

        public Task<List<WDWPennyFinder.Models.Location>> GetLocationsAsync()
        {
            return _database.Table<WDWPennyFinder.Models.Location>().ToListAsync();
        }

        public Task<WDWPennyFinder.Models.Location> GetLocationAsync(int id)
        {
            return _database.Table<WDWPennyFinder.Models.Location>().Where(location => location.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveLocationAsync(WDWPennyFinder.Models.Location location)
        {
            if (location.Id != 0)
            {
                return _database.UpdateAsync(location);
            }
            else
            {
                return _database.InsertAsync(location);
            }
        }

        public Task<int> DeleteLocationAsync(WDWPennyFinder.Models.Location location)
        {
            return _database.DeleteAsync(location);
        }

        // Machine methods

        public Task<List<Machine>> GetMachinesAsync()
        {
            return _database.Table<Machine>().ToListAsync();
        }

        public Task<List<Machine>> GetMachinesByLocationAsync(int locationId)
        {
            return _database.Table<Machine>()
                .Where(machine => machine.locationId == locationId)
                .ToListAsync();
        }
        public Task<Machine> GetMachineByIdAsync(int id)
        {
            return _database.Table<Machine>().Where(machine => machine.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveMachineAsync(Machine machine)
        {
            if (machine.Id != 0)
            {
                return _database.UpdateAsync(machine);
            }
            else
            {
                return _database.InsertAsync(machine);
            }
        }

        public Task<int> DeleteMachineAsync(Machine machine)
        {
            return _database.DeleteAsync(machine);
        }

        // Item methods

        public Task<List<Item>> GetItemsAsync()
        {
            return _database.Table<Item>().ToListAsync();
        }

        public Task<List<Item>> GetItemsByMachineAsync(int machineId)
        {
            return _database.Table<Item>()
                .Where(item => item.MachineId == machineId)
                .ToListAsync();
        }

        public Task<int> SaveItemAsync(Item item)
        {
            if (item.Id != 0)
            {
                return _database.UpdateAsync(item);
            }
            else
            {
                return _database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(Item item)
        {
            return _database.DeleteAsync(item);
        }

        public async Task SaveItemDetailAsync(ItemDetail itemDetail)
        {
            // Save or update the Location
            if (itemDetail.Location.Id == 0)
            {
                await _database.InsertAsync(itemDetail.Location); ;
            }
            else
            {
                await _database.UpdateAsync(itemDetail.Location);
            }

            // Save or update the Machine and set its LocationId
            itemDetail.Machine.locationId = itemDetail.Location.Id;
            if (itemDetail.Machine.Id == 0)
            {
                await _database.InsertAsync(itemDetail.Machine);
            }
            else
            {
                await _database.UpdateAsync(itemDetail.Machine);
            }

            // Save or update the Item and set its MachineId
            itemDetail.Machine.locationId = itemDetail.Location.Id;
            if (itemDetail.Item.Id == 0)
            {
                await _database.InsertAsync(itemDetail.Item);
            }
            else
            {
                await _database.UpdateAsync(itemDetail.Item);
            }
        }

    }

}
