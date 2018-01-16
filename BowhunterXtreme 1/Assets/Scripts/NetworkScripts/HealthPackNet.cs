using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class HealthPackNet : NetworkBehaviour {

	public int hpAmount;
    public GameObject hpSphere;

    private AudioSource audio;

    void Start(){
        audio = GetComponent<AudioSource>();
    }

	void OnTriggerEnter2D(Collider2D col){
		if (!isServer)
			return;
		
		if (col.tag == "Player") {
            col.GetComponent<PlayerHealthNet>().Heal(hpAmount);
            Debug.Log("Get healed");
			GameManagerNet.GetInstance ().GetComponent<HealthSpawnerNet> ().Remove (netId); // change to health instance
            audio.Play();
            hpSphere.SetActive(false);
            GetComponent<CircleCollider2D>().enabled = false;
            StartCoroutine("PlaySoundThenDie");
        }
	}

    IEnumerator PlaySoundThenDie(){
        yield return new WaitForSeconds(2);
        NetworkServer.Destroy(gameObject);
    }
}
