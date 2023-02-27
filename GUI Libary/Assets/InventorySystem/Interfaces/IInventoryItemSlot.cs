using UnityEngine;

namespace UnityCoreLibs.GUILibary.InventorySystem
{
    public interface IInventoryItemSlot
    {
        /// <summary>
        /// The actual handler
        /// </summary>
        IInventroryItem? Item { get; }
        /// <summary>
        /// Current stacks
        /// </summary>
        int StackSize { get; }
        /// <summary>
        /// Checks if the item is null
        /// </summary>
        bool SlotIsEmpty { get; }
        /// <summary>
        /// Reset the slot to default empty state
        /// </summary>
        void ResetSlot();
        /// <summary>
        /// Assign an item to the slot
        /// </summary>
        /// <param name="item">Actual item to assign</param>
        /// <param name="stackSize">Current stacks to start with</param>
        void AssignItem(IInventroryItem item, int stackSize);
        /// <summary>
        /// Increase the stack size
        /// </summary>
        /// <param name="value">Value to increase</param>
        void IncreaseStackSize(int value);
        /// <summary>
        /// Decrease the stack size
        /// </summary>
        /// <param name="value">Value to decrease</param>
        void DecreaseStackSize(int value);
    }
}
