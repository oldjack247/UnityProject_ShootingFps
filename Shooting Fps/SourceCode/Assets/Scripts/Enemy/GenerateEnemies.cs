using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateEnemies : MonoBehaviour
{
    [Header("References")]
    public GameObject theEnemy;
    public int xSpawnPos;
    public int zSpawnPos;
    public int enemyCount;
    public GameManager gameManager;


    private void Start()
    {
        StartCoroutine(EnemeyDrop());
    }

    private void Update()
    {

    }

    IEnumerator EnemeyDrop()
    {
        while (enemyCount < 10)
        {
            xSpawnPos = Random.Range(83, 190);
            zSpawnPos = Random.Range(-57, 50);
            Instantiate(theEnemy, new Vector3(xSpawnPos, 0.1f, zSpawnPos), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            enemyCount += 1;
        }
    }


}
