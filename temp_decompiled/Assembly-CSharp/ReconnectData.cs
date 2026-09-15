using System;
using System.Linq;
using Peak.Dev;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x0200014C RID: 332
public struct ReconnectData : IPrettyPrintable
{
	// Token: 0x06000AB8 RID: 2744 RVA: 0x0003962C File Offset: 0x0003782C
	public string ToPrettyString()
	{
		return Pretty.Print(this.currentStatuses) + string.Format("\nPosition: {0}, Dead: {1}, FullyPassedOut: {2}, DeathTimer: {3}", new object[] { this.position, this.dead, this.fullyPassedOut, this.deathTimer }) + string.Format("\nLastRevived: {0}, LastSegment: {1}", this.lastRevivedSegment, this.mapSegment);
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x000396B4 File Offset: 0x000378B4
	public static ReconnectData CreateFromCharacter(Character character)
	{
		ReconnectData reconnectData = default(ReconnectData);
		reconnectData.isValid = true;
		reconnectData.position = character.VirtualCenter;
		reconnectData.dead = character.data.dead;
		reconnectData.fullyPassedOut = character.data.fullyPassedOut;
		reconnectData.deathTimer = character.data.deathTimer;
		reconnectData.currentStatuses = character.refs.afflictions.currentStatuses;
		reconnectData.inventorySyncData = new InventorySyncData(character.player.itemSlots, character.player.backpackSlot, character.player.tempFullSlot);
		reconnectData.mapSegment = Singleton<MapHandler>.Instance.GetCurrentSegment();
		reconnectData.lastRevivedSegment = character.data.lastRevivedSegment;
		if (Time.time - character.timeLastWarped < 1f || Vector3.Magnitude(character.data.avarageVelocity) > 100f || Vector3.Magnitude(character.data.avarageLastFrameVelocity) > 100f)
		{
			reconnectData.isValid = false;
		}
		else if (reconnectData.position.z < -2000f || reconnectData.position.y > 4000f)
		{
			Debug.LogWarning("Bad position data tried to sneak into our reconnect record again!");
			reconnectData.isValid = false;
		}
		return reconnectData;
	}

	// Token: 0x170000BF RID: 191
	// (get) Token: 0x06000ABA RID: 2746 RVA: 0x00039800 File Offset: 0x00037A00
	public static ReconnectData Invalid
	{
		get
		{
			return new ReconnectData
			{
				isValid = false,
				inventorySyncData = default(InventorySyncData)
			};
		}
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x00039830 File Offset: 0x00037A30
	public byte[] Serialize()
	{
		BinarySerializer binarySerializer = new BinarySerializer(100, 2);
		binarySerializer.WriteBool(this.isValid);
		if (this.isValid)
		{
			binarySerializer.WriteFloat3(this.position);
			binarySerializer.WriteBool(this.dead);
			binarySerializer.WriteBool(this.fullyPassedOut);
			binarySerializer.WriteFloat(this.deathTimer);
			new StatusSyncData
			{
				statusList = Enumerable.ToList<float>(this.currentStatuses)
			}.Serialize(binarySerializer);
			this.inventorySyncData.Serialize(binarySerializer);
			binarySerializer.WriteByte((byte)this.mapSegment);
			binarySerializer.WriteByte((byte)this.lastRevivedSegment);
		}
		byte[] array = NativeArrayExtensions.ToByteArray(binarySerializer.buffer);
		binarySerializer.Dispose();
		return array;
	}

	// Token: 0x06000ABC RID: 2748 RVA: 0x000398E8 File Offset: 0x00037AE8
	public static ReconnectData Deserialize(byte[] data)
	{
		ReconnectData reconnectData = default(ReconnectData);
		BinaryDeserializer binaryDeserializer = new BinaryDeserializer(data, 2);
		reconnectData.isValid = binaryDeserializer.ReadBool();
		if (reconnectData.isValid)
		{
			reconnectData.position = binaryDeserializer.ReadFloat3();
			reconnectData.dead = binaryDeserializer.ReadBool();
			reconnectData.fullyPassedOut = binaryDeserializer.ReadBool();
			reconnectData.deathTimer = binaryDeserializer.ReadFloat();
			reconnectData.currentStatuses = IBinarySerializable.Deserialize<StatusSyncData>(binaryDeserializer).statusList.ToArray();
			reconnectData.inventorySyncData = IBinarySerializable.Deserialize<InventorySyncData>(binaryDeserializer);
			reconnectData.mapSegment = (Segment)binaryDeserializer.ReadByte();
			reconnectData.lastRevivedSegment = (Segment)binaryDeserializer.ReadByte();
		}
		binaryDeserializer.Dispose();
		return reconnectData;
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x00039998 File Offset: 0x00037B98
	public override string ToString()
	{
		string newLine = Environment.NewLine;
		return string.Format("IsValid: {0}{1}Position: {2}{3}Dead: {4}{5}FullyPassedOut: {6}{7}DeathTimer: {8}", new object[] { this.isValid, newLine, this.position, newLine, this.dead, newLine, this.fullyPassedOut, newLine, this.deathTimer });
	}

	// Token: 0x040009FF RID: 2559
	public bool isValid;

	// Token: 0x04000A00 RID: 2560
	public Vector3 position;

	// Token: 0x04000A01 RID: 2561
	public bool dead;

	// Token: 0x04000A02 RID: 2562
	public bool fullyPassedOut;

	// Token: 0x04000A03 RID: 2563
	public float deathTimer;

	// Token: 0x04000A04 RID: 2564
	public Segment mapSegment;

	// Token: 0x04000A05 RID: 2565
	public Segment lastRevivedSegment;

	// Token: 0x04000A06 RID: 2566
	public float[] currentStatuses;

	// Token: 0x04000A07 RID: 2567
	public InventorySyncData inventorySyncData;

	// Token: 0x04000A08 RID: 2568
	private const float maxAcceptedVelocity = 100f;
}
