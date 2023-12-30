using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Template.Manager
{
    public class ManagerBoss : ManagerBase<ManagerBoss>
    {
        static GameObject[] managerList;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ManagerInstantiate()
        {
            ManagerList m_list = (ManagerList)Resources.Load("Managers/Manager List");
            foreach (GameObject manager in m_list.managerList)
            {
                Instantiate(manager);
            }
        }
    }
}