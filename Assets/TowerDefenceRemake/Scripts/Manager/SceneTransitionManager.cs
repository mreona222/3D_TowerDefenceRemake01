using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using DG.Tweening;

using Template.Manager;
using Fade;
using UnityEditor;

namespace TowerDefenseRemake.Manager
{
    public class SceneTransitionManager : ManagerBase<SceneTransitionManager>
    {
        // TODO: enumを他のファイルにまとめる
        public enum Scenes
        {
            Title,
            Menu,

        }

        public enum RuleImage
        {
            none,
            radial,
            linear,
            linear_invert,

        }

        [SerializeField] FadeImage loadingScreen;

        [SerializeField] Texture[] ruleImage;

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // カメラとライトの無効化
            //GameObject.Find("Main Camera")?.SetActive(false);
            //GameObject.Find("Directional Light")?.SetActive(false);
        }


        // -----------------------------------------------------------------------------------------------------
        // フェード
        // -----------------------------------------------------------------------------------------------------
        /// <summary>
        /// フェードアウト
        /// </summary>
        /// <param name="fadeout">ルール画像</param>
        private async UniTask FadeOut(RuleImage fadeout)
        {
            // ローディング画面生成
            loadingScreen.gameObject.SetActive(true);
            loadingScreen.MaskTexture = ruleImage[(int)fadeout];

            // フェードアウトアニメーション
            await DOVirtual.Float(0, 1.0f, 0.5f, value => 
            {
                ((IFade)loadingScreen).Range = value;
            })
                //.SetEase(Ease.OutQuint)
                .AsyncWaitForCompletion();
        }

        /// <summary>
        /// フェードイン
        /// </summary>
        /// <param name="fadein">ルール画像</param>
        private async UniTask FadeIn(RuleImage fadein)
        {
            // フェードイン
            loadingScreen.MaskTexture = ruleImage[(int)fadein];
            await DOVirtual.Float(1.0f, 0, 0.5f, value => 
            {
                ((IFade)loadingScreen).Range = value; 
            })
                //.SetEase(Ease.InQuint)
                .AsyncWaitForCompletion();

            // ローディング画面消去
            loadingScreen.gameObject.SetActive(false);
        }



        // -----------------------------------------------------------------------------------------------------
        // シーン遷移
        // -----------------------------------------------------------------------------------------------------

        /// <summary>
        /// ロード付きでシーン遷移
        /// </summary>
        /// <param name="fadeout">フェードアウトルール画像</param>
        /// <param name="fadein">フェードインルール画像</param>
        /// <param name="scene">シーン先のシーン</param>
        /// <param name="initialize">初期化処理</param>
        /// <returns></returns>
        //public async UniTask LoadSceneWithLoading(int fadeout, int fadein, string scene, System.Func<UniTask> initialize, System.Action afterLoad)
        //{
        //    // フェードアウト
        //    //GameObject loadingScreenInstance = await FadeOut(fadeout);
        //    await FadeOut(fadeout);

        //    // シーン遷移
        //    await SceneManager.LoadSceneAsync(scene);

        //    // 初期化処理
        //    if (initialize != null)
        //    {
        //        await initialize();
        //    }

        //    // フェードイン
        //    //await FadeIn(fadein, loadingScreenInstance);
        //    await FadeIn(fadein);

        //    // シーン遷移後
        //    if (afterLoad != null)
        //    {
        //        afterLoad();
        //    }
        //}

        public async UniTask LoadingSceneWithLoading(RuleImage fadeout ,RuleImage fadein, Scenes scene, System.Func<UniTask> initialize)
        {
            // フェードアウト
            await FadeOut(fadeout);

            SceneManager.LoadSceneAsync(scene.ToString()).completed += async (AsyncOperation obj) =>
            {
                // 初期化処理
                if(initialize != null)
                {
                    await initialize();
                }

                // フェードイン
                await FadeIn(fadein);
            };
        }

        /// <summary>
        /// ロード付きでマルチシーン遷移
        /// </summary>
        /// <param name="fadeout">フェードアウトルール画像</param>
        /// <param name="fadein">フェードインルール画像</param>
        /// <param name="scenes">遷移先のシーン</param>
        /// <param name="initialize">初期化処理</param>
        /// <param name="afterLoad">ロード後の処理</param>
        /// <returns></returns>
        //public async UniTask LoadMultiSceneWithLoading(int fadeout, int fadein, string[] scenes, System.Func<UniTask> initialize, System.Action afterLoad)
        //{
        //    // フェードアウト
        //    GameObject loadingScreenInstance = await FadeOut(fadeout);

        //    // コモンシーン以外の現在の全シーン名取得
        //    List<string> sceneNameList = new List<string> { };

        //    for (int i = 0; i < SceneManager.sceneCount; i++)
        //    {
        //        if (SceneManager.GetSceneAt(i).name != Template_Scenes.Template_Common.ToString())
        //        {
        //            sceneNameList.Add(SceneManager.GetSceneAt(i).name);
        //        }
        //    }

        //    // シーン破棄
        //    foreach (string sceneName in sceneNameList)
        //    {
        //        await SceneManager.UnloadSceneAsync(sceneName);
        //    }

        //    // シーン読み込み
        //    foreach (string scene in scenes)
        //    {
        //        if (!SceneManager.GetSceneByName(scene).isLoaded)
        //        {
        //            await SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        //        }
        //    }

        //    // 初期化処理
        //    if (initialize != null)
        //    {
        //        await initialize();
        //    }

        //    // フェードイン
        //    await FadeIn(fadein, loadingScreenInstance);

        //    // シーン遷移後
        //    if (afterLoad != null)
        //    {
        //        afterLoad();
        //    }
        //}

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
		Application.OpenURL("http://www.yahoo.co.jp/");
#else
		Application.Quit();
#endif
        }

        public void ReLolad()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }

        public async UniTask LoadCommon(string commonScene)
        {
            await SceneManager.LoadSceneAsync(commonScene, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(commonScene));
        }
    }
}