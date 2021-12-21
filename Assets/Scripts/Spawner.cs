using System.Collections;
using System.Collections.Generic;
using Map;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class Spawner : MonoBehaviour
{
    public GameObject mainCharacter;

    public void StartSpawn()
    {
        SpawnMainCharacter();
        GetEnemySpawnPoints();
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

    void GetEnemySpawnPoints()
    {
        
    }
    
    public void UpdateSpawn()
    {
        
    }
}
