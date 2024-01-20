using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefenseRemake.Construction;
using TowerDefenseRemake.Damage;
using TowerDefenseRemake.Grid;
using TowerDefenseRemake.Interaction;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI.Table;

namespace TowerDefenseRemake.Turret
{
    public abstract class TurretBehaviourBase : MonoBehaviour, IConstructable, IInteractable
    {
        [BoxGroup("ステート")]
        [SerializeField]
        private bool _interactable = true;
        public bool Interactable
        {
            get => _interactable;
            set => _interactable = value;
        }

        [BoxGroup("ステート")]
        [SerializeField]
        private bool _constructed = false;
        public bool Constructed
        {
            get => _constructed;
            set => _constructed = value;
        }

        [BoxGroup("ステート")]
        [SerializeField]
        private bool _constructable;
        public bool Constructable
        {
            get => _constructable;
            set => _constructable = value;
        }



        [BoxGroup("レイ")]
        [SerializeField]
        LayerMask _cellMask = 1 << 9;
        float _cellSize = 15.0f;

        [BoxGroup("レイ")]
        [SerializeField]
        private ConstructMatrix _constructableMatrix;
        public ConstructMatrix ConstructableMatrix
        {
            get => _constructableMatrix;
        }

        private List<GameObject> _prevCells = new List<GameObject>();



        [BoxGroup("見た目")]
        [SerializeField]
        protected Animator _anim;

        [BoxGroup("見た目")]
        [SerializeField]
        private Transform _appearance;




        [BoxGroup("パラメーター")]
        [Tooltip("火力")]
        [SerializeField]
        private float _currentFirePower = 1.0f;
        public float CurrentFirePower
        {
            get => _currentFirePower;
            set => _currentFirePower = value;
        }

        [BoxGroup("パラメーター")]
        [Tooltip("スタン時間")]
        [SerializeField]
        private float _currentStanTime = 0;
        public float CurrentStanTime
        {
            get => _currentStanTime;
            set => _currentStanTime = value;
        }

        [BoxGroup("パラメーター")]
        [Tooltip("射撃間隔")]
        [SerializeField]
        private ReactiveProperty<float> _currentFireSpan = new ReactiveProperty<float>(5.0f);
        public ReactiveProperty<float> CurrentFireSpan
        {
            get => _currentFireSpan;
            set => _currentFireSpan = value;
        }

        [BoxGroup("パラメーター")]
        [Tooltip("範囲の大きさ")]
        [SerializeField]
        protected float _currentRange = 20.0f;
        public float CurrentRange
        {
            get => _currentRange;
            set => _currentRange = value;
        }



        [BoxGroup("ターゲット")]
        [Tooltip("ターゲットの方を向いているか")]
        [SerializeField]
        private bool _lockOn = true;
        public bool LockOn
        {
            get => _lockOn;
            set => _lockOn = value;
        }

        [BoxGroup("ターゲット")]
        [Tooltip("ターゲットのレイヤー")]
        [SerializeField]
        protected LayerMask _enemyLayerMask = 1 << 7;

        // ターゲットのList
        private List<GameObject> _targetList = new List<GameObject>();
        private GameObject _currentTarget = null;
        private IDamageable _currentTargetInfo = null;

        // TODO:ScriptableObjectにステータスを保存



        protected virtual void Start()
        {
            // ターゲットのほうを向く
            this
                .UpdateAsObservable()
                .Where(_ => Constructed)
                .Subscribe(_ =>
                {
                    LookAtTarget();
                })
                .AddTo(this);

            // クールタイムありで発射(発射間隔の更新のたびに呼び出される)
            CurrentFireSpan
                .Select(x => Observable.EveryUpdate().Where(_ => LockOn && _currentTargetInfo != null && Constructed).ThrottleFirst(TimeSpan.FromSeconds(x)))
                .Switch()
                .Subscribe(_ =>
                {
                    Fire();
                })
                .AddTo(this);
        }

        // ------------------------------------------------------------------------------------------
        // Gizmos
        // ------------------------------------------------------------------------------------------
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // 範囲のGizmos
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_appearance.position, CurrentRange);
        }
#endif

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

        // ------------------------------------------------------------------------------------------
        // ターゲット探索
        // ------------------------------------------------------------------------------------------
        protected virtual bool FindTarget()
        {
            // 衝突情報とゲームオブジェクト
            RaycastHit[] hits = Physics.SphereCastAll(_appearance.position, CurrentRange, Vector3.up, 0, _enemyLayerMask);
            List<GameObject> hitsGO = new List<GameObject>();
            foreach(RaycastHit hit in hits)
            {
                hitsGO.Add(hit.collider.gameObject);
            }

            // ターゲットリストの中から今回も見つかったターゲットを追加する
            foreach (GameObject target in _targetList.ToArray())
            {
                if (!hitsGO.Contains(target))
                {
                    _targetList.Remove(target);
                }
            }

            // 衝突情報から今回新しく見つかったターゲットを追加する
            foreach (GameObject hitGO in hitsGO)
            {
                if (!_targetList.Contains(hitGO))
                {
                    _targetList.Add(hitGO);
                }
            }

            bool count = _targetList.Count > 0;

            // ターゲットがいるとき
            if (count)
            {
                // ターゲットが変更されたとき
                if (_currentTarget != _targetList[0])
                {
                    _currentTarget = _targetList[0];
                    _currentTargetInfo = _currentTarget.GetComponent<IDamageable>();
                }
            }
            else
            {
                _currentTarget = null;
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
        // Constructableインターフェース
        // ------------------------------------------------------------------------------------------
        public void Construct()
        {
            // セルをデフォルトカラーに
            foreach (GameObject cell in RayCastCell())
            {
                GridCellBehaviour cellBehaviour = cell.GetComponentInParent<GridCellBehaviour>();

                cellBehaviour.ChangeCellCenterDefaultColor();
            }

            // 建築可能のとき
            if (Constructable)
            {
                // セルの中心に移動させる
                Vector3 centerPos = new Vector3();

                foreach (GameObject cell in RayCastCell())
                {
                    centerPos += cell.transform.position;

                    GridCellBehaviour cellBehaviour = cell.GetComponentInParent<GridCellBehaviour>();
                    cellBehaviour.ConstructableExist = true;
                }

                transform.position = centerPos / ConstructableMatrix.column / ConstructableMatrix.row + transform.up * _cellSize / 2;

                // TODO:本当に建築するか確認する



                Constructed = true;
            }
            // 建築可能でなかったとき
            else
            {
                // 削除する
                Destroy(gameObject);
            }

        }

        public List<GameObject> RayCastCell()
        {
            List<GameObject> cellsGO = new List<GameObject>();

            for (int j = 0; j < ConstructableMatrix.column; j++)
            {
                for (int i = 0; i < ConstructableMatrix.row; i++)
                {
                    Vector3 rayPos = transform.position + new Vector3(_cellSize / 2 * (-ConstructableMatrix.row + 1) + _cellSize * i, 0.5f, _cellSize / 2 * (ConstructableMatrix.column - 1) - _cellSize * j);

                    if (Physics.Raycast(new Ray(rayPos, -transform.up), out RaycastHit hit, Mathf.Infinity, _cellMask))
                    {
                        cellsGO.Add(hit.collider.gameObject);
                    }
                }
            }

            return cellsGO;
        }

        public void UpdateConstructable()
        {
            List<GameObject> newCells = RayCastCell();
            bool con = true;

            // 更新がない場合
            if (Enumerable.SequenceEqual(_prevCells, newCells))
            {
                con = Constructable;
            }
            // 更新がある場合
            else
            {
                // 前回のセルをデフォルトカラーに
                foreach (GameObject oldCell in _prevCells)
                {
                    GridCellBehaviour cellBehaviour = oldCell.GetComponentInParent<GridCellBehaviour>();

                    cellBehaviour.ChangeCellCenterDefaultColor();
                }

                // 新しいセルを光らせる
                foreach (GameObject cell in newCells)
                {
                    GridCellBehaviour cellBehaviour = cell.GetComponentInParent<GridCellBehaviour>();

                    if (!cellBehaviour.ConstructableExist)
                    {
                        cellBehaviour.ChangeCellCenterUnExistColor();
                    }
                    else
                    {
                        cellBehaviour.ChangeCellCenterExistColor();
                        con = false;
                    }
                }
            }

            _prevCells = newCells;
            Constructable = con;
        }

        // ------------------------------------------------------------------------------------------
        // Interactableインターフェース
        // ------------------------------------------------------------------------------------------
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!Interactable) return;
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!Interactable) return;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!Interactable) return;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!Interactable) return;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (!Interactable) return;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (!Interactable) return;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (!Interactable) return;
        }
    }
}