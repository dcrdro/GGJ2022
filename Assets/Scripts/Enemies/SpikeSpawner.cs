using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
    public GameObject Spike;
    public float EverySeconds = 1;
    private float _cooldown;
    void Update()
    {
        var hit = new RaycastHit();
        if (_cooldown <= 0 &&Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Instantiate(Spike, hit.point, Quaternion.identity);
            _cooldown = EverySeconds;
        }
        if (_cooldown > 0) _cooldown -= Time.deltaTime;
    }
}
