using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootJetpack : MonoBehaviour {

	//private GameObject player;
	private Camera mainCam;
	public Vector3 offset;
	public float forceAmount;
	public float maxPlayerSpeed;
	public float weaponOffset;
	public bool isFiring = false;

	public Gun currentGun;
	//public ParticleSystem sparks;

	private ParticleSystem.EmissionModule e;

	private Rigidbody2D rb;
	private Vector2 forceDir;
	private Plane xy;

	public int timeDelay;
	private bool canFire = true;

	// Use this for initialization
	void Start () {
		//player = this.GetComponent<Rigidbody2D> ();
		mainCam = Camera.main;
		rb = this.GetComponent<Rigidbody2D> ();
		xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));
	}

	void Update() {

		if (Input.GetMouseButton (0)) {
			if (currentGun.fullyAuto) {
				currentGun.Fire ();
			} else if(canFire) {
				currentGun.Fire ();
				canFire = false;
			}
		} else if(Input.GetMouseButtonUp (0)){
			canFire = true;
		}

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (!currentGun) {
			// Player should always have a gun
			Debug.LogError ("Player does not have a currentGun");
			return;
		}

		if (isFiring) {
			currentGun.Fire ();
		}

		// Code for aiming gun

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		float distance;
		xy.Raycast(ray, out distance);
		Vector3 aimPoint = ray.GetPoint(distance);

		// Calulate direction player arm facing
		Vector3 mousePosition = mainCam.ScreenToWorldPoint (Input.mousePosition) + offset;
		//forceDir = mousePosition - (this.transform.position);
		forceDir = aimPoint - (this.transform.position);

		Debug.DrawRay (this.transform.position, (Vector3)(forceDir.normalized * forceAmount));

		Vector2 weaponDir = forceDir.normalized * weaponOffset;
		Vector2 pos = (Vector2)this.transform.position;

		currentGun.transform.position = this.transform.position + (Vector3)(weaponDir);
		currentGun.transform.right = (Vector3)(weaponDir);

		//e.enabled = true; 

		if (rb.velocity.magnitude > maxPlayerSpeed) {
			rb.velocity = rb.velocity.normalized * maxPlayerSpeed;
		}
	}

	public void ImpulsePlayer(float force) {
		rb.AddForce (-forceDir.normalized * force, ForceMode2D.Impulse);
	}
}
