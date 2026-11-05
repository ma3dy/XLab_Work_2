using System.Collections.Generic;
using UnityEngine;
namespace Game.Task2 {

public class Inventory
{
    private const int MaxUniqueItems = 10;
    private Dictionary<string, int> m_items = new Dictionary<string, int>();

public void AddItem(string item, int count = 1)
    {
        if (count <= 0) return;
        if (m_items.ContainsKey(item)) { m_items[item] += count;
            Debug.Log($"Добавлено: {item} x {count}");
            return;
        }

        if (m_items.Count >= MaxUniqueItems)
        {
            Debug.LogWarning($"Нет места для {item}: занято {m_items.Count} из {MaxUniqueItems}");
            return;
        }

        m_items.Add(item, count);
        Debug.Log($"Добавлено: {item} x {count}");
    }

    public void RemoveItem(string item, int count = 1)
    {
        if (count <= 0) return;
        if (!m_items.ContainsKey(item))
        {
            Debug.LogWarning($"Нельзя удалить {item}: нет в инвентаре");
            return;
        }

         if (m_items[item] < count)
            {
            Debug.LogWarning($"Нельзя удалить {item} x {count}: в инвентаре только {m_items[item]}");
            return;
        }
        m_items[item] -= count;

        if (m_items[item] <= 0)
        {
            m_items.Remove(item);
            Debug.Log($"Удалено: {item}");
        }

        else
        {
            Debug.Log($"Удалено: {item} x {count}");
        }
    }

    public void Print()
    {
        Debug.Log("Содержимое инвентаря");
        int totalCount = 0;
        foreach (var pair in m_items)
        {
            Debug.Log($"{pair.Key} x {pair.Value}");
            totalCount += pair.Value;
        }

        Debug.Log($"количество уникальных предметов {m_items.Count}, общее количество {totalCount}");
        Debug.Log(" ");
    }
}
}