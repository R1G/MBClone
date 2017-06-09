using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollScript : MonoBehaviour {


	void Start () {
		Invoke ("DestroyRagdoll", 5f);
	}

	void DestroyRagdoll() {
		Destroy (this);
	}

}
