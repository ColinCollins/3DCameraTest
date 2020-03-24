using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class CharacterControllerTest : MonoBehaviour
{
	public new Camera camera;
	// 预测，预报
	public Camera predictionCamera;

	[BoxGroup("Follow")]
	public PlayerController player;
	[BoxGroup("Follow")]
	public Vector3 followOffset;
	[BoxGroup("Follow")]
	public Vector3 followViewport = new Vector3(0.5f, 0.5f, 5.0f);

	[BoxGroup("Follow")]
	public float followSmoothTime = 0.3f;
	[BoxGroup("Follow")]
	public float zoom = 1f;

	[BoxGroup("Rotation"), MinMaxSlider(0.01f, 1.0f)]
	public float yawSmoothTime = 0.3f;
	[BoxGroup("Rotation"), MinMaxSlider(0.01f, 1.0f)]
	public float pitchSmoothTime = 0.3f;
	[BoxGroup("Joystick"), MinMaxSlider(0, 90), Tooltip("摇杆相对于垂直线的角度大于此值时，相机会左右调整 yaw 值，调整速度和摇杆角度相关")]
	public float joystickHighPass = 1.0f;
	[BoxGroup("Joystick"), Tooltip("根据摇杆角度调整 yaw 值时的最大速度")]
	public float joystickYawSpeedMax = 60f;
	[BoxGroup("Joystick"), Tooltip("根据摇杆角度调整 pitch 值时的范围")]
	public Vector2 joystickPitchRange = new Vector2(10, 80);
	[BoxGroup("Joystick"), Tooltip("根据摇杆方向调整 pitch 值时的速度")]
	public float joyStickPitchSpeed = 30f;

	[BoxGroup("Collision"), Tooltip("球型射线的半径")]
	public float CollisionRadius = 0.1f;
	[BoxGroup("Collision"), Tooltip("阻挡层")]
	public LayerMask obstacleLayer;
	[BoxGroup("Collision"), Tooltip("预测时，yaw 的探测极限变化值")]
	public float yawOffsetLimit = 90;
	[BoxGroup("Collision"), Tooltip("预测时， yaw 的探测变化量")]
	public float yawOffsetDelta = 1;
	[BoxGroup("Collision"), Tooltip("预测时，pitch 的探测极限绝对值")]
	public Vector2 pitchOffsetLimit = new Vector2(-10, 80);
	[BoxGroup("Collision"), Tooltip("预测时，yaw 的探测变化量")]
	public float pitchOffsetDelta = 1;
	[BoxGroup("Collision"), Tooltip("预测时，viewportZ 的减量，适应相机位置与 nearClipPlane 的差")]
	public float viewportZDecrease = 0.4f;
	[BoxGroup("Collision"), Tooltip("决策时，角度变化量的参考倍率（乘法）")]
	public float chooseAngleTestScale = 1;
	[BoxGroup("Collision"), Tooltip("决策时， viewportZ 变化量的参考倍率（乘法）")]
	public float chooseViewportZTestScale = 1;
	[BoxGroup("Collision"), Tooltip("决策时，viewportZ 的 min value")]
	public float viewportZMin = 1;
	[BoxGroup("Collision"), Tooltip("决策时， viewportZ 的 max change value")]
	public float viewportZChangedMax = 1;


	[BoxGroup("Status"), ShowNonSerializedField]
	private Vector3 followCurrentVelocity;
	[BoxGroup("Status"), ShowNonSerializedField]
	private float yawCurrentVelocity;
	[BoxGroup("Status"), ShowNonSerializedField]
	private float pitchCurrentVelocity;

	private Transform cameraTransform;
	private CameraForPrediction prediction;

	// Target value for new fvp
	public Vector3 shouldFvp {
		get {
			var fvp = followViewport;
			fvp.z *= zoom;
			return fvp;
		}
	}

	public Vector3 finalFvp {
		get {
			var fvp = followViewport;
			var forceZ = prediction.viewport.z;
			fvp.z = forceZ.equalsZero() ? fvp.z * zoom : forceZ;
			return fvp;
		}
	}

	public Vector3 position {
		get => cameraTransform.position;
	}
}
