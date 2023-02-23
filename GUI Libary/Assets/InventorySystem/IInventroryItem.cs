using UnityEngine;

namespace UnityCoreLibs.GUILibary.InventorySystem
{
    public interface IInventroryItem
    {
        bool ItemIsStackeble();
        int GetMaxStackSize();
        GameObject GetGameObject();
    }
}
