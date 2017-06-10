using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveScript : MonoBehaviour {

	Animator anim;
	public Camera cam;
	Rigidbody rb;

	public float speed;
	public float turnSpeed;

	Vector3 directionPos;
	Vector3 lookPos;
	Vector3 storeDir;

	//Variables being used in OnAnimatorIK(), handles some look animations
	public float lookIKWeight;
	public float bodyWeight;
	public float headWeight;
	public float clampWeight;
	public Transform playerHead;

	//This gameObject is located wherever the player is currently looking, so he knows what he is aiming at
	//This will not cast on anything that is not the environment, such as other players/AI, or map obstacles
	public GameObject debugLookIndic;

	bool isGrounded = true;


	void Start() {
		anim = GetComponent<Animator> ();
		cam = Camera.main;
		rb = GetComponent<Rigidbody> ();

		if (debugLookIndic == null) {
			debugLookIndic = GameObject.Find ("debugLookIndic");
		}


	}


	void Update() {
		PointCharacterToLookPosition ();
		if (IsGrounded()) {
			HandleJumping ();
		}
	}


	void FixedUpdate () {
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		if (IsSprinting(v)) {
				HandleSprinting (h, v);
			} else {
				HandleStrafing (h, v);
		}
	}

	//Called on every animation frame to handle which way the player model looks
	void OnAnimatorIK() {
		anim.SetLookAtWeight (lookIKWeight, bodyWeight, headWeight, clampWeight);
		anim.SetLookAtPosition (new Vector3(debugLookIndic.transform.position.x, debugLookIndic.transform.position.y+30, debugLookIndic.transform.position.z));
	}

	//Will play sprinting animation if moving forward, sprint button is held and is not in the air
	bool IsSprinting(float v) {
		if (Input.GetButton ("Fire3") && isGrounded && v > 0f) {
			anim.SetBool ("isSprinting", true);
			return true;
		} else {
			anim.SetBool ("isSprinting", false);
			return false;
		}
	}

	void PointCharacterToLookPosition() {
		RaycastHit hit;
		if (cam != null) {
			if(Physics.Raycast(new Ray(cam.transform.position, cam.transform.forward), out hit, 250f)) {
				debugLookIndic.transform.position = new Vector3 (hit.point.x, hit.point.y, hit.point.z);
				transform.LookAt (new Vector3(hit.point.x, hit.point.y, hit.point.z));
			}
		}
	}

	bool IsGrounded() {
		RaycastHit hit;
		if (Physics.Raycast (gameObject.transform.position, Vector3.down, out hit, .2f, LayerMask.GetMask ("Environment"))) {
			anim.SetBool ("isGrounded", true);
			return true;
		} else {
			return false;
		}
	}

	//Character cannot jump unless already on ground. 
	void HandleJumping() {
		if (Input.GetButtonDown ("Jump") && isGrounded) {
			rb.AddForce (Vector3.up * 500, ForceMode.Impulse);
			anim.SetTrigger ("Jump");
			//isGrounded = false;
			anim.SetFloat ("vertical", 0);
		}
	}

	//When the character sprints, he may not move backwards, and sideways motion is greatly showed.
	void HandleSprinting(float h, float v) {
		if (v < 0) {
			v = 0;
			anim.SetFloat ("vertical", 0);
		}
		transform.Translate (h/9, 0, v * speed*3);
		anim.SetFloat ("vertical", v);
		anim.SetFloat ("horizontal", h);
	}

	//If the character is moving both vertically and horizontally, both speeds are divided by 1.4 so combined speed is still capped at 100%
	//1.4~sqrt.2 so the hypotenuse of the player moving diagonally is the same speed as if he was moving in just one direction
	void HandleStrafing(float h, float v) {
		if (h != 0 && v != 0) {
			transform.Translate (h * speed * 1 / 1.4f, 0, v * speed * 1 / 1.4f);
		} else {
			transform.Translate (h * speed, 0, v * speed);
			anim.SetFloat ("horizontal", h);
			anim.SetFloat ("vertical", v);
		}
	}
}
