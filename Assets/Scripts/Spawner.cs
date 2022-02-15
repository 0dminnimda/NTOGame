using System;
using System.Collections;
using System.Collections.Generic;
using Map;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(GameManager))]
public class Spawner : MonoBehaviour
{
    public GameObject mainCharacter;
    
    public float minDist;
    private Vector3[] enemySpawnPositions;

    // private ChanceTable enemyTable = new ChanceTable();
    // private uint[] enemySpawnWeights;
    // public uint[] enemySpawnWeights
    // {
    //     get => _enemySpawnWeights;
    //     set
    //     {
    //         enemyTable.SetTable(value);
    //         _enemySpawnWeights = value;
    //     }
    // }

    public GameObject[] enemies;
    public List<GameObject> spawnedEnemies = new List<GameObject>();

    public Difficulty difficultyCurve;

    public class Wave
    {
        public ChanceTable table;
        public float delay;

        public Wave(ChanceTable table, float delay)
        {
            this.table = table;
            this.delay = delay;
        }
    }

    private Wave[] waves;

    public float timeBeforeFirstWave = 15f;
    public float timeBetweenWaves = 10f;
    public float waveCountdown;
    private bool spawningWave = false;
    public int waveInd;

    public Vector3 enemyGoal;

    void Awake()
    {
        var table1 = new ChanceTable(new uint[] {10, 3,  0,  0,  0});
        var table2 = new ChanceTable(new uint[] {7,  7,  3,  0,  0});
        var table3 = new ChanceTable(new uint[] {5,  10, 5,  3,  0});
        var table4 = new ChanceTable(new uint[] {3,  7,  7,  5,  3});
        var table5 = new ChanceTable(new uint[] {0,  5,  10, 7,  5});
        var table6 = new ChanceTable(new uint[] {0,  3,  7,  10, 7});
        var table7 = new ChanceTable(new uint[] {0,  0,  5,  7,  10});
        
        waves = new[]
        {
            // new Wave(table1, 0.3f),
            // new Wave(table1, 0.2f),
            new Wave(table1, 0.1f),

            // new Wave(table2, 0.3f),
            // new Wave(table2, 0.2f),
            new Wave(table2, 0.1f),

            // new Wave(table3, 0.3f),
            // new Wave(table3, 0.2f),
            new Wave(table3, 0.1f),
            
            // new Wave(table4, 0.3f),
            // new Wave(table4, 0.2f),
            new Wave(table4, 0.1f),
            
            // new Wave(table5, 0.3f),
            // new Wave(table5, 0.2f),
            new Wave(table5, 0.1f),
            
            // new Wave(table6, 0.3f),
            // new Wave(table6, 0.2f),
            new Wave(table6, 0.1f),
            
            // new Wave(table7, 0.3f),
            // new Wave(table7, 0.2f),
            new Wave(table7, 0.1f),
        };
    }
    
    public void StartSpawn()
    {
        SpawnMainCharacter();
        PrepareEnemySpawnPositions();
    }

    void SpawnMainCharacter()
    {
        var position = GameManager.Instance.generator.generatedAreas[0].coordinates.ToVector3();
        position.y = 100;
        
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit))
        {
            mainCharacter.transform.position = hit.point + Vector3.up;
        }
        else
        {
            throw new System.Exception(
                "Nowhere to place main character : "); 
        }
        mainCharacter.SetActive(true);
    }

    void PrepareEnemySpawnPositions()
    {
        var positions = new List<Vector3>();
        MapGenerator generator = GameManager.Instance.generator;

        foreach (var area in generator.generatedAreas)
        {
            if (area.Type() == AreaType.One)
            {
                Vector3 position = area.coordinates.ToVector3();
                if ((transform.position - position).sqrMagnitude >= minDist)
                {
                    RaycastHit hit;
                    position.y = 100;
                    if (Physics.Raycast(position, Vector3.down, out hit))
                    {
                        positions.Add(hit.point + Vector3.up);
                    }
                    else
                    {
                        throw new System.Exception(
                            String.Format("Nowhere to place enemy on area '{0}': ", area)); 
                    }
                }
            }
        }

        enemySpawnPositions = positions.ToArray();
        waveInd = 0;
        waveCountdown = timeBeforeFirstWave;

        NavMeshSurface surface = generator.paternt.GetComponent<NavMeshSurface>();

        // var table = new uint[enemies.Length];
        // for (var i = 0; i < enemies.Length; i++)
        // {
        //     Enemy enemy = enemies[i].GetComponent<Enemy>();
        //     if (enemy == null)
        //         table[i] = 0;
        //     else
        //         table[i] = enemy.spawnWeight;
        // }
        // enemyTable.SetTable(table);
    }
    
    public void UpdateSpawn()
    {
        HandleWave();
    }

    void HandleWave()
    {
        if (waveCountdown <= 0)
        {
            if (!spawningWave)
            {
                StartCoroutine(SpawnEnemies());
                waveInd++;
                waveCountdown = timeBetweenWaves;
            }
        }
        else
            waveCountdown -= Time.deltaTime;
    }

    IEnumerator SpawnEnemies()
    {
        spawningWave = true;

        RemoveDestroyedEnemies();

        Wave wave = GetWave();
        for (int i = 0; i < MaxNumberOfEnemies(); i++)
        {
            GameObject o = SpawnEnemy(i, wave);
            o.GetComponent<Enemy>().MoveToLocation(enemyGoal);
            // Destroy(o, 4f);
            yield return new WaitForSeconds( wave.delay );
        }

        RemoveDestroyedEnemies();

        spawningWave = false;
    }

    public GameObject toOff;
    public GameObject toOn;

    Wave GetWave()
    {
        if (waveInd >= waves.Length)
        {
            toOff.SetActive(false);
            toOn.SetActive(true);
            Time.timeScale = 0;
            return waves[waves.Length - 1];
        }

        return waves[waveInd];
    }

    public GameObject GetClosestEnemy(Vector3 point)
    {
        RemoveDestroyedEnemies();

        // Vector3 nearestPosition = Vector3.zero;
        GameObject nearest = null;
        float nearestDistance = float.PositiveInfinity;
        int instancesCount = spawnedEnemies.Count;
        int i = 0;
        // if (instancesCount > 0)
        // {
        //     nearest = spawnedEnemies[0];
        //     nearestPosition = nearest._transform.position;
        //     nearestDistance = Vector3.Distance(point, nearestPosition);
        //     i = 1;
        // }
        for (; i < instancesCount; i++)
        {
            GameObject next = spawnedEnemies[i];
            Vector3 nextPosition = next.transform.position;
            float dist = (point - nextPosition).sqrMagnitude;
            // float dist = Vector3.Distance(point, nextPosition);
            if (dist < nearestDistance)
            {
                nearest = next;
                // nearestPosition = next.transform.position;
                nearestDistance = dist;
            }
        }

        return nearest;
    }

    void RemoveDestroyedEnemies()
    {
        spawnedEnemies.RemoveAll(s => s == null);
    }

    GameObject SpawnEnemy(int ind, Wave wave)
    {
        Vector3 pos = enemySpawnPositions[Random.Range(0, enemySpawnPositions.Length)];

        var enemy = Instantiate(wave.table.GetRandomItem(ref enemies), pos, Quaternion.identity);
        spawnedEnemies.Add(enemy);

        return enemy;
    }

    float MaxNumberOfEnemies()
    {
        return difficultyCurve.GetDifficulty(waveInd);
    }
}
