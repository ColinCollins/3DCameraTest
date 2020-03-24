using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilGizmos
{
	public static bool drawCircle(Vector3 origin, float radius, Quaternion rotation, int pieceCount = 100)
	{
		if (pieceCount < 3) return false;

		if (radius < 0) return false;

		float pieceAngle = 360.0f / pieceCount;

		Vector3 p0 = origin + rotation * Vector3.forward * radius;
		Vector3 p1 = p0;

		for (int i = 0; i < pieceCount - 1; i++)
		{
			var r = Quaternion.Euler(0, pieceAngle * (i + 1), 0);
			Vector3 p2 = origin + rotation * (r * Vector3.forward * radius);
			Gizmos.DrawLine(p1, p2);

			p1 = p2;
		}

		Gizmos.DrawLine(p0, p1);

		return true;
	}

	public static bool drawCircle(Vector3 origin, float radius, int pieceCount = 100)
	{
		return drawCircle(origin, radius, Quaternion.identity, pieceCount);
	}

	public static bool drawCylinder(Vector3 origin0, float radius, float high, Quaternion rotation, int pieceCount = 100)
	{
		if (!drawCircle(origin0, radius, rotation, pieceCount)) return false;

		var origin1 = origin0 + rotation * Vector3.up * high;
		drawCircle(origin1, radius, rotation, pieceCount);

		var p00 = origin0 + rotation * Vector3.forward * radius;
		var p01 = origin0 + rotation * Vector3.left * radius;
		var p02 = origin0 + rotation * Vector3.back * radius;
		var p03 = origin0 + rotation * Vector3.right * radius;

		var p10 = origin0 + rotation * Vector3.forward * radius;
		var p11 = origin0 + rotation * Vector3.left * radius;
		var p12 = origin0 + rotation * Vector3.back * radius;
		var p13 = origin0 + rotation * Vector3.right * radius;

		Gizmos.DrawLine(p00, p10);
		Gizmos.DrawLine(p01, p11);
		Gizmos.DrawLine(p02, p12);
		Gizmos.DrawLine(p03, p13);

		return true;
	}

	public static bool drawCylinder(Vector3 origin0, float radius, float high, int pieceCount = 100) {
		return drawCylinder(origin0, radius, high, Quaternion.identity, pieceCount);
	}
}
