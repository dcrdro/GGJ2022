using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEnemyCoordinator : MonoBehaviour
{
    public static List<DistanceEnemy> EnemiesThatSeePlayer = new List<DistanceEnemy>();
    public void Start()
    {
        StartCoroutine(ShotCoroutine());
    }
    public IEnumerator ShotCoroutine()
    {
        DistanceEnemy LastShotDistanceEnemy = null;
        while (true)
        {
            if (EnemiesThatSeePlayer.Count > 0)
            {
                DistanceEnemy newEnemy;
                do
                {
                    newEnemy = EnemiesThatSeePlayer[Random.Range(0, EnemiesThatSeePlayer.Count)];
                } while (EnemiesThatSeePlayer.Count > 1 && newEnemy == LastShotDistanceEnemy);
                LastShotDistanceEnemy = newEnemy;
                LastShotDistanceEnemy.Shot();
                yield return new WaitForSeconds(0.6f);
            }
            else
            {
                yield return null;
            }
        }
    }
}
