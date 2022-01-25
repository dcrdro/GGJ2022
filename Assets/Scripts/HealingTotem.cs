using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingTotem : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == Player.Object)
        {
            Player.health.ToMax();
        }
    }
}
