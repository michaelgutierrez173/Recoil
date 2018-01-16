using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public int player1Score;
    public int player2Score;
    public float gameTime;

    public int winner = -1;//-1 = not over; 0 = tie

    public float playerSpawn;
    public float coreSpawn;
    public float hpSpawn;
    public float barrelSpawn;

	public GameObject spawnPoint1;

    private static GameManager gm;

	void Start () {
        gm = this.gameObject.GetComponent<GameManager>();
    }
	
	void Update () {
        if(gameTime <= 0)
        {
            GameOver();
        }
    }

    public static GameManager GetInstance()
    {
        return gm;
    }

    private void GameOver()
    {
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

    public void DecreaseTime()
    {
        gameTime--;
    }

    public void IncreasePlayer1Score()
    {
        player1Score++;
    }

    public void IncreasePlayer2Score()
    {
        player2Score++;
    }

	public void Respawn(GameObject go) { // need to put a delay on respawn
		go.transform.position = spawnPoint1.transform.position;
		go.transform.Rotate (Vector3.forward * -90);
		go.GetComponent<PlayerHealth> ().currentHealth = go.GetComponent<PlayerHealth> ().startingHealth;
        go.GetComponent<PlayerHealth>().isDead = false;
	}

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
