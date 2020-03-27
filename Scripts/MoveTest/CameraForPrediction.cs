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

	public float viewportZ { get; private set; }

	public CameraForPrediction(Camera c)
	{
		camera = c;
		transform = camera.transform;
	}

	public void DrawGizmos(CameraController config)
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

	public void SetJoystickDir(Vector2 dir)
	{
		joystickDir = dir;
	}

	public void Tick(CameraController config, float time, float deltaTime)
	{
		tickCamera(config, time, deltaTime);

		var canSee = tickJoystick(config, time, deltaTime);
		if (!canSee)
		{
			tickSee(config, time, deltaTime);
		}

		tickFollow(config, time, deltaTime);
	}

	// 保持相机参数的一致性
	private void tickCamera(CameraController config, float time, float deltaTime)
	{
		camera.fieldOfView = config.camera.fieldOfView;
		camera.nearClipPlane = config.camera.nearClipPlane;
		camera.farClipPlane = config.camera.farClipPlane;
		camera.aspect = config.camera.aspect;
	}

	
	// 处理摇杆对 yaw 和 pitch 的影响
	private bool tickJoystick(CameraController config, float time, float deltaTime)
	{
		if (joystickDir.equalsZero()) return false;

		var yawSignOffset = 0f;
		var pitchSignOffset = 0f;
		var targetAngles = angles;

		if (config.joystickHighPass < 90)
		{
			float joystickAngleVertial = Vector2.Angle(Vector2.up, joystickDir);

			if (joystickAngleVertial > 90)
			{
				joystickAngleVertial = 180 - joystickAngleVertial;
			}

			if (joystickAngleVertial > config.joystickHighPass)
			{
				var speedScale = (joystickAngleVertial - config.joystickHighPass) / (90 - config.joystickHighPass);
				var deltaAngle = config.joystickYawSpeedMax * speedScale * deltaTime;
				if (joystickDir.x > 0)
				{
					yawSignOffset = deltaAngle;
				}
				else
				{
					yawSignOffset = -deltaAngle;
				}
			}
		}


		// 处理摇杆上下对 pitch 的影响
		if (!joystickDir.y.equalsZero())
		{
			var deltaAngle = config.joystickPitchSpeed * deltaTime;

			if (joystickDir.y > 0)
			{
				// 为了避免摄像机倒转，pitch 值得调整范围有最小值
				if (targetAngles.x > config.joystickPitchRange.x)
				{
					if ((targetAngles.x - deltaAngle) < config.joystickPitchRange.x)
					{
						deltaAngle = targetAngles.x - config.joystickPitchRange.x;
					}

					pitchSignOffset = -deltaAngle;
				}
				else
				{
					pitchSignOffset = config.joystickPitchRange.x - targetAngles.x;
				}
			}
			else
			{
				// 为了避免相机翻转 pitch 值有最大取值范围
				if (targetAngles.x < config.joystickPitchRange.y)
				{
					if ((targetAngles.x + deltaAngle) > config.joystickPitchRange.y)
					{
						deltaAngle = config.joystickPitchRange.y - targetAngles.x;
					}
					pitchSignOffset = deltaAngle;
				}
				else
				{
					pitchSignOffset = config.joystickPitchRange.y - targetAngles.x;
				}
			}
		}

		// 根据 yaw 和 pitch 调整集中情况，以及每种情况对目标的可见性，来决定最后的 yaw 和 pitch 的调整是否应被应用
		var cameraRightVec = transform.right;
		if (!yawSignOffset.equalsZero() && !pitchSignOffset.equalsZero())
		{
			if (rotateCanSee(
				config,
				(target2SourceVec) =>
				{
					var tempVec = target2SourceVec.rotateByAngleAxis(pitchSignOffset, cameraRightVec);
					return tempVec.rotateByAngles(Vector3.up * yawSignOffset);
				},
				out float collisionDistance
				))
			{
				// yaw 和 pitch 同时生效后可见

				targetAngles.y += yawSignOffset;
				targetAngles.x += pitchSignOffset;
				angles = targetAngles;

				viewportZ = collisionDistance;

				return true;
			}
		}
		else if (!pitchSignOffset.equalsZero() && rotateCanSee(
			config,
			(target2SourceVec) => target2SourceVec.rotateByAngleAxis(pitchSignOffset, cameraRightVec),
			out float collisionDistance
			))
		{
			// 仅对 pitch 生效可见
			targetAngles.x += pitchSignOffset;
			angles = targetAngles;
			viewportZ = collisionDistance;

			return true;
		}
		else if (!yawSignOffset.equalsZero() && rotateCanSee(
			config,
			(target2SourceVec) => target2SourceVec.rotateByAngles(Vector3.up * yawSignOffset),
			out collisionDistance
			))
		{
			targetAngles.y += yawSignOffset;
			angles = targetAngles;

			viewportZ = collisionDistance;
			return true;
		}

		return false;
	}


	private void tickSee(CameraController config, float time, float deltaTime)
	{
		var sourceP = shouldPosition;
		var targetP = config.followPosition;

		var seeRet = canSee(config, sourceP, targetP, out Vector3 collisionP, out float collisionDistance);

		if (seeRet == SeeResult.CAN_SEE)
		{
			viewportZ = 0;
			return;
		}
		else if (seeRet == SeeResult.CAN_SEE_SINGLE)
		{
			viewportZ = collisionDistance;
			return;
		}

		seeRet = canSee(config, position, targetP, out collisionP, out collisionDistance);

		// 确定当前位置的可见性
		if (seeRet == SeeResult.CAN_SEE)
		{
			return;
		}

		// 尝试旋转到可视位置
		var target2SourceVec = sourceP - targetP;
		var currentDistance = Vector3.Distance(position, targetP);

		var positiveYawOffset = findOutCanSeeAngleOffset(
			config,
			config.yawOffsetDelta,
			config.yawOffsetLimit,
			(signedOffset) => target2SourceVec.rotateByAngles(Vector3.up * signedOffset),
			sourceP,
			targetP,
			false,
			out float positiveYawViewportZ);

		var positiveYawViewportZOffset = positiveYawViewportZ.equalsZero() ? 0 : Mathf.Abs(positiveYawViewportZ - currentDistance);

		var negativeYawOffset = findOutCanSeeAngleOffset(
			config,
			config.yawOffsetDelta,
			config.yawOffsetLimit,
			(signedOffset) => target2SourceVec.rotateByAngles(Vector3.up * signedOffset),
			sourceP,
			targetP,
			true,
			out float negativeYawViewportZ
			);

		var negativeYawViewportZOffset = negativeYawViewportZ.equalsZero() ? 0 : Mathf.Abs(negativeYawViewportZ - currentDistance);

		var currentAngles = angles;
		var currentPitch = currentAngles.x;

		if (currentPitch > 180)
		{
			currentPitch = currentPitch - 360;
		}
		else if (currentPitch < -180)
		{
			currentPitch = currentPitch + 360;
		}

		var cameraRightVec = transform.right;

		var positivePitchOffset = findOutCanSeeAngleOffset(
			config,
			config.pitchOffsetDelta,
			config.pitchOffsetLimit.y - currentPitch,
			(signedOffset) => target2SourceVec.rotateByAngleAxis(signedOffset, cameraRightVec),
			sourceP,
			targetP,
			false,
			out float positivePitchViewportZ
			);
		var positivePitchViewportZOffset = positivePitchViewportZ.equalsZero() ? 0 : Mathf.Abs(positivePitchViewportZ - currentDistance);

		var negativePitchOffset = findOutCanSeeAngleOffset(
			config,
			config.pitchOffsetDelta,
			currentPitch - config.pitchOffsetLimit.x,
			(signedOffset) => target2SourceVec.rotateByAngleAxis(signedOffset, cameraRightVec),
			sourceP,
			targetP,
			true,
			out float negativePitchViewportZ
			);

		var negativePitchViewportZOffset = negativePitchViewportZ.equalsZero() ? 0 : Mathf.Abs(negativePitchViewportZ - currentDistance);


		// 从四个不同的角度选择最合适的调整选项并应用
		var positiveYawChooseTest = calcChooseTest(config, positiveYawOffset, positiveYawViewportZOffset);
		var negativeYawChooseTest = calcChooseTest(config, negativeYawOffset, negativeYawViewportZOffset);
		var positivePitchChoosetTest = calcChooseTest(config, positivePitchOffset, positivePitchViewportZOffset);
		var negativePitchChooseTest = calcChooseTest(config, negativePitchOffset, negativePitchViewportZOffset);

		// 测试值越小越合适
		if (positiveYawChooseTest < negativeYawChooseTest && positiveYawChooseTest < positivePitchChoosetTest && positiveYawChooseTest < negativePitchChooseTest)
		{
			currentAngles.y += positiveYawOffset;
			viewportZ = positiveYawViewportZ;
			angles = currentAngles;
		}
		else if (negativeYawChooseTest < positivePitchChoosetTest && negativeYawChooseTest < negativePitchChooseTest)
		{
			currentAngles.y -= negativeYawOffset;
			viewportZ = negativeYawViewportZ;
			angles = currentAngles;
		}
		else if (positivePitchChoosetTest < negativePitchChooseTest)
		{
			currentAngles.x += positivePitchOffset;
			viewportZ = positivePitchViewportZ;
			angles = currentAngles;
		}
		else if (negativePitchChooseTest < 360)
		{
			currentAngles.x -= negativePitchChooseTest;
			viewportZ = negativePitchViewportZ;
			angles = currentAngles;
		}
		else {
			if (config.viewportZMin < collisionDistance)
			{
				viewportZ = collisionDistance;
			}
			else {
				Debug.Log("Can't See");
			}
		}
	}

	// 调整 fvp 调整相机位置
	private void tickFollow(CameraController config, float time, float deltaTime)
	{
		if (null == config.player) return;

		var targetP = config.followPosition;
		shouldPosition = position + (targetP - camera.ViewportToWorldPoint(config.shouldFvp));
		position += targetP - camera.ViewportToWorldPoint(config.finalFvp);
	}

	// 旋转后是否对目标可见
	private bool rotateCanSee(CameraController config, System.Func<Vector3, Vector3> rotateFunc, out float collisionDistance)
	{
		var targetP = config.followPosition;

		var sourceP = shouldPosition;
		var target2SourceVec = sourceP - targetP;

		sourceP = targetP + rotateFunc(target2SourceVec);

		var seeRet = canSee(config, sourceP, targetP, out Vector3 tmp, out collisionDistance);

		if (seeRet == SeeResult.CAN_NOT_SEE)
		{
			sourceP = position;
			target2SourceVec = sourceP - targetP;
			sourceP = targetP + rotateFunc(target2SourceVec);

			seeRet = canSee(config, sourceP, targetP, out Vector3 _, out collisionDistance);
		}

		return seeRet != SeeResult.CAN_NOT_SEE;
	}

	// 计算镜头和目标的相互可见性

	private static SeeResult canSee(CameraController config, Vector3 cameraP, Vector3 targetP, out Vector3 collisionP, out float collisionDistance)
	{
		var dir = targetP - cameraP;
		var dist = dir.magnitude;

		collisionDistance = 0;

		var see = !UtilPhysics.sphereCast(targetP, config.collisionRadius, (dir * -1f), config.obstacleLayer, out RaycastHit hit, out collisionP, dist);
		if (see) return SeeResult.CAN_SEE;

		collisionP = Vector3.MoveTowards(collisionP, targetP, config.viewportZDecrease);
		collisionDistance = Vector3.Distance(targetP, collisionP);

		var seeSingle = !UtilPhysics.sphereCast(cameraP, config.collisionRadius, dir, config.obstacleLayer, out hit, out Vector3 _, dist);
		if (seeSingle)
		{
			if (collisionDistance > config.viewportZMin && Mathf.Abs(dir.magnitude - collisionDistance) < config.viewportZChangeMax)
			{
				return SeeResult.CAN_SEE_SINGLE;
			}
		}

		return SeeResult.CAN_NOT_SEE;
	}

	// 计算选项的测试值（集合角度偏移值和 zoom 值，以及它们各自的设定权值）
	private static float calcChooseTest(CameraController config, float angleOffset, float viewportZOffset)
	{
		var chooseTest = float.MaxValue;
		if (angleOffset > 360)
		{
			chooseTest = angleOffset * config.chooseAngleTestScale + viewportZOffset * config.chooseAngleTestScale;
		}

		return chooseTest;
	}

	// 找到可以看见目标的偏移角度
	private static float findOutCanSeeAngleOffset(CameraController config,
	float offsetDelta,
	float offsetLimit,
	System.Func<float, Vector3> vecRotateFunc,
	Vector3 sourceP,
	Vector3 targetP,
	bool negativeRotate,
	out float collisionDistance
)
	{
		int offsetSign = negativeRotate ? -1 : 1;
		float offset = 0;
		collisionDistance = 0;
		SeeResult seeRet = SeeResult.CAN_NOT_SEE;

		Vector3 collisionP = sourceP;
		var currentDistance = Vector3.Distance(sourceP, targetP);
		for (; offset < offsetLimit && SeeResult.CAN_NOT_SEE == seeRet; offset += offsetDelta)
		{
			sourceP = targetP + vecRotateFunc(offset * offsetSign);
			seeRet = canSee(config, sourceP, targetP, out collisionP, out collisionDistance);
		}

		if (SeeResult.CAN_NOT_SEE == seeRet)
		{
			offset = float.MaxValue;
		}

		return offset;
	}

}
