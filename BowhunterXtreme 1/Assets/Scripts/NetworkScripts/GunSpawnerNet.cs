using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunSpawnerNet : NetworkBehaviour {

	public GameObject[] gunPrefabs;
	//public List<GunNet> mapGuns;
	public float spawnTime;
	public int iniSpawnCount = 3;

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
			if (st.tag == "WeaponSpawn") {
				spnPoints.Add (st);
			}
		}
		spawnPoints = spnPoints.ToArray ();

		for (int i = 0; i < iniSpawnCount; i++) { // spawn 3 guns initially
			SpawnGun ();
		}
	}

	public void RemoveGun(NetworkInstanceId id){
		if (!isServer)
			return;
		
		if (gunAtSpawnPoint.ContainsKey (id)) {
			Debug.Log ("removing gun with id = " + id);
			gunAtSpawnPoint.Remove (id);
			StartCoroutine("gunRespawnTimer");
		}
	}
		
	void SpawnGun(){
		if (!isServer)
			return;
		
		Debug.Log ("Spawning gun on server? " + isServer);
		int pointIndex = Random.Range (0, spawnPoints.Length);  // kinda jank
		Debug.Log ("pointIndex = " + pointIndex);
		while (gunAtSpawnPoint.ContainsValue (pointIndex) && gunAtSpawnPoint.Count < spawnPoints.Length) {
			pointIndex = Random.Range (0, spawnPoints.Length);
			Debug.Log ("redo pointIndex = " + pointIndex);
		}

		GameObject gun = Instantiate (gunPrefabs[Random.Range(0, gunPrefabs.Length)], // spawn random gun
			spawnPoints[pointIndex].transform.position,
			spawnPoints[pointIndex].transform.rotation); 							  // spawn in random location

		Debug.Log ("dictionary = " + gunAtSpawnPoint.Count);
		NetworkServer.Spawn(gun);
		Debug.Log ("gun netid = " + gun.GetComponent<NetworkIdentity>().netId);
		gunAtSpawnPoint.Add(gun.GetComponent<NetworkIdentity>().netId, pointIndex); // add gun to list of guns live on map
	}

	IEnumerator gunRespawnTimer(){
		yield return new WaitForSeconds (spawnTime);
		SpawnGun ();
	}
}
