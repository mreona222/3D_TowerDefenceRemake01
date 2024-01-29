using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.Constructable
{
    [Serializable]
    public class ConstructMatrix
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

        public ConstructMatrix InverseMatrix()
        {
            return new ConstructMatrix(this.Column, this.Row);
        }
    }

    [Serializable]
    public class ConstructLevel
    {
        [SerializeField]
        public int Level;
        [SerializeField, InlineProperty]
        public ReactiveProperty<float> ParamValue;

        public ConstructLevel(int level, float value)
        {
            this.Level = level;
            this.ParamValue = new ReactiveProperty<float>(value);
        }

        public void ChangeLevel(int level, float value)
        {
            this.Level = level;
            this.ParamValue.Value = value;
        }
    }

    [Serializable]
    public class ConstructableUpgradeRate
    {
        [SerializeField]
        public float Ratio;
        [SerializeField, InlineProperty]
        public float Pow;

        public ConstructableUpgradeRate(float ratio, float pow)
        {
            this.Ratio = ratio;
            this.Pow = pow;
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

        ReactiveDictionary<ParamType, ConstructLevel> CurrentParams { get; set; }

        List<GameObject> RayCastCell();

        void Construct();
    }
}