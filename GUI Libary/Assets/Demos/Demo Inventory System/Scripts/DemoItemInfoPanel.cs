using TMPro;
using UnityCoreLibs.GUILibary.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

public class DemoItemInfoPanel : MonoBehaviour
{
    public static DemoItemInfoPanel Instance => _instance;
    private static DemoItemInfoPanel _instance;

    [SerializeField] TextMeshProUGUI itemNameText;
    [SerializeField] Image itemSprite;

    private IInventroryItem? _item;

    private void Awake()
    {
        if (_instance is null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void AssignItem(IInventroryItem item)
    {
        if (item is not null)
        {
            _item = item;
            itemNameText.text = item.ItemName;
            itemSprite.sprite = item.Sprite;
            gameObject.SetActive(true);
        }
    }

    public void RemoveItem()
    {
        _item = null;
        gameObject.SetActive(false);
    }
}
