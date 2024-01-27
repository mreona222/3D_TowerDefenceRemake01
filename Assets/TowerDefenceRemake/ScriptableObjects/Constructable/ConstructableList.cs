using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseRemake.Turret
{
    public enum ConstructableType
    {
        Normal,
        Gatling,

    }

    [CreateAssetMenu(menuName = "My Scriptable/Constructable/Create ConstructableList")]
    public class ConstructableList : ScriptableObject
    {
        public TurretBehaviourBase[] ConstructablePrefabs;
    }
}