using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilMath
{
	public static bool equalsZero(this float v)
	{
		return Mathf.Abs(v) < float.Epsilon;
	}

	public static float uniformAngle360(this float angle)
	{
		return angle - Mathf.Floor(angle / 360) * 360;
	}
}
