using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x0200034E RID: 846
public class TiedBalloon : MonoBehaviourPunCallbacks
{
	// Token: 0x060015B9 RID: 5561 RVA: 0x0007022B File Offset: 0x0006E42B
	public void Init(CharacterBalloons characterBalloons, float height, int colorID)
	{
		base.photonView.RPC("RPC_Init", 0, new object[]
		{
			characterBalloons.photonView.ViewID,
			height,
			colorID
		});
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x0007026C File Offset: 0x0006E46C
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if (base.photonView.IsMine)
		{
			base.photonView.RPC("RPC_Init", newPlayer, new object[]
			{
				this.characterBalloons.photonView.ViewID,
				this.characterBalloons.character.Center.y,
				this.colorIndex
			});
		}
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x000702E0 File Offset: 0x0006E4E0
	[PunRPC]
	public void RPC_Init(int characterID, float height, int colorID)
	{
		base.StartCoroutine(this.InitRoutine(characterID, height, colorID));
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x000702F2 File Offset: 0x0006E4F2
	private IEnumerator InitRoutine(int characterID, float height, int colorID)
	{
		while (!Character.localCharacter)
		{
			yield return null;
		}
		PhotonView photonView = PhotonView.Find(characterID);
		if (photonView == null)
		{
			Debug.LogError("Tried to assign balloon to nonexistent photon ID.");
			yield break;
		}
		CharacterBalloons component = photonView.GetComponent<CharacterBalloons>();
		Debug.Log(string.Format("Init Balloon for view {0} with color {1}", characterID, colorID));
		this.balloonRenderer.material = Character.localCharacter.refs.balloons.balloonColors[colorID];
		this.colorIndex = colorID;
		this.initialHeight = height;
		this.initialTime = Time.time;
		this.characterBalloons = component;
		component.tiedBalloons.Add(this);
		yield break;
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x00070316 File Offset: 0x0006E516
	private void LateUpdate()
	{
		this.UpdateLineRenderer();
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x00070320 File Offset: 0x0006E520
	private void FixedUpdate()
	{
		this.rb.AddForce(Vector3.up * this.floatForce, 5);
		this.UpdateLineRenderer();
		if (base.photonView.IsMine && (this.rb.transform.position.y > this.initialHeight + this.popHeight || Time.time > this.initialTime + this.popTime))
		{
			this.Pop();
		}
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x0007039A File Offset: 0x0006E59A
	public void Pop()
	{
		if (base.photonView.IsMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	// Token: 0x060015C0 RID: 5568 RVA: 0x000703B4 File Offset: 0x0006E5B4
	private void OnDestroy()
	{
		this.characterBalloons.RemoveBalloon(this);
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x000703C4 File Offset: 0x0006E5C4
	private void UpdateLineRenderer()
	{
		this.positions[0] = this.start.position;
		this.positions[1] = this.end.position;
		this.lr.SetPositions(this.positions);
	}

	// Token: 0x0400148F RID: 5263
	public LineRenderer lr;

	// Token: 0x04001490 RID: 5264
	public Transform anchor;

	// Token: 0x04001491 RID: 5265
	public Transform start;

	// Token: 0x04001492 RID: 5266
	public Transform end;

	// Token: 0x04001493 RID: 5267
	public Rigidbody rb;

	// Token: 0x04001494 RID: 5268
	public MeshRenderer balloonRenderer;

	// Token: 0x04001495 RID: 5269
	public float floatForce = 10f;

	// Token: 0x04001496 RID: 5270
	public int colorIndex;

	// Token: 0x04001497 RID: 5271
	private float initialHeight;

	// Token: 0x04001498 RID: 5272
	public float popHeight = 100f;

	// Token: 0x04001499 RID: 5273
	public float popTime = 10f;

	// Token: 0x0400149A RID: 5274
	private CharacterBalloons characterBalloons;

	// Token: 0x0400149B RID: 5275
	private float initialTime;

	// Token: 0x0400149C RID: 5276
	private Vector3[] positions = new Vector3[2];
}
