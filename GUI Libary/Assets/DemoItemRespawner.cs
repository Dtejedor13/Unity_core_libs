using UnityCoreLibs.GUILibary.InventorySystem;
using UnityEngine;

public class DemoItemRespawner : MonoBehaviour
{
    [SerializeField] Transform spawnTransform;
    [SerializeField] GameObject itemPrefab;

    private void Start()
    {
        var inventorySystem = InventorySystem.Instance;
        inventorySystem.OnItemRemove += OnItemRemove;
    }

    private void OnItemRemove(object sender, IInventroryItem itemThatWasRemoved, int stackSize)
    {
        var go = Instantiate(itemPrefab, spawnTransform);

        if (go.TryGetComponent(out IInventroryItem item))
            item.CopyPropsFromItem(itemThatWasRemoved, stackSize);
        else 
            throw new System.Exception("Inventory item prefab needs to implement the IInventroryItem interface!");
    }
}
