using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandsScript : MonoBehaviour {


	GameObject[] friendlies;


	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		friendlies = GameObject.FindGameObjectsWithTag ("Friendly");




		//charge command
		if (Input.GetKey ("1")) {
			Debug.Log ("charge!");
			foreach (GameObject troop in friendlies) {

				FriendlyScript script = troop.GetComponent<FriendlyScript> ();

				script.setCommand ("charge");
			}
		}

		//follow command
		if (Input.GetKey ("2")) {
			Debug.Log ("follow me!");
			foreach (GameObject troop in friendlies) {
				FriendlyScript script = troop.GetComponent<FriendlyScript> ();
				script.setCommand ("follow");
			}
		}

	}
}
