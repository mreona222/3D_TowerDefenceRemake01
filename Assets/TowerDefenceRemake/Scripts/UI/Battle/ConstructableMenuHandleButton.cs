using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.UI
{
    public class ConstructableMenuHandleButton : ButtonBase
    {
        [SerializeField]
        bool _hide = true;

        [SerializeField]
        RectTransform _panel;

        [SerializeField]
        CanvasDisableButton _disableButton;

        protected override void Start()
        {
            base.Start();

            _disableButton.OnClick += async () =>
            {
                await HideMenu();
            };
        }

        protected override void OnPointerClickInternal(PointerEventData eventData)
        {
            base.OnPointerClickInternal(eventData);

            // 隠れていたら
            if (_hide)
            {
                // 展開
                UniTask.Create(async () => await ExpandMenu());
            }
            // 隠れていなかったら
            else
            {
                // 隠す
                UniTask.Create(async () => await HideMenu());
            }
        }

        public async UniTask ExpandMenu()
        {
            _hide = false;

            await _panel.DOAnchorPosY(_panel.sizeDelta.y / 2, 0.3f).SetLink(gameObject);

            _disableButton.gameObject.SetActive(true);
            _disableButton.Interactable = true;
        }

        public async UniTask HideMenu()
        {
            _hide = true;

            await _panel.DOAnchorPosY(-_panel.sizeDelta.y / 2 + 10.0f, 0.3f).SetLink(gameObject);

            _disableButton.gameObject.SetActive(false);
        }
    }
}