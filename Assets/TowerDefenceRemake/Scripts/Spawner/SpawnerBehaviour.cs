using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Damage;
using TowerDefenseRemake.Enemy;
using TowerDefenseRemake.Manager;
using UniRx;
using UnityEngine;

namespace TowerDefenseRemake.Spawner
{
    public class SpawnerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private EnemyList _enemyList;
        [SerializeField]
        private Transform _enemyParent;

        private List<GameObject> _enemyInst = new List<GameObject>();

        void Start()
        {
            Observable
                .Timer(TimeSpan.Zero,TimeSpan.FromSeconds(6.0f))
                .Subscribe(_ =>
                {
                    SpawnEnemy(EnemyType.Slime, ((BattleInstanceManager)GameManager.Instance._IM).EnemyTarget[0]);
                })
                .AddTo(this);
        }

        public void SpawnEnemy(EnemyType type, Transform target)
        {
            // 敵生成
            EnemyBehaviourBase enemy = Instantiate(_enemyList.Enemy[(int)type].EnemyPrefab, transform.position, transform.rotation, _enemyParent);

            // 敵初期化
            enemy.SetTarget(target);
        }
    }
}