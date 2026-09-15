using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200022B RID: 555
public class CheckpointFlag : MonoBehaviour
{
	// Token: 0x06001088 RID: 4232 RVA: 0x0005323C File Offset: 0x0005143C
	public void Initialize(Character flagPlanterCharacter)
	{
		this.planterCharacter = flagPlanterCharacter;
		this.currentStatuses = new float[Enum.GetNames(typeof(CharacterAfflictions.STATUSTYPE)).Length];
		Array.Copy(flagPlanterCharacter.refs.afflictions.currentStatuses, this.currentStatuses, flagPlanterCharacter.refs.afflictions.currentStatuses.Length);
		this.planterCharacter.data.checkpointFlags.Add(this);
		base.transform.rotation = Quaternion.identity;
		base.Invoke("DisableAnim", 1f);
		if (base.TryGetComponent<PhotonView>(ref this._networkView))
		{
			this._networkView.RPC("SetColor", 3, new object[] { this.planterCharacter.refs.customization.PlayerColorAsVector });
			return;
		}
		Debug.LogWarning("Can't SetColor because " + base.name + " has no PhotonView", this);
	}

	// Token: 0x06001089 RID: 4233 RVA: 0x0005332D File Offset: 0x0005152D
	private void DisableAnim()
	{
		this.anim.enabled = false;
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x0005333C File Offset: 0x0005153C
	[PunRPC]
	public void SetColor(Vector3 c)
	{
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetColor(CheckpointFlag.FlagColorPropertyId, new Color(c.x, c.y, c.z));
		this.flagRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x0005337D File Offset: 0x0005157D
	public void DestroySelf()
	{
		PhotonNetwork.Destroy(base.gameObject);
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x0005338A File Offset: 0x0005158A
	private void OnDisable()
	{
		if (this.planterCharacter != null)
		{
			this.planterCharacter.data.checkpointFlags.Remove(this);
		}
		Debug.Log("Checkpoint Flag Disabled");
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x000533BC File Offset: 0x000515BC
	private Color MaxSatVal(Color input)
	{
		float num;
		float num2;
		float num3;
		Color.RGBToHSV(input, ref num, ref num2, ref num3);
		num2 = 1f;
		num3 = 1f;
		return Color.HSVToRGB(num, num2, num3);
	}

	// Token: 0x04000EB9 RID: 3769
	private static readonly int FlagColorPropertyId = Shader.PropertyToID("_BaseColor");

	// Token: 0x04000EBA RID: 3770
	private PhotonView _networkView;

	// Token: 0x04000EBB RID: 3771
	[HideInInspector]
	public float[] currentStatuses;

	// Token: 0x04000EBC RID: 3772
	[HideInInspector]
	public Character planterCharacter;

	// Token: 0x04000EBD RID: 3773
	public Renderer flagRenderer;

	// Token: 0x04000EBE RID: 3774
	public Animator anim;
}
