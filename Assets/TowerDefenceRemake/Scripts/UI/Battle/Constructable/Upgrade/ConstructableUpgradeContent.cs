using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TowerDefenseRemake.Constructable;
using TowerDefenseRemake.Manager;
using UnityEngine;

namespace TowerDefenseRemake.UI
{
    public class ConstructableUpgradeContent : MonoBehaviour
    {
        [SerializeField]
        ConstructableUpgradeButton[] _buttons;
        public ConstructableUpgradeButton[] Buttons => _buttons;

        [SerializeField]
        TextMeshProUGUI _currentValue;

        [SerializeField]
        TextMeshProUGUI _nextValue;

        [SerializeField]
        TextMeshProUGUI _cost;

        [SerializeField]
        TMP_Dropdown _dropdown;

        public ParamType ContentType { get; set; }

        public enum DropdownTypeEnum
        {
            Value,
            DPS,
        }
        public DropdownTypeEnum DropdownType { get; private set; }

        // ---------------------------------------------------------------

        private event Action<ParamType, int> _onUpdateParams;
        public event Action<ParamType, int> OnUpdateParams
        {
            add { _onUpdateParams += value; }
            remove { _onUpdateParams -= value; }
        }

        // ---------------------------------------------------------------

        protected virtual void Start()
        {
            SetCallback();

            Initialize();
        }

        // ---------------------------------------------------------------

        private void SetCallback()
        {
            foreach (ConstructableUpgradeButton button in _buttons)
            {
                button.OnClickButton += UpdateParams;
                button.OnClickButton += ChangeTextCurrent;
                button.OnClickButton += ChangeTextNext;

                button.OnEnterButton += ChangeTextNext;

                button.OnExitButton += ChangeTextNext;
            }
        }

        private void Initialize()
        {
            ChangeTextCurrent(0);
            ChangeTextNext(0);
        }

        private void UpdateParams(int raiseLevel)
        {
            _onUpdateParams?.Invoke(ContentType, raiseLevel);
        }

        public void ChangeTextCurrent(int raiseLevel)
        {
            switch (DropdownType)
            {
                case DropdownTypeEnum.Value:
                    _currentValue.text = $"{Buttons[0].CurrentValue:N2}";
                    break;

                case DropdownTypeEnum.DPS:
                    _currentValue.text = $"{Buttons[0].CurrentDPS:N2}";
                    break;
            }
        }

        public void ChangeTextNext(int raiseLevel)
        {
            int index = Array.IndexOf(ConstructableUpgradeButton.Level, raiseLevel);

            if (index == -1)
            {
                switch (DropdownType)
                {
                    case DropdownTypeEnum.Value:
                        _nextValue.text = $"{Buttons[0].CurrentValue:N2}";
                        _cost.text = $"{0:N0}";
                        break;

                    case DropdownTypeEnum.DPS:
                        _nextValue.text = $"{Buttons[0].CurrentDPS:N2}";
                        _cost.text = $"{0:N0}";
                        break;
                }
            }
            else
            {
                switch (DropdownType)
                {
                    case DropdownTypeEnum.Value:
                        _nextValue.text = $"{Buttons[index].NextValue:N2}";
                        _cost.text = $"{Buttons[index].Coin:N0}";
                        break;

                    case DropdownTypeEnum.DPS:
                        _nextValue.text = $"{Buttons[index].NextDPS:N2}";
                        _cost.text = $"{Buttons[index].Coin:N0}";
                        break;
                }
            }
        }

        public void OnValueChanged()
        {
            DropdownType = (DropdownTypeEnum)_dropdown.value;

            Initialize();
        }
    }
}