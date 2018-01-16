using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

public class PlayerLoadedGame : LobbyHook {

	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		Debug.Log ("Testing LobbyHook: " + lobbyPlayer.GetComponent<LobbyPlayer> ().slot);

		ShootJetpackNet gPlayer = gamePlayer.GetComponent<ShootJetpackNet> ();
		LobbyPlayer lPlayer = lobbyPlayer.GetComponent<LobbyPlayer> ();

		// set player number
		gPlayer.playerNumber = (int)lPlayer.slot;

		// set player name
		gPlayer.playerName = lPlayer.playerName;

		// set player Color
		gPlayer.playerColor = lPlayer.playerColor;
	}
}
