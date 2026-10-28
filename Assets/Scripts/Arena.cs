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

    void Start()
    {
        SpawnTeams();
    }

    private void SpawnTeams()
    {
        for (int i = 0; i < heroPrefabs.Count; i++)
        {
            int col = i % 2;
            float xOffset = col * 2.5f;
            float zOffset = i * 4f;

            Vector3 spawnPos = heroSpawnPoint.position + new Vector3(-xOffset, 0, zOffset);
            UnitController heroInstance = Instantiate(heroPrefabs[i], spawnPos, heroSpawnPoint.rotation);

            Unit heroData = new Unit("Hero " + i, 100, 3, 0.3f, new Weapon("Sword", 10, ElementType.Fire, 10));
            heroInstance.Setup(heroData);
            activeHeroes.Add(heroInstance);
        }

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            int col = i % 2;
            float xOffset = col * 2.5f;
            float zOffset = i * 4f;

            Vector3 spawnPos = enemySpawnPoint.position + new Vector3(xOffset, 0, zOffset);
            UnitController enemyInstance = Instantiate(enemyPrefabs[i], spawnPos, enemySpawnPoint.rotation);

            if (enemyPrefabs[i].name.Contains("Goblin"))
            {
                enemyInstance.transform.Rotate(0, 180, 0);
            }

            Unit enemyData = new Unit("Enemy " + i, 100, 2, 0.1f, new Weapon("Club", 8, ElementType.None, 5));
            enemyInstance.Setup(enemyData);
            activeEnemies.Add(enemyInstance);
        }
    }
}