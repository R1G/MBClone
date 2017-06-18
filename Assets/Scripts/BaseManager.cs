using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour {

	public string teamName;
	public float baseHealth;
	int spawnCount;
	int spawnLimit = 20;



	void Start () {

		if (teamName == "BLUE") {
			InvokeRepeating ("SpawnFriendlies", 1f, 20f);
		} else if (teamName == "RED") {
			spawnLimit += 10;
			InvokeRepeating ("SpawnEnemies", 1f, 20f);
		}
	}

	void Update () {
		if (baseHealth <= 0) {
			DestroyBase ();
		}


	}

	void OnTriggerEnter(Collider coll) {
		if (coll.tag == "Weapon") {
			baseHealth -= 50f;
		}
	}


	void SpawnFriendlies() {
		if (spawnCount < spawnLimit - 3) {
			Instantiate (Resources.Load ("Friendly"), transform.position, Quaternion.identity);
			Instantiate (Resources.Load ("Friendly"), transform.position + new Vector3 (1, 0, 1), Quaternion.identity);
			Instantiate (Resources.Load ("Friendly"), transform.position + new Vector3 (-1, 0, -1), Quaternion.identity);
			spawnCount += 3;
		}

	}

	void SpawnEnemies() {
		if (spawnCount < spawnLimit - 5) {
			Instantiate (Resources.Load ("Enemy"), transform.position, Quaternion.identity);
			Instantiate (Resources.Load ("Enemy"), transform.position + new Vector3 (1, 0, 1), Quaternion.identity);
			Instantiate (Resources.Load ("Enemy"), transform.position + new Vector3 (-1, 0, -1), Quaternion.identity);
			Instantiate (Resources.Load ("Enemy"), transform.position + new Vector3 (-1, 0, 1), Quaternion.identity);
			Instantiate (Resources.Load ("Enemy"), transform.position + new Vector3 (1, 0, -1), Quaternion.identity);
			spawnCount += 5;
		}
	}

	void DestroyBase() {
		Destroy (gameObject.GetComponent<MeshRenderer>());
		Destroy (gameObject.GetComponent<MeshCollider>());
		Destroy (this);
		Destroy (gameObject);
	}
}
