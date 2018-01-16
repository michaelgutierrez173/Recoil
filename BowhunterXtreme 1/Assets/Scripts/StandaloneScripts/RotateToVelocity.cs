using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToVelocity : MonoBehaviour {

	public float veloLengthScale;
	[SerializeField] private Rigidbody2D bullet;

	// Use this for initialization
	void Start () {
		if (!bullet)
			bullet = GetComponentInParent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.rotation = Quaternion.LookRotation(bullet.velocity.normalized);
		transform.Rotate(new Vector3 (0, 90, 0));
		transform.localScale = new Vector3 (bullet.velocity.magnitude/veloLengthScale, transform.localScale.y, 0);
	}
}
