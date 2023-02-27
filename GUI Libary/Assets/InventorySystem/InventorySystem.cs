using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityCoreLibs.GUILibary.InventorySystem
{
    public class InventorySystem : MonoBehaviour
    {
        public static InventorySystem Instance { get { return _instance; } }
        private static InventorySystem _instance;

        /// <summary>
        /// Gets raised when adding item to inventory was successful
        /// </summary>
        public event OnItemAddHandler OnItemAdd;
        public delegate void OnItemAddHandler(object sender, IInventroryItem itemThatWasAdded, int stackSize);

        /// <summary>
        /// Gets raised when removing item from inventory was successful
        /// </summary>
        public event OnItemRemoveHandler OnItemRemove;
        public delegate void OnItemRemoveHandler(object sender, IInventroryItem itemThatWasRemoved, int stackSize);

        /// <summary>
        /// Gets raised when adding item to inventory was not successful
        /// </summary>
        public event OnItemAddFailHandler OnItemAddFail;
        public delegate void OnItemAddFailHandler(object sender, IInventroryItem itemThatCanotBeAdded, int stackSize);

        /// <summary>
        /// Gets raised when removing item from inventory was not successful
        /// </summary>
        public event OnItemRemoveFailHandler OnItemRemoveFail;
        public delegate void OnItemRemoveFailHandler(object sender, IInventroryItem itemThatCanotBeRemoved, int stackSize);

        [SerializeField] RectTransform itemsTransform;

        [Header("Configuration")]
        [SerializeField] GameObject inventorySlotPrefab;
        public int MaxInventorySlots = 0;

        private List<IInventoryItemSlot> _itemSlots = new List<IInventoryItemSlot>();

        private void Awake()
        {
            _instance = this;
            var spawnedItems = new List<IInventoryItemSlot>();
            if (inventorySlotPrefab != null)
            {
                for (int i = 0; i < MaxInventorySlots; i++)
                {
                    // spawn prefab slots in inventory object
                    var go = Instantiate(inventorySlotPrefab, itemsTransform);
                    if (go.TryGetComponent(out IInventoryItemSlot slot))
                        spawnedItems.Add(slot);
                    else
                        throw new System.Exception("Inventory slot prefab needs to implement the IInventoryItemSlot interface!");
                }
            }
            Init(spawnedItems);
        }

        public void Init(List<IInventoryItemSlot> initSlots)
        {
            foreach (IInventoryItemSlot initSlot in initSlots)
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
            if (InventoryHasSpaceForMoreStacks(item, stackSize))
            {
                var existingSameItems = _itemSlots.Where(x => x.SlotIsEmpty == false && x.Item.Id == item.Id);

                // try to add more stacks
                if (existingSameItems.Any())
                {
                    // try stacking on existing stacks
                    foreach (var slot in existingSameItems)
                    {
                        if (slot.Item.GetMaxStackSize() >= stackSize + slot.StackSize)
                        {
                            slot.IncreaseStackSize(stackSize);
                            success = true;
                        }
                    }
                }
            }
            else if (InventoryHasEmptySpace())
                success = SearchForEmptySlotAndAssignItem(item, stackSize);

            if (success)
            {
                ReOrgenizeInventory();
                OnItemAdd?.Invoke(this, item, stackSize);
            }
            else OnItemAddFail?.Invoke(this, item, stackSize);
        }

        /// <summary>
        /// Remove and despawn item from the inventory system
        /// Raise event when task was successfull
        /// </summary>
        /// <param name="item">Item to remove & despawn</param>
        /// <param name="stackSize">Stacksize</param>
        public void RemoveItemFromInventory(IInventroryItem item, int stackSize = 1)
        {
            var slots = _itemSlots.Where(x => x.SlotIsEmpty == false && x.Item.Id == item.Id);
            if (slots is not null && slots.Count() > 0)
            {
                foreach (var slot in slots)
                    if (slot.StackSize >= stackSize)
                    {
                        // ToDo: what do i do when stackSize to remove is greater then the current stackSize?
                        slot.DecreaseStackSize(stackSize);
                        if (slot.StackSize <= 0)
                            slot.ResetSlot();
                        ReOrgenizeInventory();
                        OnItemRemove?.Invoke(this, item, stackSize);
                        return;
                    }
            }

            OnItemRemoveFail?.Invoke(this, item, stackSize);
        }

        /// <summary>
        /// Checks if the Inventory has empty slots to add new items
        /// </summary>
        public bool InventoryHasEmptySpace()
        {
            return _itemSlots.Where(x => x.SlotIsEmpty).Count() > 0;
        }

        /// <summary>
        /// Get the slot with given index
        /// </summary>
        public IInventoryItemSlot GetInventorySlotByIndex(int index)
        {
            return _itemSlots[index];
        }

        /// <summary>
        /// Checks if stacks of the item can be added to the inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="stacksize">Stacks to add</param>
        public bool InventoryHasSpaceForMoreStacks(IInventroryItem item, int stacksize = 1)
        {
            var slot = _itemSlots.Where(x => x.SlotIsEmpty == false).FirstOrDefault(x => x.Item.Id == item.Id);
            if (slot != null)
            {
                if (slot.Item.ItemIsStackeble())
                {
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
            foreach (var slot in _itemSlots)
            {
                if (slot.SlotIsEmpty)
                {
                    slot.AssignItem(item, stackSize);
                    return true;
                }
            }

            return false;
        }


        private void ReOrgenizeInventory()
        {
            // sort stacks
            for (int i = 0; i < _itemSlots.Count; i++)
            {
                if (_itemSlots[i].SlotIsEmpty) continue;

                for (int j = i + 1; j < _itemSlots.Count; j++)
                {
                    if (_itemSlots[j].SlotIsEmpty == false && _itemSlots[i].Item.Id == _itemSlots[j].Item.Id)
                    {
                        // fill out stacks
                        var stacksToFill = _itemSlots[i].Item.GetMaxStackSize() - _itemSlots[i].StackSize;
                        if (stacksToFill <= 0) continue;

                        if (_itemSlots[j].StackSize >= stacksToFill)
                        {
                            // sibling has enogh stacks to transfer them
                            _itemSlots[i].IncreaseStackSize(stacksToFill);
                            _itemSlots[j].DecreaseStackSize(stacksToFill);
                        }
                        else
                        {
                            // sibling dont have enogh stacks, transfer the possible amount
                            _itemSlots[i].IncreaseStackSize(_itemSlots[j].StackSize);
                            _itemSlots[j].DecreaseStackSize(_itemSlots[j].StackSize);
                        }
                    }
                }

            }

            // reorder items
            for (int i = 0; i < _itemSlots.Count; i++)
            {
                if (_itemSlots[i].SlotIsEmpty)
                {
                    // search for a non empty slot
                    for (int j = i + 1; j < _itemSlots.Count; j++)
                    {
                        if (_itemSlots[j].SlotIsEmpty == false)
                        {
                            // swap slots
                            _itemSlots[i].AssignItem(_itemSlots[j].Item, _itemSlots[j].StackSize);
                            _itemSlots[j].ResetSlot();
                            break;
                        }
                    }
                }
            }
        }
    }
}