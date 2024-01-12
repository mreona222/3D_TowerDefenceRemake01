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
        private int row, column;

#if UNITY_EDITOR
        [Button]
        public void GenerateGridCell()
        {
            //if (_gridCellInst.Count != 0)
            //{
            //    foreach(GameObject go in _gridCellInst)
            //    {
            //        DestroyImmediate(go);
            //    }
            //    _gridCellInst.Clear();
            //}

            DestroyAllCells();

            GameObject gridCell = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/TowerDefenceRemake/Prefabs/Grid/Grid.prefab");

            for (int j = 0; j < column; j++)
            {
                for (int i = 0; i < row; i++)
                {
                    GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(gridCell);

                    go.transform.position = new Vector3(
                            gridCell.transform.localScale.x / 2 * (-row + 1) + gridCell.transform.localScale.x * i,
                            transform.position.y,
                            gridCell.transform.localScale.z / 2 * (column - 1) - gridCell.transform.localScale.z * j);

                    go.transform.SetParent(transform, true);
                }
            }
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
#endif
    }
}