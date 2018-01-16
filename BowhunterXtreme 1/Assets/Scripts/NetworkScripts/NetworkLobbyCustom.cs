using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkLobbyCustom : NetworkLobbyManager {

	public int startCountdown = 5;
	float countTimer = 0;

	public override void OnLobbyServerPlayersReady()
	{
		countTimer = Time.time + startCountdown;
	}

	void Update()
	{
		if (Mathf.Abs(countTimer) <= float.Epsilon)
			return;

		if (Time.time > countTimer)
		{
			countTimer = 0;
			ServerChangeScene(playScene);
			foreach(NetworkLobbyPlayer p in lobbySlots){
				p.readyToBegin = false;
			}

		}
		else
		{
			Debug.Log("Counting down " + (countTimer - Time.time));
		}
	}
}
