using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace TowerDefenseRemake.Manager
{
    public class InstanceManagerBattle : InstanceManagerBase
    {
        [BoxGroup("ターゲット")]
        [SerializeField]
        private Transform[] _enemyTarget;
        public Transform[] EnemyTarget => _enemyTarget;

        [BoxGroup("リソース")]
        [SerializeField, InlineProperty]
        private ReactiveProperty<int> _stuff;
        public ReactiveProperty<int> Stuff
        {
            get => _stuff;
            set => _stuff = value;
        }

        [BoxGroup("リソース")]
        [SerializeField, InlineProperty]
        private ReactiveProperty<int> _coin;
        public ReactiveProperty<int> Coin
        {
            get => _coin;
            set => _coin = value;
        }

        private event Action<int> _onCoinChanged;
        public event Action<int> OnCoinChanged
        {
            add => _onCoinChanged += value;
            remove => _onCoinChanged -= value;
        }

        // ----------------------------------------------------
        // 初期化
        // ----------------------------------------------------
        // このシーンから始めたときの初期化
        protected override async void Start()
        {
            // 二重初期化防止
            if (initialLoad) return;

            // カメラとライトの無効化
            //GameObject.Find("Main Camera")?.SetActive(false);
            //GameObject.Find("Directional Light")?.SetActive(false);

            // 初期化
            await Initialize(GameManager.GameState.Battle);
        }

        public override async UniTask Initialize(GameManager.GameState state)
        {
            await base.Initialize(state);

            Coin.
                Subscribe(x =>
                {
                    _onCoinChanged?.Invoke(x);
                })
                .AddTo(this);
        }
    }
}