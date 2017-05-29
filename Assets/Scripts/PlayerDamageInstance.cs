using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageInstance : MonoBehaviour {

	void Update() {
		Invoke ("SelfDestroy", .3f);
	}
		
	void OnTriggerEnter(Collider coll) {
		coll.gameObject.SendMessageUpwards ("TakeDamage", 50f, SendMessageOptions.DontRequireReceiver);
	}

	void SelfDestroy() {
		Destroy (gameObject);
	}
}
