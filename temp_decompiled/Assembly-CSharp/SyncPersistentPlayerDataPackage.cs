using System;
using ExitGames.Client.Photon;
using Zorro.Core.Serizalization;
using Zorro.PhotonUtility;

// Token: 0x02000080 RID: 128
public class SyncPersistentPlayerDataPackage : CustomCommandPackage<CustomCommandType>
{
	// Token: 0x17000065 RID: 101
	// (get) Token: 0x0600055E RID: 1374 RVA: 0x0001F614 File Offset: 0x0001D814
	// (set) Token: 0x0600055F RID: 1375 RVA: 0x0001F61C File Offset: 0x0001D81C
	public PersistentPlayerData Data { get; set; }

	// Token: 0x06000560 RID: 1376 RVA: 0x0001F625 File Offset: 0x0001D825
	protected override void SerializeData(BinarySerializer binarySerializer)
	{
		binarySerializer.WriteInt(this.ActorNumber);
		this.Data.Serialize(binarySerializer);
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0001F63F File Offset: 0x0001D83F
	public override void DeserializeData(BinaryDeserializer binaryDeserializer)
	{
		this.ActorNumber = binaryDeserializer.ReadInt();
		this.Data = IBinarySerializable.DeserializeClass<PersistentPlayerData>(binaryDeserializer);
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x0001F659 File Offset: 0x0001D859
	public override CustomCommandType GetCommandType()
	{
		return CustomCommandType.SyncPersistentPlayerData;
	}

	// Token: 0x06000563 RID: 1379 RVA: 0x0001F65C File Offset: 0x0001D85C
	public override SendOptions GetSendOptions()
	{
		return SendOptions.SendReliable;
	}

	// Token: 0x040005A7 RID: 1447
	public int ActorNumber;
}
