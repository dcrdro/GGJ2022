using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorsGroup : ActivateTrigger
{
    public List<ActivateTrigger> Activators;
    public override event Action Activated;
    public override bool IsActivated
    {
        get
        {
            int count = 0;
            Activators.ForEach(p => { if (p.IsActivated) count++; });
            return count >= Activators.Count;
        }
    }
    public float Deepth = 1;
    public override bool IsBladeInserted
    {
        get
        {
            int count = 0;
            Activators.ForEach(p => { if (p.IsBladeInserted) count++; });
            return count >= Activators.Count;
        }
    }
    private void Update()
    {
        Deepth = Vector3.Distance(transform.position, Player.Object.transform.position) / 5;
    }
    private void Start()
    {
        foreach (BladePedestal Activator in Activators)
        {
            Activated += Activator.IsActivatedTrue;
        }
        foreach (var Activator in Activators)
        {
            Activator.Activated += () => 
            {
                if (IsBladeInserted) Activated?.Invoke(); 
            };
            (Activator as BladePedestal).Group = this;
        }
    }
}
