using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Manager;
using UnityEngine;

namespace TowerDefenseRemake.UI
{
    public class StartGameButton : BaseButton
    {
        [SerializeField]
        SceneTransitionManager.Scenes next;

        protected override void OnClick()
        {
            base.OnClick();

            UniTask.Create(async () =>
            {
                await SceneTransitionManager.Instance.LoadingSceneWithLoading(SceneTransitionManager.RuleImage.linear_invert, SceneTransitionManager.RuleImage.linear, next, async () =>
                {
                    await UniTask.Delay(1);
                });
            });
        }
    }
}