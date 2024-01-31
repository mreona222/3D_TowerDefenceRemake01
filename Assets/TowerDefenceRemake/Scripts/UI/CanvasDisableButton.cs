using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Template.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.UI
{
    public class CanvasDisableButton : ButtonBase
    {
        public new bool Interactable
        {
            get => _interactable;
            set => _interactable = value;
        }

        [SerializeField]
        private Canvas _canvsRoot;

        private event Func<UniTask> _onClick;
        public event Func<UniTask> OnClick
        {
            add { _onClick += value; }
            remove { _onClick -= value; }
        }


        protected override void OnPointerClickInternal(PointerEventData eventData)
        {
            Interactable = false;

            UniTask.Create(async () =>
            {
                await _onClick.Invoke();

                if(_canvsRoot != null)
                {
                    _canvsRoot.gameObject.SetActive(false);
                }
            });
        }

        protected override void OnPointerDownInternal(PointerEventData eventData)
        {

        }

        protected override void OnPointerEnterInternal(PointerEventData eventData)
        {

        }

        protected override void OnPointerExitInternal(PointerEventData eventData)
        {

        }
    }
}