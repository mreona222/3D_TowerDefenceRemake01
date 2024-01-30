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
        private int _row;
        public int Row => _row;

        [SerializeField]
        private int _column;
        public int Column => _column;

        public ConstructMatrix(int row, int column)
        {
            this._row = row;
            this._column = column;
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
        private int _level;
        public int Level => _level;

        [SerializeField, InlineProperty]
        private ReactiveProperty<float> _paramValue;
        public ReactiveProperty<float> ParamValue => _paramValue;

        public ConstructLevel(int level, float value)
        {
            this._level = level;
            this._paramValue = new ReactiveProperty<float>(value);
        }

        public void ChangeLevel(int level, float value)
        {
            this._level = level;
            this._paramValue.Value = value;
        }
    }

    [Serializable, InlineProperty]
    public class ConstructableUpgradeRate
    {
        [SerializeField, InlineProperty]
        private float _ratio;
        public float Ratio => _ratio;

        [SerializeField, InlineProperty]
        private float _pow;
        public float Pow => _pow;

        public ConstructableUpgradeRate(float ratio, float pow)
        {
            this._ratio = ratio;
            this._pow = pow;
        }
    }

    [Serializable]
    public class ConstructableCost
    {
        [SerializeField]
        private float _initialCoin;
        public float InitialCoin => _initialCoin;

        [SerializeField]
        private ConstructableUpgradeRate _coinCost;
        public ConstructableUpgradeRate CoinCost => _coinCost;

        public ConstructableCost(float initialStuff, float initialCoin, ConstructableUpgradeRate coinCost)
        {
            this._initialCoin = initialCoin;
            this._coinCost = coinCost;
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