using UnityEngine;

namespace UnityCoreLibs.GUILibary.InventorySystem
{
    public interface IInventroryItem
    {
        int Id { get; }
        bool ItemIsStackeble();
        int GetMaxStackSize();
        GameObject GetGameObject();
    }
}
