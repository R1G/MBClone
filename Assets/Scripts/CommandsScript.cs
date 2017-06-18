using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandsScript : MonoBehaviour {


	GameObject[] friendlies;

	private string command;

	private float time;

	public GUIStyle commandText;



	void Start () {
		time = 0;
		command = "charge!";
	}
	
	// Update is called once per frame
	void Update () {
		friendlies = GameObject.FindGameObjectsWithTag ("Friendly");




		//charge command
		if (Input.GetKey ("1")) {

			command = "charge!";

			foreach (GameObject troop in friendlies) {

				FriendlyScript script = troop.GetComponent<FriendlyScript> ();


				script.setCommand ("charge");
			}

			time = 0;
		}

		//follow command
		if (Input.GetKey ("2")) {

			command = "follow me!";

			foreach (GameObject troop in friendlies) {
				FriendlyScript script = troop.GetComponent<FriendlyScript> ();
				script.setCommand ("follow");
			}
			time = 0;
		}



		//v formation command
		//This one's a little weird, since the number of troops currently on the battlefeild affects the positioning of each troop
		//What I do here is I assign a number to each troop, which lets them know what position in the V they are
		//Odd are on the left, even on the right.
		if (Input.GetKey ("3")) {

			command = "V-Formation!";

			for(int i = 0; i < friendlies.Length; i++) {
				GameObject troop = friendlies[i];
				FriendlyScript script = troop.GetComponent<FriendlyScript> ();
				script.setCommand ("vFormation");
				script.setPositionOffset (i + 1);
			}

			time = 0;
		}





	}

	void OnGUI(){
		time += Time.deltaTime;


		Rect commandLabel = new Rect (100, Screen.height-100, 2000, Screen.height);
		if (time <= 2) {
			GUI.Label(commandLabel,"Command: " + command,commandText);
		}


	
	}
}

