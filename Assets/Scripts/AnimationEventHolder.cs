﻿using UnityEngine;

// Должен висеть на том же объекте, что и аниматор
public class AnimationEventHolder : MonoBehaviour
{
    public DistanceEnemy DistanceEnemy;
    
    public void OnShotProjectile()
    {
        if (DistanceEnemy)
        {
            DistanceEnemy.ShotProjectile();
        }
    }        
}