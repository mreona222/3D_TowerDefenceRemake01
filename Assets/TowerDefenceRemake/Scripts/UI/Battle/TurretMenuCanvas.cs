using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Grid;
using TowerDefenseRemake.Turret;
using UnityEngine;

namespace TowerDefenseRemake.UI
{
    public class TurretMenuCanvas : MonoBehaviour
    {
        [SerializeField]
        private TurretGeneratorButton _generateButtonPrefab;

        [SerializeField]
        private TurretMenuHandleButton _handle;

        [SerializeField]
        private Transform _content;

        GridCellBehaviour[] _allCells;

        private void Start()
        {
            _allCells = FindObjectsOfType<GridCellBehaviour>();

            GenerateButtons();
        }

        void GenerateButtons()
        {
            for (int i = 0; i < System.Enum.GetValues(typeof(TurretType)).Length; i++)
            {
                // ボタンを生成
                TurretGeneratorButton buttonInst = Instantiate(_generateButtonPrefab, _content);
                buttonInst.Type = (TurretType)i;

                // ドラッグ開始コールバック
                buttonInst.OnEnterDragAction += () =>
                {
                    // メニューを非表示
                    if (_handle != null)
                    {
                        _handle.HideMenu();
                    }

                    // セルを光らせる
                    foreach (GridCellBehaviour cell in _allCells)
                    {
                        if (cell.ConstructableExist)
                        {
                            cell.ChangeOutlineExistColor();
                        }
                        else
                        {
                            cell.ChangeOutlineUnExistColor();
                        }
                    }
                };

                // ドラッグ終了コールバック
                buttonInst.OnExitDragAction += () =>
                {
                    // セルをデフォルトカラーに
                    foreach (GridCellBehaviour cell in _allCells)
                    {
                        cell.ChangeOutlineDefaultColor();
                    }
                };
            }
        }
    }
}