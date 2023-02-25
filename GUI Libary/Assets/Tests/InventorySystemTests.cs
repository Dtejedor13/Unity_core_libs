using NUnit.Framework;
using System.Collections.Generic;
using UnityCoreLibs.GUILibary.InventorySystem;

public class InventorySystemTests
{
    [Test]
    public void TestSystemCreation()
    {
        var system = CreateInventorySystem(1);
        Assert.True(system is not null);
    }

    [Test]
    public void TestItemCreation()
    {
        var item = CreateItem(0, 1);
        Assert.IsTrue(item is not null);
    }

    [Test]
    public void TestAddingToInventory()
    {
        var item = CreateItem(0, 1); 
        var inventory = CreateInventorySystem(1);
        inventory.OnItemAdd += (object sender, IInventroryItem itemThatWasAdded, int stackSize) => {
            Assert.AreEqual(item.Id, itemThatWasAdded.Id);
        };

        inventory.OnItemAddFail += (object sender, IInventroryItem itemThatCanotBeAdded, int stackSize) => {
            Assert.IsTrue(false);
        };

        inventory.AddItemToInventory(item);
    }

    [Test]
    public void TestRemovingFromInventory()
    {
        var inventory = CreateInventorySystem(2);
        var item1 = CreateItem(0, 1);
        var item2 = CreateItem(1, 1);
        inventory.OnItemRemove += (object sender, IInventroryItem itemThatWasRemoved, int stackSize) => {
            Assert.AreEqual(item2.Id, itemThatWasRemoved.Id);
        };
        inventory.OnItemRemoveFail += (object sender, IInventroryItem itemThatCanotBeRemoved, int stackSize) => {
            Assert.AreNotEqual(item2.Id, itemThatCanotBeRemoved.Id);
        };

        inventory.AddItemToInventory(item1);
        inventory.AddItemToInventory(item2);
        inventory.RemoveItemFromInventory(item2);
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
            CreateItem(5, 10), // this will fail
            CreateItem(6, 1) // this will fail
        };

        inventory.OnItemAdd += (object sender, IInventroryItem itemThatWasAdded, int stackSize) => {
            Assert.IsTrue(itemThatWasAdded.Id < 5);
        };

        inventory.OnItemAddFail += (object sender, IInventroryItem itemThatCanotBeAdded, int stackSize) => {
            Assert.IsTrue(itemThatCanotBeAdded.Id >= 5);
        };

        for (int i = 0; i < items.Count; i++)
            inventory.AddItemToInventory(items[i]);
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
        for (int i = 0; i < maxSlots; i++) {
            var slot = new DemoSlot();
            slots.Add(slot);
        }

        inventory.MaxInventorySlots = maxSlots;
        inventory.Init(slots);
        return inventory;
    }

    private IInventroryItem CreateItem(int id, int maxStacks)
    {
        var item = new DemoItem();
        item.InitForTests(id, maxStacks);
        return item;
    }
}
