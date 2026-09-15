using System;
using UnityEngine;

// Token: 0x02000030 RID: 48
public static class ConfigurableJointExtensions
{
	// Token: 0x06000367 RID: 871 RVA: 0x000174AA File Offset: 0x000156AA
	public static void SetTargetRotationLocal(this ConfigurableJoint joint, Quaternion targetLocalRotation, Quaternion startLocalRotation)
	{
		if (joint.configuredInWorldSpace)
		{
			Debug.LogError("SetTargetRotationLocal should not be used with joints that are configured in world space. For world space joints, use SetTargetRotation.", joint);
		}
		ConfigurableJointExtensions.SetTargetRotationInternal(joint, targetLocalRotation, startLocalRotation, 1);
	}

	// Token: 0x06000368 RID: 872 RVA: 0x000174C8 File Offset: 0x000156C8
	public static void SetTargetRotation(this ConfigurableJoint joint, Quaternion targetWorldRotation, Quaternion startWorldRotation)
	{
		if (!joint.configuredInWorldSpace)
		{
			Debug.LogError("SetTargetRotation must be used with joints that are configured in world space. For local space joints, use SetTargetRotationLocal.", joint);
		}
		ConfigurableJointExtensions.SetTargetRotationInternal(joint, targetWorldRotation, startWorldRotation, 0);
	}

	// Token: 0x06000369 RID: 873 RVA: 0x000174E8 File Offset: 0x000156E8
	private static void SetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation, Quaternion startRotation, Space space)
	{
		Vector3 axis = joint.axis;
		Vector3 normalized = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
		Vector3 normalized2 = Vector3.Cross(normalized, axis).normalized;
		Quaternion quaternion = Quaternion.LookRotation(normalized, normalized2);
		Quaternion quaternion2 = Quaternion.Inverse(quaternion);
		if (space == null)
		{
			quaternion2 *= startRotation * Quaternion.Inverse(targetRotation);
		}
		else
		{
			quaternion2 *= Quaternion.Inverse(targetRotation) * startRotation;
		}
		quaternion2 *= quaternion;
		joint.targetRotation = quaternion2;
	}
}
