using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform heroSpawnPoint;
    public Transform enemySpawnPoint;

    [Header("Hero Team Prefabs")]
    public List<UnitController> heroPrefabs;

    [Header("Enemy Team Prefabs")]
    public List<UnitController> enemyPrefabs;

    private List<UnitController> activeHeroes = new List<UnitController>();
    private List<UnitController> activeEnemies = new List<UnitController>();

    private WaitForSeconds startDelay = new WaitForSeconds(1f);
    private WaitForSeconds attackDelay = new WaitForSeconds(0.6f);
    private WaitForSeconds turnDelay = new WaitForSeconds(0.5f);

    private const string GoblinPrefabName = "Goblin";

    void Start()
    {
        SpawnTeam(heroPrefabs, heroSpawnPoint, activeHeroes, true);
        SpawnTeam(enemyPrefabs, enemySpawnPoint, activeEnemies, false);

        StartCoroutine(CombatLoop());
    }

    private void SpawnTeam(List<UnitController> prefabs, Transform spawnPoint, List<UnitController> activeList, bool isHero)
    {
        for (int i = 0; i < prefabs.Count; i++)
        {
            int col = i % 2;
            float xOffset = col * 2.5f;
            float zOffset = i * 4f;
            float direction = isHero ? -1f : 1f;

            Vector3 spawnPos = spawnPoint.position + new Vector3(xOffset * direction, 0, zOffset);
            UnitController instance = Instantiate(prefabs[i], spawnPos, spawnPoint.rotation);

            if (!isHero && prefabs[i].name.Contains(GoblinPrefabName))
            {
                instance.transform.Rotate(0, 180, 0);
            }

            string unitName = (isHero ? "Hero " : "Enemy ") + i + " (" + prefabs[i].name + ")";

            Weapon startWeapon = isHero ? new Weapon("Sword", 20, ElementType.Fire, 10) : new Weapon("Club", 16, ElementType.None, 5);
            Unit unitData = new Unit(unitName, 100, isHero ? 3 : 2, isHero ? 0.3f : 0.1f, startWeapon);

            instance.Setup(unitData);
            activeList.Add(instance);
        }
    }

    private IEnumerator CombatLoop()
    {
        yield return startDelay;

        while (HasAliveUnits(activeHeroes) && HasAliveUnits(activeEnemies))
        {
            for (int i = 0; i < activeHeroes.Count; i++)
            {
                if (!activeHeroes[i].UnitData.IsAlive) continue;

                var target = GetRandomAliveTarget(activeEnemies);
                if (target == null) break;

                yield return StartCoroutine(ProcessAttack(activeHeroes[i], target));
            }

            if (!HasAliveUnits(activeEnemies)) break;

            for (int i = 0; i < activeEnemies.Count; i++)
            {
                if (!activeEnemies[i].UnitData.IsAlive) continue;

                var target = GetRandomAliveTarget(activeHeroes);
                if (target == null) break;

                yield return StartCoroutine(ProcessAttack(activeEnemies[i], target));
            }
        }

        Debug.Log("=== BATTLE OVER ===");
        if (HasAliveUnits(activeHeroes)) Debug.Log("Heroes win!");
        else if (HasAliveUnits(activeEnemies)) Debug.Log("Enemies win!");
        else Debug.Log("Draw!");
    }

    private IEnumerator ProcessAttack(UnitController attacker, UnitController defender)
    {
        attacker.PlayAttackAnimation();

        yield return attackDelay;

        int damage = attacker.UnitData.Attack(defender.UnitData);

        Debug.Log($"{attacker.UnitData.Name} hits {defender.UnitData.Name} for {damage} damage. {defender.UnitData.Name} HP: {defender.UnitData.Health}");

        if (!defender.UnitData.IsAlive)
            defender.PlayDeathReaction();
        else
            defender.PlayHitReaction();

        yield return turnDelay;
    }

    private bool HasAliveUnits(List<UnitController> team)
    {
        for (int i = 0; i < team.Count; i++)
        {
            if (team[i].UnitData.IsAlive) return true;
        }
        return false;
    }

    private UnitController GetRandomAliveTarget(List<UnitController> team)
    {
        int aliveCount = 0;

        for (int i = 0; i < team.Count; i++)
        {
            if (team[i].UnitData.IsAlive) aliveCount++;
        }

        if (aliveCount == 0) return null;

        int randomTargetIndex = Random.Range(0, aliveCount);
        int currentIndex = 0;

        for (int i = 0; i < team.Count; i++)
        {
            if (team[i].UnitData.IsAlive)
            {
                if (currentIndex == randomTargetIndex) return team[i];
                currentIndex++;
            }
        }

        return null;
    }
}