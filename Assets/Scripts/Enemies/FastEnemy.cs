using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastEnemy : MonoBehaviour
{
    public float Speed = 7f;
    public float RotateSpeed = 300f;
    public float Distance = 10;

    public HaveHealth health;
    private Rigidbody _rb;
    private float _punchCooldown = 0;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        health = GetComponent<HaveHealth>();
        health.Death += Death;
        health.Damaged += Damaged;
    }
    private void Death()
    {
        Destroy(gameObject);
    }
    private void Damaged()
    {
        _punchCooldown = 0.2f;
    }

    void Update()
    {
        if(_punchCooldown > 0) _punchCooldown -= Time.deltaTime;
        if (Vector3.Distance(transform.position, Player.Object.transform.position) <= Distance)
        {
            RotateTowardPlayer();
            if (_punchCooldown <= 0)
            {
                Move();
            }
        }
    }

    private void Move()
    {
        _rb.velocity = transform.forward * Speed;
    }

    private void RotateTowardPlayer()
    {
        var look = Quaternion.LookRotation(Player.Object.transform.position - transform.position);
        look.x = 0;
        look.z = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, RotateSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == Player.Object)
        {
            Player.health.TakeDamage(10);
        }
    }
}
