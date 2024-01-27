using Cysharp.Threading.Tasks;
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
        private ConstructableGeneratorButton _generateButtonPrefab;

        [SerializeField]
        private ConstructableMenuHandleButton _handle;

        [SerializeField]
        private Transform _content;

        GridCellBehaviour[] _allCells;

        [SerializeField]
        ConstructableRotateButton _rotateButton;

        private void Start()
        {
            _allCells = FindObjectsOfType<GridCellBehaviour>();

            GenerateButtons();
        }

        void GenerateButtons()
        {
            for (int i = 0; i < System.Enum.GetValues(typeof(ConstructableType)).Length; i++)
            {
                // ボタンを生成
                ConstructableGeneratorButton buttonInst = Instantiate(_generateButtonPrefab, _content);
                buttonInst.Type = (ConstructableType)i;

                // ドラッグ開始コールバック
                buttonInst.OnEnterDragAction += () =>
                {
                    // メニューを非表示
                    if (_handle != null)
                    {
                        UniTask.Create(async () => await _handle.HideMenu());
                    }

                    // 回転情報
                    buttonInst.Rotate = _rotateButton.Rotate;

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