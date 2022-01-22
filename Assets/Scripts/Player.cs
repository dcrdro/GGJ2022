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

    public Material DarkMaterial;
    public Material LightMaterial;

    public LayerMask EnemyLayers;
    public float AttackRange;
    public float Speed = 100f;
    public float RotationSpeed = 420f;
    public float DashModifier = 5f;

    private bool _onLight;
    public bool OnLight => LightSourse.RaysCount > 0;

    public event Action LightsChanged;

    private Rigidbody _rb;
    private float _attackCooldown = 0;
    private float _dashCooldown = 0;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Object = gameObject;
        if (health == null)
        {
            health = gameObject.AddComponent<HaveHealth>();
            health.MaxHealth = 100;
            health.ToMax();
        }
        health.Death += Death;
        LightsChanged += ChangeVisualLightForm;
    }
    private void ChangeVisualLightForm()
    {
        var mesh = Model.GetComponent<MeshRenderer>();
        if (OnLight) mesh.material = LightMaterial;
        else mesh.material = DarkMaterial;
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

        if (_onLight != OnLight)
        {
            LightsChanged?.Invoke();
        }
        _onLight = OnLight;

        Timer();

        Attack();

        if (Dash()) { }
        else if (_attackCooldown <= 0)
        {
            Move(vertical, horizontal);
            RotateTowardMoveDir(vertical, horizontal);
        }
        else if (_dashCooldown <= 0)
        {
            Speed /= 2;
            Move(vertical, horizontal);
            Speed *= 2;
        }
        AttackIndicator.transform.localRotation = Quaternion.AngleAxis(-180 * (_attackCooldown - 0.3f) / 0.2f, Vector3.up);
    }

    private void Timer()
    {
        if (_attackCooldown > 0) _attackCooldown -= Time.deltaTime;
        else if (_attackCooldown < 0) _attackCooldown = 0;
        
        if (_attackCooldown <= 0.3) AttackIndicator.SetActive(false);

        if (_dashCooldown > 0) _dashCooldown -= Time.deltaTime;
        else if (_dashCooldown < 0) _dashCooldown = 0;
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
    private void RotateTowardMouse()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            var look = Quaternion.LookRotation(hit.point - Model.transform.position);
            look.x = 0;
            look.z = 0;
            Model.transform.rotation = look;
        }
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.cyan);
    }
    private bool Dash()
    {
        if (Input.GetMouseButtonDown(1) && _dashCooldown <= 0 && _attackCooldown <= 0)
        {
            RotateTowardMouse();
            _rb.velocity = Model.transform.forward * (Speed * DashModifier);
            _dashCooldown = 0.5f;
            _attackCooldown = 0.2f;
            return true;
        }
        return false;
    }
    private void Attack()
    {
        if (Input.GetMouseButton(0) && _attackCooldown<=0)
        {
            AttackIndicator.SetActive(false);
            RotateTowardMouse();
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
