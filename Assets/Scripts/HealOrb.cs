using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOrb : MonoBehaviour
{
    void Update()
    {
        if(Player.OnLight)
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.Object.transform.position, 8 * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
