using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static GameObject Object;
    public static HaveHealth health;

    public GameObject Model;
    public Transform AttackPoint;
    public GameObject AttackIndicator;
    public GameObject Projectile;

    public LayerMask EnemyLayers;
    public float AttackRange;
    public float Speed = 100f;
    public float RotationSpeed = 420f;

    public bool OnLight => LightSourse.RaysCount > 0;

    private Rigidbody _rb;
    private float _attackCooldown = 0;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Object = gameObject;
        if (health == null)
        {
            health = gameObject.AddComponent<HaveHealth>();
            health.MaxHealth = 100;
        }
        health.Death += Death;
    }
    private void Death()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        var space = Input.GetKey(KeyCode.Space);

        Timer();
        Attack();
        Move(vertical, horizontal);
        RotateTowardMoveDir(vertical, horizontal);
    }

    private void Timer()
    {
        if (_attackCooldown > 0) _attackCooldown -= Time.deltaTime;
        else if (_attackCooldown <= 0.2) AttackIndicator.SetActive(false);
    }

    private void Move(float vertical, float horizontal)
    {
        var direction = new Vector2(horizontal * Speed, vertical * Speed);

        _rb.velocity = new Vector3(direction.x, _rb.velocity.y, direction.y);
    }
    private void RotateTowardMoveDir(float vertical, float horizontal)
    {
        if (horizontal != 0 || vertical != 0)
        {
            float angle = Mathf.Atan2(horizontal * Speed, vertical * Speed) * Mathf.Rad2Deg;
            Quaternion smooth = Quaternion.Euler(0, angle, 0);
            Model.transform.rotation = Quaternion.RotateTowards(Model.transform.rotation, smooth, RotationSpeed * Time.deltaTime);
        }
    }
    private void Attack()
    {
        if (Input.GetMouseButton(0) && _attackCooldown<=0)
        {
            if (OnLight)
            {
                Instantiate(Projectile, AttackPoint.position, Model.transform.rotation);
                _attackCooldown = 0.5f;
            }
            else
            {
                AttackIndicator.SetActive(true);
                Collider[] HitEnemies = Physics.OverlapSphere(AttackPoint.position, AttackRange, EnemyLayers);
                foreach (var Enemy in HitEnemies)
                {
                    //Debug.Log($"EnemyDetected {Enemy.gameObject.name}");
                    Enemy.GetComponent<HaveHealth>()?.TakeDamage(10);
                }
                _attackCooldown = 0.5f;
            }
        }
    }
}
