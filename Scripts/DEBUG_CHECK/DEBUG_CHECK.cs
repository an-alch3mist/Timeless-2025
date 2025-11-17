using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;
using SPACE_DrawSystem;

using SPACE_GAME_0;

namespace SPACE_CHECK
{
	public class DEBUG_CHECK : MonoBehaviour
	{
		public string doorName;
		public string playerInsideOrOutsideorNone = "none";


		private void Start()
		{
			Debug.Log(C.method(this));
			//this.checkAllAnimatorControllerParamExists();
			// this.checkGameObjHierarchy();
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
				.deep("outside").gf();
			Debug.Log($"outside: {outside.name}".colorTag("lime"));
			Debug.Log($"total matches in deep search: {_doorHingedSimple.Q().deep("trigger").count()}".colorTag("lime"));
			LOG.AddLog(_doorHingedSimple.Q().deep("trigger").gall().ToTable(name: "LIST<> GameObject from hierarchy query", toString: true));
			/*
			// 2. Get depth while searching
			int depth = _doorHingedSimple.Q()
				.leaf("trigger")
				.leaf("outside")
				.depth(); // Returns 2
			


			// 3. Find all doors in a building
			List<GameObject> doors = building.Q()
				.deep("door")
				.with<Animator>()
				.gall();

			// 4. Get component directly
			BoxCollider bc = _doorHingedSimple.Q()
				.leaf("trigger")
				.leaf("outside")
				.gc<BoxCollider>();

			// 5. Check existence without throwing errors
			if (door.Q().leaf("handle").exists())
			{
				var handle = door.Q().leaf("handle").gf();
				// Do something
			}

			// 6. Find multiple matches
			List<GameObject> allTriggers = _doorHingedSimple.Q()
				.leaf("trigger")
				.gall(); // Returns all children starting with "trigger"

			// 7. Debug your query
			_doorHingedSimple.Q()
				.leaf("door")
				.contains("frame")
				.debug("Door Frames Found")
				.gall();

			// 8. Exact name matching
			GameObject frame = _doorHingedSimple.Q()
				.leaf("door")
				.exact("frame(visual + collider)")
				.gf();

			// 9. Complex query with filtering
			List<TextMeshPro> labels = levelRoot.Q()
				.deep("door")
				.contains("Text")
				.gcall<TextMeshPro>();

			// 10. Count matches
			int triggerCount = door.Q()
				.leaf("trigger")
				.count(); // How many direct children start with "trigger"?

			*/
		}
	}
}
