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
				foreach (RaycastHit hit in Physics.RaycastAll(camera.position, camera.forward, 100.0f))
				{
					if (hit.transform.tag == "Glass") { break; }
					if (hit.transform.tag == "Object")
					{
						selectedObject = hit.transform.gameObject.GetComponent<GravityObject>();
						break;
					}
					else if (hit.transform.tag == "Ground")
					{
						if (PlayerStats.CanSetObjectGravity && selectedObject != null)
						{
							//change gravity of that object
							selectedObject.ChangeGravity(-hit.normal);

							selectedObject = null;
						}
						else if(PlayerStats.CanSetPlayerGravity)
						{
							gravityDirection = -hit.normal;
							gravityMagnitude = 0.0f;
							Debug.DrawRay(Vector3.zero, gravityDirection, Color.black, 10);

							//Rotate Player
							startRot = transform.rotation;
							endRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
							rotateTimer = 1.0f;
						}

						break;
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
		}
	}
}
