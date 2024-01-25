using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Damage;
using UnityEngine;

namespace TowerDefenseRemake.Enemy
{
    public enum EnemyType
    {
        Slime,

    }

    [CreateAssetMenu(menuName = "My Scriptable/Create EnemyList")]
    public class EnemyList : ScriptableObject
    {
        [SerializeField]
        public EnemyInfo[] Enemy;
    }
}