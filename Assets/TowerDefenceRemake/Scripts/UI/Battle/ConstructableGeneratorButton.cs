using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Template.UI;
using TowerDefenseRemake.Constructable;
using TowerDefenseRemake.Constructable.Turret;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefenseRemake.UI
{
    public class ConstructableGeneratorButton : ButtonBase
    {
        private enum DragState
        {
            Judge,
            Scroll,
            Drag,
        }
        private DragState _currentDragState;



        public ConstructableName Type { get; set; }

        [BoxGroup("タレット")]
        [SerializeField]
        ConstructableList _constructableList;
        TurretBehaviourBase _turretInst;

        [BoxGroup("タレット")]
        [SerializeField]
        LayerMask _cellLayerMask;



        /// しきい値計算に使用するダウン時の座標
        private Vector2 downPosition;

        /// ScrollRectへドラッグを切り替えるしきい値
        [SerializeField]
        private float changeScrollDragThreshold = 75.0f;

        /// スクロール対象となるScrollRect(基本的に親）
        private ScrollRect scrollRect = null;


        // コールバック
        private event Action _onEnterDragAction;
        public event Action OnEnterDragAction
        {
            add { _onEnterDragAction += value; }
            remove { _onEnterDragAction -= value; }
        }

        private event Action _onExitDragAction;
        public event Action OnExitDragAction
        {
            add { _onExitDragAction += value; }
            remove { _onExitDragAction -= value; }
        }

        // タレットの回転
        public bool Rotate { get; set; }


        protected override void Start()
        {
            base.Start();

            // GetComponentを必要以上に呼ばないよう事前にキャッシュする
            scrollRect = GetComponentInParent<ScrollRect>();
        }


        // ---------------------------------------------------------------------------------------------------------
        // コールバック
        // ---------------------------------------------------------------------------------------------------------
        protected override void OnPointerDownInternal(PointerEventData eventData)
        {
            base.OnPointerDownInternal(eventData);

            _currentDragState = DragState.Judge;
            downPosition = eventData.position;
        }

        protected override void OnDragInternal(PointerEventData eventData)
        {
            base.OnDragInternal(eventData);

            switch (_currentDragState)
            {
                case DragState.Judge:
                    {
                        // 横方向に動いたらスクロール
                        if (Mathf.Abs(downPosition.x - eventData.position.x) > changeScrollDragThreshold)
                        {
                            _currentDragState = DragState.Scroll;
                        }
                        // 縦方向に動いたらドラッグ
                        else if (eventData.position.y - downPosition.y > changeScrollDragThreshold)
                        {
                            _currentDragState = DragState.Drag;

                            _onEnterDragAction?.Invoke();

                            // タレット生成
                            _turretInst = Instantiate(_constructableList.ConstructablePrefabs[(int)Type]);

                            if (Rotate)
                            {
                                _turretInst.CurrentMatrix = _turretInst.Info.Matrix.InverseMatrix();
                            }
                            else
                            {
                                _turretInst.CurrentMatrix = _turretInst.Info.Matrix;
                            }

                            // タレットの移動
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _cellLayerMask))
                            {
                                if (_turretInst != null)
                                {
                                    _turretInst.transform.position = hit.point;
                                }
                            }
                        }
                    }
                    break;

                case DragState.Scroll:
                    {
                        //選択されているオブジェクトをScrollRectへ変更する            
                        eventData.pointerDrag = scrollRect.gameObject;
                        EventSystem.current.SetSelectedGameObject(scrollRect.gameObject);

                        //ドラッグの初期化
                        scrollRect.OnInitializePotentialDrag(eventData);
                        //次のフレームからIdragHandlerの呼び出しが始まるのでBeginさせる
                        scrollRect.OnBeginDrag(eventData);
                    }
                    break;

                case DragState.Drag:
                    {
                        // タレットの移動
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _cellLayerMask))
                        {
                            if (_turretInst != null)
                            {
                                _turretInst.UpdateConstructionCell();

                                _turretInst.transform.position = hit.point;
                            }
                        }
                    }
                    break;
            }
        }

        protected override void OnEndDragInternal(PointerEventData eventData)
        {
            base.OnEndDragInternal(eventData);

            _onExitDragAction?.Invoke();

            // タレットを配置する
            if (_turretInst != null)
            {
                _turretInst.UpdateConstructionCell();

                _turretInst.Construct();
            }

            _turretInst = null;
        }
    }
}