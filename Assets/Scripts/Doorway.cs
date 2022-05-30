using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorway : MonoBehaviour
{
        [SerializeField] bool openOnProximity = true;
        [SerializeField] Transform leftDoor;
        [SerializeField] Transform rightDoor;

	public void Open()
	{
		leftDoor.position -= leftDoor.right * 2;
		rightDoor.position += leftDoor.right * 2;
	}

	public void Close()
	{
		leftDoor.position += leftDoor.right * 2;
		rightDoor.position -= leftDoor.right * 2;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(openOnProximity && other.tag == "Player")
		{
			Open();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (openOnProximity && other.tag == "Player")
		{
			Close();
		}
	}
}
