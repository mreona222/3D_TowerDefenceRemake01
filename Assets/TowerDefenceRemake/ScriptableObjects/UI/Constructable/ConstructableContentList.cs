using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseRemake.UI
{
    [CreateAssetMenu(menuName = "My Scriptable/Create ConstructableContentList")]
    public class ConstructableContentList : ScriptableObject
    {
        public ConstructableUpgradeContent[] Content;
    }
}