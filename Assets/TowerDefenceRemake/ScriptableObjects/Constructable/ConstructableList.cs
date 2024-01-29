using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Constructable.Turret;
using UnityEngine;

namespace TowerDefenseRemake.Constructable
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