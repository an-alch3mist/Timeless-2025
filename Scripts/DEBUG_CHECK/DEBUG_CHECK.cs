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

		void checkHierarchyQuery()
		{
			// HierarchyQueryTests hierarchyQueryTests = new HierarchyQueryTests();
			// hierarchyQueryTests.RunAllChecks();
		}
	}

	public class HierarchyQueryTests : MonoBehaviour
	{
		private bool destroyImmediate = false;
		public void RunAllChecks()
		{
			// TestShallowSearch();
			// TestDeepComponentSearch();
			// TestAncestorSearch();
			// TestFullPath();
			// TestSepSpace();
			TestWhere();
		}
		private void TestShallowSearch()
		{
			// Setup: Root > Child1 > GrandChild1 > GreatGrandChild1
			//             > Child2 > GrandChild2
			GameObject root = new GameObject("Root");
			GameObject child1 = new GameObject("Child1");
			child1.transform.SetParent(root.transform);
			GameObject grandChild1 = new GameObject("Target");
			grandChild1.transform.SetParent(child1.transform);
			GameObject greatGrandChild1 = new GameObject("Target"); // Deeper target
			greatGrandChild1.transform.SetParent(grandChild1.transform);

			GameObject child2 = new GameObject("Child2");
			child2.transform.SetParent(root.transform);
			GameObject grandChild2 = new GameObject("Target");
			grandChild2.transform.SetParent(child2.transform);

			// Test: Should find shallowest "Target" (grandChild1 or grandChild2, depth 2)
			GameObject result = root.Q().downNamed("target").gf();

			bool success = result != null && result.transform.parent.name.StartsWith("Child");
			Debug.Log($"[Test] Shallow Search: {(success ? "PASS" : "FAIL")}".colorTag(success ? "lime" : "red"));
			Debug.Log($"  Found: {result?.name} at depth 2 (parent: {result?.transform.parent.name})".colorTag("cyan"));

			if (destroyImmediate) DestroyImmediate(root);
		}
		private void TestDeepComponentSearch()
		{
			GameObject root = new GameObject("Root");
			GameObject child1 = new GameObject("Child1");
			child1.transform.SetParent(root.transform);
			child1.AddComponent<Rigidbody>();

			GameObject child2 = new GameObject("Child2");
			child2.transform.SetParent(root.transform);

			GameObject grandChild = new GameObject("GrandChild");
			grandChild.transform.SetParent(child2.transform);
			grandChild.AddComponent<Rigidbody>();

			List<GameObject> results = root.Q().deepDown<Rigidbody>().all();
			bool success = results.Count == 2;

			Debug.Log($"[Test] Deep Component Search: {(success ? "PASS" : "FAIL")}".colorTag(success ? "lime" : "red"));
			Debug.Log($"  Found {results.Count} Rigidbodies (expected 2)".colorTag("cyan"));

			if (destroyImmediate) DestroyImmediate(root);
		}
		private void TestAncestorSearch()
		{
			GameObject grandParent = new GameObject("GrandParent");
			grandParent.AddComponent<Canvas>();

			GameObject parent = new GameObject("Parent");
			parent.transform.SetParent(grandParent.transform);

			GameObject child = new GameObject("Child");
			child.transform.SetParent(parent.transform);

			GameObject result = child.Q().up<Canvas>().gf();
			bool shallowFail = result == null; // Immediate parent doesn't have Canvas

			GameObject deepResult = child.Q().deepUp<Canvas>().gf();
			bool deepSuccess = deepResult != null && deepResult.name == "GrandParent";

			Debug.Log($"[Test] Ancestor Search (shallow should fail): {(shallowFail ? "PASS" : "FAIL")}".colorTag(shallowFail ? "lime" : "red"));
			Debug.Log($"[Test] Ancestor Search (deep should succeed): {(deepSuccess ? "PASS" : "FAIL")}".colorTag(deepSuccess ? "lime" : "red"));

			if (destroyImmediate) DestroyImmediate(grandParent);
		}
		private void TestFullPath()
		{
			GameObject root = new GameObject("Root");
			GameObject child = new GameObject("Child");
			child.transform.SetParent(root.transform);
			GameObject grandChild = new GameObject("GrandChild");
			grandChild.transform.SetParent(child.transform);

			string path = grandChild.getFullPath();
			bool success = path == "Root/Child/GrandChild";

			Debug.Log($"[Test] Full Path: {(success ? "PASS" : "FAIL")}".colorTag(success ? "lime" : "red"));
			Debug.Log($"  Path: {path}".colorTag("cyan"));

			// Test multiple paths
			GameObject sibling = new GameObject("Sibling");
			sibling.transform.SetParent(child.transform);

			List<string> paths = root.Q().deepDownNamed("child").getFullPath();
			bool multiSuccess = paths.Count >= 1 && paths[0].Contains("Root/Child");

			Debug.Log($"[Test] Multiple Paths: {(multiSuccess ? "PASS" : "FAIL")}".colorTag(multiSuccess ? "lime" : "red"));

			if (destroyImmediate) DestroyImmediate(root);
		}
		private void TestSepSpace()
		{
			GameObject root = new GameObject("Root");
			GameObject match1 = new GameObject("door_handle"); // Should match "door" with sepSpace
			match1.transform.SetParent(root.transform);
			GameObject match2 = new GameObject("indoor"); // Should NOT match "door" with sepSpace
			match2.transform.SetParent(root.transform);

			int countWithSep = root.Q().deepDownNamed("door", sepSpace: true).count();
			int countWithoutSep = root.Q().deepDownNamed("door", sepSpace: false).count();

			bool success = countWithSep == 1 && countWithoutSep == 2;

			Debug.Log($"[Test] SepSpace: {(success ? "PASS" : "FAIL")}".colorTag(success ? "lime" : "red"));
			Debug.Log($"  With sepSpace=true: {countWithSep} (expected 1)".colorTag("cyan"));
			Debug.Log($"  With sepSpace=false: {countWithoutSep} (expected 2)".colorTag("cyan"));

			if (destroyImmediate) DestroyImmediate(root);
		}
		private void TestWhere()
		{
			GameObject root = new GameObject("Root");
			GameObject active1 = new GameObject("Active1");
			active1.transform.SetParent(root.transform);
			active1.SetActive(true);

			GameObject inactive = new GameObject("Inactive");
			inactive.transform.SetParent(root.transform);
			inactive.SetActive(false);

			GameObject active2 = new GameObject("Active2");
			active2.transform.SetParent(root.transform);
			active2.SetActive(true);

			int activeCount = root.Q().deepDownNamed("active").where(go => go.activeSelf).count();
			bool success = activeCount == 2;

			Debug.Log($"[Test] Where Filter: {(success ? "PASS" : "FAIL")}".colorTag(success ? "lime" : "red"));
			Debug.Log($"  Active count: {activeCount} (expected 2)".colorTag("cyan"));

			if (destroyImmediate) DestroyImmediate(root);
		}
	}
}
