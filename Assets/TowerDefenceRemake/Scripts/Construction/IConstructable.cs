using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.Construction
{
    [Serializable]
    public struct ConstructMatrix
    {
        [SerializeField]
        public int row;
        [SerializeField]
        public int column;
    }



    public interface IConstructable
    {
        bool Constructed { get; set; }

        bool Constructable { get; }

        ConstructMatrix ConstructableMatrix { get; }

        List<GameObject> RayCastCell();

        void Construct();
    }
}