using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ShootJetpackNet : NetworkBehaviour {

	public Vector3 offset;
	public float forceAmount;
	public float jumpJetForce;
	public float maxPlayerSpeed;
	public float weaponOffset;
	public bool isFiring = false;
	public GameObject gunToEquip;
	public GunNet currentGun;
	public int timeDelay;
	public Text playerText;
	public LayerMask layers;
	public ObjectiveDetector od;
    public GameObject fireParticles;

	public GameObject jet1;
	public GameObject jet2;
    public bool jumpJetsActive = true;
    public AudioSource jetAudio;

	private Camera mainCam;
	private Rigidbody2D rb;
	private Vector2 forceDir;
	private Plane xy;
	private bool canFire = true;
	private bool canShoot = true;

	[SyncVar]
	public bool isFlipped = false;
	[SyncVar]
	public int playerNumber = -1;
	[SyncVar]
	public string playerName = "Player";
	[SyncVar]
	public Color playerColor = Color.white;

	private SpriteRenderer spriteRendPlayer;
	private SpriteRenderer spriteRendGun;
	private Transform iniGunOffset;
	private float iniYOffset;
	private bool gameStarted = false;
    private float iniForce;
	private Vector2 dir;


	// Use this for initialization
	void Start () {
		if (GameManagerNet.GetInstance ()) {
			SetupPlayer ();
			gameStarted = true;
		}
	}

	void SetupPlayer(){
		
		//if (isServer) {
			//Debug.Log ("Starting on player server: " + playerNumber);
			if (playerNumber == -1) {
				playerNumber = 0;
			}
			GameManagerNet.GetInstance ().players [playerNumber] = this;
		//}
		if (isLocalPlayer) {
			Debug.Log ("local player's number = " + playerNumber);
			UIManager.localPlayer = playerNumber + 1; // to match GameManager winner
            //GetComponent<AudioListener>().enabled = true;
		}
        else{
            Debug.Log("not local player's number: " + playerNumber);
        }
		mainCam = Camera.main;
		rb = this.GetComponent<Rigidbody2D> ();
		xy = new Plane(Vector3.forward, new Vector3(0, 0, 0));

		if(isLocalPlayer){
			Debug.Log ("Before Equip gun command");
			CmdEquipWeapon (); // spawn pistol on server for this player
            jetAudio.enabled = true;
		}

        od.Initialize(); // setup objective pointer

        iniForce = jumpJetForce;

		spriteRendPlayer = GetComponent<SpriteRenderer> ();
		//spriteRendGun = currentGun.GetComponentInChildren<SpriteRenderer> ();
		//iniGunOffset = spriteRendGun.gameObject.GetComponent<Transform> ();

		// Set player name tag
		if (playerText) {
			if (playerName == "") {
				Debug.Log ("Player name was empty");
				playerName = "Player";
				playerColor = Color.white;
			}
			playerText.text = playerName;
			playerText.color = playerColor;
		}
	}

	void OnDestroy(){
		if (!isServer)
			return;
		Debug.Log("Destroyed player");
		GameManagerNet.GetInstance ().players[playerNumber] = null;
	}

	void Update() {

		if (!gameStarted) {
			if (GameManagerNet.GetInstance ()) {
				SetupPlayer ();
				gameStarted = true;
			}
			return;
		}

		if(spriteRendPlayer.flipX != isFlipped){
			spriteRendPlayer.flipX = isFlipped;
			foreach (CircleCollider2D col in GetComponents<CircleCollider2D>()) {
				col.offset = new Vector2 (-col.offset.x, col.offset.y);
			}
		}
			
		if (currentGun) {
			if (!spriteRendGun) {
				spriteRendGun = currentGun.GetComponentInChildren<SpriteRenderer> ();
				iniGunOffset = spriteRendGun.gameObject.GetComponent<Transform> ();
			} else if (spriteRendGun.flipY != isFlipped) {
				spriteRendGun.flipY = isFlipped;
				iniGunOffset.localPosition = new Vector3 (iniGunOffset.localPosition.x, -iniGunOffset.localPosition.y, 0);
			} 
		}

		if (!isLocalPlayer) {
			return;
		}

        if (currentGun)
        {
            if (Input.GetMouseButton(0))
            {
                if (currentGun.fullyAuto)
                {
                    currentGun.Fire(canShoot);
                }
                else if (canFire)
                {
                    canFire = !currentGun.Fire(canShoot); // returns true if gun is fired
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                canFire = true;
            }
        }


        // jump jet controls
        if (jumpJetsActive){
            jumpJetForce = iniForce;
        }
        else{
            jumpJetForce = iniForce/2;
        }

        dir = Vector2.zero;
        if (Input.GetKey(KeyCode.W))
        {   // UP
            dir += new Vector2(0, 1);
            jetAudio.pitch = 1.4f;
        }
        if (Input.GetKey(KeyCode.S))
        {  // DOWN
            dir += new Vector2(0, -1);
            jetAudio.pitch = 1.2f;
        }
        if (Input.GetKey(KeyCode.D))
        {   // RIGHT
            dir += new Vector2(1, 0);
            jetAudio.pitch = 1.3f;
        }
        if (Input.GetKey(KeyCode.A))
        {  // LEFT
            dir += new Vector2(-1, 0);
            jetAudio.pitch = 1.3f;
        }
        

	}

	// Update is called once per frame
	void FixedUpdate () {

		if (!gameStarted)
			return;

		if (!isLocalPlayer) {
			return;
		}

		if (!currentGun) {
			// Player should always have a gun
			Debug.Log ("Player does not have a currentGun");
			return;
		}

		AimGun (); // Code for aiming gun

		JumpJets(dir);  // Code for jetpack

		// cap player velocity
		if (rb.velocity.magnitude > maxPlayerSpeed) {
			rb.velocity = rb.velocity.normalized * maxPlayerSpeed;
		}
	}

	[Command]
	void CmdFlipPlayerSprite(bool flip){
		isFlipped = flip;
	}

	private void JumpJets(Vector2 dir){
        if(dir.magnitude > float.Epsilon){ // burning
            jetAudio.volume = 0.3f;
            //jetAudio.pitch = 1.3f;
        }
        else{ // idling
            jetAudio.volume = 0.1f;
            jetAudio.pitch = Random.Range(0.9f, 1.1f);
        }
		Vector2 forceDir = dir.normalized * jumpJetForce;
		rb.AddRelativeForce (forceDir);
		float t = 0.5f;
		jet1.transform.rotation = Quaternion.Lerp(jet1.transform.rotation, Quaternion.Euler(-90 *(new Vector3(-forceDir.normalized.y, forceDir.normalized.x, 0))), t);
		jet2.transform.rotation = Quaternion.Lerp(jet2.transform.rotation, Quaternion.Euler(-90 *(new Vector3(-forceDir.normalized.y, forceDir.normalized.x, 0))), t);
	}

	private void AimGun(){

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		float distance;
		xy.Raycast(ray, out distance);
		Vector3 aimPoint = ray.GetPoint(distance);

		// Calulate direction player arm facing
		Vector3 mousePosition = mainCam.ScreenToWorldPoint (Input.mousePosition) + offset;
		//forceDir = mousePosition - (this.transform.position);
		forceDir = aimPoint - (this.transform.position);

		// Don't let players shoot through terrain
		Debug.DrawRay (this.transform.position, (Vector3)(forceDir.normalized * (currentGun.offset + forceAmount)));
		RaycastHit2D hit = Physics2D.Raycast ((Vector2)this.transform.position, (forceDir.normalized), (currentGun.offset + forceAmount), layers.value);
		if (hit.collider) {
			canShoot = false;
		} else {
			canShoot = true;
		}

		Vector2 weaponDir = forceDir.normalized * currentGun.gunOffset;
		Vector2 pos = (Vector2)this.transform.position;

		currentGun.transform.position = this.transform.position + (Vector3)(weaponDir);
		currentGun.transform.right = (Vector3)(weaponDir);

		// Rotate gun and player to face the correct direction (might need to be optimized)
		if (!isFlipped && currentGun.transform.rotation.eulerAngles.z > 90 && currentGun.transform.rotation.eulerAngles.z < 270) {
			CmdFlipPlayerSprite (true);
		} 
		else if(isFlipped && (currentGun.transform.rotation.eulerAngles.z <= 90 || currentGun.transform.rotation.eulerAngles.z >= 270)) {
			CmdFlipPlayerSprite(false);
		}
	}

	public void ImpulsePlayer(float force) {
		rb.AddForce (-forceDir.normalized * force, ForceMode2D.Impulse);
	}

	[Command]
	public void CmdEquipWeapon(){

		if(currentGun) NetworkServer.Destroy (currentGun.gameObject); // get rid of current weapon

		//Debug.Log ("Equiping weapon for player: "+ this.netId);
		var weaponObject = Instantiate (gunToEquip, // gun prefab to spawn
			this.transform.position, // this could change to a hand position
			Quaternion.Euler (0f, 0f, 0f)) as GameObject;
		weaponObject.GetComponent<GunNet>().parentNetId = this.netId; // Set the parent network ID
		//weaponObject.transform.parent = transform; // Set the parent transform on the server

		// Spawn the object and calls start functions on gun before returning
		NetworkServer.SpawnWithClientAuthority(weaponObject,connectionToClient);

		/*
		GameObject gun = Instantiate (iniGun, this.transform);
		currentGun = gun.GetComponent<GunNet> ();
		currentGun.currentPlayer = this;
		NetworkServer.Spawn (gun);
		*/
	}
}
