using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManagerNet : NetworkBehaviour {

	[SyncVar]
    public int player1Score;
	[SyncVar]
    public int player2Score;
	[SyncVar]
    public float gameTime;
	[SyncVar]
	public float endTime;
	[SyncVar]
	public int winner = -1;//-1 = not over; 0 = tie

    public enum GameMode{ Oddball, Capture, Deathmatch };
    public GameMode currentGameMode;

	public ShootJetpackNet[] players; // should have only 2 players
	public NeutronCoreNet nc;
    public NeutronCoreCaptureNet ncCap;

	public int maxScore;
    public float playerSpawn;
    public float coreSpawn;
    public float hpSpawn;
    public float barrelSpawn;

	public string playSceneToLoad;

	private static GameManagerNet gm;

	private float iniGameTime;
	private float iniEndTime;

    private bool gameStarted = false;
    private bool dontEnd = false;


	void Start(){
		players = new ShootJetpackNet[2];
		iniGameTime = gameTime;
		iniEndTime = endTime;
		//SceneManager.sceneLoaded += OnSceneLoaded;
		Debug.Log ("Starting GameManager");
		Debug.Log ("GameManager isServer Awake = " + isServer);
		gm = this.gameObject.GetComponent<GameManagerNet>();

        if (currentGameMode == GameMode.Oddball)
        {
            InvokeRepeating("GameTimerOddball", 1, 1);
        }
        else if (currentGameMode == GameMode.Capture)
        {
            InvokeRepeating("GameTimerCapture", 1, 1);
        }
        else if (currentGameMode == GameMode.Deathmatch)
        {
            InvokeRepeating("GameTimerDeathmatch", 1, 1); 
        }
	}

	public virtual void GameTimerOddball(){
		if (!isServer) // if not server or game is over return
			return;

		if (endTime < 0) {
			//ResetMap ();
			EndRound();
		}

        if (winner != -1)
        {
            endTime--;
            return;
        }

        // if gametime is over, check if player with lower score is holding core
        // if so, then don't end game until their score hits max, or they drop it

        if (player1Score >= maxScore || player2Score >= maxScore)
        {
            GameOver();
        }
		else if(gameTime <= 0)
		{
            if(nc.curPlayer == 0 && player1Score < player2Score){
                dontEnd = true;
            }
            else if(nc.curPlayer == 1 && player2Score < player1Score){
                dontEnd = true;
            }
            else if(nc.curPlayer == -1){
                dontEnd = false;
            }
            if(!dontEnd){
                GameOver();
            }
		}
		gameTime--;
	}

    public virtual void GameTimerCapture()
    {
        if (!isServer) // if not server or game is over return
            return;

        if (endTime < 0)
        {
            //ResetMap ();
            EndRound();
        }

        if (winner != -1)
        {
            endTime--;
            return;
        }

        // if gametime is over, check if player with lower score is holding core
        // if so, then don't end game until their score hits max, or they drop it

        if (gameTime <= 0 || player1Score >= maxScore || player2Score >= maxScore)
        {
            GameOver();
        }
        gameTime--;
    }

    public virtual void GameTimerDeathmatch()
    {
        if (!isServer) // if not server or game is over return
            return;

        if (endTime < 0)
        {
            //ResetMap ();
            EndRound();
        }

        if (winner != -1)
        {
            endTime--;
            return;
        }

        if (gameTime <= 0 || player1Score >= maxScore || player2Score >= maxScore)
        {
            GameOver();
        }
        gameTime--;
    }


	public static GameManagerNet GetInstance()
    {
		/*if (!gm) {
			gm = new GameManagerNet (); // idk if this needs to be here
		}*/
		return gm;
    }

    private void GameOver()
    {
		if (!isServer) // if not server return
			return;
		
        if(player1Score == player2Score)
        {
            winner = 0;
        }
        else if (player1Score > player2Score)
        {
            winner = 1;
        }
        else
        {
            winner = 2;
        }
    }

    public void testDebug(){
        Debug.Log("Testing debug on GameManagerNet!");
    }

    public void IncreasePlayer1Score(int score)
    {
		if (!isServer)
			return;
        player1Score += score;
		Debug.Log ("Player1 score = " + player1Score);
    }

    public void IncreasePlayer2Score(int score)
    {
		if (!isServer)
			return;
        player2Score += score;
		Debug.Log ("Player2 score = " + player2Score);
    }

	public void ResetMap(){
		if (!isServer)
			return;

		Debug.Log("Resetting map");
		player1Score = 0;
		player2Score = 0;
		winner = -1;
		endTime = iniEndTime;
		gameTime = iniGameTime;
		nc.shouldRespawn = true;
		nc.curPlayer = -1;
		foreach (ShootJetpackNet pl in players) {
			PlayerHealthNet p = pl.gameObject.GetComponent<PlayerHealthNet>();
			p.currentHealth = PlayerHealthNet.startingHealth;
			p.RpcRespawn ();
			//p.transform.rotation = Quaternion.Euler(new Vector3 (p.transform.rotation.x , p.transform.rotation.y, 0));
		}
		//Restart ();
	}

    public void EndRound()
    {
		//NetworkManager.singleton.sc
		Debug.Log("Round Over, returning to lobby");
		NetworkLobbyManager.singleton.ServerChangeScene ("LobbySceneBetter");
		//SceneManager.LoadScene("LobbySceneBetter"); // Lobby Scene should be 0 index
		//NetworkServer.SpawnObjects();
    }
}
