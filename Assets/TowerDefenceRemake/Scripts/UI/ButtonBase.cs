using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefenseRemake.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefenseRemake.UI 
{
    [RequireComponent(typeof(Image))]
    public class ButtonBase : MonoBehaviour, IInteractable
    {
        [BoxGroup("Base")]
        [SerializeField]
        protected bool _interactable = true;
        public bool Interactable
        {
            get => _interactable;
            set
            {
                _interactable = value;
                if (!_interactable)
                {
                    if (_image != null)
                    {
                        _image.color = _disableColor;
                    }
                }
            }
        }

        [BoxGroup("Base")]
        [Space(10)]
        [SerializeField]
        private ButtonBase[] buttons;

        protected Image _image;



        [BoxGroup("Color")]
        [SerializeField]
        Color _disableColor = new Color32(100, 100, 100, 200);

        [BoxGroup("Color")]
        [SerializeField]
        Color _clickColor = new Color32(200, 0, 0, 255);

        [BoxGroup("Color")]
        [SerializeField]
        Color _downColor = new Color32(255, 0, 0, 255);

        [BoxGroup("Color")]
        [SerializeField]
        Color _enterColor = new Color32(0, 255, 0, 255);

        [BoxGroup("Color")]
        [SerializeField]
        Color _exitColor = new Color32(255, 255, 255, 255);

        protected virtual void Start()
        {
            _image = GetComponent<Image>();
        }

        // ------------------------------------------------------------------------------------------------
        // コールバック
        // ------------------------------------------------------------------------------------------------
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Interactable) return;

            OnPointerClickInternal(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Interactable) return;

            OnPointerEnterInternal(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Interactable) return;

            OnPointerExitInternal(eventData);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Interactable) return;

            OnPointerDownInternal(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!Interactable) return;

            OnBeginDragInternal(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!Interactable) return;

            OnDragInternal(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!Interactable) return;

            OnEndDragInternal(eventData);
        }

        // ------------------------------------------------------------------------------------------------
        // コールバックInternal
        // ------------------------------------------------------------------------------------------------
        protected virtual void OnPointerClickInternal(PointerEventData eventData)
        {
            foreach (ButtonBase button in buttons)
            {
                button.Interactable = false;
            }

            if (_image != null)
            {
                _image.color = _clickColor;
                DOVirtual
                    .Float(0.9f, 1.0f, 0.1f, (value) =>
                    {
                        _image.GetComponent<RectTransform>().localScale = new Vector3(value, value, value);
                    })
                    .SetLink(gameObject);
            }
        }

        protected virtual void OnPointerEnterInternal(PointerEventData eventData)
        {
            if (_image != null)
            {
                _image.color = _enterColor;
                DOVirtual
                    .Float(1.0f, 1.1f, 0.1f, (value) =>
                    {
                        _image.GetComponent<RectTransform>().localScale = new Vector3(value, value, value);
                    })
                    .SetLink(gameObject);
            }
        }

        protected virtual void OnPointerExitInternal(PointerEventData eventData)
        {
            if (_image != null)
            {
                _image.color = _exitColor;
                DOVirtual
                    .Float(1.1f, 1.0f, 0.1f, (value) =>
                    {
                        _image.GetComponent<RectTransform>().localScale = new Vector3(value, value, value);
                    })
                    .SetLink(gameObject);
            }
        }

        protected virtual void OnDragInternal(PointerEventData eventData)
        {

        }

        protected virtual void OnPointerDownInternal(PointerEventData eventData)
        {
            if (_image != null)
            {
                _image.color = _downColor;
                DOVirtual
                    .Float(1.1f, 0.9f, 0.1f, (value) =>
                    {
                        _image.GetComponent<RectTransform>().localScale = new Vector3(value, value, value);
                    })
                    .SetLink(gameObject);
            }
        }

        protected virtual void OnBeginDragInternal(PointerEventData eventData)
        {

        }

        protected virtual void OnEndDragInternal(PointerEventData eventData)
        {

        }
    }
}