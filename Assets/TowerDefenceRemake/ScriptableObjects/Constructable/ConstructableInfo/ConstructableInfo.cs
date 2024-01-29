using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefenseRemake.Constructable;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

namespace TowerDefenseRemake.Constructable
{
    [CreateAssetMenu(menuName = "My Scriptable/Constructable/Create ConstructableInfo")]
    public class ConstructableInfo : ScriptableObject
    {
        // --------------------------------------------------------------------------

        [BoxGroup("基本情報")]
        [Tooltip("名称")]
        [SerializeField]
        private string _name;
        public string Name => _name;

        [BoxGroup("基本情報")]
        [Tooltip("アイコン")]
        [SerializeField]
        private Sprite _icon;
        public Sprite Icon => _icon;

        [BoxGroup("基本情報")]
        [Tooltip("スケール")]
        [SerializeField, InlineProperty]
        private ConstructMatrix _matrix;
        public ConstructMatrix Matrix { get => _matrix; }

        // --------------------------------------------------------------------------

        [BoxGroup("パラメータ")]
        [Tooltip("所持パラメータタイプ")]
        [ValueDropdown(nameof(ParamTypeCast), IsUniqueList = true)]
        [OnValueChanged(nameof(ParamTypesChanged))]
        [SerializeField, InlineProperty]
        private ParamType[] _paramTypes;
        public ParamType[] ParamTypes => _paramTypes;

        private static IEnumerable<ParamType> ParamTypeCast = Enumerable.Range(0, System.Enum.GetValues(typeof(ParamType)).Length).Cast<ParamType>();

        [BoxGroup("パラメータ")]
        [Tooltip("初期値")]
        [SerializeField, InlineProperty]
        private SerializedDictionary<ParamType, ConstructLevel> _initialParam;
        public SerializedDictionary<ParamType, ConstructLevel> InitialParam => _initialParam;

        [BoxGroup("パラメータ")]
        [Tooltip("最大値")]
        [SerializeField, InlineProperty]
        private SerializedDictionary<ParamType, ConstructLevel> _max;
        public SerializedDictionary<ParamType, ConstructLevel> Max => _max;

        [BoxGroup("パラメータ")]
        [Tooltip("上がり幅")]
        [SerializeField, InlineProperty]
        private SerializedDictionary<ParamType, ConstructableUpgradeRate> _increaseRate;
        public SerializedDictionary<ParamType, ConstructableUpgradeRate> IncreaseRate => _increaseRate;

        // --------------------------------------------------------------------------

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
                    if (!Max.ContainsKey(paramType))
                    {
                        Max.Add(paramType, new ConstructLevel(0, 0));
                    }
                    if (!IncreaseRate.ContainsKey(paramType))
                    {
                        IncreaseRate.Add(paramType, new ConstructableUpgradeRate(0, 0));
                    }
                }
                else
                {
                    InitialParam.Remove(paramType);
                    Max.Remove(paramType);
                    IncreaseRate.Remove(paramType);
                }
            }
        }
    }
}