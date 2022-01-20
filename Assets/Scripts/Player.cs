using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static GameObject Object;
    
    public float Speed = 100f;
    public bool OnLight => LightSourse.RaysCount > 0;

    private Rigidbody _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Object = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = Input.GetAxis("Horizontal");
        var vertical = Input.GetAxis("Vertical");

        var direction = new Vector2(horizontal, vertical);

        _rb.velocity = new Vector3(direction.x * Speed, _rb.velocity.y, direction.y * Speed);
    }
}
