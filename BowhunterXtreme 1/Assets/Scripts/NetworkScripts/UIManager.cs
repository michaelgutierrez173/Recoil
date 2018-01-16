using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UIManager : NetworkBehaviour {

    public Canvas scoreboard;
    public Text player1Text;
    public Text player2Text;
    public Text player1Score;
    public Text player2Score;
    public Text timeLeft;

    public Canvas endScreen;
    public Text winner;
    public Text winnerText;
    public Text tieText;

	public string winMessage;
	public string loseMessage;

	public static int localPlayer = -1;

    private bool setup = false;

	void Start () {
        endScreen.enabled = false;
	}

    void SetupScoreboard(){
        Debug.Log("Setting up scoreboard");
        if (GameManagerNet.GetInstance().players[0])
        {
            Debug.Log("player1 name = " + GameManagerNet.GetInstance().players[0].playerName);
            if (GameManagerNet.GetInstance().players[0].playerName.Length > 10)
            {
                player1Text.text = GameManagerNet.GetInstance().players[0].playerName.Remove(10) + ":";
            }
            else
            {
                player1Text.text = GameManagerNet.GetInstance().players[0].playerName + ":";
            }
            player1Text.color = GameManagerNet.GetInstance().players[0].playerColor;
        }

        if (GameManagerNet.GetInstance().players[1])
        {
            Debug.Log("player2 name = " + GameManagerNet.GetInstance().players[1].playerName);
            if (GameManagerNet.GetInstance().players[1].playerName.Length > 10)
            {
                player2Text.text = GameManagerNet.GetInstance().players[1].playerName.Remove(10) + ":";
            }
            else
            {
                player2Text.text = GameManagerNet.GetInstance().players[1].playerName + ":";
            }
            player2Text.color = GameManagerNet.GetInstance().players[1].playerColor;
        }
    }
	
	void Update () {

		if (!GameManagerNet.GetInstance ())
			return;
        
        if(!setup && GameManagerNet.GetInstance().players[0] && GameManagerNet.GetInstance().players[1]){
            SetupScoreboard();
            setup = true;
        }

        if(GameManagerNet.GetInstance().winner != -1)
        {
			//Debug.Log ("Displaying winner");
            DisplayWinner();
        }
        else
        {
			scoreboard.enabled = true;
			endScreen.enabled = false;
            //GameManagerNet.GetInstance().

            player1Score.text = GameManagerNet.GetInstance().player1Score.ToString();
            player2Score.text = GameManagerNet.GetInstance().player2Score.ToString();
			if (GameManagerNet.GetInstance ().gameTime < 0) {
				timeLeft.text = "Overtime";
			} else {
				timeLeft.text = GameManagerNet.GetInstance ().gameTime.ToString ();
			}
        }
    }

    public void DisplayWinner()
    {
        scoreboard.enabled = false;
        endScreen.enabled = true;
		if(GameManagerNet.GetInstance().winner == 0)
        {
			winner.enabled = false;
			winnerText.enabled = false;
			tieText.enabled = true;
        }
        else
        {
			tieText.enabled = false;
			winner.enabled = false;
			winnerText.enabled = true;
			if (localPlayer == GameManagerNet.GetInstance ().winner) {
				winnerText.text = winMessage;
			} else {
				winnerText.text = loseMessage;
			}
			//winner.text = GameManagerNet.GetInstance().winner.ToString();
        }
    }

}
