using System;
using System.Collections.Generic;
using Assets.Scripts.Services.Database;

namespace Assets.Scripts.Services
{
    public class BagDataHelper
    {
        private IGameDatabase _database;
        public Action OnBagDataChange; 
        public BagDataHelper(IGameDatabase database)
        {
            _database = database;
        }

        public bool TestItem(string itemId, int diff)
        {
            _database.Inventory.TryAdd(itemId, 0);
            if (_database.Inventory[itemId] + diff < 0) return false;
            else return true;
        }
        public void ChangeItem(string itemId, int num)
        {
            _database.Inventory.TryAdd(itemId, 0);
            _database.Inventory[itemId] += num;
            OnBagDataChangeFin();
        }

        public int GetItemNum(string itemId)
        {
            _database.Inventory.TryAdd(itemId, 0);
            return _database.Inventory[itemId];
        }
        
        public void ChangeItems(Dictionary<string, int> items)
        {
            foreach (var x in items)
            {
                _database.Inventory.TryAdd(x.Key, 0);
                _database.Inventory[x.Key] += x.Value;
            }
            OnBagDataChangeFin();
        }

        private void OnBagDataChangeFin()
        {
            OnBagDataChange?.Invoke();
        }
    }
}