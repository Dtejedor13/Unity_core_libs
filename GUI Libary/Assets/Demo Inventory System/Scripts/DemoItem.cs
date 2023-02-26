using TMPro;
using UnityCoreLibs.GUILibary.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

public class DemoItem : MonoBehaviour, IInventroryItem
{
    public int Id => id;
    public Sprite Sprite => sprite;
    public string ItemName => itemName;

    [SerializeField] Sprite sprite;
    [SerializeField] int id;
    [SerializeField] string itemName;
    [SerializeField] int maxStacks = 1;
    [SerializeField] int stacks = 1;
    [SerializeField] TextMeshProUGUI stackInfo;
    [SerializeField] Image itemSpriteImage;

    // Do not use Awake() for this, because the InventorySystem is not initialized yet
    private void Start()
    {
        InventorySystem.Instance.OnItemAdd += OnItemAdd;
        itemSpriteImage.sprite = Sprite;
        stackInfo.text = $"{stacks}x";
    }

    private void OnDestroy()
    {
        InventorySystem.Instance.OnItemAdd -= OnItemAdd;
    }

    public GameObject GetGameObject()
    {
        try
        {
            return gameObject;
        }
        catch (System.NullReferenceException ex)
        {
            // Exceptions happens in tests
            Debug.LogWarning("Gameobject not found");
            return null;
        }
    }

    public int GetMaxStackSize()
    {
        return maxStacks;
    }

    public bool ItemIsStackeble()
    {
        return maxStacks > 1;
    }

    public void CopyPropsFromItem(IInventroryItem item, int stacks)
    {
        this.id = item.Id;
        this.maxStacks = item.GetMaxStackSize();
        this.stacks = stacks;
        this.sprite = item.Sprite;
        this.itemName = item.ItemName;
        itemSpriteImage.sprite = Sprite;
        stackInfo.text = $"{stacks}x";
    }

    // button event
    public void AddToInventory()
    {
        InventorySystem.Instance.AddItemToInventory(this);
    }

    /// <summary>
    /// Sets the SerializeField vars for tests
    /// </summary>
    public void InitForTests(int id, int maxStacks)
    {
        this.id = id;
        this.maxStacks = maxStacks;
    }

    private void OnItemAdd(object sender, IInventroryItem itemThatWasAdded, int stackSize)
    {
        if (itemThatWasAdded.Id == this.Id)
        {
            Destroy(gameObject);
        }
    }
}
