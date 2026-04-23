using UnityEngine;
using System.Collections.Generic;

public class StorageBox : MonoBehaviour
{
    [Header("Точки хранения (Empty GameObjects)")]
    public Transform[] slots; // Перетащи сюда пустышки, где должны лежать предметы

    // Внутренний список, чтобы знать, какой слот занят
    private GameObject[] storedItems;

    void Start()
    {
        // Инициализируем массив размером с количество слотов
        storedItems = new GameObject[slots.Length];
    }

    // Попытка положить предмет
    public bool TryAddItem(GameObject item, out Transform targetSlot)
    {
        targetSlot = null;

        for (int i = 0; i < slots.Length; i++)
        {
            if (storedItems[i] == null) // Нашли пустой слот
            {
                storedItems[i] = item;
                targetSlot = slots[i];
                return true;
            }
        }
        return false; // Нет места
    }

    // Попытка забрать предмет (возвращает последний добавленный или тот, на который смотрим - упростим до LIFO или первого найденного)
    public GameObject TryTakeItem()
    {
        // Ищем последний заполненный слот (чтобы брать предметы "сверху", если это стопка, или просто по порядку)
        for (int i = slots.Length - 1; i >= 0; i--)
        {
            if (storedItems[i] != null)
            {
                GameObject itemToGive = storedItems[i];
                storedItems[i] = null; // Освобождаем слот
                return itemToGive;
            }
        }
        return null; // Пусто
    }
}