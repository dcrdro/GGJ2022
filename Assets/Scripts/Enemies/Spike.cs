using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    private Vector3 StartPosition;
    public float Lifetime;
    private float Timer = -1;
    void Start()
    {
        StartPosition = transform.position;
    }
    private void Update()
    {
        transform.position = new Vector3(StartPosition.x, StartPosition.y + (-Mathf.Abs(Timer)+1), StartPosition.z);
        Timer += Time.deltaTime / Lifetime ;
        if (Timer >= Lifetime) Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player.Object && Player.PlayerComponent._dashCooldown <= 0.1)
        {
            Player.health.TakeDamage(5);
        }
    }
}
