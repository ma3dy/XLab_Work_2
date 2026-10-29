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

    [Header("Weapon Prefabs")]
    public GameObject mageStaffPrefab;
    public GameObject warriorSwordPrefab;
    public GameObject goblinSwordPrefab;
    public GameObject orcAxePrefab;

    private List<UnitController> activeHeroes = new List<UnitController>();
    private List<UnitController> activeEnemies = new List<UnitController>();

    private WaitForSeconds startDelay = new WaitForSeconds(1f);
    private WaitForSeconds attackDelay = new WaitForSeconds(0.6f);
    private WaitForSeconds turnDelay = new WaitForSeconds(0.5f);

    private const float X_OFFSET = 2.5f;
    private const float Z_OFFSET = 4f;

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
            float direction = isHero ? -1f : 1f;
            Vector3 spawnPos = spawnPoint.position + new Vector3(X_OFFSET * (i % 2) * direction, 0, Z_OFFSET * i);

            UnitController instance = Instantiate(prefabs[i], spawnPos, spawnPoint.rotation);
            string lowerName = prefabs[i].name.ToLower();

            if (!isHero && lowerName.Contains("goblin"))
            {
                instance.transform.Rotate(0, 180, 0);
            }

            string unitName = (isHero ? "Hero " : "Enemy ") + i + " (" + prefabs[i].name + ")";

            (Weapon weapon, GameObject visual) = GetCharacterLoadout(lowerName);
            Unit unitData = new Unit(unitName, 100, isHero ? 3 : 2, isHero ? 0.3f : 0.1f, weapon);

            instance.Setup(unitData, visual);
            activeList.Add(instance);
        }
    }

    private (Weapon weapon, GameObject visual) GetCharacterLoadout(string lowerName)
    {
        if (lowerName.Contains("goblin")) return (new Weapon("Sword", 15, ElementType.None, 6), goblinSwordPrefab);
        if (lowerName.Contains("orc")) return (new Weapon("Axe", 25, ElementType.None, 9), orcAxePrefab);
        if (lowerName.Contains("troll") || lowerName.Contains("foe")) return (new Weapon("Claws", 10, ElementType.None, 0), null);
        if (lowerName.Contains("wizard") || lowerName.Contains("mage")) return (new Weapon("Staff", 15, ElementType.Fire, 8), null);
        if (lowerName.Contains("davi") || lowerName.Contains("warrior")) return (new Weapon("Sword", 35, ElementType.Fire, 10), warriorSwordPrefab);

        return (new Weapon("Fists", 5, ElementType.None, 0), null);
    }

    private bool IsWeaponExcluded(string unitName)
    {
        string lowerName = unitName.ToLower();
        return lowerName.Contains("mage") || lowerName.Contains("wizard") || lowerName.Contains("troll") || lowerName.Contains("foe");
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

        Debug.Log($"{attacker.UnitData.Name} ({attacker.UnitData.EquippedWeapon.Name}) hits {defender.UnitData.Name} for {damage} damage. {defender.UnitData.Name} HP: {defender.UnitData.Health}");

        if (!defender.UnitData.IsAlive)
        {
            defender.PlayDeathReaction();
            HandleWeaponDrop(defender);
        }
        else
        {
            defender.PlayHitReaction();
        }

        yield return turnDelay;
    }

    private void HandleWeaponDrop(UnitController deadUnit)
    {
        if (IsWeaponExcluded(deadUnit.UnitData.Name)) return;

        Weapon droppedWeapon = deadUnit.UnitData.EquippedWeapon;
        if (droppedWeapon == null || droppedWeapon.Utility == 0) return;

        GameObject droppedVisual = deadUnit.DropWeaponVisual();

        if (droppedVisual != null)
        {
            droppedVisual.transform.position = deadUnit.transform.position + new Vector3(0, 0.5f, 0);
        }

        if (TryPickupWeapon(activeHeroes, droppedWeapon, droppedVisual, deadUnit.UnitData.Name)) return;
        TryPickupWeapon(activeEnemies, droppedWeapon, droppedVisual, deadUnit.UnitData.Name);
    }

    private bool TryPickupWeapon(List<UnitController> team, Weapon droppedWeapon, GameObject droppedVisual, string deadUnitName)
    {
        for (int i = 0; i < team.Count; i++)
        {
            if (IsWeaponExcluded(team[i].UnitData.Name)) continue;

            if (team[i].UnitData.IsAlive && droppedWeapon.Utility > team[i].UnitData.EquippedWeapon.Utility)
            {
                Debug.Log($"[DROP] {team[i].UnitData.Name} picked up {droppedWeapon.Name} (Utility: {droppedWeapon.Utility}) from {deadUnitName}!");
                team[i].UnitData.EquipWeapon(droppedWeapon);

                if (droppedVisual != null)
                {
                    droppedVisual.transform.SetParent(team[i].WeaponHand);
                    droppedVisual.transform.localPosition = team[i].weaponPositionOffset;
                    droppedVisual.transform.localRotation = Quaternion.Euler(team[i].weaponRotationOffset);
                    team[i].SetCurrentWeaponVisual(droppedVisual);
                }

                return true;
            }
        }
        return false;
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