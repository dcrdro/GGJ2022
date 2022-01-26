using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastEnemy : MonoBehaviour
{
    public float Speed = 7f;
    public float RotateSpeed = 300f;
    public float Distance = 10;

    public GameObject HealOrb;

    public HaveHealth health;
    private Rigidbody _rb;
    private float _punchCooldown = 0;
    private float _scanCooldown = 0;

    private Vector3 LastSeen;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        health = GetComponent<HaveHealth>();
        health.Death += Death;
        health.Damaged += Damaged;
    }
    private void Death()
    {
        Instantiate(HealOrb, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    private void Damaged(float _)
    {
        _punchCooldown = 0.2f;
    }

    void Update()
    {
        if(_punchCooldown > 0) _punchCooldown -= Time.deltaTime;
        Ray SeePlayerRay = new Ray(transform.position, Player.Object.transform.position - transform.position);
        var Hit = new RaycastHit();
        if (Vector3.Distance(transform.position, Player.Object.transform.position) <= Distance && Physics.Raycast(SeePlayerRay, out Hit))
        {
            if (_scanCooldown > 0) _scanCooldown -= Time.deltaTime;
            if (Hit.collider.gameObject == Player.Object && _scanCooldown <= 0)
            {
                LastSeen = Hit.point;
                _scanCooldown = 0.5f;
            }
            RotateTowardLastSeen();
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

    private void RotateTowardLastSeen()
    {
        var look = Quaternion.LookRotation(LastSeen - transform.position);
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
