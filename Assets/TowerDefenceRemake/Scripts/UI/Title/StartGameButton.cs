using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.UI
{
    public class StartGameButton : ButtonBase
    {
        [SerializeField]
        SceneTransitionManager.Scenes menu;

        protected override void OnPointerClickInternal(PointerEventData eventData)
        {
            base.OnPointerClickInternal(eventData);

            // 処理
            UniTask.Create(async () =>
            {
                await SceneTransitionManager.Instance.LoadingSceneWithLoading(SceneTransitionManager.RuleImage.linear_invert, SceneTransitionManager.RuleImage.linear, menu, async () =>
                {
                    // 初期化
                    await UniTask.Delay(1);
                });
            });
        }
    }
}