using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Rigidbody _rb;
    private float _attackCooldown = 0;
    private float _punchCooldown = 0;

    private bool SeePlayer = false;
    private static List<DistanceEnemy> EnemiesThatSeePlayer = new List<DistanceEnemy>();
    private static Coroutine ExistShotCoroutine = null;
    private static IEnumerator ShotCoroutine()
    {
        Player.health.Death += PlayerDeath;
        DistanceEnemy LastShotDistanceEnemy = null;
        while (true)
        {
            if (EnemiesThatSeePlayer.Count > 0)
            {
                DistanceEnemy newEnemy;
                do
                {
                    newEnemy = EnemiesThatSeePlayer[Random.Range(0, EnemiesThatSeePlayer.Count)];
                } while (EnemiesThatSeePlayer.Count > 1 && newEnemy == LastShotDistanceEnemy);
                LastShotDistanceEnemy = newEnemy;
                LastShotDistanceEnemy.Shot();
                yield return new WaitForSeconds(0.6f);
            }
            else
            {
                yield return null;
            }
        }
    }
    private static void PlayerDeath()
    {
        EnemiesThatSeePlayer.Clear();
        ExistShotCoroutine = null;
    }
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        health = GetComponent<HaveHealth>();
        health.Death += Death;
        health.Damaged += Damaged;
        if (ExistShotCoroutine == null)
        {
            ExistShotCoroutine = StartCoroutine(ShotCoroutine());
        }
    }
    private void Death()
    {
        SeePlayer = false;
        EnemiesThatSeePlayer.Remove(this);
        Destroy(gameObject);
    }
    private void Damaged(float _)
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
                if (SeePlayer)
                {
                    SeePlayer = false;
                    EnemiesThatSeePlayer.Remove(this);
                }
                RotateToPlayer();
                MoveTowardPlayer();
            }
            else if (Vector3.Distance(transform.position, Player.Object.transform.position) <= MediumDistance && Vector3.Distance(transform.position, Player.Object.transform.position) >= MinDistance)
            {
                if (!SeePlayer)
                {
                    SeePlayer = true;
                    EnemiesThatSeePlayer.Add(this);
                }
                RotateToPlayer();
            }
            else if (Vector3.Distance(transform.position, Player.Object.transform.position) <= MinDistance && _punchCooldown <= 0)
            {
                if (SeePlayer)
                {
                    SeePlayer = false;
                    EnemiesThatSeePlayer.Remove(this);
                }
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
            var projectile = Instantiate(Projectile, AttackPoint.position, Model.transform.rotation).GetComponent<Projectile>();
            projectile.Damage = Damage;
            projectile.CollisionEnter += (sender, Damage, collision) =>
            {
                collision.gameObject.GetComponent<HaveHealth>()?.TakeDamage(Damage);
                Destroy(sender);
            };
            _attackCooldown = 1f;
        }
    }
}
