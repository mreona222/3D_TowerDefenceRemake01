using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefenseRemake.Turret
{
    public class TurretBehaviourNormal : TurretBehaviourBase
    {
        public override void Fire()
        {
            base.Fire();

            FireAnimation();
        }

        protected override void FireAnimationStop()
        {

        }
    }
}