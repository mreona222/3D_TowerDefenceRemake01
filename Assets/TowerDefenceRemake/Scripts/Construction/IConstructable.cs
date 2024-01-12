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

    public struct Constructable
    {
        public bool constructable;
        public Vector3 center;

        public Constructable(bool constructable, Vector3 center)
        {
            this.constructable = constructable;
            this.center = center;
        }
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