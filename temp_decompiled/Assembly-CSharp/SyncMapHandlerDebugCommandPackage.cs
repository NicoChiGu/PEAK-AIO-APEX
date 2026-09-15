using System;
using ExitGames.Client.Photon;
using Zorro.Core.Serizalization;
using Zorro.PhotonUtility;

// Token: 0x0200007F RID: 127
public class SyncMapHandlerDebugCommandPackage : CustomCommandPackage<CustomCommandType>
{
	// Token: 0x06000558 RID: 1368 RVA: 0x0001F55C File Offset: 0x0001D75C
	public SyncMapHandlerDebugCommandPackage()
	{
	}

	// Token: 0x06000559 RID: 1369 RVA: 0x0001F564 File Offset: 0x0001D764
	public SyncMapHandlerDebugCommandPackage(Segment segment, int[] playersToTeleport)
	{
		this.Segment = segment;
		this.PlayerToTeleport = playersToTeleport;
	}

	// Token: 0x0600055A RID: 1370 RVA: 0x0001F57C File Offset: 0x0001D77C
	protected override void SerializeData(BinarySerializer binarySerializer)
	{
		binarySerializer.WriteByte((byte)this.Segment);
		binarySerializer.WriteByte((byte)this.PlayerToTeleport.Length);
		foreach (int num in this.PlayerToTeleport)
		{
			binarySerializer.WriteInt(num);
		}
	}

	// Token: 0x0600055B RID: 1371 RVA: 0x0001F5C4 File Offset: 0x0001D7C4
	public override void DeserializeData(BinaryDeserializer binaryDeserializer)
	{
		this.Segment = (Segment)binaryDeserializer.ReadByte();
		byte b = binaryDeserializer.ReadByte();
		this.PlayerToTeleport = new int[(int)b];
		for (int i = 0; i < (int)b; i++)
		{
			this.PlayerToTeleport[i] = binaryDeserializer.ReadInt();
		}
	}

	// Token: 0x0600055C RID: 1372 RVA: 0x0001F60A File Offset: 0x0001D80A
	public override CustomCommandType GetCommandType()
	{
		return CustomCommandType.SyncMapHandlerDebugCommand;
	}

	// Token: 0x0600055D RID: 1373 RVA: 0x0001F60D File Offset: 0x0001D80D
	public override SendOptions GetSendOptions()
	{
		return SendOptions.SendReliable;
	}

	// Token: 0x040005A5 RID: 1445
	public int[] PlayerToTeleport;

	// Token: 0x040005A6 RID: 1446
	public Segment Segment;
}
