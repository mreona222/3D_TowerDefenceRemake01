using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.UI
{
    public class TurretMenuHandleButton : BaseButton
    {
        [SerializeField]
        bool _hide = true;

        [SerializeField]
        RectTransform _panel;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            // 隠れていたら
            if (_hide)
            {
                // 展開
                ExpandMenu();
            }
            // 隠れていなかったら
            else
            {
                // 隠す
                HideMenu();
            }
        }

        public void ExpandMenu()
        {
            _hide = false;

            _panel.DOAnchorPosY(_panel.sizeDelta.y / 2, 0.3f);
        }

        public void HideMenu()
        {
            _hide = true;

            _panel.DOAnchorPosY(-_panel.sizeDelta.y / 2 + 10.0f, 0.3f);
        }
    }
}