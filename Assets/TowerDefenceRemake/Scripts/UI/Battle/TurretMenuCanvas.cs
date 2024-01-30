using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Constructable;
using TowerDefenseRemake.Grid;
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
            for (int i = 0; i < System.Enum.GetValues(typeof(ConstructableName)).Length; i++)
            {
                // ボタンを生成
                ConstructableGeneratorButton buttonInst = Instantiate(_generateButtonPrefab, _content);
                buttonInst.Type = (ConstructableName)i;

                // 回転情報
                _rotateButton.OnClick += () =>
                {
                    buttonInst.Rotate = !buttonInst.Rotate;
                };

                // ドラッグ開始コールバック
                buttonInst.OnEnterDragAction += () =>
                {
                    // メニューを非表示
                    if (_handle != null)
                    {
                        UniTask.Create(async () => await _handle.HideMenu());
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