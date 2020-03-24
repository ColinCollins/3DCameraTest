using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilPhysics
{
	public const float COLLISON_ACCURACY = 0.05f;
	public static bool physicsEqualsZero(this float v) 
	{
		return Mathf.Abs(v) < 0.00001;	
	}

	public static bool sphereCast(Vector3 origin, float radius, Vector3 dir, LayerMask obstacleLayers, out RaycastHit hit, out Vector3 hitPoint, float maxDis = float.MaxValue) {
		if (!Physics.SphereCast(origin, radius, dir, out hit, maxDis, obstacleLayers)) {
			hitPoint = Vector3.zero;
			return false;
		}

		hitPoint = origin + dir.normalized * hit.distance;

		return true;
 	}
}
