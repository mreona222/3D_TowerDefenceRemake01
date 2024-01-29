using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.UI
{
    public class ConstructableRotateButton : ButtonBase
    {
        private event Action _onClick;
        public event Action OnClick
        {
            add { _onClick += value; }
            remove { _onClick -= value; }
        }

        protected override void OnPointerClickInternal(PointerEventData eventData)
        {
            base.OnPointerClickInternal(eventData);

            _onClick?.Invoke();
        }
    }
}