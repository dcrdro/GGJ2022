using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivateTrigger : MonoBehaviour
{
    public abstract bool IsActivated { get; }
    public abstract bool IsBladeInserted { get; }
    public abstract event Action Activated;
}
