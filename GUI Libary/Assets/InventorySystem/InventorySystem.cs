using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityCoreLibs.GUILibary.InventorySystem
{
    public class InventorySystem : MonoBehaviour
    {
        public static InventorySystem Instance { get { return _instance; } }
        private static InventorySystem _instance;

        public delegate void OnItemAddHandler(object sender, IInventroryItem itemThatWasAdded, int stackSize);
        public event OnItemAddHandler OnItemAdd;

        public delegate void OnItemRemoveHandler(object sender, IInventroryItem itemThatWasRemoved, int stackSize);
        public event OnItemRemoveHandler OnItemRemove;

        [SerializeField] RectTransform itemTransform;

        [Header("Configuration")]
        public int MaxInventorySlots = 0;

        private List<InventorySystemItemSlot> _spawnedItems;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            _instance = this;
            _spawnedItems = new List<InventorySystemItemSlot>();
            for (int i = 0; i < MaxInventorySlots; i++) {
                _spawnedItems.Add(new InventorySystemItemSlot { Item = null, StackSize = 0 });
                // spawn items in transform
            }
        }

        /// <summary>
        /// Add and spawn item in the inventory system
        /// Raise event when task was successfull
        /// </summary>
        /// <param name="item">Item to add & spawn</param>
        /// <param name="stackSize">Stacksize</param>
        public void AddItemToInventory(IInventroryItem item, int stackSize = 1)
        {
            bool success = false;
            if (InventoryHasSpaceForMoreStacks(item, stackSize)) {
                var existingSameItems = _spawnedItems.Where(x => x.Item.Id == item.Id);

                // try to add more stacks
                if (existingSameItems.Any()) {
                    // try stacking on existing stacks
                    foreach (var slot in existingSameItems) {
                        if (slot.Item.GetMaxStackSize() >= stackSize + slot.StackSize) {
                            slot.StackSize += stackSize;
                            success = true;
                        }
                    }
                }
            }
            else if (InventoryHasEmptySpace()) {
                success = SearchForEmptySlotAndAssignItem(item, stackSize);
            }

            if (success) {
                ReOrgenizeInventory();
                OnItemAdd?.Invoke(this, item, stackSize);
            }
        }

        /// <summary>
        /// Remove and despawn item from the inventory system
        /// Raise event when task was successfull
        /// </summary>
        /// <param name="item">Item to remove & despawn</param>
        /// <param name="stackSize">Stacksize</param>
        public void RemoveItemToInventory(IInventroryItem item, int stackSize = 1)
        {
            var slot = _spawnedItems.FirstOrDefault(x => x.Item.Id == item.Id && x.StackSize >= stackSize);
            if (slot is not null)
            {
                // ToDo: what do i do when stackSize to remove is greater then the current stackSize?
                slot.StackSize -= stackSize;
                if (slot.StackSize <= 0)
                    slot.RemoveItem();

                ReOrgenizeInventory();
                OnItemRemove?.Invoke(this, item, stackSize);
            }
        }

        /// <summary>
        /// Checks if the Inventory has empty slots to add new items
        /// </summary>
        public bool InventoryHasEmptySpace()
        {
            return _spawnedItems.Where(x => x.SlotIsEmpty).Count() > 0;
        }

        /// <summary>
        /// Get the number of items that are registerd in the inventory
        /// </summary>
        public int GetSizeOfUsedSlots()
        {
            return _spawnedItems.Where(x => x.SlotIsEmpty == false).Count();
        }

        /// <summary>
        /// Get the number of items that are registerd in the inventory
        /// </summary>
        public int GetSizeOfUnusedSlots()
        {
            return _spawnedItems.Where(x => x.SlotIsEmpty == true).Count();
        }

        /// <summary>
        /// Checks if stacks of the item can be added to the inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="stacksize">Stacks to add</param>
        public bool InventoryHasSpaceForMoreStacks(IInventroryItem item, int stacksize = 1)
        {
            var slot = _spawnedItems.FirstOrDefault(x => x.Item.Id == item.Id);
            if (slot != null) {
                if (slot.Item.ItemIsStackeble()) {
                    // item was found and is stackeble
                    var afterInsertStackSize = slot.StackSize + stacksize;
                    return afterInsertStackSize <= slot.Item.GetMaxStackSize();
                }
                else return InventoryHasEmptySpace(); // item is not stackeble
            }
            else return false; // item is not in the list
        }

        /// <summary>
        /// Search empty slot and assign item if slot was found
        /// </summary>
        /// <param name="item">item to add</param>
        /// <param name="stackSize">Stacksize</param>
        /// <returns>true whenn assign was successfull</returns>
        private bool SearchForEmptySlotAndAssignItem(IInventroryItem item, int stackSize)
        {
            // search of empty slot and insert item
            foreach (var slot in _spawnedItems) {
                if (slot.SlotIsEmpty) {
                    slot.AssignItem(item, stackSize);
                    return true;
                }
            }

            return false;
        }


        private void ReOrgenizeInventory()
        {
            // ToDo: add magic here
        }
    }
}