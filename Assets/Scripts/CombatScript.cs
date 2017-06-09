using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatScript : MonoBehaviour {

	Animator anim;
	public GameObject playerAttackOrigin;
	public float playerHealth;
	public GameObject weaponOne;
	public GameObject weaponTwo;
	public Transform spawnPoint;
	public GameObject previousCam;
	MoveScript moveScript;

	void Start() {
		anim = GetComponent<Animator> ();
		moveScript = GetComponent<MoveScript> ();
	}

	void TakeDamage() {
			playerHealth -= 50f;
			anim.SetTrigger ("GetHit");
			Debug.Log ("Player has taken damage");
		
	}

	void Update() {

		if (Input.GetButton ("Fire2")) {
			anim.SetBool ("rightHeld", true);
		} 

		if (Input.GetButtonUp ("Fire2")) {
			anim.SetBool ("rightHeld", false);
			anim.SetTrigger ("rightHit");
		}

		if (Input.GetButton ("Fire1")) {
			anim.SetBool ("leftHeld", true);

		} 

		if (Input.GetButtonUp ("Fire1")) {
			anim.SetBool ("leftHeld", false);
			anim.SetTrigger ("LeftHit");
		}

		if (Input.GetKey (KeyCode.Q)) {
			anim.SetBool ("LeftCounter", true);
		} else {
			anim.SetBool ("LeftCounter", false);
		}

		if (Input.GetKey (KeyCode.E)) {
			anim.SetBool ("RightCounter", true);
		} else {
			anim.SetBool ("RightCounter", false);
		}

		if (playerHealth <= 0) {
			PlayerDeath ();
		}
	}

	void PlayerRespawn() {
		GameObject newPlayer = Instantiate (Resources.Load ("PlayerPlusCam"), GameObject.Find ("BlueBase").transform.position, Quaternion.identity) as GameObject;
		//GameObject cam = GameObject.FindGameObjectWithTag("CameraHolder");
		//cam.GetComponent<FreeCameraLook> ().SetTarget (newPlayer.transform);
	
		Destroy (moveScript.cam);
		Destroy (this);


	}

	void PlayerDeath() {
		Destroy (gameObject.GetComponentInChildren<SkinnedMeshRenderer> ());
		Instantiate (Resources.Load ("Ragdoll"), transform.position, Quaternion.identity);
		Destroy (gameObject.GetComponent<Rigidbody> ());
		Destroy (gameObject.GetComponent<CapsuleCollider> ());
		Destroy (weaponOne);
		Destroy(moveScript.debugLookIndic);
		Destroy (moveScript);
		Destroy (weaponTwo);
		playerHealth = 250f;
		Invoke ("PlayerRespawn", 5f);

	}
}
