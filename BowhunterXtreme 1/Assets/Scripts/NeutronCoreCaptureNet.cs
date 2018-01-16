using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NeutronCoreCaptureNet : NetworkBehaviour
{

    public int scorePerCapture = 5;
    public float respawnTime;

    private GameObject[] capturePoints;

    [SyncVar(hook = "MoveCore")]
    private Vector3 coreNewPosition;

	private bool scoreLock = false;

    private AudioSource audioSource;

    void Start()
    {
        capturePoints = GameObject.FindGameObjectsWithTag("CapturePoint");
        audioSource = GetComponent<AudioSource>();
    }

    void MoveCore(Vector3 pos){
		Debug.Log ("curPos = " + this.transform.position + " newPos = " + pos);
		this.transform.position = pos;

		StartCoroutine("EnableCollider");
    }

	IEnumerator EnableCollider(){
		yield return new WaitForSeconds (2);
		GetComponent<BoxCollider2D>().enabled = true;
		scoreLock = false;
	}

    private void IncreaseScore(int curPlayer)
    {
        if (!isServer)
            return;
		Debug.Log ("Scoreincrease for: " + curPlayer);
        if (curPlayer == 0)
        {
            GameManagerNet.GetInstance().IncreasePlayer1Score(scorePerCapture);
        }
        if (curPlayer == 1)
        {
            GameManagerNet.GetInstance().IncreasePlayer2Score(scorePerCapture);
        }
    }

    void GoToNewPoint()
    {
        GameObject curPoint = capturePoints[0]; // default point to spawn at
        ShootJetpackNet[] playerPositions = GameManagerNet.GetInstance().players;
        Vector3 playerPos1 = playerPositions[0] ? playerPositions[0].transform.position : Vector3.zero;
        Vector3 playerPos2 = playerPositions[1] ? playerPositions[1].transform.position : Vector3.zero;

        Dictionary<float, int> d = new Dictionary<float, int>();
        List<float> spawnPool = new List<float>(); // pick random point in list

        //float minDist = 0f; // initial dist at 0
        for (int i = 0; i < capturePoints.Length; i++)
        {
            // take smallest of 2 dists
            float curDist = Mathf.Min(Vector3.Distance(playerPos1, capturePoints[i].transform.position),
                                      Vector3.Distance(playerPos2, capturePoints[i].transform.position));

            d.Add(curDist, i);
            spawnPool.Add(curDist);
        }

        spawnPool.Sort();
        int rand = (int)(Random.Range(0, 5));

        // this 5 could change

        int val = 0;
        float[] ar = spawnPool.ToArray();
        float v = ar[spawnPool.Count - (rand + 1)];
        d.TryGetValue(v, out val);
        curPoint = capturePoints[val];

        // respawn at new position
        coreNewPosition = curPoint.transform.position;

        //StartCoroutine("RespawnTimer");
    }

    void OnTriggerEnter2D(Collider2D col)
    {

        if (audioSource && !audioSource.isPlaying) audioSource.PlayOneShot(audioSource.clip);

        if (!isServer)
            return;

        //Debug.Log ("Colliding with player");
        if (col.tag == "Player")
        {
			if (!scoreLock) { 
				scoreLock = true;
				Debug.Log ("Scorelock = " + scoreLock + " Increasing score: ");
				IncreaseScore (col.gameObject.GetComponent<ShootJetpackNet> ().playerNumber);
				GetComponent<BoxCollider2D> ().enabled = false;
				//StopCoroutine ("RespawnTimer");
				GoToNewPoint ();
			}
        }
    }

    IEnumerator RespawnTimer(){
        yield return new WaitForSeconds(respawnTime);
        GoToNewPoint();
    }

}
