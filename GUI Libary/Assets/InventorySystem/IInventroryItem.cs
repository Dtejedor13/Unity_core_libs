using UnityEngine;

namespace UnityCoreLibs.GUILibary.InventorySystem
{
    public interface IInventroryItem
    {
        int Id { get; }
        Sprite Sprite { get; }
        string ItemName { get; }
        bool ItemIsStackeble();
        int GetMaxStackSize();
        GameObject GetGameObject();
    }
}
