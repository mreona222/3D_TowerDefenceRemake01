using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Constructable;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.UI
{
    public class ConstructableUpgradeButton : ButtonBase
    {
        [SerializeField]
        [ValueDropdown(nameof(Level))]
        private int _raiseLevel;

        private readonly static int[] Level = { 1, 10, 100, 1000, };

        // -------------------------------------------------------

        private event Action<int> _onClickButton;
        public event Action<int> OnClickButton
        {
            add { _onClickButton += value; }
            remove { _onClickButton -= value; }
        }

        private event Action<int> _onEnterButton;
        public event Action<int> OnEnterButton
        {
            add { _onEnterButton += value; }
            remove { _onEnterButton -= value; }
        }

        private event Action<int> _onExitButton;
        public event Action<int> OnExitButton
        {
            add { _onExitButton += value; }
            remove { _onExitButton -= value; }
        }

        // -------------------------------------------------------

        protected override void OnPointerClickInternal(PointerEventData eventData)
        {
            base.OnPointerClickInternal(eventData);

            _onClickButton?.Invoke(_raiseLevel);
        }

        protected override void OnPointerEnterInternal(PointerEventData eventData)
        {
            base.OnPointerEnterInternal(eventData);

            _onEnterButton?.Invoke(_raiseLevel);
        }

        protected override void OnPointerExitInternal(PointerEventData eventData)
        {
            base.OnPointerExitInternal(eventData);

            _onExitButton?.Invoke(0);
        }
    }
}