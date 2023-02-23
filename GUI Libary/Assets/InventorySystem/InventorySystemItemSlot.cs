using UnityEngine;

namespace UnityCoreLibs.GUILibary.InventorySystem
{
    public class InventorySystemItemSlot : MonoBehaviour
    {
        public IInventroryItem? Item { get; set; } = null;
        public int StackSize { get; set; } = 0;
        public bool SlotIsEmpty
        {
            get { return Item == null; }
        }

        public void RemoveItem()
        {
            if (SlotIsEmpty) return;

            StackSize = 0;
            Destroy(Item.GetGameObject());
            Item = null;
        }

        public void AssignItem(IInventroryItem item, int stackSize)
        {
            if (!SlotIsEmpty) return;

            Item = item;
            StackSize = stackSize;
        }
    }
}
