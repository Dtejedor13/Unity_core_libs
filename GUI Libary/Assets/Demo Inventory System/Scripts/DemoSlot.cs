using System;
using TMPro;
using UnityCoreLibs.GUILibary.InventorySystem;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class DemoSlot : MonoBehaviour, IInventoryItemSlot
{
    public IInventroryItem? Item => _item;
    public int StackSize => _stackSize;

    private IInventroryItem? _item = null;
    private int _stackSize = 0;

    public bool SlotIsEmpty
    {
        get { return Item == null; }
    }

    [SerializeField] TextMeshProUGUI stackInfo;

    private void Awake()
    {
        stackInfo.text = string.Empty;
    }

    private void FixedUpdate()
    {
        // enable or disable buttons
        transform.GetChild(1).gameObject.SetActive(SlotIsEmpty == false);
        transform.GetChild(2).gameObject.SetActive(SlotIsEmpty == false);
    }

    public void ResetSlot()
    {
        if (SlotIsEmpty) return;
        
        _item = null;
        _stackSize = 0;
        stackInfo.text = string.Empty;
        SetImageSprite(null);
    }

    public void AssignItem(IInventroryItem item, int stackSize)
    {
        if (!SlotIsEmpty) return;

        _item = item;
        stackInfo.text = $"{stackSize}x";
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

        InventorySystem.Instance.RemoveItemFromInventory(Item, StackSize);
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
