using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladePedestal : MonoBehaviour
{
    private KeyBlade Blade;

    public Transform CurrentPoint;
    public Transform StartPoint;
    public Transform EndPoint;

    public bool ReusableBlade = true;
    public float Distance = 5f;
    public bool ReusablePedestal = false;
    public bool Activated = false;

    public event Action KeyActivate;

    void Update()
    {
        if (Vector3.Distance(transform.position, Player.Object.transform.position) <= Distance)
        {
            if (Blade == null) Blade = Player.PlayerComponent.GetKeyBlade();

            Blade.Postition = CurrentPoint;

            var Deepth = Vector3.Distance(transform.position, Player.Object.transform.position) / Distance;

            if (!ReusableBlade) Deepth = Vector3.Distance(transform.position, Player.Object.transform.position) / Distance <= Deepth ? Vector3.Distance(transform.position, Player.Object.transform.position) / Distance : Deepth;
            else Deepth = Vector3.Distance(transform.position, Player.Object.transform.position) / Distance;

            CurrentPoint.position = Vector3.Lerp(EndPoint.position, StartPoint.position, Deepth);
        }
        else if (Blade != null && ReusableBlade)
        {
            Blade.Postition = Player.PlayerComponent.GetEmptyKeyPosition(Blade);
            Blade = null;
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == Player.Object && Blade != null && (!Activated || ReusablePedestal))
        {
            KeyActivate?.Invoke();
        }
    }
}
