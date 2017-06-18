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

	private int positionOffset;


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
			

		if (currentCommand.Equals ("charge")) {
			friendlyAICharge ();
		} else if (currentCommand.Equals ("follow")) {
			friendlyAIFollow ();
		} else if (currentCommand.Equals ("vFormation")) {
			friendlyAIVFormation ();
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

				attack ();


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


			if (!hasTarget) {
				//notice that the range is much smaller, so the troops will mostly stick next to you
				target = DesignateTarget (10f);
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


			} else {

				attack ();

			}

			 
		}
	

	}


	private void friendlyAIVFormation() {
		//This first part is used to determine what position each troop should stand in
		//Basically what I do here is I create two rays shooting out behind the player, like in a V
		//Then, I use the positionOffset variable to determine where in the ray the troop should be standing
		//Even means the right ray, odd is the left ray. I subtract 1 from the right ray so the multiplier for the left and right will be the same
		//I do this every frame. I know that seems pretty wasteful. I tried making it so we wouldn't have to do that, but I couldn't get it to work, for some reason
		//The rest of the code should just be the same as your code from earlier
		//Sorry if the code is a little unclear.


		Ray ray;
		int tempPosOffset = positionOffset;
		Vector3 direction = player.transform.forward;


		//I honestly couldn't figure out how to change the euler angle from what you get with transform.forward, so it would be pointing at an angle behind the player
		//I tried messing with quaternions, but that was even worse. What I ended up doing here is making a ghost object, and rotating THAT 130 degrees, which I fortunately do know how to do
		GameObject rotatorTool = new GameObject ();
		rotatorTool.transform.position = player.transform.position;
		rotatorTool.transform.forward = player.transform.forward;
		

		if (!(positionOffset % 2 == 0)) {
			rotatorTool.transform.Rotate (Vector3.up, 130);



		} else {
			rotatorTool.transform.Rotate (Vector3.up, -130);
			tempPosOffset--;
		}

		ray = new Ray (player.transform.position, rotatorTool.transform.forward);
		Destroy (rotatorTool);


		Debug.DrawRay (player.transform.position + Vector3.up, rotatorTool.transform.forward, Color.cyan);
		//The number here represents how far each troop will be from each other in the formation
		Vector3 troopPos = ray.GetPoint (1f * tempPosOffset);







		if (!hasTarget) {
			//notice that the range is much smaller, so the troops will mostly stick next to you
			target = DesignateTarget (10f);
			if (target != null && Vector3.Distance (gameObject.transform.position, player.transform.position) <= followRadius) {
				hasTarget = true;
			} else {
				CancelInvoke ();
				Vector3 targetPos;
				targetPos = troopPos;
				//Debug.DrawLine (player.transform.position, targetPos, Color.green);
				if (gameObject.transform.position != targetPos) {
					if (agent.isActiveAndEnabled)
						agent.SetDestination (targetPos);
				}
					

			}


		} else {

			attack ();

		} 







	}

	private void attack() {
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


	public void setPositionOffset(int num){
		positionOffset = num;
	}


	private Vector3 newRelativeDistancePos(float distance, float angle) {
		//Thanks Ms. Meiman!
		float x = distance * Mathf.Cos(angle * Mathf.Deg2Rad);
		float z = distance * Mathf.Sin(angle * Mathf.Deg2Rad);
		Vector3 newPosition = player.transform.position;
		Vector3 addPos = new Vector3(x,0,z);
		newPosition += addPos;
		return newPosition;
		//Bean there done that
	}





	





		
}
