using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
	[SerializeField] private bool canSkip = false;
	[SerializeField] private KeyCode skipButton;

	private CharacterController player;
	private bool played = false;
	private Coroutine runScene = null;

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

			runScene = StartCoroutine(Wait(duration));
		}
	}

	public void Update()
	{
		if (runScene != null && canSkip && Input.GetKeyDown(skipButton))
		{
			StopCoroutine(runScene);
			runScene = null;
			PlayerStats.CanLook = true;
			PlayerStats.CanMove = true;
		}
	}

	IEnumerator Wait(float amt)
	{
		yield return new WaitForSeconds(amt);
		runScene = null;
		PlayerStats.CanLook = true;
		PlayerStats.CanMove = true;
	}
}
