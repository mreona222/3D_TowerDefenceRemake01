using System;
using System.Collections;
using System.Collections.Generic;
using Template.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace TowerDefenseRemake.Enemy
{
    public class EnemyBehaviourSlime : EnemyBehaviourBase
    {
        private enum SlimeState
        {
            Idle,
            Move,
            Damage,
            Die,
            ReachGoal,
        }
        [SerializeField]
        private SlimeState _currentState;

        protected override void Start()
        {
            base.Start();
            ChangeState(new EnemyBehaviourSlime.Idle(this));
        }

        // -------------------------------------------------------------------------------------
        // Idle
        // -------------------------------------------------------------------------------------
        private class Idle : StateBase<EnemyBehaviourBase>
        {
            public Idle(EnemyBehaviourBase _machine) : base(_machine)
            {
            }

            public override void OnEnter()
            {
                ((EnemyBehaviourSlime)machine)._currentState = SlimeState.Idle;
                ((EnemyBehaviourSlime)machine).OnEnterIdle();
            }

            public override void OnExit()
            {
                ((EnemyBehaviourSlime)machine).OnExitIdle();
            }
        }

        public override void ChangeState2Idle()
        {
            if (_currentState == SlimeState.Die || 
                _currentState == SlimeState.ReachGoal) return;

            ChangeState(new EnemyBehaviourSlime.Idle(this));
        }

        // -------------------------------------------------------------------------------------
        // Move
        // -------------------------------------------------------------------------------------
        private class Move : StateBase<EnemyBehaviourBase>
        {
            public Move(EnemyBehaviourBase _machine) : base(_machine)
            {
            }

            public override void OnEnter()
            {
                ((EnemyBehaviourSlime)machine)._currentState = SlimeState.Move;
                ((EnemyBehaviourSlime)machine).OnEnterMove();
            }

            public override void OnUpdate()
            {
                ((EnemyBehaviourSlime)machine).OnUpdateMove();
            }
        }

        public override void ChangeState2Move()
        {
            if (_currentState == SlimeState.Die || 
                _currentState == SlimeState.ReachGoal) return;

            ChangeState(new EnemyBehaviourSlime.Move(this));
        }

        // -------------------------------------------------------------------------------------
        // Damage
        // -------------------------------------------------------------------------------------
        private class Damage : StateBase<EnemyBehaviourBase>
        {
            float _damage;
            float _stanTime;

            public Damage(EnemyBehaviourBase _machine, float damage, float stanTime) : base(_machine)
            {
                this._damage = damage;
                this._stanTime = stanTime;
            }

            public override void OnEnter()
            {
                ((EnemyBehaviourSlime)machine)._currentState = SlimeState.Damage;
                ((EnemyBehaviourSlime)machine).OnEnterDamage(_damage, _stanTime);
            }

            public override void OnExit()
            {
                ((EnemyBehaviourSlime)machine).OnExitDamage();
            }
        }

        public override void ChangeState2Damage(float damage, float stanTime)
        {
            if (_currentState == SlimeState.Die || 
                _currentState == SlimeState.ReachGoal) return;

            ChangeState(new EnemyBehaviourSlime.Damage(this, damage, stanTime));
        }

        // -------------------------------------------------------------------------------------
        // Die
        // -------------------------------------------------------------------------------------
        private class Die : StateBase<EnemyBehaviourBase>
        {
            public Die(EnemyBehaviourBase _machine) : base(_machine)
            {
            }

            public override void OnEnter()
            {
                ((EnemyBehaviourSlime)machine)._currentState = SlimeState.Die;
                ((EnemyBehaviourSlime)machine).OnEnterDie();
            }
        }

        public override void ChangeState2Die()
        {
            if (_currentState == SlimeState.Die ||
                _currentState == SlimeState.ReachGoal) return;

            ChangeState(new EnemyBehaviourSlime.Die(this));
        }

        // -------------------------------------------------------------------------------------
        // ReachGoal
        // -------------------------------------------------------------------------------------
        private class ReachGoal : StateBase<EnemyBehaviourBase>
        {
            public ReachGoal(EnemyBehaviourBase _machine) : base(_machine)
            {
            }

            public override void OnEnter()
            {
                ((EnemyBehaviourSlime)machine)._currentState = SlimeState.ReachGoal;
                ((EnemyBehaviourSlime)machine).OnEnterReachGoal();
            }
        }

        public override void ChangeState2ReachGoal()
        {
            if (_currentState == SlimeState.Die ||
                _currentState == SlimeState.ReachGoal) return;

            ChangeState(new EnemyBehaviourSlime.ReachGoal(this));
        }

    }
}