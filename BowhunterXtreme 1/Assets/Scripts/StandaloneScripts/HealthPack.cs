using UnityEngine;

public class HealthPack : MonoBehaviour {

	public int hpAmount;

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
            col.GetComponent<PlayerHealth>().Heal(hpAmount);
            Debug.Log("Here");
            DestroyObject(gameObject);
        }
	}
}
