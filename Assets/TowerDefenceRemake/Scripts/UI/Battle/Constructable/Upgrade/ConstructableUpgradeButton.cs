using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Template.UI;
using TowerDefenseRemake.Constructable;
using TowerDefenseRemake.Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.UI
{
    public class ConstructableUpgradeButton : ButtonBase
    {
        [SerializeField]
        [ValueDropdown(nameof(Level))]
        private int _raiseLevel;
        public int RaiseLevel => _raiseLevel;

        public readonly static int[] Level = { 1, 10, 100, 1000, };

        public float CurrentValue { get; set; }
        public float NextValue { get; set; }

        public float CurrentDPS { get; set; }
        public float NextDPS { get; set; }

        public float Coin { get; set; }

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

        protected override void Start()
        {
            base.Start();

            ((InstanceManagerBattle)GameManager.Instance._IM).OnCoinChanged += ChangeInteractable;

            ChangeInteractable(((InstanceManagerBattle)GameManager.Instance._IM).Coin.Value);
        }

        // -------------------------------------------------------

        protected override void OnPointerClickInternal(PointerEventData eventData)
        {
            base.OnPointerClickInternal(eventData);

            _onClickButton?.Invoke(RaiseLevel);
        }

        protected override void OnPointerEnterInternal(PointerEventData eventData)
        {
            base.OnPointerEnterInternal(eventData);

            _onEnterButton?.Invoke(RaiseLevel);
        }

        protected override void OnPointerExitInternal(PointerEventData eventData)
        {
            base.OnPointerExitInternal(eventData);

            _onExitButton?.Invoke(0);
        }

        // -------------------------------------------------------

        void ChangeInteractable(int coin)
        {
            if (coin < Coin)
            {
                Interactable = false;
            }
            else
            {
                Interactable = true;
            }
        }
    }
}