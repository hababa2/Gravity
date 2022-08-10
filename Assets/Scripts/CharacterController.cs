using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
	private static readonly float MAX_VELOCITY = 25.0f;
	private static readonly float SPEED = 800.0f;
	private static readonly float JUMP = 8.0f;
	private static readonly float JUMP_FALLOFF = JUMP;

	private Vector3 velocity = Vector3.zero;
	private Vector3 jumpVelocity = Vector3.zero;

	private Vector3 gravityDirection = Vector3.down;
	private float gravityMagnitude = 0.0f;
	private float gravityForce = 9.8f;
	private bool grounded = false;

	private Quaternion startRot;
	private Quaternion endRot;
	private float rotateTimer = -1.0f;

	private float cutsceneTimer = -1.0f;
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

	void FixedUpdate()
	{
		if (PlayerStats.CanMove)
		{
			grounded = Physics.Raycast(groundCheck.position, -transform.up, 0.06f);

			if (Input.GetButton("Fire2") && Input.GetButtonDown("Fire1"))
			{
				RaycastHit hit;

				if (Physics.Raycast(camera.position, camera.forward, out hit, 100.0f))
				{
					if (hit.transform.tag == "Object")
					{
						selectedObject = hit.transform.gameObject.GetComponent<GravityObject>();
					}
					else if (hit.transform.tag == "Ground")
					{
						if (PlayerStats.CanSetObjectGravity && selectedObject != null)
						{
							selectedObject.ChangeGravity(-hit.normal);

							selectedObject = null;
						}
						else if (PlayerStats.CanSetPlayerGravity)
						{
							gravityDirection = -hit.normal;
							gravityMagnitude = 0.0f;

							startRot = transform.rotation;
							endRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
							rotateTimer = 1.0f;
						}
					}
					else if (hit.transform.tag == "Anti-Ground")
					{
						if (PlayerStats.CanSetObjectGravity && selectedObject != null)
						{
							selectedObject.ChangeGravity(hit.normal);

							selectedObject = null;
						}
						else if (PlayerStats.CanSetPlayerGravity)
						{
							gravityDirection = hit.normal;
							gravityMagnitude = 0.0f;

							startRot = transform.rotation;
							endRot = Quaternion.FromToRotation(Vector3.up, -hit.normal);
							rotateTimer = 1.0f;
						}
					}
				}
			}

			if (rotateTimer >= 0.0f)
			{
				rotateTimer -= Time.fixedDeltaTime;
				transform.rotation = Quaternion.Slerp(endRot, startRot, Mathf.Max(rotateTimer, 0.0f));
			}

			velocity = Vector3.zero;
			if (grounded) { gravityMagnitude = 0; }
			else { gravityMagnitude = Mathf.Min(gravityMagnitude + gravityForce * Time.fixedDeltaTime, MAX_VELOCITY); }

			velocity += gravityDirection * gravityMagnitude;

			velocity += orientation.forward * Input.GetAxis("Vertical") * SPEED * Time.fixedDeltaTime;
			velocity += orientation.right * Input.GetAxis("Horizontal") * SPEED * Time.fixedDeltaTime;

			if (jumpVelocity.sqrMagnitude > 0.0f) { jumpVelocity -= transform.up * JUMP_FALLOFF * Time.fixedDeltaTime; }
			else { jumpVelocity = Vector3.zero; }
			if (grounded && Input.GetAxis("Jump") > 0.0f) { jumpVelocity = transform.up * JUMP; }

			velocity += jumpVelocity;

			rb.velocity = velocity;
		}
		else
		{
			rb.velocity = Vector3.zero;

			if (cutsceneTimer >= 0.0f)
			{
				cutsceneTimer -= Time.fixedDeltaTime;
				camera.localRotation = Quaternion.Slerp(endCamX, startCamX, Mathf.Max(cutsceneTimer, 0.0f));
				camera.parent.localRotation = Quaternion.Slerp(endCamY, startCamY, Mathf.Max(cutsceneTimer, 0.0f));
				transform.rotation = Quaternion.Slerp(endRot, startRot, Mathf.Max(cutsceneTimer, 0.0f));
				transform.position = Vector3.Lerp(endPos, startPos, Mathf.Max(cutsceneTimer, 0.0f));
			}
		}
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
