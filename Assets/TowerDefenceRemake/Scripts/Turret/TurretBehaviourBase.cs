using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
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
        [Header("発射")]
        [SerializeField]
        private bool _canFire = true;
        [SerializeField]
        private float _currentFirePower = 1.0f;
        [SerializeField]
        private float _currentFireSpan = 5.0f;

        [Space(10)]
        [Header("ターゲット")]
        [Tooltip("ターゲットのレイヤー")]
        [SerializeField]
        protected LayerMask _layerMask = 1 << 7;

        // ターゲットのList
        private List<RaycastHit> _targetList = new List<RaycastHit>();

        [Tooltip("範囲の大きさ")]
        [SerializeField]
        protected float _currentRange = 20.0f;

        // TODO:ScriptableObjectにステータスを保存



        private void Start()
        {
            // クールタイムありで発射
            this
                .UpdateAsObservable()
                .Where(_ => _canFire && _targetList.Count > 0)
                .ThrottleFirst(TimeSpan.FromSeconds(_currentFireSpan))
                .Subscribe(_ =>
                {
                    Fire();
                })
                .AddTo(this);

            // ターゲットのほうを向く
            this
                .UpdateAsObservable()
                .Subscribe(_ =>
                {
                    LookAtTarget();
                })
                .AddTo(this);
        }

        // ------------------------------------------------------------------------------------------
        // 射撃
        // ------------------------------------------------------------------------------------------
        public abstract void Fire();

        protected virtual void FireAnimation()
        {
            // 発射のアニメーション
            _anim.SetTrigger("Shoot");

            // マズルフラッシュと硝煙

        }

        protected virtual void FireAnimationStop()
        {
            _anim.speed = 0;
        }

        // ------------------------------------------------------------------------------------------
        // 回転
        // ------------------------------------------------------------------------------------------
        public virtual void LookAtTarget()
        {
            RaycastHit[] hits = Physics.SphereCastAll(_appearance.position, _currentRange, Vector3.up, 0, _layerMask);

            if(hits != null)
            {
                // ターゲットリストの中から今回見つからなかったターゲットを削除する
                foreach (RaycastHit target in _targetList.ToArray())
                {
                    if (!hits.Contains(target))
                    {
                        _targetList.Remove(target);
                    }
                }

                // 衝突情報から今回新しく見つかったターゲットを追加する
                foreach (RaycastHit h in hits)
                {
                    if (!_targetList.Contains(h))
                    {
                        _targetList.Add(h);
                    }
                }
            }

            // 一番目のターゲットの方を向く
            if (_targetList.Count > 0)
            {
                _appearance.rotation = Quaternion.Lerp(
                    _appearance.rotation,
                    Quaternion.LookRotation(new Vector3(_targetList[0].transform.position.x, 0, _targetList[0].transform.position.z) - new Vector3(_appearance.position.x, 0, _appearance.position.z)), 
                    0.1f);
            }
        }

        // ------------------------------------------------------------------------------------------
        // Gizmos
        // ------------------------------------------------------------------------------------------
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_appearance.position, _currentRange);
        }

        private void OnGUI()
        {
            GUILayout.Label($"{_targetList.Count}");
        }

    }
}