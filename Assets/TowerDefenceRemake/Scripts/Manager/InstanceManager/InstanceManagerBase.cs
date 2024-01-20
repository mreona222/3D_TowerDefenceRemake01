using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using System;

namespace TowerDefenseRemake.Manager
{
    public abstract class InstanceManagerBase : MonoBehaviour
    {
        // 既にロードしたか
        protected static bool initialLoad = false;

        // シーンごとのカメラ
        //public GameObject vCam;

        // ----------------------------------------------------
        // 初期化
        // ----------------------------------------------------
        // このシーンから始めたときの初期化
        protected virtual async void Start()
        {
            // 二重初期化防止
            if (initialLoad) return;

            // カメラとライトの無効化
            //GameObject.Find("Main Camera")?.SetActive(false);
            //GameObject.Find("Directional Light")?.SetActive(false);

            // 初期化
            await Initialize(GameManager.GameState.Title);
        }

        public virtual async UniTask Initialize(GameManager.GameState state)
        {
            // ロード済み
            initialLoad = true;

            // ゲームステートの変更
            GameManager.Instance.UpdateGameState(state);

            // ロード処理
            await UniTask.Delay(1);
            
            Debug.Log($"{state.ToString()} Initialized");
        }
    }
}