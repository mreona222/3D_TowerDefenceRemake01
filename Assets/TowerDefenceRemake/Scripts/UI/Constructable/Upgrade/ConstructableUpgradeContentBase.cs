using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Manager;
using UnityEngine;

namespace TowerDefenseRemake.UI
{
    public abstract class ConstructableUpgradeContentBase : MonoBehaviour
    {
        public enum ContentType
        {
            Power,
            Range,
            Span,
        }

        [SerializeField]
        ConstructableUpgradeButton[] _buttons;

        private event Action<int> _onClickButton;
        public event Action<int> OnClickButton
        {
            add { _onClickButton += value; }
            remove { _onClickButton -= value; }
        }

        private void Start()
        {
            foreach (var button in _buttons)
            {
                button.OnClickButton += _onClickButton;
            }
        }
    }
}