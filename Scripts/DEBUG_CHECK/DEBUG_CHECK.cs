using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;
using SPACE_DrawSystem;

using SPACE_GAME_0;

namespace SPACE_CHECK
{
	public enum TagType
	{
		player,
	}

	public class DEBUG_CHECK : MonoBehaviour
	{
		private void Start()
		{
			Debug.Log(C.method(this));

			// this.checkAllAnimatorControllerParamExists();
			// this.checkGameObjHierarchy();
			this.checkHierarchyQuery();
		}

		[SerializeField] Animator _animator;
		void checkAllAnimatorControllerParamExists()
		{
			this._animator.checkAllParamExistInAnimatorController<DoorAnimParamType>();
			return;

			foreach (var doorParam in C.getEnumValues<DoorAnimParamType>())
			{
				if (this._animator.trySetTrigger(doorParam))
					Debug.Log(C.method(null, "cyan", adMssg: $"success setting boolean {doorParam}"));
				else if (this._animator.trySetBool(doorParam, true))
					Debug.Log(C.method(null, "cyan", adMssg: $"success setting boolean {doorParam}"));
			}
		}

		[SerializeField] GameObject _doorHingedSimple;
		void checkGameObjHierarchy()
		{
			// 1. Your original request - clean and chainable!
			GameObject outside = _doorHingedSimple.Q()
				.deepDownNamed("outside").gf();
			Debug.Log($"outside: {outside.name}".colorTag("lime"));
			Debug.Log($"total matches in deep search: {_doorHingedSimple.Q().deepDownNamed("trigger").count()}".colorTag("lime"));
			// LOG.AddLog(_doorHingedSimple.Q().deepDownNamed("trigger").gall().ToTable(name: "LIST<> GameObject from hierarchy query", toString: true));
			LOG.AddLog(_doorHingedSimple.Q().deepDownNamed("trigger").all().ToTable(name: "LIST<> GameObject from hierarchy query", toString: true));
		}

		[SerializeField] GameObject _root;
		void checkHierarchyQuery()
		{
			var TRIGGER = this._root.Q().deepDownNamed("cube", "trigger").all();
			LOG.AddLog(TRIGGER.ToTable(name: "LIST<>", toString: true));
		}
	}
}
