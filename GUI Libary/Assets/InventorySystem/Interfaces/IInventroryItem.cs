using UnityEngine;

namespace UnityCoreLibs.GUILibary.InventorySystem
{
    public interface IInventroryItem
    {
        /// <summary>
        /// The primary key of the item
        /// </summary>
        int Id { get; }
        /// <summary>
        /// The sprite of the item for visualisation
        /// </summary>
        Sprite Sprite { get; }
        /// <summary>
        /// The Name of the item
        /// </summary>
        string ItemName { get; }
        /// <summary>
        /// Checks if the item is stackable
        /// </summary>
        /// <returns>true if it is stackeble</returns>
        bool ItemIsStackeble();
        /// <summary>
        /// Returns the max stack size of the item
        /// </summary>
        /// <returns>Max Stacks</returns>
        int GetMaxStackSize();
        /// <summary>
        /// Gets the gameobject for comparson
        /// </summary>
        GameObject GetGameObject();
        /// <summary>
        /// Init item with the properties of another item for respawning the item
        /// </summary>
        /// <param name="item">Item to copy the properties from</param>
        /// <param name="stacks">Current stacks to start with</param>
        void CopyPropsFromItem(IInventroryItem item, int stacks);
    }
}
