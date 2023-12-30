using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Template.Utility;

namespace Template.Manager
{
    public class ManagerBase<T> : SingletonMonoBehaviour<T> where T : MonoBehaviour
    {
        protected override void Init()
        {
            base.Init();
            DontDestroyOnLoad(this.gameObject);
        }
    }
}