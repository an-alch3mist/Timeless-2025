using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using SPACE_UTIL;

namespace SPACE_GAME
{
	/*
		load at begining scene,
		save can be made anytime
	*/
	[DefaultExecutionOrder(-50)] // just after UnityEngine.InputSystem Init
	public class GameStore : MonoBehaviour
	{
		[SerializeField] InputActionAsset _IA;
		public static InputActionAsset IA;
		public static PlayerStats playerStats;

		private void Awake()
		{
			Debug.Log(C.method(this, "white"));
			GameStore.IA = this._IA;
			GameStore.LoadAllSavedGameDataJsonToField();
		}

		static void LoadAllSavedGameDataJsonToField()
		{
			GameStore.IA.LoadBindingOverridesFromJson(LOG.LoadGameData(GameDataType.inputActionAsset));
			GameStore.playerStats = LOG.LoadGameData<PlayerStats>(GameDataType.playerStats);
		}
	}

	// ================ GLOBAL CLASS ============== //
	[System.Serializable]
	public class PlayerStats
	{
		public float gameTime = 0f;
		public List<float> HISTORY = new List<float>();
		public void Save()
		{
			LOG.SaveGameData(GameDataType.playerStats, this.ToJson());
		}
	}

	// ================ GLOBAL CLASS ============== //

	// ========== GLOBAL ENUM ============= //
	public enum GameDataType
	{
		inputActionAsset,
		playerStats,
	}

	public enum AnimParamType
	{
		doorOpen,
		doorClose
	}

	public enum ResourceType
	{
		audio__doorLocked,
		audio__doorOpen,
	}
	// ========== GLOBAL ENUM ============= //
}