using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats
{
	public static bool CanSetPlayerGravity { get; private set; } = false;
	public static bool CanSetObjectGravity { get; private set; } = false;

	public static bool CanMove { get; set; } = true;
	public static bool CanLook { get; set; } = true;

	public static void Unlock(int i)
	{
		switch (i)
		{
			case 0: CanSetPlayerGravity = true; break;
			case 1: CanSetObjectGravity = true; break;
		}
	}
}
