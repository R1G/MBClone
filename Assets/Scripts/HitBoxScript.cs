using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxScript : MonoBehaviour {

	CombatScript script;
	float hitDamage = 50f;

	void Start () {
		script = GameObject.FindGameObjectWithTag ("Player").GetComponentInChildren<CombatScript> ();
		if (gameObject.name == "HeadHitBox") {
			hitDamage = hitDamage * 6;
		} 
		Debug.Log (hitDamage + " " + gameObject.name);
	}

	void OnTriggerEnter(Collider coll) {
		if (coll.gameObject.tag == "Weapon") {
			script.SendMessage ("TakeDamage");
			Debug.Log ("Player has taken " + hitDamage + " to the " + gameObject.name);
		}
	}
}
