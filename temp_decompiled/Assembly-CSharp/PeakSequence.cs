using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002BA RID: 698
public class PeakSequence : MonoBehaviour
{
	// Token: 0x0600130A RID: 4874 RVA: 0x000609DA File Offset: 0x0005EBDA
	private void Start()
	{
		this.view = base.GetComponent<PhotonView>();
	}

	// Token: 0x0600130B RID: 4875 RVA: 0x000609E8 File Offset: 0x0005EBE8
	private void OnDisable()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.Log("Destroying ropes");
			if (this.ropeAnchorInstance != null)
			{
				PhotonNetwork.Destroy(this.ropeAnchorInstance.photonView);
			}
			if (this.ropeInstance != null)
			{
				PhotonNetwork.Destroy(this.ropeInstance.photonView);
				return;
			}
		}
		else
		{
			if (this.ropeAnchorInstance != null)
			{
				this.ropeAnchorInstance.gameObject.SetActive(false);
			}
			if (this.ropeInstance != null)
			{
				this.ropeInstance.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x0600130C RID: 4876 RVA: 0x00060A84 File Offset: 0x0005EC84
	private void Update()
	{
		if (this.waitTime > this.timeToWait)
		{
			if (!this.spawnedRope)
			{
				if (PhotonNetwork.IsMasterClient)
				{
					this.spawnedRope = true;
					GameObject gameObject = PhotonNetwork.Instantiate(this.ropeAnchorWithRopePref.name, this.ropeSpawnPoint.position, Quaternion.identity, 0, null);
					this.ropeAnchorInstance = gameObject.GetComponent<RopeAnchorWithRope>();
					this.ropeAnchorInstance.ropeSegmentLength = 40f;
					Rope rope = this.ropeAnchorInstance.SpawnRope();
					this.view.RPC("SetRopeToClients", 0, new object[] { rope.GetComponent<PhotonView>() });
				}
			}
			else
			{
				this.CheckGameComplete();
			}
		}
		this.waitTime += Time.deltaTime;
	}

	// Token: 0x0600130D RID: 4877 RVA: 0x00060B40 File Offset: 0x0005ED40
	private void CheckGameComplete()
	{
		if (this.endingGame)
		{
			return;
		}
		if (PhotonNetwork.IsMasterClient)
		{
			int num = 0;
			List<Character> allPlayerCharacters = PlayerHandler.GetAllPlayerCharacters();
			for (int i = allPlayerCharacters.Count - 1; i >= 0; i--)
			{
				if (allPlayerCharacters[i].data.dead)
				{
					allPlayerCharacters.RemoveAt(i);
				}
			}
			List<Character> list = new List<Character>();
			foreach (Character character in allPlayerCharacters)
			{
				if (character.data.fullyConscious)
				{
					list.Add(character);
				}
			}
			for (int j = 0; j < allPlayerCharacters.Count; j++)
			{
				if (Character.CheckWinCondition(allPlayerCharacters[j]))
				{
					num++;
				}
			}
			if (num > 0)
			{
				this.timerElapsed += Time.deltaTime;
				if (this.timerElapsed >= this.lengthOfASecond)
				{
					if (num >= list.Count && this.secondsElapsed < this.totalSeconds - this.totalWinningSeconds)
					{
						this.secondsElapsed = this.totalSeconds - this.totalWinningSeconds;
					}
					this.timerElapsed = 0f;
					this.view.RPC("RPCUpdateTimer", 0, new object[] { this.secondsElapsed });
					this.secondsElapsed++;
					if (this.secondsElapsed > this.totalSeconds)
					{
						this.endingGame = true;
						Character.localCharacter.EndGame();
						return;
					}
				}
			}
			else
			{
				this.secondsElapsed = 0;
				this.timerElapsed = 0f;
				this.view.RPC("RPCUpdateTimer", 0, new object[] { -1 });
			}
		}
	}

	// Token: 0x0600130E RID: 4878 RVA: 0x00060D00 File Offset: 0x0005EF00
	[PunRPC]
	public void SetRopeToClients(PhotonView v)
	{
		this.ropeInstance = v.GetComponent<Rope>();
		Debug.Log(string.Format("ROPE AS BEEN SET TO {0}", this.ropeInstance));
	}

	// Token: 0x0600130F RID: 4879 RVA: 0x00060D23 File Offset: 0x0005EF23
	[PunRPC]
	private void RPCUpdateTimer(int seconds)
	{
		if (seconds == -1)
		{
			GUIManager.instance.endgame.Disable();
			return;
		}
		GUIManager.instance.endgame.UpdateCounter(this.totalSeconds - seconds);
	}

	// Token: 0x040011AC RID: 4524
	private PhotonView view;

	// Token: 0x040011AD RID: 4525
	public GameObject ropeAnchorWithRopePref;

	// Token: 0x040011AE RID: 4526
	public Transform ropeSpawnPoint;

	// Token: 0x040011AF RID: 4527
	private float waitTime;

	// Token: 0x040011B0 RID: 4528
	public float timeToWait = 5f;

	// Token: 0x040011B1 RID: 4529
	public int totalSeconds = 30;

	// Token: 0x040011B2 RID: 4530
	public int totalWinningSeconds = 5;

	// Token: 0x040011B3 RID: 4531
	public float lengthOfASecond = 1.5f;

	// Token: 0x040011B4 RID: 4532
	private bool spawnedRope;

	// Token: 0x040011B5 RID: 4533
	public RopeAnchorWithRope ropeAnchorInstance;

	// Token: 0x040011B6 RID: 4534
	public Rope ropeInstance;

	// Token: 0x040011B7 RID: 4535
	private float timerElapsed;

	// Token: 0x040011B8 RID: 4536
	private int secondsElapsed;

	// Token: 0x040011B9 RID: 4537
	private bool endingGame;
}
