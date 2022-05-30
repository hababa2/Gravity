using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
	private static readonly float MAX_ANGLE = 90.0f;
	private static readonly float SENSITIVITY = 100.0f;

	private Transform player;
	private Quaternion center;

	void Start()
	{
		player = transform.parent;
		center = transform.rotation;
	}

	void Update()
	{
		player.Rotate(Vector3.up, Input.GetAxis("Mouse X") * SENSITIVITY * Time.deltaTime);

		float mouseY = Input.GetAxis("Mouse Y") * SENSITIVITY * Time.deltaTime;
		Quaternion yQuat = transform.localRotation * Quaternion.Euler(-mouseY, 0.0f, 0.0f);
		if (Quaternion.Angle(center, yQuat) < MAX_ANGLE) { transform.localRotation = yQuat; }
	}
}
