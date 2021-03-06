using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FastEnemy : MonoBehaviour
{
    public float Speed = 7f;
    public float RotateSpeed = 300f;
    public float Distance = 10;

    public HaveHealth health;
    private Rigidbody _rb;
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _punchCooldown = 0;
    private float _scanCooldown = 0;
    private float _runOffTimer = 0;

    private Vector3 LastSeen;

    public event Action Attacked;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        
        health = GetComponent<HaveHealth>();
        health.Death += Death;
        health.Damaged += Damaged;
        Attacked += WhenAttacked;

        _agent.speed = Speed;
    }
    private void Death()
    {
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
            if (_punchCooldown <= 0 && _runOffTimer <= 0)
            {
                _animator.SetTrigger("Move");
                RotateTowardLastSeen();
                MoveTo(LastSeen);
            }
        }
    }

    private void Move()
    {
        _agent.isStopped = true;
        _rb.isKinematic = false;
        _rb.velocity = transform.forward * Speed;
    }
    
    private void MoveTo(Vector3 target)
    {
        _agent.isStopped = false;
        _rb.isKinematic = true;
        _agent.SetDestination(target);
    }

    private void RotateTowardLastSeen()
    {
        var look = Quaternion.LookRotation(LastSeen - transform.position);
        look.x = 0;
        look.z = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, RotateSpeed * Time.deltaTime);
    }
    private void RotateAwayFromPlayer()
    {
        var look = Quaternion.Inverse(Quaternion.LookRotation(Player.Object.transform.position - transform.position));
        look.x = 0;
        look.z = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, RotateSpeed * Time.deltaTime); ;
    }

    private void WhenAttacked()
    {
        StartCoroutine(RunOff());
    }

    IEnumerator RunOff()
    {
        yield return new WaitForSeconds(0.4f);
        _runOffTimer = 1;
        while (_runOffTimer > 0)
        {
            RotateAwayFromPlayer();
            Move();
            _runOffTimer -= Time.deltaTime;
            yield return null;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == Player.Object)
        {
            _animator.SetTrigger("Attack");
        }
    }

    public void EndAttack()
    {
        Player.health.TakeDamage(10);
        Attacked?.Invoke();
    }
}
