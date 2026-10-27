using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeroClass
{
    Warrior,
    Mage
}

public class Arena : MonoBehaviour
{
    [Header("Settings")]
    public HeroClass heroClass = HeroClass.Warrior;
    public int maxRounds = 10;

    [Header("Visuals")]
    public UnitVisual heroVisual;
    public UnitVisual enemyVisual;

    private Unit hero;
    private Unit enemy;
    private List<Unit> enemyPool;

    void Awake()
    {
        hero = heroClass == HeroClass.Warrior
            ? new Unit("Warrior", 100, 10, 16)
            : new Unit("Mage", 130, 6, 12);

        enemyPool = new List<Unit>
        {
            new Unit("Goblin", 50, 4, 8),
            new Unit("Orc", 80, 5, 10),
            new Unit("Troll", 140, 8, 12)
        };

        enemy = enemyPool[Random.Range(0, enemyPool.Count)];

        Debug.Log($"Hero: {hero.Name} ({hero.Health} HP) vs {enemy.Name} ({enemy.Health} HP)");
    }

    void Start()
    {
        StartCoroutine(StartCombat());
    }

    IEnumerator StartCombat()
    {
        int round = 1;

        while (hero.Health > 0 && enemy.Health > 0 && round <= maxRounds)
        {
            Debug.Log($"Round {round}:");

            yield return new WaitForSeconds(0.5f);

            float heroDirection = 1f;
            if (heroVisual != null && enemyVisual != null)
            {
                heroDirection = Mathf.Sign(enemyVisual.transform.position.x - heroVisual.transform.position.x);
            }

            if (heroVisual != null) heroVisual.PlayAttackEffect(heroDirection);
            yield return new WaitForSeconds(0.1f);

            int heroDamage = hero.GetRandomDamage();
            enemy.TakeDamage(heroDamage);
            Debug.Log($"{hero.Name} deals {heroDamage} damage, enemy has {enemy.Health} HP");

            if (enemyVisual != null) enemyVisual.PlayHitEffect();

            if (enemy.Health <= 0) break;

            yield return new WaitForSeconds(0.5f);

            float enemyDirection = -1f;
            if (heroVisual != null && enemyVisual != null)
            {
                enemyDirection = Mathf.Sign(heroVisual.transform.position.x - enemyVisual.transform.position.x);
            }

            if (enemyVisual != null) enemyVisual.PlayAttackEffect(enemyDirection);
            yield return new WaitForSeconds(0.1f);

            int enemyDamage = enemy.GetRandomDamage();
            hero.TakeDamage(enemyDamage);
            Debug.Log($"{enemy.Name} deals {enemyDamage} damage, hero has {hero.Health} HP");

            if (heroVisual != null) heroVisual.PlayHitEffect();

            yield return new WaitForSeconds(0.5f);

            round++;
        }

        DetermineWinner();
    }

    private void DetermineWinner()
    {
        if (hero.Health <= 0 && enemy.Health <= 0)
        {
            Debug.Log("Draw! Both fighters are dead.");
        }
        else if (hero.Health <= 0)
        {
            Debug.Log($"Defeat! Enemy has {enemy.Health} HP left");
        }
        else if (enemy.Health <= 0)
        {
            Debug.Log($"Victory! Hero has {hero.Health} HP left");
        }
        else
        {
            if (hero.Health > enemy.Health)
                Debug.Log($"Victory by HP! Hero: {hero.Health} HP vs Enemy: {enemy.Health} HP");
            else if (enemy.Health > hero.Health)
                Debug.Log($"Defeat by HP! Hero: {hero.Health} HP vs Enemy: {enemy.Health} HP");
            else
                Debug.Log($"Draw by HP! Both have {hero.Health} HP");
        }
    }
}