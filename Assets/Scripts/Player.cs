using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static GameObject Object;
    public static Player PlayerComponent;
    public static HaveHealth health;
    public static bool OnLight => LightSourse.RaysCount > 0;

    public GameObject Model;
    public GameObject Projectile;
    [NonSerialized]
    public GameObject Boss;

    public Transform AttackPoint;
    public Transform Mouse;

    #region Keyblades
    [Serializable]
    public class PlacePosition
    {
        public Transform Transform;
        public KeyBlade Blade;
    }
    [SerializeField]
    private List<PlacePosition> _keyPositions = new List<PlacePosition>();
    public Transform GetEmptyKeyPosition(KeyBlade keyBlade)
    {
        var Duplicate = _keyPositions.Find(o => o.Blade == keyBlade);
        if (Duplicate != null) Duplicate.Blade = null;
        var el = _keyPositions.Find(o => o.Blade == null);
        if (el != null)
        {
            el.Blade = keyBlade;
            return el.Transform;
        }
        else return null;
    }
    public KeyBlade GetKeyBlade()
    {
        var el = _keyPositions.FindLast(o => o.Blade != null);
        if (el != null)
        {
            var blade = el.Blade;
            el.Blade = null;
            return blade;
        }
        else return null;
    }
    #endregion

    public Slider HPSlider;
    public Material DarkMaterial;
    public Material LightMaterial;
    public SkinnedMeshRenderer MeshRenderer;
    
    public LayerMask EnemyLayers;

    [Serializable]
    public class Characteristics
    {
        public float AttackRange = 1.5f;
        public float Speed = 5f;
        public float RotationSpeed = 420f;
        public float DashModifier = 5f;
        public float MeleeAttackDamage = 20f;
        public float DistanceAttackDamage = 10f;
    }
    public Characteristics characteristics;

    private bool _onLight;

    public event Action LightsChanged;

    private Rigidbody _rb;
    private Animator _animator;
    private PlayerAudioController _audioController;
    private float _attackCooldown = 0;
    public float _dashCooldown { get; private set; } = 0;

    private void Awake()
    {
        if (health == null)
        {
            health = gameObject.AddComponent<HaveHealth>();
            health.MaxHealth = 100;
            health.ToMax();
        }
        health.slider = HPSlider;
    }
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _audioController = GetComponentInChildren<PlayerAudioController>();
        Object = gameObject;
        PlayerComponent = this;
        health.Death += Death;
        LightsChanged += ChangeVisualLightForm;
    }
    private void ChangeVisualLightForm()
    {
        if (OnLight) MeshRenderer.material = LightMaterial;
        else MeshRenderer.material = DarkMaterial;
    }

    private void Death()
    {
        SceneManager.LoadScene(0);
    }
    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");
        for (int i = 0; i < _keyPositions.Count; i++)
        {
            if (_keyPositions[i].Blade != null)
            {
                _keyPositions[i].Transform.position += Vector3.up * Mathf.Sin(Time.unscaledTime + i * 10) / 1300;
            }
        }

        if (_onLight != OnLight)
        {
            LightsChanged?.Invoke();
        }
        _onLight = OnLight;

        Timer();

        if (Boss == null) Mouse.position = Vector3.Lerp(transform.position, MousePos().HasValue ? MousePos().Value.point : Vector3.LerpUnclamped(transform.position, Mouse.position, 1 / 0.02f), 0.02f);
        else Mouse.position = Vector3.Lerp(transform.position, Boss.transform.position, 0.2f);

        Attack();

        if (Dash()) { }
        else if (_attackCooldown <= 0)
        {
            Move(vertical, horizontal);
            RotateTowardMoveDir(vertical, horizontal);
        }
        else if (_dashCooldown <= 0)
        {
            characteristics.Speed /= 2;
            Move(vertical, horizontal);
            characteristics.Speed *= 2;
        }

        if (_attackCooldown >= 0.3)
        {
            Collider[] HitProjectiles = Physics.OverlapSphere(AttackPoint.position, characteristics.AttackRange / 1.5f);
            foreach (var Projectile in HitProjectiles)
            {
                Projectile.GetComponent<Projectile>()?.Reflect(Model.transform.rotation);
            }
        }
    }

    private void Timer()
    {
        if (_attackCooldown > 0) _attackCooldown -= Time.deltaTime;
        else if (_attackCooldown < 0) _attackCooldown = 0;

        if (_dashCooldown > 0) _dashCooldown -= Time.deltaTime;
        else if (_dashCooldown < 0) _dashCooldown = 0;
    }

    private void Move(float vertical, float horizontal)
    {
        var direction = new Vector2(horizontal * characteristics.Speed, vertical * characteristics.Speed);

        _rb.velocity = new Vector3(direction.x, _rb.velocity.y, direction.y);
        _animator.SetBool("IsMoving", Mathf.Abs(vertical) + Mathf.Abs(horizontal) > 0.01f);
    }
    private void RotateTowardMoveDir(float vertical, float horizontal)
    {
        if (horizontal != 0 || vertical != 0)
        {
            float angle = Mathf.Atan2(horizontal * characteristics.Speed, vertical * characteristics.Speed) * Mathf.Rad2Deg;
            Quaternion smooth = Quaternion.Euler(0, angle, 0);
            Model.transform.rotation = Quaternion.RotateTowards(Model.transform.rotation, smooth, characteristics.RotationSpeed * Time.deltaTime);
        }
    }
    private RaycastHit? MousePos()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.cyan);
            return hit;
        }
        else
        {
            return null;
        }

    }
    private void RotateTowardMouse()
    {
        var hit = MousePos();
        if (hit != null)
        {
            var From = new Vector3(Model.transform.position.x, Model.transform.position.y, Model.transform.position.z);
            var to = new Vector3(hit.Value.point.x, Model.transform.position.y, hit.Value.point.z);
            var look = Quaternion.LookRotation(to - From);
            look.x = 0;
            look.z = 0;
            Model.transform.rotation = look;
        }
    }
    private bool Dash()
    {
        if (Input.GetMouseButtonDown(1) && _dashCooldown <= 0 && _attackCooldown <= 0.3f)
        {
            RotateTowardMouse();
            var speed = characteristics.Speed * characteristics.DashModifier;
            _rb.velocity = new Vector3(Model.transform.forward.x * speed, _rb.velocity.y, Model.transform.forward.z * speed);
            _dashCooldown = 0.5f;
            _attackCooldown = 0.2f;
            return true;
        }
        return false;
    }
    private void Attack()
    {
        if (Input.GetMouseButton(0) && _attackCooldown <= 0 && _dashCooldown <= 0)
        {
            RotateTowardMouse();
            if (OnLight)
            {
                _attackCooldown = 0.5f;
                
                _animator.SetTrigger("AttackRange");
            }
            else
            {
                _attackCooldown = 0.5f;

                _audioController.PlayMeleePreAttack();
                _animator.SetTrigger("AttackMelee");
            }
        }
    }

    public void EndMeleeAttack()
    {
        Collider[] HitEnemies = Physics.OverlapSphere(AttackPoint.position, characteristics.AttackRange, EnemyLayers);
        foreach (var Enemy in HitEnemies)
        {
            //Debug.Log($"EnemyDetected {Enemy.gameObject.name}");
            Enemy.GetComponent<HaveHealth>()?.TakeDamage(characteristics.MeleeAttackDamage);

            Rigidbody EnemyRb;
            if (Enemy.gameObject.TryGetComponent<Rigidbody>(out EnemyRb))
            {
                EnemyRb.velocity = (Enemy.transform.position - transform.position).normalized * 5;
            }
        }
        if (HitEnemies.Length > 0) _audioController.PlayMeleeHitAttack();
    }

    public void EndRangeAttack()
    {
        var projectile = Instantiate(Projectile, AttackPoint.position, Model.transform.rotation).GetComponent<Projectile>();
        projectile.Damage = characteristics.DistanceAttackDamage;
        projectile.TriggerEnter += (sender, Damage, collider) =>
        {
            var Hp = collider.gameObject.GetComponent<HaveHealth>();
            if (Hp != null)
            {
                Hp.TakeDamage(characteristics.DistanceAttackDamage);
                _audioController.PlayRangeHitShotAttack();
            }
            else if (!collider.isTrigger) Destroy(sender);
        };
        
        _audioController.PlayRangePreShotAttack();
    }
}
