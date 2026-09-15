using System;
using System.Text;
using Steamworks;
using UnityEngine;
using Zorro.Core;

// Token: 0x020001B1 RID: 433
public class SteamAuthTicketService : GameService
{
	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000D59 RID: 3417 RVA: 0x0004324D File Offset: 0x0004144D
	public Optionable<SteamAuthTicketService.GeneratedTicket> CurrentTicket
	{
		get
		{
			return this.m_currentTicket;
		}
	}

	// Token: 0x06000D5A RID: 3418 RVA: 0x00043255 File Offset: 0x00041455
	public override void Update()
	{
		base.Update();
		this.VerifyHasValidTicket();
	}

	// Token: 0x06000D5B RID: 3419 RVA: 0x00043264 File Offset: 0x00041464
	public void VerifyHasValidTicket()
	{
		if (this.m_currentTicket.IsNone || (this.m_currentTicket.IsSome && this.m_currentTicket.Value.TimePassed > 60f))
		{
			this.GenerateNewTicket();
		}
	}

	// Token: 0x06000D5C RID: 3420 RVA: 0x000432AC File Offset: 0x000414AC
	private void GenerateNewTicket()
	{
		if (this.m_currentTicket.IsSome)
		{
			this.CancelSteamTicket(false);
		}
		ValueTuple<string, HAuthTicket> steamAuthTicket = SteamAuthTicketService.GetSteamAuthTicket();
		string item = steamAuthTicket.Item1;
		HAuthTicket item2 = steamAuthTicket.Item2;
		SteamAuthTicketService.GeneratedTicket generatedTicket = new SteamAuthTicketService.GeneratedTicket(item2, item);
		this.m_currentTicket = Optionable<SteamAuthTicketService.GeneratedTicket>.Some(generatedTicket);
	}

	// Token: 0x06000D5D RID: 3421 RVA: 0x000432F4 File Offset: 0x000414F4
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.CancelSteamTicket(true);
	}

	// Token: 0x06000D5E RID: 3422 RVA: 0x00043304 File Offset: 0x00041504
	public void CancelSteamTicket(bool immediate)
	{
		if (this.m_currentTicket.IsSome)
		{
			SteamAuthTicketService.<>c__DisplayClass9_0 CS$<>8__locals1 = new SteamAuthTicketService.<>c__DisplayClass9_0();
			CS$<>8__locals1.ticket = this.m_currentTicket.Value.Ticket;
			this.m_currentTicket = Optionable<SteamAuthTicketService.GeneratedTicket>.None;
			if (immediate)
			{
				SteamUser.CancelAuthTicket(CS$<>8__locals1.ticket);
				return;
			}
			GameHandler.Instance.StartCoroutine(CS$<>8__locals1.<CancelSteamTicket>g__CancelOverTime|0());
		}
	}

	// Token: 0x06000D5F RID: 3423 RVA: 0x00043368 File Offset: 0x00041568
	public static ValueTuple<string, HAuthTicket> GetSteamAuthTicket()
	{
		byte[] array = new byte[1024];
		CSteamID steamID = SteamUser.GetSteamID();
		SteamNetworkingIdentity steamNetworkingIdentity = default(SteamNetworkingIdentity);
		steamNetworkingIdentity.SetSteamID(steamID);
		uint num;
		HAuthTicket authSessionTicket = SteamUser.GetAuthSessionTicket(array, array.Length, ref num, ref steamNetworkingIdentity);
		Array.Resize<byte>(ref array, (int)num);
		StringBuilder stringBuilder = new StringBuilder();
		int num2 = 0;
		while ((long)num2 < (long)((ulong)num))
		{
			stringBuilder.AppendFormat("{0:x2}", array[num2]);
			num2++;
		}
		return new ValueTuple<string, HAuthTicket>(stringBuilder.ToString(), authSessionTicket);
	}

	// Token: 0x04000B90 RID: 2960
	private Optionable<SteamAuthTicketService.GeneratedTicket> m_currentTicket;

	// Token: 0x04000B91 RID: 2961
	private const float TICKET_MAX_LIFETIME = 60f;

	// Token: 0x020004A4 RID: 1188
	public struct GeneratedTicket
	{
		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06001BD5 RID: 7125 RVA: 0x00083774 File Offset: 0x00081974
		// (set) Token: 0x06001BD6 RID: 7126 RVA: 0x0008377C File Offset: 0x0008197C
		public string TicketData { readonly get; private set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06001BD7 RID: 7127 RVA: 0x00083785 File Offset: 0x00081985
		// (set) Token: 0x06001BD8 RID: 7128 RVA: 0x0008378D File Offset: 0x0008198D
		public HAuthTicket Ticket { readonly get; private set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06001BD9 RID: 7129 RVA: 0x00083796 File Offset: 0x00081996
		// (set) Token: 0x06001BDA RID: 7130 RVA: 0x0008379E File Offset: 0x0008199E
		public float TimeCreated { readonly get; private set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06001BDB RID: 7131 RVA: 0x000837A7 File Offset: 0x000819A7
		public float TimePassed
		{
			get
			{
				return Time.realtimeSinceStartup - this.TimeCreated;
			}
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x000837B5 File Offset: 0x000819B5
		public GeneratedTicket(HAuthTicket ticket, string data)
		{
			this.TicketData = data;
			this.TimeCreated = Time.realtimeSinceStartup;
			this.Ticket = ticket;
		}
	}
}
