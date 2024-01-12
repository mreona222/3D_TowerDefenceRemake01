using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseRemake.Turret
{
    public enum TurretType
    {
        Normal,
        Gatling,

    }

    [CreateAssetMenu(menuName = "My Scriptable/Create TurretList")]
    public class TurretList : ScriptableObject
    {
        public TurretInfo[] Turret;
    }
}