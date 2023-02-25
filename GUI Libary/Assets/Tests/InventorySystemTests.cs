using NUnit.Framework;
using System.Collections.Generic;
using UnityCoreLibs.GUILibary.InventorySystem;

public class InventorySystemTests
{
    [Test]
    public void TestAddingToInventory()
    {
        var item = CreateItem(0, 1); 
        var inventory = CreateInventorySystem(1);
        inventory.OnItemAdd += (object sender, IInventroryItem itemThatWasAdded, int stackSize) => {
            Assert.AreEqual(item.Id, itemThatWasAdded.Id);
        };
        inventory.AddItemToInventory(item);
    }

    [Test]
    public void TestRemovingFromInventory()
    {
        var inventory = CreateInventorySystem(2);
        var item1 = CreateItem(0, 1);
        var item2 = CreateItem(2, 1);
        inventory.OnItemRemove += (object sender, IInventroryItem itemThatWasRemoved, int stackSize) => {
            Assert.AreEqual(item2.Id, itemThatWasRemoved.Id);
        };
        inventory.AddItemToInventory(item1);
        inventory.AddItemToInventory(item2);
        inventory.RemoveItemToInventory(item2);
    }

    [Test]
    public void TestMaximumSizeInventory()
    {
        var inventory = CreateInventorySystem(2);
        var item1 = CreateItem(0, 1);
        var item2 = CreateItem(1, 1);
        var item3 = CreateItem(2, 1);
        inventory.OnItemAdd += (object sender, IInventroryItem itemThatWasAdded, int stackSize) => {
            Assert.AreNotEqual(item3.Id, itemThatWasAdded.Id);
        };
        inventory.AddItemToInventory(item1);
        inventory.AddItemToInventory(item2);
        inventory.AddItemToInventory(item3);
    }

    [Test]
    public void TestIfInventoryDetectEmptySlots()
    {
        var inventory = CreateInventorySystem(5);
        var items = new List<IInventroryItem> {
            CreateItem(0, 1),
            CreateItem(1, 10),
            CreateItem(2, 5),
            CreateItem(3, 4),
            CreateItem(4, 1),
            CreateItem(5, 10),
            CreateItem(6, 1)
        };

        for(int i = 0; i < items.Count; i++) {
            var shouldBeSuccessfull = inventory.InventoryHasSpaceForMoreStacks(items[i]);
            inventory.AddItemToInventory(items[i]);
            // i = 0, 1, 2, 3 should be addeble
            if (i < 4)
                Assert.AreEqual(inventory.GetSizeOfUnusedSlots() - 1 > 0, shouldBeSuccessfull);
            else
                Assert.False(shouldBeSuccessfull);
        }
    }

    [Test]
    public void TestIfItemCanBeStacked()
    {
        var inventory = CreateInventorySystem(2);
        var item = CreateItem(0, 10);
        var item2 = CreateItem(1, 10);

        inventory.AddItemToInventory(item, 2);
        inventory.AddItemToInventory(item2, 2);

        var shouldBePossible = inventory.InventoryHasSpaceForMoreStacks(item2, 6);
        Assert.True(shouldBePossible);
        inventory.AddItemToInventory(item2, 6);

        shouldBePossible = inventory.InventoryHasSpaceForMoreStacks(item2, 6);
        Assert.False(shouldBePossible);
    }

    private InventorySystem CreateInventorySystem(int maxSlots)
    {
        var inventory = new InventorySystem();
        var slots = new List<IInventoryItemSlot>();

        for (int i = 0; i < maxSlots; i++)
            slots.Add(new DemoSlot());

        inventory.Init(slots);
        inventory.MaxInventorySlots = maxSlots;
        return inventory;
    }

    private IInventroryItem CreateItem(int id, int maxStacks)
    {
        var item = new DemoItem();
        item.InitForTests(id, maxStacks);
        return item;
    }
}
