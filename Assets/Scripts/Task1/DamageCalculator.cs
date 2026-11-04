using UnityEngine;
namespace Game.Task1
{
    public class DamageCalculator : MonoBehaviour
    {
        [Header("Оружие")]
        public int weaponDamage = 10;
        public ElementType weaponElement = ElementType.Fire;

        [Header("Броня")]
        public int targetArmor = 3;
        public ElementType targetElement = ElementType.Ice;

        [Header("Критический удар")]
        public float critChance = 0.3f;

        private void Start()
        {
            for (int i = 1; i <= 3; i++)
            {
                int finalDamage = CalculateDamage(
                weaponDamage, weaponElement, targetArmor, targetElement, critChance, out string logDetails);
                Debug.Log($"Удар {i}: {finalDamage} урона {logDetails}");
            }
        }
        public static int CalculateDamage(
            int weaponDamage, ElementType weaponElement, int targetArmor, ElementType targetElement, float critChance, out string details)
        {
            int currentDamage = Mathf.Max(1, weaponDamage - targetArmor);

            string elementText = "";
            string critText = "без крита";

            if (weaponElement != ElementType.None && targetElement != ElementType.None)
            {
                if (weaponElement != targetElement)
                {
                    currentDamage = Mathf.FloorToInt(currentDamage * 1.5f);
                    elementText = "стихия x1.5, ";
                }
                else
                {
                    currentDamage = 0;
                    elementText = "стихии совпали, ";
                }
            }

            if (currentDamage > 0)
            {
                if (Random.Range(0f, 1f) < Mathf.Clamp01(critChance))
                {
                    currentDamage *= 2;
                    critText = "крит х2";

                }
            }

            details = $"{elementText}{critText}";
            return currentDamage;
        }
    }
}