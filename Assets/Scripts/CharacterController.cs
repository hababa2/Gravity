using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
	private static readonly float WALK_SPEED = 400.0f;
	private static readonly float RUN_SPEED = 600.0f;
	private static readonly float AIR_SPEED = 300.0f;
	private static readonly float JUMP_FORCE = 100.0f;
	private static readonly float MASS = 5.0f;
	private static readonly float INV_MASS = 1.0f / MASS;
	private static readonly float GRAVITY = 9.8f;
	private static readonly float JUMP_COOLDOWN = 0.1f;
	private static readonly int GROUND_MASK = 1 << 10;
	private static readonly int GROUND_OBJECT_MASK = (1 << 10) | (1 << 13);
	private static readonly int GROUND_CHECK_MASK = (1 << 10) | (1 << 13) | (1 << 14);

	private Vector3 movement = Vector3.zero;
	private Vector3 velocity = Vector3.zero;
	private Vector3 force = Vector3.zero;

	private Vector3 gravityDirection = Vector3.down;
	private bool grounded = false;

	private Quaternion startRot;
	private Quaternion endRot;

	private float rotateTimer = 0.0f;
	private float jumpTimer = 0.0f;
	private float cutsceneTimer = 0.0f;

	private Vector3 startPos;
	private Vector3 endPos;
	private Quaternion startCamX;
	private Quaternion endCamX;
	private Quaternion startCamY;
	private Quaternion endCamY;

	private Rigidbody rb;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private Transform orientation;
	private new Transform camera;

	private GravityObject selectedObject;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.detectCollisions = true;

		camera = Camera.main.transform;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	//Left click to select wall, Right click to select object and wall

	void FixedUpdate()
	{
		jumpTimer -= Time.fixedDeltaTime;
		cutsceneTimer -= Time.fixedDeltaTime;
		rotateTimer -= Time.fixedDeltaTime;

		grounded = Physics.Raycast(groundCheck.position, -transform.up, out _, 0.1f, GROUND_CHECK_MASK);

		if (PlayerStats.CanMove)
		{
			float speed;
			//TODO: implement air speed
			if (Input.GetKey(KeyCode.LeftShift)) { speed = RUN_SPEED; }
			else { speed = WALK_SPEED; }

			movement += orientation.forward * Input.GetAxis("Vertical") * speed * Time.fixedDeltaTime;
			movement += orientation.right * Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime;

			//TODO: Add velocity toward movement direction
			if (grounded && jumpTimer < 0.0f && Input.GetAxis("Jump") > 0.0f)
			{
				force += transform.up * JUMP_FORCE;
				jumpTimer = JUMP_COOLDOWN;
			}

			RaycastHit hit;
			if (PlayerStats.CanSetPlayerGravity && Input.GetButtonDown("Fire1") &&
				Physics.Raycast(camera.position, camera.forward, out hit, Mathf.Infinity, GROUND_MASK))
			{
				Vector3 direction = GetGravityDirection(hit);
				startRot = transform.rotation;
				endRot = Quaternion.FromToRotation(Vector3.up, -direction);
				rotateTimer = 1.0f;
			}

			if (PlayerStats.CanSetObjectGravity && Input.GetButtonDown("Fire2") &&
				Physics.Raycast(camera.position, camera.forward, out hit, Mathf.Infinity, GROUND_OBJECT_MASK))
			{
				if (hit.transform.tag == "Object")
				{
					hit.transform.gameObject.TryGetComponent<GravityObject>(out selectedObject);
				}
				else if (selectedObject != null)
				{
					Vector3 direction = GetGravityDirection(hit);
					selectedObject.ChangeGravity(hit.normal);
				}
			}
		}
		else if (cutsceneTimer >= 0.0f)
		{
			cutsceneTimer -= Time.fixedDeltaTime;
			camera.localRotation = Quaternion.Slerp(endCamX, startCamX, Mathf.Max(cutsceneTimer, 0.0f));
			camera.parent.localRotation = Quaternion.Slerp(endCamY, startCamY, Mathf.Max(cutsceneTimer, 0.0f));
			transform.rotation = Quaternion.Slerp(endRot, startRot, Mathf.Max(cutsceneTimer, 0.0f));
			transform.position = Vector3.Lerp(endPos, startPos, Mathf.Max(cutsceneTimer, 0.0f));
		}

		velocity += gravityDirection * GRAVITY * Time.fixedDeltaTime * MASS;                    //Apply Gravity
		velocity += force * INV_MASS;                                                           //Apply Instant Forces
		velocity -= velocity.normalized * velocity.sqrMagnitude * 0.5f * Time.fixedDeltaTime;   //Apply Drag
		rb.velocity = velocity + movement;

		force = Vector3.zero;
		movement = Vector3.zero;
	}

	public Vector3 GetGravityDirection(RaycastHit hit)
	{
		switch (hit.transform.tag)
		{
			case "Ground": return -hit.normal;
			case "Anti-Ground": return hit.normal;
			case "Mirror-Ground":
				Physics.Raycast(hit.transform.position, hit.transform.forward, out RaycastHit newHit, Mathf.Infinity, GROUND_MASK);
				return GetGravityDirection(newHit);
		}

		Debug.LogError($"Unkown ground type: {hit.transform.tag}");
		return Vector3.zero;
	}

	public void SetCharacter(Vector3 position, Quaternion rotation, float camX, float camY)
	{
		startRot = transform.rotation;
		endRot = rotation;
		startPos = transform.position;
		endPos = position;
		startCamX = camera.localRotation;
		endCamX = Quaternion.Euler(Vector3.right * camX);
		startCamY = camera.parent.localRotation;
		endCamY = Quaternion.Euler(Vector3.up * camY);

		cutsceneTimer = 1.0f;
	}
}
