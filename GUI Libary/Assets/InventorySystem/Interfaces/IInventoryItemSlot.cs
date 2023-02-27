using UnityEngine;

namespace UnityCoreLibs.GUILibary.InventorySystem
{
    public interface IInventoryItemSlot
    {
        IInventroryItem? Item { get; }
        int StackSize { get; }
        bool SlotIsEmpty { get; }
        GameObject GetGameObject();
        void ResetSlot();
        void AssignItem(IInventroryItem item, int stackSize);
        void IncreaseStackSize(int value);
        void DecreaseStackSize(int value);
    }
}
