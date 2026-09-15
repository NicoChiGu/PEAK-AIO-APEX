using System;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

// Token: 0x02000146 RID: 326
[DefaultExecutionOrder(-100)]
public class CharacterSyncer : PhotonBinaryStreamSerializer<CharacterSyncData>
{
	// Token: 0x06000A86 RID: 2694 RVA: 0x00038116 File Offset: 0x00036316
	protected override void Awake()
	{
		base.Awake();
		this.m_character = base.GetComponent<Character>();
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x0003812C File Offset: 0x0003632C
	public override CharacterSyncData GetDataToWrite()
	{
		return new CharacterSyncData
		{
			hipLocation = this.m_character.GetBodypart(BodypartType.Hip).Rig.position,
			lookValues = this.m_character.data.lookValues,
			movementInput = this.m_character.input.movementInput,
			sprintIsPressed = this.m_character.input.sprintIsPressed,
			sinceGrounded = this.m_character.data.sinceGrounded,
			ropePercent = this.m_character.data.ropePercent,
			ropeClimbing = this.m_character.data.isRopeClimbing,
			averageVelocity = this.GetAverageVelocity(),
			isClimbing = this.m_character.data.isClimbing,
			isGrounded = this.m_character.data.isGrounded,
			climbPos = this.m_character.data.climbPos,
			stammina = this.m_character.data.currentStamina,
			extraStammina = this.m_character.data.extraStamina,
			spectateZoom = this.m_character.data.spectateZoom,
			isChargingThrow = (float)(this.m_character.refs.items.isChargingThrow ? 1 : 0)
		};
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x000382B8 File Offset: 0x000364B8
	public Vector3 GetAverageVelocity()
	{
		if (this.m_character.warping)
		{
			return Vector3.zero;
		}
		Vector3 vector = Vector3.zero;
		foreach (Bodypart bodypart in this.m_character.refs.ragdoll.partList)
		{
			vector += bodypart.Rig.linearVelocity;
		}
		vector /= (float)this.m_character.refs.ragdoll.partList.Count;
		return vector;
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x00038364 File Offset: 0x00036564
	public override void OnDataReceived(CharacterSyncData data)
	{
		this.sinceLastPackageUpdate = 0f;
		base.OnDataReceived(data);
		this.lastPosition = Optionable<float3>.Some(this.m_character.GetBodypart(BodypartType.Hip).Rig.position);
		this.lastLook = Optionable<float2>.Some(this.m_character.data.lookValues);
		Vector3 averageVelocity = this.GetAverageVelocity();
		Vector3 vector = data.averageVelocity - averageVelocity;
		if (Vector3.Magnitude(vector) > 20f)
		{
			vector = Vector3.ClampMagnitude(vector, 20f);
		}
		List<Bodypart> partList = this.m_character.refs.ragdoll.partList;
		bool flag = Vector3.Magnitude(this.m_character.data.avarageVelocity) > 100f;
		foreach (Bodypart bodypart in partList)
		{
			if (!bodypart.Rig.isKinematic && !this.m_character.warping && !flag)
			{
				bodypart.Rig.linearVelocity += vector;
			}
		}
		this.m_character.input.movementInput = data.movementInput;
		this.m_character.input.sprintIsPressed = data.sprintIsPressed;
		if (Mathf.Abs(this.m_character.data.sinceGrounded - data.sinceGrounded) > 2f)
		{
			this.m_character.data.sinceGrounded = data.sinceGrounded;
		}
		if (data.ropeClimbing)
		{
			this.m_character.data.ropePercent = data.ropePercent;
		}
		if (data.isClimbing)
		{
			this.m_character.data.climbPos = data.climbPos;
		}
		this.m_character.data.currentStamina = data.stammina;
		this.m_character.data.extraStamina = data.extraStammina;
		this.m_character.data.spectateZoom = data.spectateZoom;
		this.m_character.refs.items.isChargingThrow = data.isChargingThrow > 0.5f;
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x000385AC File Offset: 0x000367AC
	private void Update()
	{
		if (this.photonView.IsMine)
		{
			return;
		}
		if (this.RemoteValue.IsNone)
		{
			return;
		}
		if (this.lastLook.IsNone)
		{
			return;
		}
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackageUpdate += Time.deltaTime;
		float num2 = (float)((double)this.sinceLastPackageUpdate / num);
		Vector2 vector = this.RemoteValue.Value.lookValues;
		Vector2 vector2 = Vector2.Lerp(this.lastLook.Value, vector, num2);
		this.m_character.data.lookValues = vector2;
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x00038650 File Offset: 0x00036850
	private void FixedUpdate()
	{
		if (this.photonView.IsMine)
		{
			return;
		}
		if (this.RemoteValue.IsNone)
		{
			return;
		}
		if (this.lastPosition.IsNone)
		{
			return;
		}
		if (this.m_character.data.carrier)
		{
			return;
		}
		if (!this.m_character.warping)
		{
			this.InterpolateRigPositions();
		}
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x000386B4 File Offset: 0x000368B4
	private void InterpolateRigPositions()
	{
		Vector3 vector = this.RemoteValue.Value.hipLocation;
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackage += Time.fixedDeltaTime * 0.6f;
		float num2 = (float)((double)this.sinceLastPackage / num);
		Vector3 vector2 = Vector3.Lerp(this.lastPosition.Value, vector, num2);
		Vector3 position = this.m_character.GetBodypart(BodypartType.Hip).Rig.position;
		Vector3 vector3 = vector2 - position;
		if (vector3.magnitude > 10f)
		{
			this.m_character.refs.ragdoll.MoveAllRigsInDirection(vector3);
			return;
		}
		vector3.y *= 0f;
		float num3 = vector2.y - position.y;
		float num4 = Mathf.Abs(num3);
		float num5 = Mathf.InverseLerp(0.3f, 0.6f, num4) * Mathf.Sign(num3);
		vector3 += Vector3.up * num5;
		this.m_character.refs.ragdoll.MoveAllRigsInDirection(vector3 * 0.1f);
	}

	// Token: 0x040009EA RID: 2538
	private Character m_character;

	// Token: 0x040009EB RID: 2539
	private Optionable<float3> lastPosition;

	// Token: 0x040009EC RID: 2540
	private Optionable<float2> lastLook;

	// Token: 0x040009ED RID: 2541
	private float sinceLastPackageUpdate;
}
