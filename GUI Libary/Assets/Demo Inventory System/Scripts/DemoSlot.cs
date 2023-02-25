using System.Collections;
using System.Collections.Generic;
using UnityCoreLibs.GUILibary.InventorySystem;
using UnityEngine;

public class DemoSlot : MonoBehaviour, IInventoryItemSlot
{
    public IInventroryItem? Item => _item;
    public int StackSize => _stackSiuze;

    private IInventroryItem? _item = null;
    private int _stackSiuze = 0;

    public bool SlotIsEmpty
    {
        get { return Item == null; }
    }

    public void RemoveItem()
    {
        if (SlotIsEmpty) return;

        _stackSiuze = 0;
        Destroy(Item.GetGameObject());
        _item = null;
    }

    public void AssignItem(IInventroryItem item, int stackSize)
    {
        if (!SlotIsEmpty) return;

        _item = item;
        _stackSiuze = stackSize;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void SetStackSize(int newValue)
    {
        throw new System.NotImplementedException();
    }
}
