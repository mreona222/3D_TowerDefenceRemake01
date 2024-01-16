using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Construction;
using TowerDefenseRemake.Turret;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefenseRemake.UI
{
    public class GenerateTurretButton : BaseButton
    {
        private enum DragState
        {
            Judge,
            Scroll,
            Drag,
        }
        private DragState _currentDragState;



        public TurretType Type { get; set; }

        [BoxGroup("タレット")]
        [SerializeField]
        TurretList _turretList;
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
        private Action _onEnterDragAction;
        private Action _onExitDragAction;


        void Start()
        {
            // GetComponentを必要以上に呼ばないよう事前にキャッシュする
            scrollRect = GetComponentInParent<ScrollRect>();
        }


        // ---------------------------------------------------------------------------------------------------------
        // コールバック
        // ---------------------------------------------------------------------------------------------------------
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            _currentDragState = DragState.Judge;
            downPosition = eventData.position;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

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

                            _onEnterDragAction.Invoke();

                            // タレット生成
                            _turretInst = Instantiate(_turretList.Turret[(int)Type].TurretPrefab);

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
                                _turretInst.UpdateConstructable();

                                _turretInst.transform.position = hit.point;
                            }
                        }
                    }
                    break;
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            _onExitDragAction.Invoke();

            // タレットを配置する
            if (_turretInst != null)
            {
                _turretInst.UpdateConstructable();

                _turretInst.Construct();
            }

            _turretInst = null;
        }


        public void SetCallbackOnEnterDrag(Action callback)
        {
            _onEnterDragAction += callback;
        }

        public void SetCallbackOnExitDrag(Action callback)
        {
            _onExitDragAction += callback;
        }
    }
}