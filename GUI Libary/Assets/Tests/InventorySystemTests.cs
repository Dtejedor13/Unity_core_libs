using NUnit.Framework;
using System.Collections.Generic;
using UnityCoreLibs.GUILibary.InventorySystem;
using UnityEngine;

public class InventorySystemTests
{
    [Test]
    public void TestAddingToInventory()
    {
        var item = new TestInventoryItem(0, 1); 
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
        var item1 = new TestInventoryItem(0, 1);
        var item2 = new TestInventoryItem(2, 1);
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
        var item1 = new TestInventoryItem(0, 1);
        var item2 = new TestInventoryItem(1, 1);
        var item3 = new TestInventoryItem(2, 1);
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
        var items = new List<TestInventoryItem> {
            new TestInventoryItem(0, 1),
            new TestInventoryItem(1, 10),
            new TestInventoryItem(2, 5),
            new TestInventoryItem(3, 4),
            new TestInventoryItem(4, 1),
            new TestInventoryItem(5, 10),
            new TestInventoryItem(6, 1)
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
        var item = new TestInventoryItem(0, 10);
        var item2 = new TestInventoryItem(1, 10);

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
        InventorySystem inventory = new InventorySystem();
        inventory.Init();
        inventory.MaxInventorySlots = maxSlots;
        return inventory;
    }

    private class TestInventoryItem : IInventroryItem
    {
        public int Id => _id;
        private readonly int _maxStacks;
        private readonly int _id;
        public TestInventoryItem(int id, int maxStacks)
        {
            _id = id;
            _maxStacks = maxStacks;
        }


        public GameObject GetGameObject()
        {
            return null;
        }

        public int GetMaxStackSize()
        {
            return _maxStacks;
        }

        public bool ItemIsStackeble()
        {
            return _maxStacks > 1;
        }
    }
}
