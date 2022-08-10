using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
	[SerializeField] private Vector3 position;
	[SerializeField] private Vector3 rotation;
	[SerializeField] private float cameraRotationX;
	[SerializeField] private float cameraRotationY;
	[SerializeField] private float duration;
	[SerializeField] private bool canMove = false;
	[SerializeField] private bool canLook = false;

	private CharacterController player;
	private bool played = false;

	private void Awake()
	{
		player = FindObjectOfType<CharacterController>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!played)
		{
			played = true;
			PlayerStats.CanLook = canLook;
			PlayerStats.CanMove = canMove;

			player.SetCharacter(position, Quaternion.Euler(rotation), cameraRotationX, cameraRotationY);

			StartCoroutine(Wait(duration));
		}
	}

	IEnumerator Wait(float amt)
	{
		yield return new WaitForSeconds(amt);

		PlayerStats.CanLook = true;
		PlayerStats.CanMove = true;
	}
}
