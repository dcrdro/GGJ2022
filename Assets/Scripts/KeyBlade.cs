using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBlade : MonoBehaviour
{
    public Transform Postition;
    public float FlySpeed = 10f;
    public float FlyRotationSpeed = 300f;

    private void Update()
    {
        if (Postition != null)
        {
            GetComponent<Collider>().enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, Postition.position, FlySpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Postition.rotation, FlyRotationSpeed * Time.deltaTime);
        }
        else GetComponent<Collider>().enabled = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == Player.Object)
        {
            Postition = Player.PlayerComponent.GetEmptyKeyPosition(this);
        }
    }
}
