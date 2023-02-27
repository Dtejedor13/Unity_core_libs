using System;
using TMPro;
using UnityCoreLibs.GUILibary.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

public class DemoSlot : MonoBehaviour, IInventoryItemSlot
{
    public IInventroryItem? Item => _item;
    public int StackSize => _stackSize;
    public bool SlotIsEmpty
    {
        get { return Item == null; }
    }

    [SerializeField] TextMeshProUGUI stackInfo;

    private IInventroryItem? _item = null;
    private int _stackSize = 0;

    private void Awake()
    {
        stackInfo.text = string.Empty;
    }

    private void FixedUpdate()
    {
        // enable or disable buttons
        transform.GetChild(1).gameObject.SetActive(SlotIsEmpty == false);
        transform.GetChild(2).gameObject.SetActive(SlotIsEmpty == false);

        if (stackInfo is not null)
        {
            // display the stack info
            if (SlotIsEmpty)
                stackInfo.text = string.Empty;
            else
                stackInfo.text = $"{_stackSize}x";
        }
    }

    public void ResetSlot()
    {
        if (SlotIsEmpty) return;
        
        _item = null;
        _stackSize = 0;
        SetImageSprite(null);
    }

    public void AssignItem(IInventroryItem item, int stackSize)
    {
        if (!SlotIsEmpty) return;

        _item = item;
        _stackSize = stackSize;
        SetImageSprite(item.Sprite);
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void IncreaseStackSize(int value)
    {
        if (SlotIsEmpty) return;
        _stackSize += value;
    }

    public void DecreaseStackSize(int value)
    {
        if (SlotIsEmpty) return;
        _stackSize -= value;

        if (_stackSize <= 0)
            ResetSlot();
    }

    // button events
    public void DropItem()
    {
        if (SlotIsEmpty) return;

        var stacksToDrop = 1;
        InventorySystem.Instance.RemoveItemFromInventory(Item, stacksToDrop);
    }

    public void InspectItem()
    {
        if (SlotIsEmpty) return;

        DemoItemInfoPanel.Instance.AssignItem(Item);
    }

    private void SetImageSprite(Sprite sp)
    {
        try
        {
            transform.GetChild(0).GetComponent<Image>().sprite = sp;
        }
        catch (NullReferenceException ex)
        {
            // Exceptions happens in tests
            Debug.LogWarning("Image child component not found");
        }
    }
}