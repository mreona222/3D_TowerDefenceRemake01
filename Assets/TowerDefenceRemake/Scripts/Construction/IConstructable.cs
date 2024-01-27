using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.Construction
{
    [Serializable]
    public struct ConstructMatrix
    {
        [SerializeField]
        public int Row;
        [SerializeField]
        public int Column;

        public ConstructMatrix(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }
    }

    [Serializable]
    public struct ConstructLevel
    {
        [SerializeField]
        public int Level;
        [SerializeField]
        public ReactiveProperty<float> ParamValue;

        public ConstructLevel(int level, float value)
        {
            this.Level = level;
            this.ParamValue = new ReactiveProperty<float>(value);
        }
    }

    public enum ParamType
    {
        Power,
        Range,
        Interval,
        Stan,
    }


    public interface IConstructable
    {
        bool Constructed { get; set; }

        bool Constructable { get; }

        ConstructMatrix ConstructableMatrix { get; }

        ReactiveDictionary<ParamType, ConstructLevel> CurrentParams { get; set; }

        List<GameObject> RayCastCell();

        void Construct();
    }
}