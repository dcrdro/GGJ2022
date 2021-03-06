using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSourse : MonoBehaviour
{
    private Light _light;
    public static int RaysCount = 0;
    public bool IlluminatePlayer = false;

    private void Start()
    {
        _light = GetComponent<Light>();
        Player.health.Death += PlayerDeath;
    }

    private void PlayerDeath()
    {
        RaysCount = 0;
    }

    void Update()
    {
        if (_light.type == LightType.Directional)
        {
            var dir = -transform.forward;
            Ray CheckLightRay = new Ray(Player.Object.transform.position, dir);
            bool res = Physics.Raycast(CheckLightRay);
            Debug.DrawRay(Player.Object.transform.position, dir * 100, Color.red);
            if (!res && !IlluminatePlayer)
            {
                RaysCount++;
                IlluminatePlayer = true;
            }
            else if (res && IlluminatePlayer)
            {
                RaysCount--;
                IlluminatePlayer = false;
            }
        }
        if (_light.type == LightType.Point)
        {
            if (Vector3.Distance(transform.position, Player.Object.transform.position)<=_light.range)
            {
                RaycastHit hit;
                var dir = Player.Object.transform.position - transform.position;
                Ray CheckLightRay = new Ray(transform.position, dir);
                bool res = Physics.Raycast(CheckLightRay, out hit);
                Debug.DrawRay(transform.position, dir, Color.red);
                if (res && hit.collider.gameObject == Player.Object && !IlluminatePlayer)
                {
                    RaysCount++;
                    IlluminatePlayer = true;
                }
                else if (res && hit.collider.gameObject != Player.Object && IlluminatePlayer)
                {
                    RaysCount--;
                    IlluminatePlayer = false;
                }
            }
            else if(IlluminatePlayer)
            {
                RaysCount--;
                IlluminatePlayer = false;
            }
        }
    }
}
