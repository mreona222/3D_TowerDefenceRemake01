using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefenseRemake.Damage;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

namespace TowerDefenseRemake.Turret
{
    public abstract class TurretBehaviourBase : MonoBehaviour
    {
        [SerializeField]
        protected Animator _anim;
        [SerializeField]
        private Transform _appearance;



        [Space(10)]
        [Header("パラメーター")]
        [Tooltip("火力")]
        [SerializeField]
        private float _currentFirePower = 1.0f;
        public float CurrentFirePower
        {
            get => _currentFirePower;
            set => _currentFirePower = value;
        }

        [Tooltip("スタン時間")]
        [SerializeField]
        private float _currentStanTime = 0;
        public float CurrentStanTime
        {
            get => _currentStanTime;
            set => _currentStanTime = value;
        }

        [Tooltip("射撃間隔")]
        [SerializeField]
        private ReactiveProperty<float> _currentFireSpan = new ReactiveProperty<float>(5.0f);
        public ReactiveProperty<float> CurrentFireSpan
        {
            get => _currentFireSpan;
            set => _currentFireSpan = value;
        }

        [Tooltip("範囲の大きさ")]
        [SerializeField]
        protected float _currentRange = 20.0f;
        public float CurrentRange
        {
            get => _currentRange;
            set => _currentRange = value;
        }



        [Space(10)]
        [Header("ターゲット")]
        [Tooltip("ターゲットのレイヤー")]
        [SerializeField]
        protected LayerMask _layerMask = 1 << 7;

        // ターゲットのList
        private List<RaycastHit> _targetList = new List<RaycastHit>();
        private RaycastHit _currentTarget = new RaycastHit();
        private IDamageable _currentTargetInfo;

        [Tooltip("ターゲットの方を向いているか")]
        [SerializeField]
        private bool _lockOn = true;
        public bool LockOn
        {
            get => _lockOn;
            set => _lockOn = value;
        }

        // TODO:ScriptableObjectにステータスを保存



        protected virtual void Start()
        {
            // ターゲットのほうを向く
            this
                .UpdateAsObservable()
                .Subscribe(_ =>
                {
                    LookAtTarget();
                })
                .AddTo(this);

            // クールタイムありで発射(発射間隔の更新のたびに呼び出される)
            CurrentFireSpan
                .Select(x => Observable.EveryUpdate().Where(_ => LockOn && _currentTargetInfo != null).ThrottleFirst(TimeSpan.FromSeconds(x)))
                .Switch()
                .Subscribe(_ =>
                {
                    Fire();
                })
                .AddTo(this);
        }

        // ------------------------------------------------------------------------------------------
        // 射撃
        // ------------------------------------------------------------------------------------------
        public virtual void Fire()
        {
            // ターゲットを攻撃
            _currentTargetInfo.ApplyDamage(CurrentFirePower, CurrentStanTime);
        }

        protected virtual void FireAnimation()
        {
            // 発射のアニメーション
            _anim.SetTrigger("Shoot");

            // TODO:マズルフラッシュと硝煙

        }

        protected virtual void FireAnimationStop()
        {
            _anim.speed = 0;
        }

        // TODO:削除
        private void OnGUI()
        {
            GUILayout.Label($"{_targetList.Count}");
        }

        // ------------------------------------------------------------------------------------------
        // ターゲット探索
        // ------------------------------------------------------------------------------------------
        protected virtual bool FindTarget()
        {
            RaycastHit[] hits = Physics.SphereCastAll(_appearance.position, CurrentRange, Vector3.up, 0, _layerMask);

            // ターゲットリストの中から今回見つからなかったターゲットを削除する
            foreach (RaycastHit target in _targetList.ToArray())
            {
                if (!hits.Contains(target))
                {
                    _targetList.Remove(target);
                }
            }

            // 衝突情報から今回新しく見つかったターゲットを追加する
            foreach (RaycastHit hit in hits)
            {
                if (!_targetList.Contains(hit))
                {
                    _targetList.Add(hit);
                }
            }

            bool count = _targetList.Count > 0;

            // ターゲットがいるとき
            if (count)
            {
                // ターゲットが変更されたとき
                if (_currentTarget.collider != _targetList[0].collider)
                {
                    _currentTarget = _targetList[0];
                    _currentTargetInfo = _currentTarget.collider.GetComponent<IDamageable>();
                }
            }
            else
            {
                _currentTarget = new RaycastHit();
                _currentTargetInfo = null;
            }

            return count;
        }

        // ------------------------------------------------------------------------------------------
        // 回転
        // ------------------------------------------------------------------------------------------
        public virtual void LookAtTarget()
        {
            if (FindTarget())
            {
                // 一番目のターゲットの方を向く
                Vector3 direction = new Vector3(_currentTarget.transform.position.x, 0, _currentTarget.transform.position.z) - new Vector3(_appearance.position.x, 0, _appearance.position.z);

                _appearance.rotation = Quaternion.Lerp(_appearance.rotation, Quaternion.LookRotation(direction), 0.1f);

                // ターゲットの方に向いたときに発射可能
                LockOn = Vector3.Angle(_appearance.forward, direction) < 5.0f;
            }
        }

        // ------------------------------------------------------------------------------------------
        // Gizmos
        // ------------------------------------------------------------------------------------------
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_appearance.position, CurrentRange);
        }
#endif
    }
}