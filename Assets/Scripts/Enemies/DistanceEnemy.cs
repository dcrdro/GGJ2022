using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEnemy : MonoBehaviour
{
    public float Speed = 2;
    public float RotateSpeed = 300;
    public float Damage = 10;

    public float MinDistance = 5;
    public float MediumDistance = 15;
    public float MaximumDistance = 30;

    public HaveHealth health;
    public GameObject Projectile;
    public Transform AttackPoint;
    public GameObject Model;
    public GameObject HealOrb;

    private Rigidbody _rb;
    private float _attackCooldown = 0;
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
        Instantiate(HealOrb, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    private void Damaged()
    {
        _punchCooldown = 0.2f;
    }

    void Update()
    {
        if (_punchCooldown > 0) _punchCooldown -= Time.deltaTime;
        Ray SeePlayerRay = new Ray(transform.position, Player.Object.transform.position - transform.position);
        var Hit = new RaycastHit();
        if (Physics.Raycast(SeePlayerRay, out Hit) && Hit.collider.gameObject == Player.Object)
        {
            if (Vector3.Distance(transform.position, Player.Object.transform.position) <= MaximumDistance && Vector3.Distance(transform.position, Player.Object.transform.position) >= MediumDistance && _punchCooldown <= 0) 
            {
                RotateToPlayer();
                MoveTowardPlayer();
            }
            else if (Vector3.Distance(transform.position, Player.Object.transform.position) <= MediumDistance && Vector3.Distance(transform.position, Player.Object.transform.position) >= MinDistance)
            {
                RotateToPlayer();
                Shot();
            }
            else if (Vector3.Distance(transform.position, Player.Object.transform.position) <= MinDistance && _punchCooldown <= 0)
            {
                RotateAwayFromPlayer();
                MoveAwayFormPlayer();
            }
        }
        if (_attackCooldown > 0) _attackCooldown -= Time.deltaTime;
    }
    private void RotateToPlayer()
    {
        var look = Quaternion.LookRotation(Player.Object.transform.position - transform.position);
        look.x = 0;
        look.z = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, RotateSpeed * Time.deltaTime);
    }
    private void RotateAwayFromPlayer()
    {
        var look = Quaternion.Inverse(Quaternion.LookRotation(Player.Object.transform.position - transform.position));
        look.x = 0;
        look.z = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, RotateSpeed * Time.deltaTime);
    }
    private void MoveTowardPlayer()
    {
        _rb.velocity = (Player.Object.transform.position - transform.position).normalized * Speed;
    }
    private void MoveAwayFormPlayer()
    {
        _rb.velocity = (Player.Object.transform.position - transform.position).normalized * -Speed;
    }
    private void Shot()
    {
        if (_attackCooldown <= 0)
        {
            Instantiate(Projectile, AttackPoint.position, Model.transform.rotation).GetComponent<Projectile>().Damage = Damage;
            _attackCooldown = 1f;
        }
    }
}
