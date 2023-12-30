using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TowerDefenseRemake.Grid
{
    [ExecuteAlways]
    public class GridCellGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject _gridCell;

        [SerializeField]
        private int row, column;

        private List<GameObject> _gridCellInst = new List<GameObject>();

        [Button]
        public void GenerateGridCell()
        {
            if (_gridCellInst.Count != 0)
            {
                foreach(GameObject go in _gridCellInst)
                {
                    DestroyImmediate(go);
                }
                _gridCellInst.Clear();
            }

            for(int i = 0; i < column; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    _gridCellInst.Add(Instantiate(_gridCell,
                        new Vector3(
                            _gridCell.transform.localScale.x / 2 * (-row + 1) + _gridCell.transform.localScale.x * j,
                            0,
                            _gridCell.transform.localScale.z / 2 * (-column + 1) + _gridCell.transform.localScale.z * i),
                        Quaternion.identity,
                        transform));
                }
            }
        }
    }
}