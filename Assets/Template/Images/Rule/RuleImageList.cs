using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Template.Rule
{
    public enum RuleImageEnum
    {
        none,
        radial,
        linear,
        linear_invert,

    }

    [CreateAssetMenu(menuName ="My Scriptable/Rule/Create RuleImageList")]
    public class RuleImageList : ScriptableObject
    {
        public Texture[] Rules;
    }
}