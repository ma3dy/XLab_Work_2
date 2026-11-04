using UnityEngine;
namespace Game.Task2
{
    public class InventoryTester : MonoBehaviour
    {
        private Inventory m_inventory = new Inventory();

        private void Start()
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
            Debug.Log("Добавление предметов");
            m_inventory.AddItem("Меч", 1);
            m_inventory.AddItem("Щит", 1);
            m_inventory.AddItem("Меч", 1);
            m_inventory.Print();
        }
        private void Test2_FillInventory()
        {
            Debug.Log("Заполнение всех ячеек инвентаря");
            string[] itemsToAdd = { "Доспех", "Поножи", "Сапоги", "Перчатки", "Топор", "Лук", "Зелье", "Кольцо" };
            foreach (string item in itemsToAdd)
            {
                m_inventory.AddItem(item);
            }
        }
        private void Test3_TryAddExtraItem()
        {
            Debug.Log("Попытка добавить предмет при заполненном инвентаре");
            m_inventory.AddItem("Шлем");
        }
        private void Test4_RemoveNonExistentItem()
        {
            Debug.Log("Попытка удалить несуществующий предмет");
            m_inventory.RemoveItem("Посох");
        }
        private void Test5_RemoveMoreThanAvailable()
        {
            Debug.Log("Попытка удалить больше, чем есть");
            m_inventory.RemoveItem("Меч", 5);
        }
        private void Test6_RemoveItemCompletely()
        {
            Debug.Log("Успешное удаление предмета");
            m_inventory.RemoveItem("Меч", 2);
        }
        private void PrintFinalState()
        {
            Debug.Log("Инвентарь");
            m_inventory.Print();
        }
    }
}