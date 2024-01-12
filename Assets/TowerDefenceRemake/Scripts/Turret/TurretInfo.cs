using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseRemake.Turret
{
    [CreateAssetMenu(menuName = "My Scriptable/Create TurretInfo")]
    public class TurretInfo : ScriptableObject
    {
        [BoxGroup("プレハブ")]
        public TurretBehaviourBase TurretPrefab;
    }
}