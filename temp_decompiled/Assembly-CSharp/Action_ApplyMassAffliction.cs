using System;
using Peak.Afflictions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class Action_ApplyMassAffliction : Action_ApplyAffliction
{
	// Token: 0x0600080A RID: 2058 RVA: 0x0002D014 File Offset: 0x0002B214
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.radius);
	}

	// Token: 0x0600080B RID: 2059 RVA: 0x0002D036 File Offset: 0x0002B236
	public override void RunAction()
	{
		if (this.affliction == null)
		{
			Debug.LogError("Your affliction is null bro");
			return;
		}
		Debug.Log("Running RPC");
		this.item.photonView.RPC("TryAddAfflictionToLocalCharacter", 0, Array.Empty<object>());
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0002D070 File Offset: 0x0002B270
	[PunRPC]
	public void TryAddAfflictionToLocalCharacter()
	{
		if (this.ignoreCaster && this.item.holderCharacter == Character.localCharacter)
		{
			return;
		}
		if (Vector3.Distance(Character.localCharacter.Center, base.transform.position) <= this.radius)
		{
			Character.localCharacter.refs.afflictions.AddAffliction(this.affliction, false);
			if (this.extraAfflictions != null)
			{
				foreach (Affliction affliction in this.extraAfflictions)
				{
					Character.localCharacter.refs.afflictions.AddAffliction(affliction, false);
				}
			}
		}
	}

	// Token: 0x040007D3 RID: 2003
	public float radius;

	// Token: 0x040007D4 RID: 2004
	public bool ignoreCaster;
}
