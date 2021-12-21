using System.Collections;
using System.Collections.Generic;
using Map;
using UnityEngine;

[RequireComponent(typeof(MapGenerator))]
[RequireComponent(typeof(Spawner))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null; // A static reference to the GameManager instance

    public MapGenerator generator;
    public Spawner spawner;

    void Awake()
    {
        if(Instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            Instance = this;
        }
        else if (Instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
        
        generator = GetComponent<MapGenerator>();
        spawner = GetComponent<Spawner>();
    }

    void Start()
    {
        generator.Generate();
        spawner.StartSpawn();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (GameObject o in generator.generatedObjects)
            {
                Destroy(o);
            }

            generator.generatedObjects.Clear();
            generator.Generate();
        }

        spawner.UpdateSpawn();
    }
}
