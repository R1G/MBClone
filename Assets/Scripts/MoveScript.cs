using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour {

	Animator anim;
	Camera cam;
	Rigidbody rb;

	public float speed;
	public float turnSpeed;

	Vector3 directionPos;
	Vector3 lookPos;
	Vector3 storeDir;

	public float lookIKWeight;
	public float bodyWeight;
	public float headWeight;
	public float clampWeight;
	public Transform playerHead;

	public GameObject debugLookIndic;
	bool isSprinting;
	bool isGrounded = true;


	void Start() {
		anim = GetComponent<Animator> ();
		cam = Camera.main;
		isSprinting = false;
		rb = GetComponent<Rigidbody> ();
	}


	void Update() {
		if (Input.GetButton ("Fire3") && isGrounded) {
			isSprinting = true;
			anim.SetBool ("isSprinting", true);
		} else {
			isSprinting = false;
			anim.SetBool ("isSprinting", false);
		}

		RaycastHit hit;
		if(Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out hit, 50f)) {
			debugLookIndic.transform.position = new Vector3 (hit.point.x, hit.point.y, hit.point.z);
			transform.LookAt (new Vector3(hit.point.x, transform.forward.y, hit.point.z));
		}


		if (Input.GetButtonDown ("Jump") && isGrounded) {
			rb.AddForce (Vector3.up * 500, ForceMode.Impulse);
			anim.SetTrigger ("Jump");
			isGrounded = false;
			anim.SetFloat ("vertical", 0);
		}


	}


	void FixedUpdate () {
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

			if (isSprinting) {
				//should not be able to strafe while sprinting
				//should not be able to run backwards
				if (v < 0) {
					v = 0;
				}
				transform.Translate (h/9, 0, v * speed*3);

			} else {
				//Not sprinting. 
				//Should be able to strafe, 
				//If walking diagonally, speed needs to be capped at 1, instead of ~1.4 since moving in both axes
				if (h != 0 && v != 0) {
					transform.Translate (h * speed * 1 / 4, 0, v * speed * 1 / 4);
				} else {
					transform.Translate (h*speed, 0, v*speed);
				}
				anim.SetFloat ("horizontal", h);
				anim.SetFloat ("vertical", v);

				RaycastHit hit;
				if(Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, .008f, LayerMask.GetMask("Environment"))) {
					anim.SetBool ("isGrounded", true);
					isGrounded = true;
				}
		}
	}

	void OnAnimatorIK() {
		anim.SetLookAtWeight (lookIKWeight, bodyWeight, headWeight, clampWeight);
		anim.SetLookAtPosition (new Vector3(debugLookIndic.transform.position.x, debugLookIndic.transform.position.y+30, debugLookIndic.transform.position.z));
	}

}
