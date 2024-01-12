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
        public TurretType Type { get; set; }

        [BoxGroup("タレット")]
        [SerializeField]
        TurretList _turretList;
        TurretBehaviourBase _turretInst;

        [BoxGroup("タレット")]
        [SerializeField]
        LayerMask _cellLayerMask;



        public TurretMenuHandleButton Handle { get; set; }


        // 自分がドラッグ対象か？
        private bool isSelfDrag = false;

        /// しきい値計算に使用するダウン時の座標
        private Vector2 downPosition;

        /// ScrollRectへドラッグを切り替えるしきい値
        [SerializeField]
        private float changeScrollDragThreshold = 10.0f;

        /// スクロール対象となるScrollRect(基本的に親）
        private ScrollRect scrollRect = null;


        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            if (isSelfDrag)
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

                return;
            }

            //長押し判定が行われる前に指定量動けばスクロールとみなす
            if ((downPosition - eventData.position).magnitude >= changeScrollDragThreshold)
            {
                //選択されているオブジェクトをScrollRectへ変更する            
                eventData.pointerDrag = scrollRect.gameObject;
                EventSystem.current.SetSelectedGameObject(scrollRect.gameObject);

                //ドラッグの初期化
                scrollRect.OnInitializePotentialDrag(eventData);
                //次のフレームからIdragHandlerの呼び出しが始まるのでBeginさせる
                scrollRect.OnBeginDrag(eventData);
            }

        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);

            // タレットを配置する
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _cellLayerMask))
            {
                if (_turretInst != null)
                {
                    _turretInst.ChangeCellDefaultColor();

                    if (_turretInst.Constructable)
                    {
                        _turretInst.Construct();
                    }
                    else
                    {
                        Destroy(_turretInst.gameObject);
                    }
                }
            }

            _turretInst = null;
        }


        /// ポインターダウン処理
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            isSelfDrag = false;
            downPosition = eventData.position;
        }

        /// 長押し
        private void OnLongClick(Unit unit)
        {
            if (!Interactable) return;

            //長押し判定後は自分がドラッグ対象とする
            isSelfDrag = true;

            // タレットメニュー非表示
            if(Handle != null)
            {
                Handle.HideMenu();
            }

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



        // Start is called before the first frame update
        void Start()
        {
            //GetComponentを必要以上に呼ばないよう事前にキャッシュする
            scrollRect = GetComponentInParent<ScrollRect>();

            //長押し判定(0.5秒)
            var eventTrigger = this.gameObject.AddComponent<ObservableEventTrigger>();

            eventTrigger.OnPointerDownAsObservable().Select(_ => true)
                .Merge(eventTrigger.OnPointerUpAsObservable().Select(_ => false))
                .Throttle(TimeSpan.FromSeconds(0.5f))
                .Where(x => x && scrollRect.velocity.magnitude <= 10.0f)
                .AsUnitObservable()
                .TakeUntilDestroy(this)
                .Subscribe(OnLongClick);

        }
    }
}