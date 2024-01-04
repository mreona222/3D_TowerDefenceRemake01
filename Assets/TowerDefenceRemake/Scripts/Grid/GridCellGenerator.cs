using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TowerDefenseRemake.Grid
{
    public class GridCellGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject _gridCell;

        [SerializeField]
        private int row, column;

        [Button]
        public void GenerateGridCell()
        {
#if UNITY_EDITOR
            //if (_gridCellInst.Count != 0)
            //{
            //    foreach(GameObject go in _gridCellInst)
            //    {
            //        DestroyImmediate(go);
            //    }
            //    _gridCellInst.Clear();
            //}

            DestroyAllCells();

            _gridCell = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TowerDefenceRemake/Prefabs/Grid/Grid.prefab");

            for (int i = 0; i < column; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(_gridCell);

                    go.transform.position = new Vector3(
                            _gridCell.transform.localScale.x / 2 * (-row + 1) + _gridCell.transform.localScale.x * j,
                            transform.position.y,
                            _gridCell.transform.localScale.z / 2 * (column - 1) - _gridCell.transform.localScale.z * i);

                    go.transform.SetParent(transform, true);
                }
            }
#endif
        }

        private void DestroyAllCells()
        {
            GameObject[] objs = new GameObject[transform.childCount];

            for (int i = 0; i < transform.childCount; i++)
            {
                objs[i] = transform.GetChild(i).gameObject;
            }

            foreach (GameObject obj in objs)
            {
                DestroyImmediate(obj);
            }
        }
    }
}