using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseRemake.Enemy
{
    [CreateAssetMenu(menuName = "My Scriptable/Create EnemyInfo")]
    public class EnemyInfo : ScriptableObject
    {
        [BoxGroup("プレハブ")]
        public EnemyBehaviourBase EnemyPrefab;

        [BoxGroup("パラメーター")]
        [Header("体力")]
        public float MaxHp;
    }
}