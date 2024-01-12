using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseRemake.Manager
{
    public class BattleInstanceManager : BaseInstanceManager
    {
        [SerializeField]
        public Transform[] EnemyTarget;

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
    }
}