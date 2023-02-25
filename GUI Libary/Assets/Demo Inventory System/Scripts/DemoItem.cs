using UnityCoreLibs.GUILibary.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

public class DemoItem : MonoBehaviour, IInventroryItem
{
    public int Id => id;
    public Sprite Sprite => itemSprite;
    public string ItemName => itemName;

    [SerializeField] int id;
    [SerializeField] int maxStacks = 1;
    [SerializeField] Sprite itemSprite;
    [SerializeField] string itemName;

    private void Start()
    {
        InventorySystem.Instance.OnItemAdd += OnItemAdd;
        transform.GetChild(0).GetComponent<Image>().sprite = itemSprite;
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
        if (itemThatWasAdded.Id == this.id) {
            Destroy(gameObject);
        }
    }
}
