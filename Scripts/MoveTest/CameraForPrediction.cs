using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraForPrediction
{
	private Transform transform;
	private Camera camera;

	private Vector2 joystickDir;
	private Vector3 shouldPosition;

	public Vector3 position {
		get => transform.position; 
		private set {
			transform.position = value;
		}
	}

	public Vector3 angles { 
		get => transform.eulerAngles;
		private set => transform.eulerAngles = value;
	}

	public CameraForPrediction(Camera c) {
		camera = c;
		transform = camera.transform;
	} 

	public void drawGizmos (CameraController config)
	{

	}
}
