using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200025D RID: 605
public class Fog : MonoBehaviour
{
	// Token: 0x1700011B RID: 283
	// (get) Token: 0x0600113D RID: 4413 RVA: 0x00056A01 File Offset: 0x00054C01
	private bool IsInFog
	{
		get
		{
			return Character.localCharacter.Center.y < base.transform.position.y;
		}
	}

	// Token: 0x0600113E RID: 4414 RVA: 0x00056A24 File Offset: 0x00054C24
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600113F RID: 4415 RVA: 0x00056A34 File Offset: 0x00054C34
	private void Update()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (this.stops == null)
		{
			Debug.LogError("Disabling fog movement: No stops were found");
			base.enabled = false;
			return;
		}
		this.Movement();
		this.MakePlayerCold();
		this.ApplyVisuals();
		if (this.view.IsMine)
		{
			this.Sync();
		}
		if (this.fogParticles == null)
		{
			return;
		}
		this.fogParticles.transform.position = Character.localCharacter.Center;
		if (this.IsInFog)
		{
			this.fogParticles.Play();
			Character.localCharacter.data.isInFog = true;
			return;
		}
		this.fogParticles.Stop();
		Character.localCharacter.data.isInFog = false;
	}

	// Token: 0x06001140 RID: 4416 RVA: 0x00056AF8 File Offset: 0x00054CF8
	private void Sync()
	{
		this.syncCounter += Time.deltaTime;
		if (this.syncCounter > 5f)
		{
			this.syncCounter = 0f;
			this.view.RPC("RPCA_SyncFog", 1, new object[] { this.fogHeight });
		}
	}

	// Token: 0x06001141 RID: 4417 RVA: 0x00056B54 File Offset: 0x00054D54
	private void ApplyVisuals()
	{
		base.transform.position = new Vector3(Character.localCharacter.Center.x, this.fogHeight, Mathf.Clamp(Character.localCharacter.Center.z, -10000f, 870f));
		Shader.SetGlobalFloat(Fog.FogHeight, base.transform.position.y);
	}

	// Token: 0x06001142 RID: 4418 RVA: 0x00056BC0 File Offset: 0x00054DC0
	private void MakePlayerCold()
	{
		if (Character.localCharacter == null)
		{
			return;
		}
		if (this.IsInFog)
		{
			if (Character.localCharacter.data.isSkeleton)
			{
				float num = this.amount / 8f * Time.deltaTime;
				Debug.Log(string.Format("Adding {0} injury to skeleton", num));
				Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Injury, num, false, true, true);
				return;
			}
			Debug.Log("Adding cold to player in fog");
			Character.localCharacter.refs.afflictions.AddStatus(CharacterAfflictions.STATUSTYPE.Cold, this.amount * Time.deltaTime, false, true, true);
		}
	}

	// Token: 0x06001143 RID: 4419 RVA: 0x00056C69 File Offset: 0x00054E69
	private void Movement()
	{
		if (this.waiting)
		{
			this.Wait();
			return;
		}
		this.Move();
	}

	// Token: 0x06001144 RID: 4420 RVA: 0x00056C80 File Offset: 0x00054E80
	private void Wait()
	{
		if (!this.view.IsMine)
		{
			return;
		}
		this.sinceStop += Time.deltaTime;
		if (this.TimeToMove() || this.PlayersHaveMovedOn())
		{
			this.view.RPC("RPCA_Resume", 0, Array.Empty<object>());
		}
	}

	// Token: 0x06001145 RID: 4421 RVA: 0x00056CD3 File Offset: 0x00054ED3
	private bool TimeToMove()
	{
		return this.sinceStop > this.maxWaitTime && this.currentStop > 0;
	}

	// Token: 0x06001146 RID: 4422 RVA: 0x00056CF0 File Offset: 0x00054EF0
	private bool PlayersHaveMovedOn()
	{
		if (Character.AllCharacters.Count == 0)
		{
			return false;
		}
		float num = this.StopHeight() + this.startMoveHeightThreshold;
		for (int i = 0; i < Character.AllCharacters.Count; i++)
		{
			if (Character.AllCharacters[i].Center.y < num)
			{
				return false;
			}
		}
		Debug.Log("Players have moved on");
		return true;
	}

	// Token: 0x06001147 RID: 4423 RVA: 0x00056D53 File Offset: 0x00054F53
	[PunRPC]
	private void RPCA_Resume()
	{
		this.currentStop++;
		this.waiting = false;
		GUIManager.instance.TheFogRises();
	}

	// Token: 0x06001148 RID: 4424 RVA: 0x00056D74 File Offset: 0x00054F74
	private void Move()
	{
		if (this.currentStop >= this.stops.Length)
		{
			return;
		}
		this.fogHeight += Time.deltaTime * this.fogSpeed;
		if (this.fogHeight > this.StopHeight())
		{
			this.Stop();
		}
	}

	// Token: 0x06001149 RID: 4425 RVA: 0x00056DB4 File Offset: 0x00054FB4
	private void Stop()
	{
		this.sinceStop = 0f;
		this.waiting = true;
	}

	// Token: 0x0600114A RID: 4426 RVA: 0x00056DC8 File Offset: 0x00054FC8
	private float StopHeight()
	{
		return this.stops[this.currentStop].transform.position.y;
	}

	// Token: 0x0600114B RID: 4427 RVA: 0x00056DE6 File Offset: 0x00054FE6
	[PunRPC]
	public void RPCA_SyncFog(float setHeight)
	{
		this.fogHeight = setHeight;
	}

	// Token: 0x04000FAE RID: 4014
	public float fogHeight;

	// Token: 0x04000FAF RID: 4015
	public float fogSpeed = 0.4f;

	// Token: 0x04000FB0 RID: 4016
	public float amount;

	// Token: 0x04000FB1 RID: 4017
	private static readonly int FogHeight = Shader.PropertyToID("FogHeight");

	// Token: 0x04000FB2 RID: 4018
	private Transform[] stops;

	// Token: 0x04000FB3 RID: 4019
	private int currentStop;

	// Token: 0x04000FB4 RID: 4020
	private float sinceStop;

	// Token: 0x04000FB5 RID: 4021
	public float maxWaitTime = 180f;

	// Token: 0x04000FB6 RID: 4022
	public float startMoveHeightThreshold = 60f;

	// Token: 0x04000FB7 RID: 4023
	private bool waiting;

	// Token: 0x04000FB8 RID: 4024
	private PhotonView view;

	// Token: 0x04000FB9 RID: 4025
	public ParticleSystem fogParticles;

	// Token: 0x04000FBA RID: 4026
	private float syncCounter;
}
