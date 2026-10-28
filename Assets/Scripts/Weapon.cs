using UnityEngine;

public enum ElementType
{
    None,
    Fire,
    Ice
}

public class Weapon
{
    public string Name { get; private set; }
    public int Damage { get; private set; }
    public ElementType Element { get; private set; }
    public int Utility { get; private set; }

    public Weapon(string name, int damage, ElementType element, int utility)
    {
        Name = name;
        Damage = damage;
        Element = element;
        Utility = utility;
    }
}