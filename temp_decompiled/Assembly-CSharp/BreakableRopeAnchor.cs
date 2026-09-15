using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Photon.Pun;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x0200021B RID: 539
public class BreakableRopeAnchor : MonoBehaviour
{
	// Token: 0x06000FED RID: 4077 RVA: 0x0004F27A File Offset: 0x0004D47A
	private void Awake()
	{
		this.anchor = base.GetComponent<RopeAnchorWithRope>();
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x0004F294 File Offset: 0x0004D494
	private void Start()
	{
		this.willBreakInTime = ExtDataTypes.PRndRange(this.breakableTimeMinMax);
	}

	// Token: 0x06000FEF RID: 4079 RVA: 0x0004F2A8 File Offset: 0x0004D4A8
	private void Update()
	{
		if (!this.photonView.IsMine)
		{
			return;
		}
		List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
		int num = 0;
		foreach (Character character in allPlayerCharacters)
		{
			if (character.data.isRopeClimbing && character.data.heldRope == this.anchor.rope)
			{
				num++;
			}
		}
		if (num > 0)
		{
			this.willBreakInTime -= Time.deltaTime;
		}
		if (this.willBreakInTime > 0f)
		{
			return;
		}
		if (this.isBreaking)
		{
			return;
		}
		base.StartCoroutine(this.<Update>g__Break|9_0());
	}

	// Token: 0x06000FF1 RID: 4081 RVA: 0x0004F39F File Offset: 0x0004D59F
	[CompilerGenerated]
	private IEnumerator <Update>g__Break|9_0()
	{
		this.isBreaking = true;
		Debug.Log(string.Format("Break: segments {0}", this.anchor.rope.Segments));
		this.anchor.rope.Segments += this.dropSegments;
		float elapsed = 0f;
		float startSegments = this.anchor.rope.Segments;
		while (elapsed < this.breakAnimTime)
		{
			elapsed += Time.deltaTime;
			this.anchor.rope.Segments = Mathf.Lerp(startSegments, startSegments + 1f, elapsed / 0.5f);
			yield return null;
		}
		Debug.Log("Detach_Rpc");
		this.anchor.rope.photonView.RPC("Detach_Rpc", 3, new object[] { this.anchor.rope.Segments });
		yield break;
	}

	// Token: 0x04000E55 RID: 3669
	public float breakAnimTime = 3f;

	// Token: 0x04000E56 RID: 3670
	public Vector2 breakableTimeMinMax = new Vector2(3f, 8f);

	// Token: 0x04000E57 RID: 3671
	public float dropSegments = 1f;

	// Token: 0x04000E58 RID: 3672
	private float willBreakInTime;

	// Token: 0x04000E59 RID: 3673
	private RopeAnchorWithRope anchor;

	// Token: 0x04000E5A RID: 3674
	private PhotonView photonView;

	// Token: 0x04000E5B RID: 3675
	private bool isBreaking;
}
