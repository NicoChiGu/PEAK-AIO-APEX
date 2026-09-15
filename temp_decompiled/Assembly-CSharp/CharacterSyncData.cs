using System;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000145 RID: 325
public struct CharacterSyncData : IBinarySerializable
{
	// Token: 0x06000A84 RID: 2692 RVA: 0x00037E18 File Offset: 0x00036018
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteFloat3(this.hipLocation);
		serializer.WriteHalf2(new half2((half)this.lookValues.x, (half)this.lookValues.y));
		CharacterSyncData.Flags flags = CharacterSyncData.Flags.NONE;
		if (this.sprintIsPressed)
		{
			flags |= CharacterSyncData.Flags.SPRINT;
		}
		if (this.movementInput.x > 0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_RIGHT;
		}
		if (this.movementInput.x < -0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_LEFT;
		}
		if (this.movementInput.y > 0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_FORWARD;
		}
		if (this.movementInput.y < -0.01f)
		{
			flags |= CharacterSyncData.Flags.WALK_BACKWARD;
		}
		if (this.ropeClimbing)
		{
			flags |= CharacterSyncData.Flags.ROPE_CLIMBING;
		}
		if (this.isClimbing)
		{
			flags |= CharacterSyncData.Flags.CLIMBING;
		}
		if (this.isGrounded)
		{
			flags |= CharacterSyncData.Flags.IS_GROUNDED;
		}
		serializer.WriteByte((byte)flags);
		serializer.WriteHalf((half)this.sinceGrounded);
		if (this.ropeClimbing)
		{
			serializer.WriteHalf((half)this.ropePercent);
		}
		serializer.WriteHalf3((half3)this.averageVelocity);
		if (this.isClimbing)
		{
			serializer.WriteHalf3((half3)this.climbPos);
		}
		serializer.WriteHalf((half)this.stammina);
		serializer.WriteHalf((half)this.extraStammina);
		serializer.WriteHalf((half)this.spectateZoom);
		serializer.WriteHalf((half)this.isChargingThrow);
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x00037F8C File Offset: 0x0003618C
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.hipLocation = deserializer.ReadFloat3();
		this.lookValues = new Vector2(deserializer.ReadHalf(), deserializer.ReadHalf());
		CharacterSyncData.Flags flags = (CharacterSyncData.Flags)deserializer.ReadByte();
		Vector2 zero = Vector2.zero;
		this.sprintIsPressed = EnumFlagExtensions.HasFlagUnsafe<CharacterSyncData.Flags>(flags, CharacterSyncData.Flags.SPRINT);
		if (EnumFlagExtensions.HasFlagUnsafe<CharacterSyncData.Flags>(flags, CharacterSyncData.Flags.WALK_RIGHT))
		{
			zero.x += 1f;
		}
		if (EnumFlagExtensions.HasFlagUnsafe<CharacterSyncData.Flags>(flags, CharacterSyncData.Flags.WALK_LEFT))
		{
			zero.x -= 1f;
		}
		if (EnumFlagExtensions.HasFlagUnsafe<CharacterSyncData.Flags>(flags, CharacterSyncData.Flags.WALK_FORWARD))
		{
			zero.y += 1f;
		}
		if (EnumFlagExtensions.HasFlagUnsafe<CharacterSyncData.Flags>(flags, CharacterSyncData.Flags.WALK_BACKWARD))
		{
			zero.y -= 1f;
		}
		this.movementInput = zero;
		this.sinceGrounded = deserializer.ReadHalf();
		this.ropeClimbing = EnumFlagExtensions.HasFlagUnsafe<CharacterSyncData.Flags>(flags, CharacterSyncData.Flags.ROPE_CLIMBING);
		if (this.ropeClimbing)
		{
			this.ropePercent = deserializer.ReadHalf();
		}
		this.averageVelocity = deserializer.ReadHalf3();
		this.isClimbing = EnumFlagExtensions.HasFlagUnsafe<CharacterSyncData.Flags>(flags, CharacterSyncData.Flags.CLIMBING);
		this.isGrounded = EnumFlagExtensions.HasFlagUnsafe<CharacterSyncData.Flags>(flags, CharacterSyncData.Flags.IS_GROUNDED);
		if (this.isClimbing)
		{
			this.climbPos = deserializer.ReadHalf3();
		}
		this.stammina = deserializer.ReadHalf();
		this.extraStammina = deserializer.ReadHalf();
		this.spectateZoom = deserializer.ReadHalf();
		this.isChargingThrow = deserializer.ReadHalf();
	}

	// Token: 0x040009DB RID: 2523
	public float3 hipLocation;

	// Token: 0x040009DC RID: 2524
	public float2 lookValues;

	// Token: 0x040009DD RID: 2525
	public Vector2 movementInput;

	// Token: 0x040009DE RID: 2526
	public bool sprintIsPressed;

	// Token: 0x040009DF RID: 2527
	public float sinceGrounded;

	// Token: 0x040009E0 RID: 2528
	public bool ropeClimbing;

	// Token: 0x040009E1 RID: 2529
	public float ropePercent;

	// Token: 0x040009E2 RID: 2530
	public float3 averageVelocity;

	// Token: 0x040009E3 RID: 2531
	public bool isClimbing;

	// Token: 0x040009E4 RID: 2532
	public bool isGrounded;

	// Token: 0x040009E5 RID: 2533
	public float3 climbPos;

	// Token: 0x040009E6 RID: 2534
	public float stammina;

	// Token: 0x040009E7 RID: 2535
	public float extraStammina;

	// Token: 0x040009E8 RID: 2536
	public float spectateZoom;

	// Token: 0x040009E9 RID: 2537
	public float isChargingThrow;

	// Token: 0x0200046F RID: 1135
	[Flags]
	public enum Flags : byte
	{
		// Token: 0x04001923 RID: 6435
		NONE = 0,
		// Token: 0x04001924 RID: 6436
		SPRINT = 1,
		// Token: 0x04001925 RID: 6437
		ROPE_CLIMBING = 2,
		// Token: 0x04001926 RID: 6438
		WALK_RIGHT = 4,
		// Token: 0x04001927 RID: 6439
		WALK_LEFT = 8,
		// Token: 0x04001928 RID: 6440
		WALK_FORWARD = 16,
		// Token: 0x04001929 RID: 6441
		WALK_BACKWARD = 32,
		// Token: 0x0400192A RID: 6442
		CLIMBING = 64,
		// Token: 0x0400192B RID: 6443
		IS_GROUNDED = 128
	}
}
