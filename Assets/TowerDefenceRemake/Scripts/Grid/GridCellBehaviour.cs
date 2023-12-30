using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TowerDefenseRemake.Grid
{
    [ExecuteAlways]
    public class GridCellBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private MeshRenderer _MR;
        //private Material _mat[0];
        //private Material _outlineMaterial;

        private Material[] _mat;

        [Space(10)]
        [SerializeField]
        [PropertyOrder(0)]
        [OnValueChanged(nameof(OnChangeGridType))]
        private bool _interactable;

        public bool Interactable
        {
            get => _interactable;
            set => _interactable = value;
        }

        [SerializeField]
        [ShowIf("@!Interactable")]
        [PropertyOrder(1)]
        [OnValueChanged(nameof(OnChangeRoadDirection))]
        private bool _N, _S, _E, _W;

        [Space(10)]
        [SerializeField]
        [PropertyOrder(2)]
        private bool _constructableExist;
        public bool ConstructableExist
        {
            get => _constructableExist;
            set => _constructableExist = value;
        }

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

        // ------------------------------------------------------------------------
        // コールバック
        // ------------------------------------------------------------------------
        void IPointerClickHandler.OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (_N || _S || _E || _W)
            {
                _N = false;
                _S = false;
                _E = false;
                _W = false;
            }

            if (!Interactable) return;

            // UI表示

        }

        void IPointerEnterHandler.OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            if (Interactable)
            {
                // Grid強調表示Interactable

                _mat[1].SetColor("_OutlineColor", Color.green * 20.0f);
            }
            else
            {
                // Grid強調表示Uninteractable

                _mat[1].SetColor("_OutlineColor", Color.red * 20.0f);
            }
        }

        void IPointerExitHandler.OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            // Grid強調表示OFF

            _mat[1].SetColor("_OutlineColor", Color.white);
        }

        // ---------------------------------
        // Editor
        // ---------------------------------

        /// <summary>
        /// インスペクターでGridの種類を変えたとき
        /// </summary>
        void OnChangeGridType()
        {
            if (Interactable)
            {
                gameObject.layer = LayerMask.NameToLayer("Default");

                // 道を非表示
                _N = false;
                _S = false;
                _E = false;
                _W = false;
                if (_mat[0] != null)
                {
                    _mat[0].SetFloat("_N", 0);
                    _mat[0].SetFloat("_S", 0);
                    _mat[0].SetFloat("_E", 0);
                    _mat[0].SetFloat("_W", 0);
                }
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Default");

                // お花畑表示

            }
        }

        /// <summary>
        /// インスペクターで道の方向を変えたとき
        /// </summary>
        void OnChangeRoadDirection()
        {
            // 道を表示
            if (_N) { _mat[0].SetFloat("_N", 1.0f); }
            else { _mat[0].SetFloat("_N", 0); }

            if (_S) { _mat[0].SetFloat("_S", 1.0f); }
            else { _mat[0].SetFloat("_S", 0); }

            if (_E) { _mat[0].SetFloat("_E", 1.0f); }
            else { _mat[0].SetFloat("_E", 0); }

            if (_W) { _mat[0].SetFloat("_W", 1.0f); }
            else { _mat[0].SetFloat("_W", 0); }

            // 東西南北に道が伸びているとき
            if (_N || _S || _E || _W)
            {
                // レイヤーをRoadに変更
                gameObject.layer = LayerMask.NameToLayer("Road");
            }
            // 道がないとき
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Default");

                // お花畑を表示


            }
        }
    }
}