using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Template.Rule;
using Template.UI;
using TowerDefenseRemake.Manager;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TowerDefenseRemake.UI
{
    public class StartGameButton : ButtonBase
    {
        [SerializeField]
        SceneEnum _menu;

        protected override void OnPointerClickInternal(PointerEventData eventData)
        {
            base.OnPointerClickInternal(eventData);

            // 処理
            UniTask.Create(async () =>
            {
                await SceneTransitionManager.Instance.LoadingSceneWithLoading(RuleImageEnum.linear_invert, RuleImageEnum.linear, _menu, async () =>
                {
                    // 初期化
                    await UniTask.Delay(1);
                });
            });
        }
    }
}