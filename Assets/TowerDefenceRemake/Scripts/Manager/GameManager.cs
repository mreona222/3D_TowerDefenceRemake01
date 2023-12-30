using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Template.Manager;

namespace TowerDefenseRemake.Manager
{
    public class GameManager : ManagerBase<GameManager>
    {
        // TODO: enumを他のファイルにまとめる
        public enum GameState
        {
            Title,
            Menu,
        }

        public GameState currentState;

        public static event Action<GameState> OnGameStateChanged;

        public BaseInstanceManager instanceManager;

        private void OnEnterGameState(GameState newState)
        {
            // シーンごとのマネージャーを取得
            instanceManager = FindObjectOfType<BaseInstanceManager>();

            switch (newState)
            {
                case GameState.Title:
                    break;
                case GameState.Menu:
                    break;
            }
        }

        private void OnExitGameState(GameState prevState)
        {
            // シーンごとのマネージャーを破棄
            instanceManager = null;

            switch (prevState)
            {
                case GameState.Title:
                    break;
                case GameState.Menu:
                    break;
            }
        }

        public void UpdateGameState(GameState newState)
        {
            OnExitGameState(currentState);
            currentState = newState;
            OnEnterGameState(currentState);

            OnGameStateChanged?.Invoke(currentState);
        }
    }
}