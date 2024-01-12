using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Template.Utility;
using UnityEngine.AI;
using System;
using Unity.VisualScripting;
using TowerDefenseRemake.Damage;
using UniRx;
using UniRx.Diagnostics;
using Sirenix.OdinInspector;
using TowerDefenseRemake.Manager;

namespace TowerDefenseRemake.Enemy
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class EnemyBehaviourBase : StateMachineBase<EnemyBehaviourBase>, IDamageable
    {
        private Animator _anim;

        private NavMeshAgent _agent;

        private Transform _target;

        [SerializeField]
        private Collider[] _collider;


        [BoxGroup("状態異常")]
        // ノックバック
        [SerializeField]
        private ReactiveProperty<bool> _isNockBacking = new ReactiveProperty<bool>(false);

        // スタン
        [BoxGroup("状態異常")]
        [ShowInInspector]
        private bool _isStaning => _stanCount.Value > 0;
        private ReactiveProperty<int> _stanCount = new ReactiveProperty<int>();

        // 生存状態
        [BoxGroup("状態異常")]
        [SerializeField]
        private bool _isDead = false;
        public bool IsDead
        {
            get => _isDead;
            set
            {
                _isDead = value;
                foreach(Collider col in _collider)
                {
                    col.enabled = false;
                }
            }
        }



        [BoxGroup("パラメーター")]
        [SerializeField]
        private float _currentHP = 200.0f;
        public float CurrentHP
        {
            get => _currentHP;
            set => _currentHP = value;
        }

        [BoxGroup("パラメーター")]
        [SerializeField]
        protected float _currentMoveSpeed;
        public float CurrentMoveSpeed
        {
            get => _currentMoveSpeed;
            set => _currentMoveSpeed = value;
        }


        protected virtual void Start()
        {
            _anim = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();

            // ノックバック
            _isNockBacking
                .Where(x => x)
                .Select(_ => Observable.Timer(TimeSpan.FromSeconds(2.0f)))
                .Switch()
                .Subscribe(_ =>
                {
                    _isNockBacking.Value = false;
                })
                .AddTo(this);
        }

        public void SetTarget(Transform tf)
        {
            _target = tf;
        }

        private void SetAnimation(int number, float speed)
        {
            _anim.SetInteger("State", number);
            _anim.speed = speed;
        }

        // -------------------------------------------------------------------------------------
        // Idle
        // -------------------------------------------------------------------------------------

        IDisposable disposableIdle;

        public virtual void OnEnterIdle()
        {
            // アニメーション
            SetAnimation(0, 1.0f);

            // 歩きの速度
            _agent.speed = 0;

            // 時間経過でステート切り替え
            // 購読開始
            disposableIdle = Observable
                .Timer(TimeSpan.FromSeconds(0.2f))
                .Subscribe(_ =>
                {
                    ChangeState2Move();
                })
                .AddTo(this);
        }

        public virtual void OnExitIdle()
        {
            // 購読停止
            disposableIdle?.Dispose();
        }

        public abstract void ChangeState2Idle();

        // -------------------------------------------------------------------------------------
        // Move
        // -------------------------------------------------------------------------------------
        public virtual void OnEnterMove()
        {
            // アニメーション
            SetAnimation(1, 1.0f);

            // 歩きの速度
            _agent.speed = CurrentMoveSpeed;

            // 目標
            _agent.SetDestination(_target.position);
        }

        public virtual void OnUpdateMove()
        {
            // 目標地点に到達
        }

        public abstract void ChangeState2Move();

        // -------------------------------------------------------------------------------------
        // Damage
        // -------------------------------------------------------------------------------------

        IDisposable _disposableDamage = new Subject<long>();

        public virtual void ApplyDamage(float damage, float stanTime)
        {
            if(CurrentHP > 0)
            {
                ChangeState2Damage(damage, stanTime);
            }
        }

        public virtual void OnEnterDamage(float damage, float stanTime)
        {
            // 歩きの速度
            _agent.speed = 0;

            // ダメージ
            CurrentHP -= damage;

            // 生存確認
            if(CurrentHP <= 0)
            {
                ChangeState2Die();
                IsDead = true;
            }

            // ノックバック
            if (!_isNockBacking.Value)
            {
                // アニメーション
                SetAnimation(2, 1.0f);

                _isNockBacking.Value = true;
            }

            // スタン
            _stanCount.Value++;

            Observable
                .Timer(TimeSpan.FromSeconds(stanTime))
                .First()
                .Subscribe(x =>
                {
                    _stanCount.Value--;
                })
                .AddTo(this);

            // 時間経過でスタン解除
            _disposableDamage = Observable
                .EveryUpdate()
                .Where(_ => _stanCount.Value == 0)
                .Subscribe(_ =>
                {
                    ChangeState2Move();
                })
                .AddTo(this);
        }

        public virtual void OnExitDamage()
        {
            _disposableDamage?.Dispose();
        }

        public abstract void ChangeState2Damage(float damage, float stanTime);

        // -------------------------------------------------------------------------------------
        // Die
        // -------------------------------------------------------------------------------------
        public virtual void OnEnterDie()
        {
            // アニメーション
            SetAnimation(3, 1.0f);

            // 歩きの速度
            _agent.speed = 0;

            // 削除
            Destroy(gameObject, 3.0f);
        }

        public abstract void ChangeState2Die();

        // -------------------------------------------------------------------------------------
        // ReachGoal
        // -------------------------------------------------------------------------------------
        public virtual void OnEnterReachGoal()
        {
            // アニメーション
            SetAnimation(4, 1.0f);

            // 歩きの速度
            _agent.speed = 0;

            // 削除
            Destroy(gameObject, 3.0f);
        }

        public abstract void ChangeState2ReachGoal();
    }
}