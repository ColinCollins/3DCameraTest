using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class PlayerController : MonoBehaviour
{
	private const float FALL_INIT_SPEED = 0.00001f;

	public CharacterController characterCtrl;

	[BoxGroup("Config"), Tooltip("Move Speed Step Per Sec")]
	public float moveSpeedSp = 1f;
	[BoxGroup("Config"), Tooltip("Move Speed")]
	public float moveSpeed = 1;
	[BoxGroup("Config"), Tooltip("Rotate Step Per Sec")]
	public float rotateDegreeSp = 1;
	[BoxGroup("Config"), Tooltip("Rotate Speed")]
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

	public bool Move(Vector2 dir)
	{
		moveDir = dir.normalized;
		rotate2Yaw = moveDir.calcYaw().uniformAngle360();

		return true;
	}

	public void StopMove()
	{
		moveDir = Vector2.zero;
	}

	public void Jump()
	{
		if (!inTheAir)
		{
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
		moveVec.z = vec.y;

		if (!inTheAir && !characterCtrl.isGrounded)
		{
			verticalSpeed = FALL_INIT_SPEED;
			inTheAir = true;
		}

		if (inTheAir)
		{
			moveVec.y = verticalSpeed * deltaTime;
		}

		// move
		var collitionFlags = characterCtrl.Move(moveVec);

		// fater move
		if (characterCtrl.isGrounded)
		{
			verticalSpeed = 0f;
			inTheAir = false;
		}
		else if (collitionFlags == CollisionFlags.CollidedAbove)
		{
			verticalSpeed = FALL_INIT_SPEED;
		}
		else
		{
			verticalSpeed += gravity * deltaTime;
		}
	}

	private void tickYaw(float time, float deltaTime)
	{
		var currentAngles = angles;
		var currentYaw = currentAngles.y.uniformAngle360();

		if ((rotate2Yaw - currentYaw).equalsZero()) return;

		var deltaDegree = rotateDegreeSp * rotateSpeed * deltaTime;
		currentAngles.y = Mathf.MoveTowardsAngle(currentYaw, rotate2Yaw, deltaDegree);
		angles = currentAngles;
	}

	private void Update()
	{
		var time = Time.time;
		var deltaTime = Time.deltaTime;

		tickMove(time, deltaTime);
		tickYaw(time, deltaTime);
	}

}
