using UnityEngine;

public class Unit
{
    public string Name { get; private set; }
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }
    public int Armor { get; private set; }
    public float CritChance { get; private set; }
    public Weapon EquippedWeapon { get; private set; }

    public bool IsAlive => Health > 0;

    public Unit(string name, int health, int armor, float critChance, Weapon startingWeapon)
    {
        Name = name;
        MaxHealth = health;
        Health = health;
        Armor = armor;
        CritChance = critChance;
        EquippedWeapon = startingWeapon;
    }

    public void TakeDamage(int damage)
    {
        Health = Mathf.Max(0, Health - damage);
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        EquippedWeapon = newWeapon;
    }

    public int Attack(Unit target)
    {
        int finalDamage = Mathf.Max(1, EquippedWeapon.Damage - target.Armor);

        ElementType targetElement = target.EquippedWeapon?.Element ?? ElementType.None;

        if (EquippedWeapon.Element != ElementType.None && targetElement != ElementType.None)
        {
            if (EquippedWeapon.Element != targetElement)
            {
                finalDamage = Mathf.FloorToInt(finalDamage * 1.5f);
            }
            else
            {
                finalDamage = 1;
            }
        }

        if (Random.Range(0f, 1f) < CritChance)
        {
            finalDamage *= 2;
        }

        target.TakeDamage(finalDamage);
        return finalDamage;
    }
}