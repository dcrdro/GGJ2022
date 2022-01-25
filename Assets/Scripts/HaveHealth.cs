using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HaveHealth : MonoBehaviour
{
    public float MaxHealth = 0f;
    public float CurHealth = 0f;
    public event Action Death;
    public event Action<float> Damaged;
    public Slider slider;

    public virtual void ToMax()
    {
        CurHealth = MaxHealth;
        if (slider != null)SliderUpdate();         
    }  
    public virtual void TakeHeal(float Heal)
    {
        CurHealth += Heal;
        if (slider!=null) SliderUpdate();
        
    }
    public virtual void TakeDamage(float Damage)
    {
        TakeHeal(-Damage);
        Damaged?.Invoke(Damage);
        CheckDeath();
    }
    private void CheckDeath()
    {
        if (CurHealth <= 0) Death?.Invoke();
    }
    public virtual void SliderUpdate()
    {
        slider.maxValue = MaxHealth;
        slider.value = CurHealth;
    }
}