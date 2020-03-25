using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraForPrediction
{
	private enum SeeResult
	{
		CAN_NOT_SEE,        // 看不见镜头，看不见目标
		CAN_SEE,            // 目标和相机可以相互看见
		CAN_SEE_SINGLE      // 目标看不见镜头，但是镜头可以看见目标
	}

	private Transform transform;
	private Camera camera;

	private Vector2 joystickDir;
	private Vector3 shouldPosition;

	public Vector3 position
	{
		get => transform.position;
		private set
		{
			transform.position = value;
		}
	}

	public Vector3 angles
	{
		get => transform.eulerAngles;
		private set => transform.eulerAngles = value;
	}

	public CameraForPrediction(Camera c)
	{
		camera = c;
		transform = camera.transform;
	}

	public void drawGizmos(CameraController config)
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(shouldPosition, config.collisionRadius);

		var targetP = config.followPosition;
		var seeRet = canSee(config, position, targetP, out Vector3 collisionP, out float collisionDistance);

		if (SeeResult.CAN_SEE == seeRet)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(position, targetP);
		}
		else
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(collisionP, targetP);
			Gizmos.DrawWireSphere(collisionP, config.collisionRadius);
		}
	}

	public void setJoystickDir(Vector2 dir) {
		joystickDir = dir;
	}

	public void tick(CameraController config, float time, float deltaTime) {
		tickCamera(config, time, deltaTime);

		var canSee = tickJoystick(config, time, deltaTime);
		if (!canSee) {
			tickSee(config, time, deltaTime);
		}

		tickFollow(config, time, deltaTime);
	}



}
