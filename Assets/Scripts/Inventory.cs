using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {


	//private Item[][] inventory;
	public int width;
	public int height;

	public GUISkin skin;
	public GUIStyle medeivalText;

	private bool buttonPressed = false;

	void Start () {
		int[] g = new int[2];
		Item[,] inventory = new Item[width,height];
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown("tab")){
			buttonPressed = !buttonPressed;
		
		}


	}

	void OnGUI() {
		GameObject camera = GameObject.FindGameObjectWithTag ("CameraHolder");
		if (buttonPressed) {
			Time.timeScale = 0;

			camera.SetActive (false);
			camera.SetActive (true);
			GUI.skin = skin;

			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {

					//TODO: Fix this inventory box to be icer
					//Make it conform to the size of the window
					GUI.Box (new Rect (100 + i * 70, 100 + j * 70, 70, 70), "", skin.GetStyle ("Slot"));

			
				}
			}
			GUI.Box (new Rect (100, 10,70*width,90), "Inventory", skin.GetStyle ("Slot"));

		} else {
			Time.timeScale = 1;
			camera.SetActive (true);
		}
	
	
	}
}
