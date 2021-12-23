using System;
using System.Collections;
using System.Collections.Generic;
using Map;
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
    
    void Awake()
    {
        var table1 = new ChanceTable(new uint[] {10, 5, 0});
        var table2 = new ChanceTable(new uint[] {7, 7, 2});
        var table3 = new ChanceTable(new uint[] {10, 10, 5});
        
        waves = new[]
        {
            new Wave(table1, 0.5f),
            new Wave(table1, 0.4f),
            new Wave(table1, 0.3f),
            new Wave(table1, 0.2f),
            new Wave(table1, 0.1f),

            new Wave(table2, 0.5f),
            new Wave(table2, 0.4f),
            new Wave(table2, 0.3f),
            new Wave(table2, 0.2f),
            new Wave(table2, 0.1f),

            new Wave(table3, 0.5f),
            new Wave(table3, 0.4f),
            new Wave(table3, 0.3f),
            new Wave(table3, 0.2f),
            new Wave(table3, 0.1f),
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
            int angle;
            GameObject d;
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
        
        Wave wave = GetWave();
        for (int i = 0; i < MaxNumberOfEnemies(); i++)
        {
            Destroy(SpawnEnemy(i, wave), 4f);
            yield return new WaitForSeconds( wave.delay );
        }

        spawningWave = false;
    }

    Wave GetWave()
    {
        if (waveInd >= waves.Length)
            return waves[waves.Length - 1];
        
        return waves[waveInd];
    }

    GameObject SpawnEnemy(int ind, Wave wave)
    {
        Vector3 pos = enemySpawnPositions[Random.Range(0, enemySpawnPositions.Length)];

        return Instantiate(wave.table.GetRandomItem(ref enemies), pos, Quaternion.identity);
    }
    
    float MaxNumberOfEnemies()
    {
        return difficultyCurve.GetDifficulty(waveInd);
    }
}
