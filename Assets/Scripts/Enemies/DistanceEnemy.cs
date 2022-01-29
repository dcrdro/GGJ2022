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
    public Animator Animator;

    private Rigidbody _rb;
    private float _punchCooldown = 0;

    private bool SeePlayer = false;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        health = GetComponent<HaveHealth>();
        health.Death += Death;
        health.Damaged += Damaged;
    }
    private void Death()
    {
        SeePlayer = false;
        DistanceEnemyCoordinator.EnemiesThatSeePlayer.Remove(this);
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
        if (Physics.Raycast(SeePlayerRay, out Hit) && Hit.collider.gameObject == Player.Object && Vector3.Distance(transform.position, Player.Object.transform.position) <= MaximumDistance)
        {
            if (Vector3.Distance(transform.position, Player.Object.transform.position) <= MediumDistance)
            {
                Animator.SetBool("Approach", false);
                if (!SeePlayer)
                {
                    SeePlayer = true;
                    DistanceEnemyCoordinator.EnemiesThatSeePlayer.Add(this);
                }
                if (Vector3.Distance(transform.position, Player.Object.transform.position) <= MinDistance && _punchCooldown <= 0)
                {
                    MoveAwayFormPlayer();
                }
                RotateToPlayer();
            }
            else if (Vector3.Distance(transform.position, Player.Object.transform.position) >= MediumDistance && _punchCooldown <= 0)
            {
                if (SeePlayer)
                {
                    SeePlayer = false;
                    DistanceEnemyCoordinator.EnemiesThatSeePlayer.Remove(this);
                }
                Animator.SetBool("Approach", true);
                RotateToPlayer();
                MoveTowardPlayer();
            }
        }
        Animator.SetBool("Approach", false);
    }
    private void RotateToPlayer()
    {
        var look = Quaternion.LookRotation(Player.Object.transform.position - transform.position);
        look.x = 0;
        look.z = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, look, RotateSpeed * Time.deltaTime);
    }
    //private void RotateAwayFromPlayer()
    //{
    //    var look = Quaternion.Inverse(Quaternion.LookRotation(Player.Object.transform.position - transform.position));
    //    look.x = 0;
    //    look.z = 0;
    //    transform.rotation = Quaternion.RotateTowards(transform.rotation, look, RotateSpeed * Time.deltaTime);
    //}
    private void MoveTowardPlayer()
    {
        _rb.velocity = (Player.Object.transform.position - transform.position).normalized * Speed;
    }
    private void MoveAwayFormPlayer()
    {
        _rb.velocity = (Player.Object.transform.position - transform.position).normalized * -Speed;
    }
    public IEnumerator Shot()
    {
        if (Random.Range(0, 2) == 0)
        {
            Animator.SetTrigger("Right");
        }
        else
        {
            Animator.SetTrigger("Left");
        }
        yield return new WaitForSeconds(0.35f);
        var projectile = Instantiate(Projectile, AttackPoint.position, Model.transform.rotation).GetComponent<Projectile>();
        projectile.Damage = Damage;
        projectile.CollisionEnter += (sender, Damage, collision) =>
        {
            collision.gameObject.GetComponent<HaveHealth>()?.TakeDamage(Damage);
            Destroy(sender);
        };
    }
}
