using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FriendlyScript : MonoBehaviour {

	GameObject player;



	float range;
	bool hasTarget;
	GameObject target;
	NavMeshAgent agent;
	Animator friendlyAnim;
	public GameObject weapon;
	Rigidbody rb;
	public float friendlyHealth;
	public GameObject attackOrigin;
	GameObject attacker;

	public bool isDead;
	bool IsInFightingRange;
	public bool isAttacking;

	private string currentCommand;
	public float followRadius;
	private float followPlayerDistance;
	private float followPlayerAngle;



	void Start() {


		//finds player. I just decided to do this by name, since the player tag is already being used for something else
		//expensive operation, but we should only need to do it once
		player = GameObject.Find ("Player");

		weapon.tag = "Untagged";
		agent = GetComponent<NavMeshAgent> ();	
		friendlyAnim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody> ();
		isDead = false;


		//default command;
		currentCommand = "charge";
		followRadius = 5;

		followPlayerDistance = -1;
		followPlayerAngle = -1;
	}
		
	void OnTriggerEnter(Collider coll) {
		if (coll.tag == "Weapon") {
			friendlyHealth -= 50;
		}
	
	}

	void Update() {
		//Checking for agent to avoid errors when it gets deleted after death
		if (friendlyHealth <= 0) {
			FriendlyDeath ();

		}
			

		if (currentCommand.Equals("charge")){
			friendlyAICharge();
		} else if (currentCommand.Equals("follow")){
			friendlyAIFollow();
		}
			


		if (isAttacking) {
			weapon.tag = "Weapon";
		} else {
			weapon.tag = "Untagged";
		}
	}

	void FriendlyTakeDamage(float damage) {
		friendlyHealth -= damage;
	}



	void RotateTowardsEnemy(Transform target) {
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime*agent.angularSpeed);
	}

	bool CheckIfInFightingRange(Transform target) {
		float distance = Vector3.Distance (target.position, transform.position);
		return distance <= agent.stoppingDistance;
	}


	GameObject FindClosestEnemy(float searchRange) {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		float distance;
		float currentDistance = searchRange;
		GameObject target = null;
		foreach (GameObject enemy in enemies) {
			distance = Vector3.Distance (gameObject.transform.position, enemy.transform.position);
			if(distance < currentDistance) {
				currentDistance = distance;
				target = enemy;
			}
		}
		return target;
	}

	void FriendlyDeath() {
		Instantiate (Resources.Load ("Ragdoll") as GameObject, transform);
		Instantiate(Resources.Load ("Sword") as GameObject, transform);
		gameObject.tag = "Dead";
		Destroy (gameObject.GetComponentInChildren<SkinnedMeshRenderer> ());
		Destroy (gameObject.GetComponent<CapsuleCollider> ());
		Destroy (weapon);
		Destroy (agent);
		Destroy (this);

	}	

	void FriendlyAttack() {
		friendlyAnim.SetTrigger ("LeftHit");
		isAttacking = false;
	}

	void FriendlyPrep() {
		friendlyAnim.SetBool ("leftHeld", true);
		isAttacking = true;
	}

	void AssignAttacker(GameObject _attacker) {
		attacker = _attacker;
	}

	GameObject DesignateTarget(float searchRange) {
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		float distance;
		float currentDistance = searchRange;
		GameObject target = null;
		foreach (GameObject enemy in enemies) {
			distance = Vector3.Distance (gameObject.transform.position, enemy.transform.position);
			if(distance < currentDistance) {
				currentDistance = distance;
				target = enemy;
			}
		}
		return target;
	}

	public void setCommand(string command) {
		currentCommand = command;
	}



	private void friendlyAICharge(){
	
		if (agent != null) {
			/*If the enemy doesn't have a friendly to move to and fight:
				Find someone new to move to and attack, or if you cant find one
					stand still and don't animate

			*/

			if (!hasTarget) {
				target = DesignateTarget (20f);
				if (target != null) {
					hasTarget = true;
				} else {
					hasTarget = false;

					//if this troop is still on the map
					//Without this, setDestination() would still be called even when the troop was eliminated
					if (agent.isActiveAndEnabled)
						agent.SetDestination (GameObject.FindGameObjectWithTag ("RedBase").transform.position);

					friendlyAnim.SetFloat ("vertical", 1);

				}
			} else {


				if (!IsInFightingRange) {
					if (target.GetComponent<CapsuleCollider> () == null) {
						hasTarget = false;
					} else {

						if (agent.isActiveAndEnabled)
							agent.SetDestination (target.transform.position);

						friendlyAnim.SetFloat ("vertical", 1f);
					}
					if (Vector3.Distance (gameObject.transform.position, target.transform.position) <= agent.stoppingDistance) {
						IsInFightingRange = true;
						friendlyAnim.SetFloat ("vertical", 0);
					} else {
						IsInFightingRange = false;
						friendlyAnim.SetFloat ("vertical", 1f);
					}
				} else {

					//if enemy is close enough to fight
					//checks if enemy is close enough to fight
					if (Vector3.Distance (gameObject.transform.position, target.transform.position) <= agent.stoppingDistance) {
						IsInFightingRange = true;
						friendlyAnim.SetFloat ("vertical", 0);
					} else {
						IsInFightingRange = false;
						friendlyAnim.SetFloat ("vertical", 1f);
					}

					if (target.GetComponent<CapsuleCollider> () == null) {
						CancelInvoke ();
						hasTarget = false;
						IsInFightingRange = false;
						friendlyAnim.SetBool ("leftHeld", false);
					} else {
						Invoke ("FriendlyPrep", 1f);
						Invoke ("FriendlyAttack", .2f);
						friendlyAnim.SetFloat ("vertical", 0);
					}

				}


			}
		}
	
	}

	//if the command is set to "follow"
	private void friendlyAIFollow(){
		if (agent != null) {

			//checks if the distance between the troop and the player is greater than "followRadius"
			//if so, it assigns the troop a random distance between the player and the followRadius
			//it sets a new position at this distance by taking a random point within a sphere of the followRadius
			//A sphere is used instead of a circle, because insideUnitSphere only returns X and Y values, and we want X and Z
			//Then, the angle between that point and player is taken
			//Since we have the angle and the distance, we will always know what point the troop should be at relative to the player
			if (Vector3.Distance (gameObject.transform.position, player.transform.position) > followRadius && followPlayerDistance == -1 && followPlayerAngle == -1) {

				followPlayerDistance = Random.Range (0, followRadius);
				Vector3 randPoint = Random.insideUnitSphere * followRadius + player.transform.position;

				followPlayerAngle = Vector3.Angle (player.transform.position, randPoint);


			
			} else if (followPlayerDistance == -1 && followPlayerAngle == -1) {
				followPlayerDistance = Vector3.Distance (gameObject.transform.position, player.transform.position);
				followPlayerAngle = Vector3.Angle (gameObject.transform.position, player.transform.position); 

			}

			//TODO: Attack stuff
			if (!hasTarget) {
				target = DesignateTarget (5f);
				if (target != null && Vector3.Distance (gameObject.transform.position, player.transform.position) <= followRadius) {
					hasTarget = true;
				} else {
					hasTarget = false;
					//if the troop is not its correct distance away from the player, it will move towards the player until it is
					//This check on the line below allows the troop to be within 1 unit of its target distance
					//I'm hoping this will account for walls
					if (Vector3.Distance (gameObject.transform.position, player.transform.position) < followPlayerDistance - 1
						|| Vector3.Distance (gameObject.transform.position, player.transform.position) > followPlayerDistance + 1) {

						Vector3 targetPos = newRelativeDistancePos (followPlayerDistance, followPlayerAngle);


						if (agent.isActiveAndEnabled)
							agent.SetDestination (targetPos);
					}

				}
			}

			 
		}
	

	}


	private Vector3 newRelativeDistancePos(float distance, float angle) {
		float x = distance * Mathf.Cos(angle * Mathf.Deg2Rad);
		float z = distance * Mathf.Sin(angle * Mathf.Deg2Rad);
		Vector3 newPosition = player.transform.position;
		newPosition.x += x;
		newPosition.z += z;
		return newPosition;
	
	}





	





		
}
