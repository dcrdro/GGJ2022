using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladePedestal : ActivateTrigger
{ 
    public KeyBlade Blade;
    public ActivatorsGroup Group;

    public Transform CurrentPoint;
    public Transform StartPoint;
    public Transform EndPoint;

    public bool ReusableBlade = true;
    public float Distance = 5f;
    public bool ReusablePedestal = false;

    private bool _isActivated = false;
    public override bool IsActivated => _isActivated;
    private bool _isBladeInserted = false;
    public override bool IsBladeInserted { get => _isBladeInserted; }

    public override event Action Activated;

    private float Deepth = 1;

    private void Start()
    {
        Activated += IsActivatedTrue;
    }
    void Update()
    {
        ActivateTrigger activate = Group?.GetComponent<ActivateTrigger>();
        if (Vector3.Distance(transform.position, Player.Object.transform.position) <= Distance)
        {
            if (Blade == null) { Blade = Player.PlayerComponent.GetKeyBlade(); _isBladeInserted = false; }
            if (Blade !=null)
            {
                _isBladeInserted = true;
                Blade.Postition = CurrentPoint;


                if (ReusableBlade || (Group != null && !Group.IsBladeInserted)) Deepth = Vector3.Distance(transform.position, Player.Object.transform.position) / Distance;
                else if (Vector3.Distance(transform.position, Player.Object.transform.position) / Distance <= Deepth) Deepth = Vector3.Distance(transform.position, Player.Object.transform.position) / Distance;
                //else if (Group != null) Deepth = Group.Deepth;
                

                CurrentPoint.position = Vector3.Lerp(EndPoint.position, StartPoint.position, Deepth);
            }
        }
        else if (Blade != null && (ReusableBlade || (activate != null && !activate.IsActivated)))
        {
            Blade.Postition = Player.PlayerComponent.GetEmptyKeyPosition(Blade);
            Blade = null;
            _isBladeInserted = false;
            if (ReusablePedestal)
            {
                _isActivated = false;
            }
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject == Player.Object && Blade != null) && (!IsActivated || ReusablePedestal) || Group?.IsBladeInserted == true)
        {
            Activated?.Invoke();
            
        }
    }
    public void IsActivatedTrue()
    {
        _isActivated = true;
    }
}
