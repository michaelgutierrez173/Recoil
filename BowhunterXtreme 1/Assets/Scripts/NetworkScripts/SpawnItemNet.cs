using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnItemNet : NetworkBehaviour {

	public GameObject[] prefabs;
	//public List<GunNet> mapGuns;
	public float spawnTime;
	public int iniSpawnCount = 3;
	public string spawnPointTag;

	private NetworkStartPosition[] spawnPoints;
	private Dictionary<NetworkInstanceId, int> gunAtSpawnPoint;

	// Use this for initialization
	void Start () {

		if (!isServer)
			return;

		gunAtSpawnPoint = new Dictionary<NetworkInstanceId, int> ();

		NetworkStartPosition[] points = FindObjectsOfType<NetworkStartPosition> ();
		List<NetworkStartPosition> spnPoints = new List<NetworkStartPosition>();
		foreach (NetworkStartPosition st in points) {
			if (st.tag == spawnPointTag) {
				spnPoints.Add (st);
			}
		}
		spawnPoints = spnPoints.ToArray ();

		for (int i = 0; i < iniSpawnCount; i++) {
			SpawnItem ();
		}
	}

	public void Remove(NetworkInstanceId id){
		if (gunAtSpawnPoint.ContainsKey (id)) {
			Debug.Log ("removing item with id = " + id);
			gunAtSpawnPoint.Remove (id);
			StartCoroutine("respawnTimer");
		}
	}

	void SpawnItem(){
		Debug.Log ("Spawning item on server? " + isServer);
		int pointIndex = Random.Range (0, spawnPoints.Length);  // kinda jank
		Debug.Log ("pointIndex = " + pointIndex);
		while (gunAtSpawnPoint.ContainsValue (pointIndex) && gunAtSpawnPoint.Count < spawnPoints.Length) {
			pointIndex = Random.Range (0, spawnPoints.Length);
			Debug.Log ("redo pointIndex = " + pointIndex);
		}

		GameObject go = Instantiate (prefabs[Random.Range(0, prefabs.Length)], // spawn random gun
			spawnPoints[pointIndex].transform.position,
			spawnPoints[pointIndex].transform.rotation); 							  // spawn in random location

		//Debug.Log ("dictionary = " + gunAtSpawnPoint.Count);
		NetworkServer.Spawn(go);
		//Debug.Log ("item netid = " + go.GetComponent<NetworkIdentity>().netId);
		gunAtSpawnPoint.Add(go.GetComponent<NetworkIdentity>().netId, pointIndex); // add gun to list of guns live on map
	}

	IEnumerator respawnTimer(){
		yield return new WaitForSeconds (spawnTime);
		SpawnItem ();
	}
}
