using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SPACE_UTIL;
using SPACE_GAME;

namespace SPACE_CHECK
{
	public class DEBUGPlayerStats : MonoBehaviour
	{
		[Header("just to log")]
		[SerializeField] float currTime = 0f;
		private void Update()
		{
			currTime += Time.unscaledDeltaTime;
		}

		private void OnApplicationQuit()
		{
			Debug.Log(C.method(this, "orange"));
			GameStore.playerStats.gameTime += this.currTime;
			GameStore.playerStats.HISTORY.Add(this.currTime);
			GameStore.playerStats.Save(); // can be done anywhere(atleast before apllication quit) but loading is done just at the start scene Awake().
			// LOG.SaveGameData(GameDataType.playerStats, GameStore.playerStats.ToJson());
		}
	}
}