using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
	[SerializeField] private Material highlight;
	[SerializeField] private bool isObject = false;

	private new MeshRenderer renderer;

	private void Start()
	{
		renderer = GetComponent<MeshRenderer>();	
	}

	private void OnMouseOver()
	{
		if (((isObject && PlayerStats.CanSetObjectGravity) || (!isObject && PlayerStats.CanSetPlayerGravity)) && Input.GetButtonUp("Fire2"))
		{
			renderer.materials = new Material[] { renderer.materials[0] };
		}
	}

	private void OnMouseUp()
	{
		if (((isObject && PlayerStats.CanSetObjectGravity) || (!isObject && PlayerStats.CanSetPlayerGravity)) && !Input.GetButton("Fire2"))
		{
			renderer.materials = new Material[] { renderer.materials[0] };
		}
	}

	private void OnMouseExit()
	{
		renderer.materials = new Material[] { renderer.materials[0] };
	}

	private void OnMouseDown()
	{
		if (((isObject && PlayerStats.CanSetObjectGravity) || (!isObject && PlayerStats.CanSetPlayerGravity)) && Input.GetButton("Fire2"))
		{
			renderer.materials = new Material[] { renderer.materials[0], highlight };
		}
	}

	private void OnMouseEnter()
	{
		if (((isObject && PlayerStats.CanSetObjectGravity) || (!isObject && PlayerStats.CanSetPlayerGravity)) && Input.GetButton("Fire2"))
		{
			renderer.materials = new Material[] { renderer.materials[0], highlight };
		}
	}
}
