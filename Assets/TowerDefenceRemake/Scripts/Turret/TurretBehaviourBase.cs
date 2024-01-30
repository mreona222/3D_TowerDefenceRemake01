using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using TowerDefenseRemake.Damage;
using TowerDefenseRemake.Grid;
using TowerDefenseRemake.Interaction;
using TowerDefenseRemake.Manager;
using TowerDefenseRemake.UI;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

namespace TowerDefenseRemake.Constructable.Turret
{
    public abstract class TurretBehaviourBase : MonoBehaviour, IConstructable, IInteractable
    {
        // ------------------------------------------------
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

        // ------------------------------------------------

        [BoxGroup("見た目")]
        [SerializeField]
        private Transform _appearance;

        protected Animator _anim;

        // ------------------------------------------------

        [BoxGroup("パラメーター")]
        [Tooltip("インフォ")]
        [SerializeField]
        private ConstructableInfo _info;
        public ConstructableInfo Info => _info;

        [BoxGroup("パラメーター")]
        [Tooltip("現在のパラメータ")]
        [SerializeField]
        private ReactiveDictionary<ParamType, ConstructLevel> _currentParams = new ReactiveDictionary<ParamType, ConstructLevel>();
        public ReactiveDictionary<ParamType, ConstructLevel> CurrentParams
        {
            get => _currentParams;
            set => _currentParams = value;
        }

        private ConstructMatrix _curretnMatrix;
        public ConstructMatrix CurrentMatrix
        {
            get => _curretnMatrix;
            set => _curretnMatrix = value;
        }

        // ------------------------------------------------

        [BoxGroup("レイヤーマスク")]
        [Tooltip("セルのレイヤー")]
        [SerializeField]
        LayerMask _cellMask = 1 << 9;
        float _cellSize = 15.0f;

        private List<GameObject> _prevCells = new List<GameObject>();

        [BoxGroup("レイヤーマスク")]
        [Tooltip("ターゲットのレイヤー")]
        [SerializeField]
        protected LayerMask _enemyLayerMask = 1 << 7;

        // ------------------------------------------------

        [BoxGroup("ターゲット")]
        [Tooltip("ターゲットの方を向いているか")]
        [SerializeField]
        private bool _lockOn = true;
        public bool LockOn
        {
            get => _lockOn;
            set => _lockOn = value;
        }

        // ターゲットのList
        private List<GameObject> _targetList = new List<GameObject>();
        private GameObject _currentTarget = null;
        private IDamageable _currentTargetInfo = null;

        // ------------------------------------------------

        [BoxGroup("UI")]
        [Tooltip("タレットのキャンバス")]
        [SerializeField]
        Canvas _infoCanvas;

        [BoxGroup("UI")]
        [Tooltip("Disableボタン")]
        [SerializeField]
        CanvasDisableButton _disableButton;

        // ------------------------------------------------

        [BoxGroup("コンテンツ")]
        [Tooltip("コンテンツリスト")]
        [SerializeField]
        private ConstructableContentList _contentList;

        Dictionary<ParamType, ConstructableUpgradeContent> _contentInsts = new Dictionary<ParamType, ConstructableUpgradeContent>();

        [BoxGroup("コンテンツ")]
        [Tooltip("コンテンツの親")]
        [SerializeField]
        Transform _upgradeContentParent;

        [BoxGroup("コンテンツ")]
        [Tooltip("コンテンツのRectTransform")]
        [SerializeField]
        RectTransform _upgradeCanvasRT;

        // ------------------------------------------------

        protected virtual void Start()
        {
            _anim = _appearance.gameObject.GetComponent<Animator>();

            // UpgradeContent作成
            GenerateContent();

            // パラメータの初期化
            InitializeParams();

            // ターゲットのほうを向く
            this
                .UpdateAsObservable()
                .Where(_ => Constructed)
                .Subscribe(_ =>
                {
                    LookAtTarget();
                })
                .AddTo(this);

            // Disableボタン
            _disableButton.OnClick += async () =>
            {
                if (_upgradeCanvasRT.anchoredPosition.x < 0)
                {
                    await _upgradeCanvasRT.DOAnchorPosX(-Screen.width * 0.75f, 0.3f).SetLink(gameObject);
                }
                else
                {
                    await _upgradeCanvasRT.DOAnchorPosX(Screen.width * 0.75f, 0.3f).SetLink(gameObject);
                }

                Interactable = true;
            };
        }

        // ------------------------------------------------------------------------------------------
        // Gizmos
        // ------------------------------------------------------------------------------------------
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // 範囲のGizmos
            Gizmos.color = Color.blue;
            if (CurrentParams.TryGetValue(ParamType.Range, out ConstructLevel rangeValue))
            {
                Gizmos.DrawWireSphere(_appearance.position, rangeValue.ParamValue.Value * _cellSize);
            }
        }
#endif

        // ------------------------------------------------------------------------------------------
        // 射撃
        // ------------------------------------------------------------------------------------------
        public virtual void Fire()
        {
            // TODO:スマートにする
            // ターゲットを攻撃
            if (CurrentParams.TryGetValue(ParamType.Stan, out ConstructLevel temp))
            {
                _currentTargetInfo.ApplyDamage(CurrentParams[ParamType.Power].ParamValue.Value, temp.ParamValue.Value);
            }
            else
            {
                _currentTargetInfo.ApplyDamage(CurrentParams[ParamType.Power].ParamValue.Value, 0);
            }
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
            RaycastHit[] hits = Physics.SphereCastAll(_appearance.position, CurrentParams[ParamType.Range].ParamValue.Value * _cellSize, Vector3.up, 0.001f, _enemyLayerMask);
            List<GameObject> hitsGO = new List<GameObject>();
            foreach (RaycastHit hit in hits)
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
        // UI
        // ------------------------------------------------------------------------------------------
        /// <summary>
        /// コンテンツの生成
        /// </summary>
        private void GenerateContent()
        {
            foreach (ParamType content in Info.ParamTypes)
            {
                _contentInsts.Add(content, Instantiate(_contentList.Content[(int)content], _upgradeContentParent));

                _contentInsts[content].ContentType = content;

                _contentInsts[content].OnUpdateCurrentParams += UpgradeParams;
                _contentInsts[content].OnUpdateNextParams += CalcurateNext;
            }
        }

        private (float value, float cost) CalcurateNext(ParamType type, int raiseLevel)
        {
            int nextLevel = CurrentParams[type].Level + raiseLevel;

            float nextValue = TurretUpgradeCalcurator.CalcurateNormal(Info, type, nextLevel);

            // TODO:コストを計算する（丸める）
            switch (_contentInsts[type].DropdownType)
            {
                case ConstructableUpgradeContent.DropdownTypeEnum.DPS:
                    if (type == ParamType.Power) nextValue /= CurrentParams[ParamType.Interval].ParamValue.Value;
                    else if (type == ParamType.Interval) nextValue = CurrentParams[ParamType.Power].ParamValue.Value / nextValue;
                    break;

                default:
                    break;
            }

            return (nextValue, raiseLevel);
        }

        // ------------------------------------------------------------------------------------------
        // アップグレード
        // ------------------------------------------------------------------------------------------
        /// <summary>
        /// パラメータの更新
        /// </summary>
        /// <param name="type"></param>
        /// <param name="raiseLevel"></param>
        float UpgradeParams(ParamType type, int raiseLevel)
        {
            int nextLevel = CurrentParams[type].Level + raiseLevel;
            int maxlevel = Info.Max[type].Level;

            if (maxlevel == CurrentParams[type].Level)
            {
                Debug.Log($"{gameObject.name}の{type.ToString()}は最大レベルです。");
            }
            else if (maxlevel >= nextLevel)
            {
                CurrentParams[type].ChangeLevel(nextLevel, TurretUpgradeCalcurator.CalcurateNormal(Info, type, nextLevel));

                Debug.Log($"{gameObject.name}の{type.ToString()}レベルを{CurrentParams[type].Level}に上げました。");
            }
            else
            {
                CurrentParams[type].ChangeLevel(maxlevel, TurretUpgradeCalcurator.CalcurateNormal(Info, type, maxlevel));

                Debug.Log($"{gameObject.name}の{type.ToString()}を最大レベルまで上げました。");
            }

            float currentValue = CurrentParams[type].ParamValue.Value;

            switch (_contentInsts[type].DropdownType)
            {
                case ConstructableUpgradeContent.DropdownTypeEnum.DPS:
                    if (type == ParamType.Power) currentValue /= CurrentParams[ParamType.Interval].ParamValue.Value;
                    else if (type == ParamType.Interval) currentValue = CurrentParams[ParamType.Power].ParamValue.Value / currentValue;
                    break;

                default:
                    break;
            }

            return currentValue;
        }

        /// <summary>
        /// パラメータの初期化
        /// </summary>
        void InitializeParams()
        {
            foreach (ParamType type in Info.ParamTypes)
            {
                CurrentParams.Add(type, new ConstructLevel(Info.InitialParam[type].Level, Info.InitialParam[type].ParamValue.Value));

                // クールタイムありで発射(発射間隔の更新のたびに呼び出される)
                if (type == ParamType.Interval)
                {
                    CurrentParams[ParamType.Interval].ParamValue
                        .Select(x => Observable.EveryUpdate().Where(_ => LockOn && _currentTargetInfo != null && Constructed).ThrottleFirst(TimeSpan.FromSeconds(x)))
                        .Switch()
                        .Subscribe(_ =>
                        {
                            Fire();
                        })
                        .AddTo(this);
                }
            }
        }

        // ------------------------------------------------------------------------------------------
        // Constructableインターフェース
        // ------------------------------------------------------------------------------------------
        public List<GameObject> RayCastCell()
        {
            List<GameObject> cellsGO = new List<GameObject>();

            for (int j = 0; j < CurrentMatrix.Column; j++)
            {
                for (int i = 0; i < CurrentMatrix.Row; i++)
                {
                    Vector3 rayPos = transform.position + new Vector3(_cellSize / 2 * (-CurrentMatrix.Row + 1) + _cellSize * i, 0.5f, _cellSize / 2 * (CurrentMatrix.Column - 1) - _cellSize * j);

                    if (Physics.Raycast(new Ray(rayPos, -transform.up), out RaycastHit hit, Mathf.Infinity, _cellMask))
                    {
                        cellsGO.Add(hit.collider.gameObject);
                    }
                }
            }

            return cellsGO;
        }

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

                transform.position = centerPos / CurrentMatrix.Column / CurrentMatrix.Row + transform.up * _cellSize / 2;

                // TODO:本当に建築するか確認する



                // TODO:フェンスを建てる



                Constructed = true;
            }
            // 建築可能でなかったとき
            else
            {
                // 削除する
                Destroy(gameObject);
            }
        }

        public void UpdateConstructionCell()
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

        /// <summary>
        /// 建築物を売る
        /// </summary>
        private void SellConstructable()
        {

        }

        // ------------------------------------------------------------------------------------------
        // Interactableインターフェース
        // ------------------------------------------------------------------------------------------
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (!Interactable) return;

            Interactable = false;

            // UIを表示
            if (Camera.main.WorldToScreenPoint(transform.position).x > Screen.width / 2)
            {
                _upgradeCanvasRT.anchoredPosition = new Vector2(-Screen.width * 0.75f, 0);
                _upgradeCanvasRT
                    .DOAnchorPosX(-Screen.width / 4, 0.3f)
                    .SetLink(gameObject);
            }
            else
            {
                _upgradeCanvasRT.anchoredPosition = new Vector2(Screen.width * 0.75f, 0);
                _upgradeCanvasRT
                    .DOAnchorPosX(Screen.width / 4, 0.3f)
                    .SetLink(gameObject);
            }
            _disableButton.Interactable = true;
            _infoCanvas.gameObject.SetActive(true);
            foreach (var scroll in GetComponentsInChildren<ScrollRect>(true))
            {
                scroll.verticalNormalizedPosition = 1.0f;
            }
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