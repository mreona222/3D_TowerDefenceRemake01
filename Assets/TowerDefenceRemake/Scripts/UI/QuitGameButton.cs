using System.Collections;
using System.Collections.Generic;
using TowerDefenseRemake.Manager;
using UnityEngine;

namespace TowerDefenseRemake.UI
{
    public class QuitGameButton : BaseButton
    {
        protected override void OnClick()
        {
            base.OnClick();

            SceneTransitionManager.Instance.QuitGame();
        }
    }
}