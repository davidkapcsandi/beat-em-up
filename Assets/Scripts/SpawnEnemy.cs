using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
 public GameObject enemyPrefab;
 public Transform[] spawnPoints;
 void Start()
 {
    foreach( var spawnPoint in spawnPoints)
    {
        enemySpawn(spawnPoint.position);
    }
 }

 void enemySpawn(Vector3 spawnPosition)
 {
      GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        BasicEnemy enemyScript = newEnemy.GetComponent<BasicEnemy>();
 
 }
}
