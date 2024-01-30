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

        protected event Func<ParamType, int, float> _onUpdateCurrentParams;
        public event Func<ParamType, int, float> OnUpdateCurrentParams
        {
            add { _onUpdateCurrentParams += value; }
            remove { _onUpdateCurrentParams -= value; }
        }

        private Func<ParamType, int, (float nextValue, float cost)> _onUpdateNextParams;
        public event Func<ParamType, int, (float nextValue, float cost)> OnUpdateNextParams
        {
            add { _onUpdateNextParams += value; }
            remove { _onUpdateNextParams -= value; }
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
                button.OnClickButton += UpdateCurrentParams;
                button.OnClickButton += UpdateNextParams;

                button.OnEnterButton += UpdateNextParams;

                button.OnExitButton += UpdateNextParams;
            }
        }

        private void Initialize()
        {
            UpdateCurrentParams(0);
            UpdateNextParams(0);
        }

        private void UpdateCurrentParams(int raiseLevel)
        {
            var result = _onUpdateCurrentParams?.Invoke(ContentType, raiseLevel);
            _currentValue.text = $"{result:N2}";
        }

        private void UpdateNextParams(int raiseLevel)
        {
            var result = _onUpdateNextParams?.Invoke(ContentType, raiseLevel);

            _nextValue.text = $"{result.Value.nextValue:N2}";
            _cost.text = $"{result.Value.cost:N0}";
        }


        public void OnValueChanged()
        {
            DropdownType = (DropdownTypeEnum)_dropdown.value;

            Initialize();
        }
    }
}