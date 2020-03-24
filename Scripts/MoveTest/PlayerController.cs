using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class PlayerController : MonoBehaviour
{
	private const float FALL_INIT_SPEED = 0.0000f;

	public CharacterControllerTest characterCtrl;

	[BoxGroup("Config"), Tooltip("Move Speed Step Per Sec")]
	public float moveSpeedSp = 1f;
	[BoxGroup("Config"), Tooltip("Move Speed")]
	public float moveSpeed = 1;
	[BoxGroup("Config"), Tooltip("Rotate Step Per Sec")]
	public float rotateDegreeSp = 1;
	[BoxGroup("Condfig"), Tooltip("Rotate Speed")]
	public float rotateSpeed = 1;
	[BoxGroup("Config")]
	public float jumpInitSpeed = 1;
	[BoxGroup("Config")]
	public float gravity = -9.8f;
	[BoxGroup("Status"), ShowNonSerializedField]
	private Vector2 moveDir;
	[BoxGroup("Status"), ShowNonSerializedField]
	private float rotate2Yaw;
	[BoxGroup("Status"), ShowNonSerializedField]
	private float verticalSpeed;
	[BoxGroup("Status"), ShowNonSerializedField]
	private bool inTheAir;

	public Vector3 position
	{
		get
		{
			return transform.position;
		}
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

	public bool move(Vector2 dir)
	{
		moveDir = dir.normalized;
		rotate2Yaw = moveDir.calcYaw().uniformAngle360();

		return true;
	}

	public void stopMove() 
	{
		moveDir = Vector2.zero;
	}

	public void jump() 
	{
		if (!inTheAir) {
			verticalSpeed = jumpInitSpeed;
			inTheAir = true;
		}
	}

	private void tickMove(float time, float deltaTime) 
	{
		var moveVec = Vector3.zero;

		// horizantial 
		var vec = moveDir.normalized * moveSpeedSp * moveSpeed * deltaTime;

		moveVec.x = vec.x;
		moveVec.y = vec.y;

		if (!inTheAir && !characterCtrl.isGrounded) {
			verticalSpeed = FALL_INIT_SPEED;
			inTheAir = true;
		}

		if ()

	}



}
