using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{

    private int _maxHealth;
    private int _health;

    public HealthSystem(int healthMax)
    {
        this._maxHealth = healthMax;
        this._health = healthMax;
    }

    public int GetHealth()
    {
        return _health;
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    public float GetHealthNormalized()
    {
        return (float)_health / _maxHealth;
    }

    public void Damage(int amount)
    {
        _health -= amount;
        if (_health < 0)
        {
            _health = 0;
        }
    }

    public bool IsDead()
    {
        return _health <= 0;
    }

    public void Heal(int amount)
    {
        if (IsDead()) return;

        _health += amount;
        if (_health > _maxHealth)
        {
            _health = _maxHealth;
        }
    }

    public void SetHealthMax(int healthMax, bool fullHealth)
    {
        this._maxHealth = healthMax;
        if (fullHealth) _health = healthMax;
    }
}
