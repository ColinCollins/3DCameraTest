using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class CameraController : MonoBehaviour
{
	public new Camera camera;
	public Camera predictionCamera;

	[BoxGroup("Follow")]
	public PlayerController player;
	[BoxGroup("Follow")]
	public Vector3 followTargetOffset;
	[BoxGroup("Follow")]
	public Vector3 followViewport = new Vector3(0.5f, 0.5f, 5f); // why?
	[BoxGroup("Follow"), MinMaxSlider(0.01f, 1.0f)]
	public float followSmoothTime = 0.3f;
	[BoxGroup("Follow")]
	public float zoom = 1f;

	[BoxGroup("Rotation"), MinMaxSlider(0.01f, 1.0f)]
	public float yawSmoothTime = 0.3f;
	[BoxGroup("Rotation"), MinMaxSlider(0.01f, 1.0f)]
	public float pitchSmoothTime = 0.3f;

	[BoxGroup("Joystick"), MinMaxSlider(0f, 90f), Tooltip("摇杆相对于垂直线的角度大于此值，相机会左右调整 yaw 值，调整速度和摇杆角度有关")]
	public float joystickHighPass = 0.3f;
	[BoxGroup("Joystick"), Tooltip("根据摇杆角度调整 yaw 值时的最大速度")]
	public float joystickYawSpeedMax = 60f;
	[BoxGroup("Joystick"), Tooltip("根据摇杆方向调整 pitch 值时的范围")]
	public Vector2 joystickPitchRange = new Vector2(10, 80);
	[BoxGroup("Joystick"), Tooltip("根据摇杆方向调整 pitch 值时的速度")]
	public float joystickPitchSpeed = 30f;

	[BoxGroup("Collision"), Tooltip("球形射线的半径")]
	public float collisionRadius = 0.1f;
	[BoxGroup("Collision"), Tooltip("阻挡层")]
	public LayerMask obstacleLayer;
	[BoxGroup("Collision"), Tooltip("预测时，yaw 的探测极限变化值")]
	public float yawOffsetLimit = 90;
	[BoxGroup("Collision"), Tooltip("预测时，yaw 的探测变化量")]
	public float yawOffsetDelta = 1;
	[BoxGroup("Collision"), Tooltip("预测时，pitch 的探测极限绝对值")]
	public Vector2 pitchOffsetLimit = new Vector2(-10, 80);
	[BoxGroup("Collision"), Tooltip("预测时， viewport_z 的减量，适应相机位置 newarClipPlane 的差")]
	public float viewport_z_decrease = 0.4f;
	[BoxGroup("Collision"), Tooltip("预测时，角度变化量的参考倍率（乘法）")]
	public float chooseAngleTestScale = 1;
	[BoxGroup("Collision"), Tooltip("决策时， viewport_z 变化量的参考倍率（mulitply)")]
	public float viewportZMin = 1;
	[BoxGroup("Collision"), Tooltip("决策时，viewport_z 的最大变化量")]
	public float viewportZChangeMax = 1;

	[BoxGroup("Status"), ShowNonSerializedField]
	private Vector3 followCurrentVelocity;
	[BoxGroup("Status"), ShowNonSerializedField]
	private float yawCurrentVelocity;
	[BoxGroup("Status"), ShowNonSerializedField]
	private float pitchCurrentVelocity;

	private Transform cameraTransform;
	private CameraForPrediction prediction;

	public Vector3 shouldFvp
	{
		get
		{
			var fvp = followViewport;
			fvp.z *= zoom;
			return fvp;
		}
	}

	public Vector3 finalFvp
	{
		get
		{
			var fvp = followViewport;
			var forceZ = prediction.viewportZ;
			fvp.z = forceZ.equalsZero() ? fvp.z * zoom : forceZ;
			return fvp;
		}
	}

	public Vector3 position
	{
		get => cameraTransform.position;
		private set => cameraTransform.position = value;
	}

	public Vector3 angles
	{
		get => cameraTransform.eulerAngles;
		private set => cameraTransform.eulerAngles = value;
	}

	public Vector3 followPosition
	{
		get => player.position + followTargetOffset;
	}

	public void RotateByInput(Vector2 joystickDir)
	{
		prediction.setJoystickDir(joystickDir);
	}

	public void RotateByInput()
	{
		prediction.setJoystickDir(Vector2.zero);
	}

	private void tickRotate(float time, float deltaTime)
	{
		var currentAngles = angles;
		var targetAngles = prediction.angles;

		currentAngles.y = Mathf.SmoothDampAngle(currentAngles.y, targetAngles.y, ref yawCurrentVelocity, yawSmoothTime);
		currentAngles.x = Mathf.SmoothDampAngle(currentAngles.x, targetAngles.x, ref pitchCurrentVelocity, pitchSmoothTime);

		angles = currentAngles;
	}

	private void tickFollow(float time, float deltaTime)
	{
		if (player == null) return;

		var currentP = position;
		var targetP = currentP + (followPosition - camera.ViewportToWorldPoint(finalFvp));
		if (!currentP.Equals(targetP))
		{
			position = Vector3.SmoothDamp(currentP, targetP, ref followCurrentVelocity, followSmoothTime, float.MaxValue, deltaTime);
		}
	}

	#region MonoBehaviour

	private void Awake()
	{
		cameraTransform = camera.transform;
		predictionCamera.transform.eulerAngles = cameraTransform.eulerAngles;
		prediction = new CameraForPrediction(predictionCamera);
	}

	private void Update()
	{
		/// ????? wtf 还有这种东西
		var time = Time.time;
		var deltaTime = Time.deltaTime;

		prediction.tick(this, time, deltaTime);
		tickRotate(time, deltaTime);
	}

	private void LateUpdate()
	{
		var time = Time.time;
		var deltaTime = Time.deltaTime;

		tickFollow(time, deltaTime);
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying) return;

		prediction.drawGizmos(this);
	}

	#endregion
}

