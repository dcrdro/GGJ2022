using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public ActivateTrigger ActivateTrigger;
    public HaveHealth health;
    private Rigidbody _rb;
    private Collider _collider;
    public LayerMask ObstacleLayers;

    private void Start()
    {
        LineRay.enabled = false;
        health = GetComponent<HaveHealth>();
        health.Death += Death;
        ActivateTrigger.Activated += Activated;
        Attack += Wait;
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }
    private void Death()
    {
        BladePedestal.ActivatedCount++;
        Destroy(gameObject);
    }
    public void Activated()
    {
        Player.PlayerComponent.Boss = gameObject;
        StartCoroutine(SpecialAttack());
    }

    IEnumerator SpecialAttack()
    {
        var rand = UnityEngine.Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                StartCoroutine(DashAttack());
                break;
            case 1:
                StartCoroutine(SpikeAttack());
                break;
            case 2:
                StartCoroutine(DeathRay());
                break;
        }
        yield return null;
    }

    #region Circle Walk
    public float RotationSpeed = 300;
    public float WalkSpeed = 4;
    public float WaitSeconds = 4;
    public event Action Attack;

    void Wait()
    {
        StartCoroutine(CircleWalk());
    }
    IEnumerator CircleWalk()
    {
        var clockwise = UnityEngine.Random.Range(0, 2);
        var Timer = WaitSeconds;
        while (Timer > 0)
        {
            RotateTowardPlayer();
            _rb.velocity =  -transform.right * WalkSpeed * (clockwise == 0 ? -1 : 1);
            Timer -= Time.deltaTime;
            yield return null;
        }
        StartCoroutine(SpecialAttack());
    }
    #endregion

    #region Dash Attack (0)
    public Transform RaycastPosition;
    public Transform Crown;
    public float CrownRotationSpeed = 5;
    public int DashVelocity = 6;
    private bool AttackBody = false;
    private bool _rotateCrown = false;

    IEnumerator DashAttack()
    {
        _rotateCrown = true;
        StartCoroutine(RotateCrown());
        yield return new WaitForSeconds(1);
        _rotateCrown = false;
        AttackBody = true;
        for (int i = 0; i < 3; i++)
        {
            RotateTowardPlayer();
            var cor = StartCoroutine(Dash());
            yield return new WaitForSeconds(1f);
            if (cor!= null)
            {
                StopCoroutine(cor);
                _rb.constraints = RigidbodyConstraints.FreezeRotation;
                _collider.isTrigger = false;
            }
            
        }
        RotateAwayFromPlayer();
        StartCoroutine(Dash());
        AttackBody = false;
        Attack?.Invoke();
    }
    private IEnumerator Dash()
    {
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        _collider.isTrigger = true;
        var ray = new Ray(RaycastPosition.position, new Vector3(Player.Object.transform.position.x - RaycastPosition.position.x, 0, Player.Object.transform.position.z - RaycastPosition.position.z));
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 1);
        var hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, 40, ObstacleLayers.value))
        {
            Debug.Log(hit.collider.gameObject.name);
            Vector3 DashToPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Debug.DrawLine(transform.position, DashToPosition, Color.magenta, 0.4f);
            while (Vector3.Distance(transform.position, DashToPosition) >= 2f)
            {
                DashToPosition.y = transform.position.y;
                transform.position = Vector3.MoveTowards(transform.position, DashToPosition, DashVelocity * Time.deltaTime);
                yield return null;
            }
        }
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
        _collider.isTrigger = false;
    }
    private IEnumerator RotateCrown()
    {
        float angle = 0;
        while (_rotateCrown)
        {
            angle += CrownRotationSpeed * Time.deltaTime;
            Crown.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            yield return null;
        }
    }
    private void RotateTowardPlayer()
    {
        RotateTowardPosition(Player.Object.transform.position);
    }
    private void RotateAwayFromPlayer()
    {
        var look = Quaternion.Inverse(Quaternion.LookRotation(Player.Object.transform.position - transform.position));
        look.x = 0;
        look.z = 0;
        transform.rotation = look;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (AttackBody && other.gameObject == Player.Object && Player.PlayerComponent._dashCooldown <= 0.1f)
        {
            Player.health.TakeDamage(5);
        }
    }
    #endregion

    #region Spike Attack (1)
    public Transform ProjectilePosition;
    public Transform CenterPosition;
    public GameObject BossProjectile;

    IEnumerator SpikeAttack()
    {
        _rb.velocity = Vector3.zero;
        transform.position = new Vector3(CenterPosition.position.x, CenterPosition.position.y + 3, CenterPosition.position.z);
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 360 / 15 + 1; i++)
        {
            var projectile = Instantiate(BossProjectile, ProjectilePosition.position, ProjectilePosition.rotation).GetComponent<Projectile>();
            projectile.TriggerEnter += (sender, _, Collider) =>
            {
                if (Collider.gameObject.layer == ObstacleLayers.value)
                {
                    Destroy(sender);
                }
            };
            transform.rotation = Quaternion.AngleAxis(i * 15, Vector3.up);
        }
        yield return new WaitForSeconds(1);
        Attack?.Invoke();
    }
    #endregion

    #region Death Ray Attack (2)
    public LineRenderer LineRay;

    private void RayToPlayer()
    {
        LineRay.SetPosition(0, transform.position);
        LineRay.SetPosition(1, Player.Object.transform.position);
    }
    IEnumerator DeathRay()
    {
        float Timer = 0.5f;
        LineRay.enabled = true;
        var _playerOnLight = Player.OnLight;
        while (!(_playerOnLight != Player.OnLight))
        {
            if (Timer <= 0)
            {
                Player.health.TakeDamage(2.5f);
                Timer = 0.5f;
            }
            Timer -= Time.deltaTime;
            RayToPlayer();
            yield return null;
        }
        LineRay.enabled = false;
        Attack?.Invoke();
    }
    #endregion

    private void RotateTowardPosition(Vector3 position)
    {
        var look = Quaternion.LookRotation(position - transform.position);
        look.x = 0;
        look.z = 0;
        transform.rotation = look;
    }
}
