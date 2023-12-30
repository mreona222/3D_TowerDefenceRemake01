using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Template.Utility
{
	public class StateBase<T> where T : StateMachineBase<T>
	{
		protected T machine;
		public StateBase(T _machine) { machine = _machine; }
		public virtual void OnEnter() { }
		public virtual void OnUpdate() { }
		public virtual void OnFixedUpdate() { }
		public virtual void OnLateUpdate() { }
		public virtual void OnExit() { }
	}
}