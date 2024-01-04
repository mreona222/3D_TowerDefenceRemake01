using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseRemake.Damage
{
    public interface IDamageable
    {
        public void ApplyDamage(float damage, float stanTime);

        public bool IsDead { get; set; }
    }
}