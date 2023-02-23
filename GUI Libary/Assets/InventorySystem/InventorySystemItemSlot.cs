using UnityEngine;

namespace UnityCoreLibs.GUILibary.InventorySystem
{
    public class InventorySystemItemSlot : MonoBehaviour
    {
        public IInventroryItem Item { get; set; }
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
        }

        public void AssignItem(IInventroryItem item, int stackSize)
        {
            if (!SlotIsEmpty) return;

            Item = item;
            StackSize = stackSize;
        }
    }
}
