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
        private event Action<int> _onClickButton;
        public event Action<int> OnClickButton
        {
            add { _onClickButton += value; }
            remove { _onClickButton -= value; }
        }

        [SerializeField]
        [ValueDropdown(nameof(Level))]
        private int _raiseLevel;

        private readonly static int[] Level = { 1, 10, 100, 1000, };

        protected override void OnPointerClickInternal(PointerEventData eventData)
        {
            base.OnPointerClickInternal(eventData);

            _onClickButton?.Invoke(_raiseLevel);
        }
    }
}