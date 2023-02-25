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

        [SerializeField] RectTransform itemsTransform;

        [Header("Configuration")]
        [SerializeField] IInventoryItemSlot inventorySlotPrefab;
        public int MaxInventorySlots = 0;

        private List<IInventoryItemSlot> _itemSlots;

        private void Start()
        {
            _instance = this;
            var spawnedItems = new List<IInventoryItemSlot>();
            for (int i = 0; i < MaxInventorySlots; i++) {
                // spawn prefab slots in inventory object
                var go = Instantiate(inventorySlotPrefab.GetGameObject(), itemsTransform);
                spawnedItems.Add(go.GetComponent<IInventoryItemSlot>());
            }

            Init(spawnedItems);
        }

        public void Init(List<IInventoryItemSlot> initSlots)
        {
            _itemSlots = new List<IInventoryItemSlot>();
            foreach(IInventoryItemSlot initSlot in initSlots)
                _itemSlots.Add(initSlot);
        }

        /// <summary>
        /// Add and spawn item in the inventory system
        /// Raise event when task was successfull
        /// </summary>
        /// <param name="item">Item to add & spawn</param>
        /// <param name="stackSize">Stacksize</param>
        public void AddItemToInventory(IInventroryItem item, int stackSize = 1)
        {
            var success = false;
            if (InventoryHasSpaceForMoreStacks(item, stackSize)) {
                var existingSameItems = _itemSlots.Where(x => x.Item.Id == item.Id);

                // try to add more stacks
                if (existingSameItems.Any()) {
                    // try stacking on existing stacks
                    foreach (var slot in existingSameItems) {
                        if (slot.Item.GetMaxStackSize() >= stackSize + slot.StackSize) {
                            slot.SetStackSize(slot.StackSize + stackSize);
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
            var slot = _itemSlots.FirstOrDefault(x => x.Item.Id == item.Id && x.StackSize >= stackSize);
            if (slot is not null)
            {
                // ToDo: what do i do when stackSize to remove is greater then the current stackSize?
                slot.SetStackSize(slot.StackSize - stackSize);
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
            return _itemSlots.Where(x => x.SlotIsEmpty).Count() > 0;
        }

        /// <summary>
        /// Get the number of items that are registerd in the inventory
        /// </summary>
        public int GetSizeOfUsedSlots()
        {
            return _itemSlots.Where(x => x.SlotIsEmpty == false).Count();
        }

        /// <summary>
        /// Get the number of items that are registerd in the inventory
        /// </summary>
        public int GetSizeOfUnusedSlots()
        {
            return _itemSlots.Where(x => x.SlotIsEmpty == true).Count();
        }

        /// <summary>
        /// Checks if stacks of the item can be added to the inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="stacksize">Stacks to add</param>
        public bool InventoryHasSpaceForMoreStacks(IInventroryItem item, int stacksize = 1)
        {
            var slot = _itemSlots.FirstOrDefault(x => x.Item.Id == item.Id);
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
            foreach (var slot in _itemSlots) {
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