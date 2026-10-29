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

            if (!isHero && prefabs[i].name.ToLower().Contains("goblin"))
            {
                instance.transform.Rotate(0, 180, 0);
            }

            string unitName = (isHero ? "Hero " : "Enemy ") + i + " (" + prefabs[i].name + ")";
            Weapon startWeapon = GetWeaponForCharacter(prefabs[i].name);
            GameObject startWeaponVisual = GetWeaponPrefabForCharacter(prefabs[i].name);

            Unit unitData = new Unit(unitName, 100, isHero ? 3 : 2, isHero ? 0.3f : 0.1f, startWeapon);

            instance.Setup(unitData, startWeaponVisual);
            activeList.Add(instance);
        }
    }

    private Weapon GetWeaponForCharacter(string prefabName)
    {
        string name = prefabName.ToLower();

        if (name.Contains("goblin")) return new Weapon("Sword", 15, ElementType.None, 6);
        if (name.Contains("orc")) return new Weapon("Axe", 25, ElementType.None, 9);
        if (name.Contains("troll") || name.Contains("foe")) return new Weapon("Claws", 10, ElementType.None, 0);
        if (name.Contains("wizard") || name.Contains("mage")) return new Weapon("Staff", 15, ElementType.Fire, 8);
        if (name.Contains("davi") || name.Contains("warrior")) return new Weapon("Sword", 35, ElementType.Fire, 10);

        return new Weapon("Fists", 5, ElementType.None, 0);
    }

    private GameObject GetWeaponPrefabForCharacter(string prefabName)
    {
        string name = prefabName.ToLower();

        if (name.Contains("goblin")) return goblinSwordPrefab;
        if (name.Contains("orc")) return orcAxePrefab;
        if (name.Contains("davi") || name.Contains("warrior")) return warriorSwordPrefab;

        return null;
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
        string deadName = deadUnit.UnitData.Name.ToLower();

        if (deadName.Contains("mage") || deadName.Contains("wizard") || deadName.Contains("troll") || deadName.Contains("foe")) return;

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
            string name = team[i].UnitData.Name.ToLower();

            if (name.Contains("mage") || name.Contains("wizard") || name.Contains("troll") || name.Contains("foe")) continue;

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