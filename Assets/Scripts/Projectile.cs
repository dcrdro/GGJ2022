using System;
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

    public event Action<GameObject, float, Collision> CollisionEnter;
    public event Action<GameObject, float, Collider> TriggerEnter;

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
        CollisionEnter?.Invoke(gameObject, Damage, collision);
    }
    private void OnTriggerEnter(Collider other)
    {
        TriggerEnter?.Invoke(gameObject, Damage, other);
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
