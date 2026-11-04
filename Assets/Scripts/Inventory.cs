using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private Dictionary<string, int> items = new Dictionary<string, int>();

    private const int MaxUniqueItems = 10;

    public void AddItem(string item, int count = 1)
    {
        if (items.ContainsKey(item))
        {
            items[item] += count;
            Debug.Log($"Added: {item} x {count}");
            return;
        }

        if (items.Count >= MaxUniqueItems)
        {
            Debug.LogWarning($"No space for '{item}': {items.Count}/{MaxUniqueItems} slots occupied.");
            return;
        }

        items.Add(item, count);
        Debug.Log($"Added: {item} x {count}");
    }

    public void RemoveItem(string item, int count = 1)
    {
        if (!items.ContainsKey(item))
        {
            Debug.LogWarning($"Unable to remove '{item}': it is not in the inventory.");
            return;
        }

        if (items[item] < count)
        {
            Debug.LogWarning($"Unable to remove '{item}' x{count}: only {items[item]} available.");
            return;
        }

        items[item] -= count;

        if (items[item] <= 0)
        {
            items.Remove(item);
            Debug.Log($"Item completely removed: {item}");
        }
        else
        {
            Debug.Log($"Removed: {item} x {count}");
        }
    }

    public void Print()
    {
        Debug.Log("--- Inventory Content ---");

        int totalCount = 0;

        foreach (var pair in items)
        {
            Debug.Log($"{pair.Key} x{pair.Value}");
            totalCount += pair.Value;
        }

        Debug.Log($"Unique items: {items.Count}, Total items: {totalCount}");
        Debug.Log("-------------------------");
    }
}