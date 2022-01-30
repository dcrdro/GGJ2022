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
    public float ColorSpeed = 5f;
    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponentInChildren<Renderer>();
    }

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
        
        StopAllCoroutines();
        StartCoroutine(ChangeColor());
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

    private IEnumerator ChangeColor()
    {
        float v = 1f;
        Color attackedColor = Color.red * 0.9f;
        Color defaultColor = _renderer.material.color;
        
        while (v > 0)
        {
            var c = Color.Lerp(attackedColor, defaultColor, v);
            _renderer.material.color = c;
            v -= ColorSpeed * Time.deltaTime;
            yield return null;
        }
        
        _renderer.material.color = defaultColor;

    }
}