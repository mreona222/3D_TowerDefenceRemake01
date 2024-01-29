using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Constructable;
using TowerDefenseRemake.Manager;
using UnityEngine;

namespace TowerDefenseRemake.UI
{
    public class ConstructableUpgradeContent : MonoBehaviour
    {
        [SerializeField]
        ConstructableUpgradeButton[] _buttons;

        public ParamType ContentType { get; set; }

        protected event Action<ParamType, int> _onClickButton;
        public event Action<ParamType, int> OnClickButton
        {
            add { _onClickButton += value; }
            remove { _onClickButton -= value; }
        }

        protected virtual void Start()
        {
            SetCallback();
        }

        private void SetCallback()
        {
            foreach (ConstructableUpgradeButton button in _buttons)
            {
                button.OnClickButton += Upgrade;
            }
        }

        protected virtual void Upgrade(int raiseLevel)
        {
            _onClickButton?.Invoke(ContentType, raiseLevel);
        }
    }
}