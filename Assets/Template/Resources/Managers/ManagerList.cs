using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Template.Manager
{
    [CreateAssetMenu(menuName = "My Scriptable/Create ManagerList")]
    public class ManagerList : ScriptableObject
    {
        public GameObject[] managerList;
    }
}