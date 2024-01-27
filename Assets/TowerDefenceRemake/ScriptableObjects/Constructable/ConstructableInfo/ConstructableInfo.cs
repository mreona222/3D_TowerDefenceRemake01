using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefenseRemake.Construction;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace TowerDefenseRemake.Turret
{
    [CreateAssetMenu(menuName = "My Scriptable/Constructable/Create ConstructableInfo")]
    public class ConstructableInfo : ScriptableObject
    {
        // --------------------------------------------------------------------------

        [BoxGroup("基本情報")]
        [Tooltip("名称")]
        public string Name;

        [BoxGroup("基本情報")]
        [Tooltip("アイコン")]
        public Sprite Icon;

        // --------------------------------------------------------------------------

        [BoxGroup("パラメータ")]
        [Tooltip("所持パラメータ")]
        [ValueDropdown(nameof(ParamTypeCast), IsUniqueList = true)]
        [OnValueChanged(nameof(ParamTypesChanged))]
        public ParamType[] ParamTypes;

        private static IEnumerable<ParamType> ParamTypeCast = Enumerable.Range(0, System.Enum.GetValues(typeof(ParamType)).Length).Cast<ParamType>();

        [BoxGroup("パラメータ")]
        [Tooltip("初期値")]
        public SerializedDictionary<ParamType, ConstructLevel> InitialParam;

        [BoxGroup("パラメータ")]
        [Tooltip("最大値")]
        public SerializedDictionary<ParamType, int> MaxLevel;

        void ParamTypesChanged()
        {
            foreach (ParamType paramType in ParamTypeCast)
            {
                if (ParamTypes.Contains(paramType))
                {
                    if (!InitialParam.ContainsKey(paramType))
                    {
                        InitialParam.Add(paramType, new ConstructLevel(0, 0));
                    }
                    if (!MaxLevel.ContainsKey(paramType))
                    {
                        MaxLevel.Add(paramType, 0);
                    }
                }
                else
                {
                    InitialParam.Remove(paramType);
                    MaxLevel.Remove(paramType);
                }
            }
        }
    }
}