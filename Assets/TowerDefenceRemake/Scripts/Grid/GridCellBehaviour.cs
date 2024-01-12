using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TowerDefenseRemake.Grid
{
    public enum CellType
    {
        None,
        Road,
        Garden,
        Start,
        Goal,
    }

    [ExecuteAlways]
    public class GridCellBehaviour : MonoBehaviour
    {
        [BoxGroup("ステート")]
        [SerializeField]
        private bool _constructableExist;
        public bool ConstructableExist
        {
            get => _constructableExist;
            set => _constructableExist = value;
        }



        private enum CellMat
        {
            Road,
            Outline,
        }

        [BoxGroup("見た目")]
        [SerializeField]
        private Color _interactableColor = Color.green;
        public Color InteractableColor => _interactableColor;

        [BoxGroup("見た目")]
        [SerializeField]
        private Color _uninteractableColor = Color.red;
        public Color UninteractableColor => _uninteractableColor;

        [BoxGroup("見た目")]
        [SerializeField]
        private float _intensity = 25.0f;
        public float Intensity => _intensity;

        private MeshRenderer _MR;
        private Material[] _mat;




        [BoxGroup("タイプ")]
        [SerializeField]
        [EnumToggleButtons]
        [OnValueChanged(nameof(OnChangeGridType))]
        private CellType _type = CellType.None;

        private readonly static string[] Direction = { "N", "S", "E", "W" };

        [BoxGroup("タイプ")]
        [SerializeField]
        [ShowIf(nameof(_type), CellType.Road)]
        [ValueDropdown(nameof(Direction), IsUniqueList = true)]
        [OnValueChanged(nameof(OnChangeRoadDirection))]
        private string[] _roadDirection;

        [BoxGroup("タイプ")]
        [SerializeField]
        [ShowIf(nameof(_type), CellType.Start)]
        [ValueDropdown(nameof(Direction))]
        [OnValueChanged(nameof(OnChangeStartDirection))]
        private string _startDirection;

        [BoxGroup("タイプ")]
        [SerializeField]
        [ShowIf(nameof(_type), CellType.Goal)]
        [ValueDropdown(nameof(Direction))]
        [OnValueChanged(nameof(OnChangeGoalDirection))]
        private string _goalDirection;




        private void Start()
        {
            _MR = GetComponent<MeshRenderer>();

            _mat = new Material[_MR.sharedMaterials.Length];
#if UNITY_EDITOR
            for (int i = 0; i < _MR.sharedMaterials.Length; i++)
            {
                if (_mat[i] == null)
                {
                    if (EditorApplication.isPlaying)
                    {
                        _mat[i] = _MR.materials[i];
                    }
                    else
                    {
                        _mat[i] = new Material(_MR.sharedMaterials[i]);
                    }
                }
            }

            if (!EditorApplication.isPlaying) 
            { 
                _MR.sharedMaterials = _mat; 
            }
#else
            _mat = _MR.materials;
#endif
        }

        private void OnDestroy()
        {
            foreach (Material mat in _mat)
            {
                if(mat != null)
                {
#if UNITY_EDITOR
                    if (EditorApplication.isPlaying)
                    {
                        Destroy(mat);
                    }
                    else
                    {
                        DestroyImmediate(mat);
                    }
#else
                    Destroy(mat);
#endif
                }
            }
        }

        public void BrightenCell(Color color, float intensity)
        {
            _mat[(int)CellMat.Outline].SetColor("_OutlineColor", color * intensity);
        }



        // ---------------------------------
        // Editor
        // ---------------------------------

        /// <summary>
        /// インスペクターでGridの種類を変えたとき
        /// </summary>
        void OnChangeGridType()
        {
            // リセット
            for(int i = 0; i < transform.childCount; i++)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            _mat[(int)CellMat.Road].SetFloat("_N", 0);
            _mat[(int)CellMat.Road].SetFloat("_S", 0);
            _mat[(int)CellMat.Road].SetFloat("_E", 0);
            _mat[(int)CellMat.Road].SetFloat("_W", 0);

            // 初期設定
            switch (_type)
            {
                case CellType.None:
                    // レイヤーをNotRoadに変更
                    gameObject.layer = LayerMask.NameToLayer("NotRoad");
                    // インタラクト
                    ConstructableExist = false;
                    break;
                case CellType.Road:
                    // レイヤーをRoadに変更
                    gameObject.layer = LayerMask.NameToLayer("Road");
                    // インタラクト
                    ConstructableExist = true;
                    OnChangeRoadDirection();
                    break;
                case CellType.Garden:
                    // TODO:Garden生成

                    // レイヤーをNotRoadに変更
                    gameObject.layer = LayerMask.NameToLayer("NotRoad");
                    // インタラクト
                    ConstructableExist = true;
                    break;
                case CellType.Start:
                    // TODO:Spawner生成

                    // レイヤーをNotRoadに変更
                    gameObject.layer = LayerMask.NameToLayer("NotRoad");
                    // インタラクト
                    ConstructableExist = true;
                    OnChangeStartDirection();
                    break;
                case CellType.Goal:
                    // TODO:Goal生成

                    // レイヤーをNotRoadに変更
                    gameObject.layer = LayerMask.NameToLayer("NotRoad");
                    // インタラクト
                    ConstructableExist = true;
                    OnChangeGoalDirection();
                    break;
            }
        }

        /// <summary>
        /// インスペクターで道の方向を変えたとき
        /// </summary>
        void OnChangeRoadDirection()
        {
            // 道を表示
            if (_roadDirection.Contains("N")) { _mat[(int)CellMat.Road].SetFloat("_N", 1.0f); }
            else { _mat[(int)CellMat.Road].SetFloat("_N", 0); }

            if (_roadDirection.Contains("S")) { _mat[(int)CellMat.Road].SetFloat("_S", 1.0f); }
            else { _mat[(int)CellMat.Road].SetFloat("_S", 0); }

            if (_roadDirection.Contains("E")) { _mat[(int)CellMat.Road].SetFloat("_E", 1.0f); }
            else { _mat[(int)CellMat.Road].SetFloat("_E", 0); }

            if (_roadDirection.Contains("W")) { _mat[(int)CellMat.Road].SetFloat("_W", 1.0f); }
            else { _mat[(int)CellMat.Road].SetFloat("_W", 0); }
        }

        void OnChangeStartDirection()
        {
            switch (_startDirection)
            {
                case "N":
                    break;       
                case "S":
                    break;
                case "E":
                    break;
                case "W":
                    break;
                default:
                    break;
            }
        }

        void OnChangeGoalDirection()
        {
            switch (_goalDirection)
            {
                case "N":
                    break;
                case "S":
                    break;
                case "E":
                    break;
                case "W":
                    break;
                case "None":
                    break;
                default:
                    break;
            }
        }
    }
}