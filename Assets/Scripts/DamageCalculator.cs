using UnityEngine;

public enum ElementType
{
    None,
    Fire,
    Ice
}

public class DamageCalculator : MonoBehaviour
{
    [Header("Weapon")]
    public int weaponDamage = 10;
    public ElementType weaponElement = ElementType.Fire;

    [Header("Armor")]
    public int targetArmor = 3;
    public ElementType targetElement = ElementType.Ice;

    [Header("Critical Damage")]
    public float critChance = 0.3f;

    private const float ElementMultiplier = 1.5f;
    private const int CritMultiplier = 2;
    private const int MinDamage = 1;

    void Start()
    {
        for (int i = 1; i <= 3; i++)
        {
            int finalDamage = CalculateDamage(out string logDetails);
            Debug.Log($"Attack {i}: {finalDamage} damage ({logDetails})");
        }
    }

    private int CalculateDamage(out string details)
    {
        int currentDamage = weaponDamage - targetArmor;

        if (currentDamage < MinDamage)
        {
            currentDamage = MinDamage;
        }

        string elementText = "";
        string critText = "";

        if (weaponElement != ElementType.None && targetElement != ElementType.None)
        {
            if (weaponElement != targetElement)
            {
                currentDamage = Mathf.FloorToInt(currentDamage * ElementMultiplier);
                elementText = "element x1.5, ";
            }
            else
            {
                currentDamage = MinDamage;
                elementText = "identical elements, ";
            }
        }

        float randomRoll = Random.Range(0f, 1f);
        if (randomRoll < Mathf.Clamp01(critChance))
        {
            currentDamage *= CritMultiplier;
            critText = "critical damage x2";
        }
        else
        {
            critText = "no critical damage";
        }

        details = $"{elementText}{critText}";
        return currentDamage;
    }
}