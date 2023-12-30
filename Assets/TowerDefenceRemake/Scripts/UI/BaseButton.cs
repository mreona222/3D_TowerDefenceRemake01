using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TowerDefenseRemake.UI {
    public class BaseButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
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

        [Space(10)]
        [SerializeField]
        private BaseButton[] baseButtons;

        private Image _image;

        [Space(10)]
        [SerializeField]
        Color _disableColor = new Color32(100, 100, 100, 200);
        [SerializeField]
        Color _clickColor = new Color32(200, 0, 0, 255);
        [SerializeField]
        Color _downColor = new Color32(255, 0, 0, 255);
        [SerializeField]
        Color _enterColor = new Color32(0, 255, 0, 255);
        [SerializeField]
        Color _exitColor = new Color32(255, 255, 255, 255);


        // コールバック
        protected Action onClick;
        protected Action onDown;
        protected Action onEnter;
        protected Action onExit;

        private void Start()
        {
            _image = GetComponent<Image>();

            onClick += OnClick;
            onDown += OnDown;
            onEnter += OnEnter;
            onExit += OnExit;
        }

        private void OnDestroy()
        {
            onClick -= OnClick;
            onDown -= OnDown;
            onEnter -= OnEnter;
            onExit -= OnExit;
        }

        void IPointerClickHandler.OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (Interactable)
            {
                onClick.Invoke();
            }
        }

        void IPointerDownHandler.OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (Interactable)
            {
                onDown.Invoke();
            }
        }

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (Interactable)
            {
                onEnter.Invoke();
            }
        }

        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (Interactable)
            {
                onExit.Invoke();
            }
        }

        protected virtual void OnClick()
        {
            foreach (BaseButton button in baseButtons)
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

        protected virtual void OnDown()
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

        protected virtual void OnEnter()
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

        protected virtual void OnExit()
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
    }
}