using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class InputController : MonoBehaviour
{
	[BoxGroup("Config")]
	public string axisHorizontalName = "Horizontal";
	[BoxGroup("Config")]
	public string axisVerticalName = "Vertical";
	[BoxGroup("Config")]
	public float axisHighPass = 0.1f;
	[BoxGroup("Config")]
	public string buttonJumpName = "Jump";

	[BoxGroup("Controller")]
	public PlayerController playerCtrl;
	[BoxGroup("Controller")]
	public CameraController cameraCtrl;

	private bool getMoveDir(out Vector2 dir, out Vector2 joystickDir)
	{
		joystickDir = new Vector2(Input.GetAxis(axisHorizontalName), Input.GetAxis(axisVerticalName));

		if (axisHighPass > Mathf.Abs(joystickDir.x) && axisHighPass > Mathf.Abs(joystickDir.y))
		{
			dir = Vector2.zero;
			return false;
		}

		var tempDir = new Vector3(joystickDir.x, 0, joystickDir.y);
		tempDir = Quaternion.Euler(new Vector3(0, cameraCtrl.angles.y, 0)) * tempDir;

		dir = tempDir.toVector2XZ();
		dir.Normalize();

		return true;
	}

	private void tickMove(float time, float deltaTime)
	{
		if (getMoveDir(out Vector2 dir, out Vector2 joystickDir))
		{
			playerCtrl.Move(dir);
			cameraCtrl.RotateByInput(joystickDir);
		}
		else
		{
			playerCtrl.StopMove();
			cameraCtrl.StopRotate();
		}
	}

	private void tickJump(float time, float deltaTime)
	{
		var v = Input.GetAxis(buttonJumpName);
		if (v.equalsZero())
		{
			return;
		}

		playerCtrl.Jump();
	}

	private void Update()
	{
		if (cameraCtrl == null || playerCtrl == null)
		{
			return;
		}

		var time = Time.time;
		var deltaTime = Time.deltaTime;

		tickMove(time, deltaTime);
		tickJump(time, deltaTime);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawIcon(transform.position, "Cross", true);

		if (Application.isPlaying)
		{
			if (getMoveDir(out Vector2 dir, out Vector2 joystickDir))
			{
				Gizmos.color = Color.green;
				Gizmos.DrawRay(transform.position, joystickDir.normalized * 0.2f);
			}

			var v = Input.GetAxis(buttonJumpName);
			if (!v.equalsZero())
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(transform.position, v * 0.1f);
			}
		}
	}
}
