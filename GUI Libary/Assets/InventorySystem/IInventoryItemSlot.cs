using UnityEngine;

namespace UnityCoreLibs.GUILibary.InventorySystem
{
    public interface IInventoryItemSlot
    {
        IInventroryItem? Item { get; }
        int StackSize { get; }
        bool SlotIsEmpty { get; }
        GameObject GetGameObject();
        void RemoveItem();
        void AssignItem(IInventroryItem item, int stackSize);
        void SetStackSize(int newValue);
    }
}
