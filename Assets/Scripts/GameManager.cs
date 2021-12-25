using System;
using System.Collections;
using System.Collections.Generic;
using Map;
using UnityEngine;

// [RequireComponent(typeof(MapGenerator))]
[RequireComponent(typeof(Spawner))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null; // A static reference to the GameManager instance

    public MapGenerator[] generators;

    public MapGenerator generator
    {
        get { return generators[a]; }
    }

    public Spawner spawner;
    private PlayerMovement movement;

    public bool generate;
    public int a = 0;
    
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
        
        // generator = GetComponent<MapGenerator>();
        spawner = GetComponent<Spawner>();
        movement = spawner.mainCharacter.GetComponent<PlayerMovement>();
    }

    void Start()
    {
        generator.Generate();
        spawner.StartSpawn();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || generate)
        {
            generate = false;

            foreach (MapGenerator mapGenerator in generators)
            {
                foreach (GameObject o in mapGenerator.generatedObjects)
                {
                    Destroy(o);
                }
                mapGenerator.generatedObjects.Clear();
            }

            generator.Generate();
            spawner.StartSpawn();
        }
        else
        {
            spawner.UpdateSpawn();
        }
    }

    private void FixedUpdate()
    {
        movement.UpdatePlayer();
    }
}
