using System;
using ExitGames.Client.Photon;
using Zorro.Core.Serizalization;
using Zorro.PhotonUtility;

// Token: 0x0200007E RID: 126
public class SyncLavaRisingPackage : CustomCommandPackage<CustomCommandType>
{
	// Token: 0x06000552 RID: 1362 RVA: 0x0001F4C1 File Offset: 0x0001D6C1
	public SyncLavaRisingPackage()
	{
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x0001F4C9 File Offset: 0x0001D6C9
	public SyncLavaRisingPackage(bool started, bool ended, float time, float timeWaited)
	{
		this.Started = started;
		this.Ended = ended;
		this.Time = time;
		this.TimeWaited = timeWaited;
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0001F4EE File Offset: 0x0001D6EE
	protected override void SerializeData(BinarySerializer binarySerializer)
	{
		binarySerializer.WriteBool(this.Started);
		binarySerializer.WriteBool(this.Ended);
		binarySerializer.WriteFloat(this.Time);
		binarySerializer.WriteFloat(this.TimeWaited);
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x0001F520 File Offset: 0x0001D720
	public override void DeserializeData(BinaryDeserializer binaryDeserializer)
	{
		this.Started = binaryDeserializer.ReadBool();
		this.Ended = binaryDeserializer.ReadBool();
		this.Time = binaryDeserializer.ReadFloat();
		this.TimeWaited = binaryDeserializer.ReadFloat();
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x0001F552 File Offset: 0x0001D752
	public override CustomCommandType GetCommandType()
	{
		return CustomCommandType.SyncLavaRising;
	}

	// Token: 0x06000557 RID: 1367 RVA: 0x0001F555 File Offset: 0x0001D755
	public override SendOptions GetSendOptions()
	{
		return SendOptions.SendReliable;
	}

	// Token: 0x040005A1 RID: 1441
	public bool Started;

	// Token: 0x040005A2 RID: 1442
	public bool Ended;

	// Token: 0x040005A3 RID: 1443
	public float Time;

	// Token: 0x040005A4 RID: 1444
	public float TimeWaited;
}
