using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Turret;
using UnityEngine;

namespace TowerDefenseRemake.UI
{
    public class TurretMenuCanvas : MonoBehaviour
    {
        [SerializeField]
        private GenerateTurretButton _generateButtonPrefab;

        [SerializeField]
        private TurretMenuHandleButton _handle;

        [SerializeField]
        private Transform _content;

        private void Start()
        {
            GenerateButtons();
        }

        void GenerateButtons()
        {
            for (int i = 0; i < System.Enum.GetValues(typeof(TurretType)).Length; i++)
            {
                GenerateTurretButton buttonInst = Instantiate(_generateButtonPrefab, _content);
                buttonInst.Type = (TurretType)i;
                buttonInst.Handle = _handle;
            }
        }
    }
}