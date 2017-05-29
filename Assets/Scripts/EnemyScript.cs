using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour {

	public float EnemyHealth;

	Animator enemyAnim;
	Rigidbody rb;
	NavMeshAgent agent;

	GameObject player;
	public GameObject attackOrigin;
	public GameObject weapon;

	GameObject attacker;

	public bool isDead;
	bool hasTarget;
	bool IsInFightingRange;
	public bool isAttacking;

	void Start() {
		weapon.tag = "Untagged";
		enemyAnim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody> ();
		agent = GetComponent<NavMeshAgent> ();
		isDead = false;

	}

	void OnTriggerEnter(Collider coll) {
		if (coll.gameObject.tag == "Weapon") {
			EnemyHealth -= 50f;
		}

	}



	void Update() {
		//Checking for agent to avoid errors when it gets deleted after death
		if (EnemyHealth <= 0) {
			EnemyDeath ();

		}
		if (agent != null) {
			/*If the enemy doesn't have a friendly to move to and fight:
				Find someone new to move to and attack, or if you cant find one
					stand still and don't animate

			*/

			if (!hasTarget) {
				player = DesignateTarget (20f);
				if (player != null) {
					hasTarget = true;
				} else {
					hasTarget = false;
					if (GameObject.FindGameObjectWithTag ("BlueBase") != null) {
						agent.SetDestination (GameObject.FindGameObjectWithTag ("BlueBase").transform.position);
					} else {
						return;
					}
					enemyAnim.SetFloat ("vertical", 1);

				}
			} else {


				if (!IsInFightingRange) {
					if (player.GetComponent<CapsuleCollider> () == null) {
						hasTarget = false;
					} else {
						agent.SetDestination (player.transform.position);

						enemyAnim.SetFloat ("vertical", 1f);
					}
					if (Vector3.Distance (gameObject.transform.position, player.transform.position) <= agent.stoppingDistance) {
						IsInFightingRange = true;
						enemyAnim.SetFloat ("vertical", 0);
					} else {
						IsInFightingRange = false;
						enemyAnim.SetFloat ("vertical", 1f);
					}
				} else {
					
					//if enemy is close enough to fight
					//checks if enemy is close enough to fight
					if (Vector3.Distance (gameObject.transform.position, player.transform.position) <= agent.stoppingDistance) {
						IsInFightingRange = true;
						enemyAnim.SetFloat ("vertical", 0);
					} else {
						IsInFightingRange = false;
						enemyAnim.SetFloat ("vertical", 1f);
					}

					if (player.GetComponent<CapsuleCollider> () == null) {
						CancelInvoke ();
						hasTarget = false;
						IsInFightingRange = false;
						enemyAnim.SetBool ("leftHeld", false);
					} else {
						Invoke ("EnemyPrep", 1f);
						Invoke ("EnemyAttack", .2f);
						enemyAnim.SetFloat ("vertical", 0);
					}

				}


			}
		}

		if (isAttacking) {
			weapon.tag = "Weapon";
		} else {
			weapon.tag = "Untagged";
		}
	}

	void EnemyTakeDamage(float damage) {
		EnemyHealth -= damage;
	}

	void RotateTowardsPlayer(Transform target) {
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime*agent.angularSpeed);
	}

	bool CheckIfInFightingRange(Transform target) {
		float distance = Vector3.Distance (target.position, transform.position);
		return distance <= 6f;
	}


	void EnemyDeath() {
		Instantiate (Resources.Load ("Ragdoll") as GameObject, transform);
		Instantiate(Resources.Load ("Sword") as GameObject, transform);
		gameObject.tag = "Dead";
		Destroy (gameObject.GetComponentInChildren<SkinnedMeshRenderer> ());
		Destroy (gameObject.GetComponent<CapsuleCollider> ());
		Destroy (weapon);
		Destroy (agent);
		Destroy (this);

	}

	void EnemyAttack() {
		enemyAnim.SetTrigger ("LeftHit");
		isAttacking = false;
	}

	void EnemyPrep() {
		enemyAnim.SetBool ("leftHeld", true);
		isAttacking = true;
	}

	GameObject DesignateTarget(float searchRange) {
		GameObject[] friendlies = GameObject.FindGameObjectsWithTag ("Friendly");
		float distance;
		float currentDistance = searchRange;
		GameObject target = null;
		foreach (GameObject friendly in friendlies) {
			distance = Vector3.Distance (gameObject.transform.position, friendly.transform.position);
				if(distance < currentDistance) {
					currentDistance = distance;
					target = friendly;
			}
		}
		return target;
	}

	void AssignAttacker(GameObject _attacker) {
		attacker = _attacker;
	}
}
