using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.UI
{
    public class ConstructableRotateButton : ButtonBase
    {
        [SerializeField]
        private bool _rotate;
        public bool Rotate
        {
            get => _rotate;
            set => _rotate = value;
        }

        protected override void OnPointerClickInternal(PointerEventData eventData)
        {
            base.OnPointerClickInternal(eventData);

            if (Rotate)
            {
                Rotate = false;
            }
            else
            {
                Rotate = true;
            }
        }
    }
}