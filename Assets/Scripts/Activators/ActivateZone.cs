using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateZone : ActivateTrigger
{
    private bool _isActivated = false;
    public override bool IsActivated => _isActivated;

    public override bool IsBladeInserted => IsActivated;

    public override event Action Activated;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player.Object)
        {
            Activated?.Invoke();
            _isActivated = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player.Object)
        {
            _isActivated = false;
        }
    }
}
