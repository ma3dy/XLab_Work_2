using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    private Inventory inventory = new Inventory();

    void Start()
    {
        Test1_AddInitialItems();
        Test2_FillInventory();
        Test3_TryAddExtraItem();
        Test4_RemoveNonExistentItem();
        Test5_RemoveMoreThanAvailable();
        Test6_RemoveItemCompletely();

        PrintFinalState();
    }

    private void Test1_AddInitialItems()
    {
        Debug.Log("=== Test 1: Adding initial items ===");
        inventory.AddItem("Sword", 1);
        inventory.AddItem("Helmet", 1);
        inventory.AddItem("Sword", 1);
        inventory.Print();
    }

    private void Test2_FillInventory()
    {
        Debug.Log("=== Test 2: Filling inventory to 10 unique items ===");
        string[] itemsToAdd = { "Breastplate", "Greaves", "Boots", "Axe", "Staff", "Potion", "Bow", "Ring" };
        foreach (string item in itemsToAdd)
        {
            inventory.AddItem(item);
        }
    }

    private void Test3_TryAddExtraItem()
    {
        Debug.Log("=== Test 3: Trying to add 11th unique item ===");
        inventory.AddItem("Amulet");
    }

    private void Test4_RemoveNonExistentItem()
    {
        Debug.Log("=== Test 4: Trying to remove a non-existent item ===");
        inventory.RemoveItem("Shield");
    }

    private void Test5_RemoveMoreThanAvailable()
    {
        Debug.Log("=== Test 5: Trying to remove more than available ===");
        inventory.RemoveItem("Helmet", 2);
    }

    private void Test6_RemoveItemCompletely()
    {
        Debug.Log("=== Test 6: Successfully removing an item completely ===");
        inventory.RemoveItem("Sword", 2);
    }

    private void PrintFinalState()
    {
        Debug.Log("=== FINAL INVENTORY STATE ===");
        inventory.Print();
    }
}