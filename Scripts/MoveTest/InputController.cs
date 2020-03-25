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

	private bool getMoveDir(out Vector2 dir, out Vector2 joystickDir) {
		joystickDir = new Vector2(Input.GetAxis(axisHorizontalName), Input.GetAxis(axisVerticalName));

		if (axisHighPass > Mathf.Abs(joystickDir.x) && axisHighPass > Mathf.Abs(joystickDir.y)) {
			dir = Vector2.zero;
			return false;
		}

		var tempDir = new Vector3(joystickDir.x, 0, joystickDir.y);
		tempDir = Quaternion.Euler(new Vector3(0, cameraCtrl.angles.y, 0)) * tempDir;

		dir = tempDir.toVector2XZ();
		dir.Normalize();

		return true;
	}

	private void tickMove(float time, float deltaTime) {
		if (getMoveDir(out Vector2 dir, out Vector2 joystickDir))
		{
			playerCtrl.move(dir);
			cameraCtrl.RotateByInput(joystickDir);
		}
		else {
			playerCtrl.stopMove();
			cameraCtrl.stopRotate();
		}
	}


}
