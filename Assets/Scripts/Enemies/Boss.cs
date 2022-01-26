using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public ActivateTrigger ActivateTrigger;
    public HaveHealth health;
    private Rigidbody _rb;
    public event Action Attack;
    public MeshRenderer DefendIndicator;
    public float WaitSeconds = 4;

    private void Start()
    {
        LineRay.enabled = false;
        health = GetComponent<HaveHealth>();
        health.Death += Death;
        health.Damaged += Damaged;
        ActivateTrigger.Activated += SpecialAttack;
        Attack += SpecialAttack;
        _rb = GetComponent<Rigidbody>();
    }
    private void Death()
    {
        BladePedestal.ActivatedCount++;
        Destroy(gameObject);
    }
    
    public void SpecialAttack()
    {
        var rand = UnityEngine.Random.Range(0, 4);
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
            case 3:
                StartCoroutine(Immune());
                break;
        }

    }

    #region Dash Attack (0)
    public int DashVelocity = 6;
    private bool AttackBody = false;

    IEnumerator DashAttack()
    {
        AttackBody = true;
        for (int i = 0; i < 3; i++)
        {
            RotateTowardPlayer();
            Dash();
            yield return new WaitForSeconds(0.4f);
        }
        RotateAwayFromPlayer();
        Dash();
        AttackBody = false;
        yield return new WaitForSeconds(WaitSeconds);
        Attack?.Invoke();
    }
    private void Dash()
    {
        _rb.velocity = transform.forward * DashVelocity;
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
        if (AttackBody && other.gameObject == Player.Object)
        {
            Player.health.TakeDamage(10);
        }
    }
    #endregion

    #region Spike Attack (1)
    public Transform ProjectilePosition;
    public Transform CenterPosition;
    public GameObject BossProjectile;

    IEnumerator SpikeAttack()
    {
        transform.position = new Vector3(CenterPosition.position.x, CenterPosition.position.y + 3, CenterPosition.position.z);
        yield return new WaitForSeconds(1);
        for (int i = 0; i < 360 / 15 + 1; i++)
        {
            Instantiate(BossProjectile, ProjectilePosition.position, ProjectilePosition.rotation);
            transform.rotation = Quaternion.AngleAxis(i * 15, Vector3.up);
        }
        yield return new WaitForSeconds(WaitSeconds);
        Attack?.Invoke();
    }
    #endregion

    #region Death Ray Attack (2)
    public LineRenderer LineRay;
    private bool _playerOnLight;
    private bool _changedForm;

    private void RayToPlayer()
    {
        LineRay.SetPosition(0, transform.position);
        LineRay.SetPosition(1, Player.Object.transform.position);
    }
    private void Update()
    {
        if (ActivateTrigger.IsActivated)
        {
            RayToPlayer();
        }
        if (!_playerOnLight != Player.OnLight)
        {
            _changedForm = true;
            LineRay.enabled = false;
        }
    }
    IEnumerator DeathRay()
    {
        LineRay.enabled = true;
        _playerOnLight = Player.OnLight;
        _changedForm = false;
        while (!_changedForm)
        {
            Player.health.TakeDamage(5);
            yield return new WaitForSeconds(0.5f);
        }
        
        yield return new WaitForSeconds(WaitSeconds);
        Attack?.Invoke();
    }
    #endregion

    #region Immune Form (3)
    enum Form
    {
        None,
        Light,
        Dark
    }
    private Form ImmuneToForm = Form.None;
    private void Damaged(float Damage)
    {
        if ((ImmuneToForm == Form.Light && Player.OnLight) ||
            (ImmuneToForm == Form.Dark && !Player.OnLight))
        {
            health.TakeHeal(Damage);
            _rb.velocity = Vector3.zero;
        }
    }
    IEnumerator Immune()
    {
        DefendIndicator.enabled = true;
        switch (UnityEngine.Random.Range(0, 2))
        {
            case 0:
                ImmuneToForm = Form.Light;
                DefendIndicator.material = Player.PlayerComponent.LightMaterial;
                break;
            case 1:
                ImmuneToForm = Form.Dark;
                DefendIndicator.material = Player.PlayerComponent.DarkMaterial;
                break;
        }
        Attack?.Invoke();
        yield return new WaitForSeconds(15);
        ImmuneToForm = Form.None;
        DefendIndicator.enabled = false;
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
