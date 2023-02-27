# GUI libary
This library contains basic ui elements for all sort of games.
## Content
- Inventory System

## Inventory System How to use
First you will need to download the project and import the selected files as unity package or copy the files in your project directly.
The needed files for the inventory system are as follows:
- InventorySystem.cs (Class)
- IInventoryItemSlot.cs (Interface)
- IInventroryItem.cs (Interface)
U can find them under following path:
````
Assets/InventorySystem/
````
After a successfull import of the files, create a new gameobject what will function als inventory system object and assign the InventorySystem class to the object.
Then u need to implement both interfaces for the inventory slot and inventory item.
The Inventory Item is the item that can be added to the inventory, the Slot is the actual handler for the item in the inventory slot.
The Inventory system will spawn depending on his configuration slots from the given prefab that implement the slot interface.

They are a demo for the implementation that can be used to setup the system.
Enjoy!
