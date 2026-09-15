using System;
using System.Runtime.CompilerServices;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000159 RID: 345
public class PositionSyncer : PhotonBinaryStreamSerializer<PositionSyncer.Pos>
{
	// Token: 0x06000B11 RID: 2833 RVA: 0x0003AFB8 File Offset: 0x000391B8
	public override PositionSyncer.Pos GetDataToWrite()
	{
		this.lastSent = Optionable<float3>.Some(base.transform.position);
		return new PositionSyncer.Pos
		{
			Position = base.transform.position
		};
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0003B000 File Offset: 0x00039200
	public override void OnDataReceived(PositionSyncer.Pos data)
	{
		base.OnDataReceived(data);
		this.currentPos = base.transform.position;
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0003B01C File Offset: 0x0003921C
	public override bool ShouldSendData()
	{
		PositionSyncer.<>c__DisplayClass6_0 CS$<>8__locals1;
		CS$<>8__locals1.last = this.lastSent.Value;
		CS$<>8__locals1.n = base.transform.position;
		if (!PositionSyncer.<ShouldSendData>g__IsSame|6_0(ref CS$<>8__locals1))
		{
			return true;
		}
		if (this.forceSyncFrames > 0)
		{
			this.forceSyncFrames--;
			return true;
		}
		return false;
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x0003B074 File Offset: 0x00039274
	private void Update()
	{
		if (this.photonView.IsMine)
		{
			return;
		}
		double num = (double)(1f / (float)PhotonNetwork.SerializationRate);
		this.sinceLastPackage += Time.deltaTime;
		float num2 = (float)((double)this.sinceLastPackage / num);
		if (this.RemoteValue.IsSome)
		{
			PositionSyncer.Pos value = this.RemoteValue.Value;
			base.transform.position = Vector3.Lerp(this.currentPos, value.Position, num2);
		}
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x0003B0F5 File Offset: 0x000392F5
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (this.photonView.IsMine)
		{
			this.forceSyncFrames = 10;
		}
	}

	// Token: 0x06000B17 RID: 2839 RVA: 0x0003B11C File Offset: 0x0003931C
	[CompilerGenerated]
	internal static bool <ShouldSendData>g__IsSame|6_0(ref PositionSyncer.<>c__DisplayClass6_0 A_0)
	{
		return Mathf.Approximately(A_0.last.x, A_0.n.x) && Mathf.Approximately(A_0.last.y, A_0.n.y) && Mathf.Approximately(A_0.last.z, A_0.n.z);
	}

	// Token: 0x04000A4D RID: 2637
	private Vector3 currentPos;

	// Token: 0x04000A4E RID: 2638
	private int forceSyncFrames;

	// Token: 0x04000A4F RID: 2639
	private Optionable<float3> lastSent;

	// Token: 0x02000477 RID: 1143
	public struct Pos : IBinarySerializable
	{
		// Token: 0x06001B5B RID: 7003 RVA: 0x000824DF File Offset: 0x000806DF
		public void Serialize(BinarySerializer serializer)
		{
			serializer.WriteHalf3((half3)this.Position);
		}

		// Token: 0x06001B5C RID: 7004 RVA: 0x000824F2 File Offset: 0x000806F2
		public void Deserialize(BinaryDeserializer deserializer)
		{
			this.Position = deserializer.ReadHalf3();
		}

		// Token: 0x0400193F RID: 6463
		public float3 Position;
	}
}
