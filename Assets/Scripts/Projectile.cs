using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed;
    public LayerMask EnemyLayers;
    Rigidbody _rb;
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        _rb.velocity = transform.forward * Speed;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == EnemyLayers.value)
        {
            collision.gameObject.GetComponent<HaveHealth>()?.TakeDamage(10);
            Destroy(gameObject);
        }
    }
}
