using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine;
using Zorro.Core;

// Token: 0x0200014F RID: 335
public class NetworkStats : Singleton<NetworkStats>
{
	// Token: 0x06000AC3 RID: 2755 RVA: 0x00039A94 File Offset: 0x00037C94
	private void Update()
	{
		this.m_timer += Time.deltaTime;
		if (this.m_timer > 1f)
		{
			this.m_timer -= 1f;
			this.m_lastRecievedDelta = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesIn - this.m_bytesReceivedLastSecond;
			this.m_bytesReceivedLastSecond = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesIn;
			this.m_lastSentDelta = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesOut - this.m_bytesSentLastSecond;
			this.m_bytesSentLastSecond = PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesOut;
			foreach (KeyValuePair<string, ulong> keyValuePair in this.m_binaryStreamsByType)
			{
				string key = keyValuePair.Key;
				ulong value = keyValuePair.Value;
				this.<Update>g__UpdateEntry|8_0(key, value);
			}
			this.<Update>g__UpdateEntry|8_0("VoiceData", (ulong)PhotonVoiceStats.bytesSent);
		}
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x00039BA0 File Offset: 0x00037DA0
	public static void RegisterBytesSent<T>(ulong bytesSent)
	{
		Type typeFromHandle = typeof(T);
		if (!Singleton<NetworkStats>.Instance.m_binaryStreamsByType.ContainsKey(typeFromHandle.Name))
		{
			Singleton<NetworkStats>.Instance.m_binaryStreamsByType.Add(typeFromHandle.Name, 0UL);
		}
		Dictionary<string, ulong> binaryStreamsByType = Singleton<NetworkStats>.Instance.m_binaryStreamsByType;
		string name = typeFromHandle.Name;
		binaryStreamsByType[name] += bytesSent;
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x00039C08 File Offset: 0x00037E08
	public List<ValueTuple<string, ulong>> GetBytesSent()
	{
		return Enumerable.ToList<ValueTuple<string, ulong>>(Enumerable.Select<KeyValuePair<string, ulong>, ValueTuple<string, ulong>>(this.m_binaryStreamsByType, (KeyValuePair<string, ulong> pair) => new ValueTuple<string, ulong>(pair.Key, pair.Value)));
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x00039C39 File Offset: 0x00037E39
	public List<ValueTuple<string, ulong>> GetBytesDeltaSent()
	{
		return Enumerable.ToList<ValueTuple<string, ulong>>(Enumerable.Select<KeyValuePair<string, ulong>, ValueTuple<string, ulong>>(this.m_binaryStreamsByTypeDelta, (KeyValuePair<string, ulong> pair) => new ValueTuple<string, ulong>(pair.Key, pair.Value)));
	}

	// Token: 0x06000AC8 RID: 2760 RVA: 0x00039C94 File Offset: 0x00037E94
	[CompilerGenerated]
	private void <Update>g__UpdateEntry|8_0(string key, ulong value)
	{
		if (this.m_binaryStreamsByTypeSecond.ContainsKey(key))
		{
			ulong num = this.m_binaryStreamsByTypeSecond[key];
			ulong num2 = value - num;
			this.m_binaryStreamsByTypeDelta[key] = num2;
		}
		this.m_binaryStreamsByTypeSecond[key] = value;
	}

	// Token: 0x04000A09 RID: 2569
	public long m_bytesReceivedLastSecond;

	// Token: 0x04000A0A RID: 2570
	public long m_lastRecievedDelta;

	// Token: 0x04000A0B RID: 2571
	public long m_bytesSentLastSecond;

	// Token: 0x04000A0C RID: 2572
	public long m_lastSentDelta;

	// Token: 0x04000A0D RID: 2573
	private float m_timer;

	// Token: 0x04000A0E RID: 2574
	private Dictionary<string, ulong> m_binaryStreamsByType = new Dictionary<string, ulong>();

	// Token: 0x04000A0F RID: 2575
	private Dictionary<string, ulong> m_binaryStreamsByTypeSecond = new Dictionary<string, ulong>();

	// Token: 0x04000A10 RID: 2576
	private Dictionary<string, ulong> m_binaryStreamsByTypeDelta = new Dictionary<string, ulong>();
}
