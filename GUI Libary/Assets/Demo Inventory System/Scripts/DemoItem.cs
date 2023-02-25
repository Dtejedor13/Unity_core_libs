using UnityCoreLibs.GUILibary.InventorySystem;
using UnityEngine;

public class DemoItem : MonoBehaviour, IInventroryItem
{
    [SerializeField] int id;
    [SerializeField] int maxStacks = 1;

    public int Id => id;

    public void InitForTests(int id, int maxStacks)
    {
        this.id = id;
        this.maxStacks = maxStacks;
    }

    private void Start()
    {
        InventorySystem.Instance.OnItemAdd += OnItemAdd;
    }

    private void OnDestroy()
    {
        InventorySystem.Instance.OnItemAdd -= OnItemAdd;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
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

    private void OnItemAdd(object sender, IInventroryItem itemThatWasAdded, int stackSize)
    {
        if (itemThatWasAdded.Id == this.id) {
            Destroy(gameObject);
        }
    }
}
