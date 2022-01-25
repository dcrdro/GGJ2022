using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyBlade : MonoBehaviour
{
    public Transform Postition;
    public float FlySpeed = 10f;
    public float FlyRotationSpeed = 300f;

    private void Update()
    {
        if (Postition != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, Postition.position, FlySpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Postition.rotation, FlyRotationSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Collider>().enabled = false;
        if (other.gameObject == Player.Object)
        {
            Postition = Player.PlayerComponent.GetEmptyKeyPosition(this);
        }
        else GetComponent<Collider>().enabled = true;
    }
}
