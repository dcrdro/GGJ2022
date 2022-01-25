using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool CanReflect = true;
    public float Speed;
    public float Damage;
    private bool reflected = false;
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
        collision.gameObject.GetComponent<HaveHealth>()?.TakeDamage(Damage);
        Destroy(gameObject);
    }
    public void Reflect(Quaternion Rotation)
    {
        if(CanReflect && !reflected)
        {
            transform.rotation = Rotation;
            reflected = true;
        }
    }
}
