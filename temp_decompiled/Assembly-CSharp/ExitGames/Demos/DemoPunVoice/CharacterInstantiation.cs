using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice
{
	// Token: 0x02000387 RID: 903
	public class CharacterInstantiation : MonoBehaviourPunCallbacks, IOnEventCallback
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06001723 RID: 5923 RVA: 0x00075B48 File Offset: 0x00073D48
		// (remove) Token: 0x06001724 RID: 5924 RVA: 0x00075B7C File Offset: 0x00073D7C
		public static event CharacterInstantiation.OnCharacterInstantiated CharacterInstantiated;

		// Token: 0x06001725 RID: 5925 RVA: 0x00075BB0 File Offset: 0x00073DB0
		public override void OnJoinedRoom()
		{
			if (!this.AutoSpawn)
			{
				return;
			}
			if (this.PrefabsToInstantiate != null)
			{
				int num = PhotonNetwork.LocalPlayer.ActorNumber;
				if (num < 1)
				{
					num = 1;
				}
				int num2 = (num - 1) % this.PrefabsToInstantiate.Length;
				Vector3 vector;
				Quaternion quaternion;
				this.GetSpawnPoint(out vector, out quaternion);
				Camera.main.transform.position += vector;
				if (this.manualInstantiation)
				{
					this.ManualInstantiation(num2, vector, quaternion);
					return;
				}
				GameObject gameObject = this.PrefabsToInstantiate[num2];
				gameObject = PhotonNetwork.Instantiate(gameObject.name, vector, quaternion, 0, null);
				if (CharacterInstantiation.CharacterInstantiated != null)
				{
					CharacterInstantiation.CharacterInstantiated(gameObject);
				}
			}
		}

		// Token: 0x06001726 RID: 5926 RVA: 0x00075C58 File Offset: 0x00073E58
		private void ManualInstantiation(int index, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = this.PrefabsToInstantiate[index];
			GameObject gameObject2;
			if (this.differentPrefabs)
			{
				gameObject2 = Object.Instantiate<GameObject>(Resources.Load(string.Format("{0}{1}", gameObject.name, this.localPrefabSuffix)) as GameObject, position, rotation);
			}
			else
			{
				gameObject2 = Object.Instantiate<GameObject>(gameObject, position, rotation);
			}
			PhotonView component = gameObject2.GetComponent<PhotonView>();
			if (PhotonNetwork.AllocateViewID(component))
			{
				object[] array = new object[]
				{
					index,
					gameObject2.transform.position,
					gameObject2.transform.rotation,
					component.ViewID
				};
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions
				{
					Receivers = 0,
					CachingOption = 4
				};
				PhotonNetwork.RaiseEvent(this.manualInstantiationEventCode, array, raiseEventOptions, SendOptions.SendReliable);
				if (CharacterInstantiation.CharacterInstantiated != null)
				{
					CharacterInstantiation.CharacterInstantiated(gameObject2);
					return;
				}
			}
			else
			{
				Debug.LogError("Failed to allocate a ViewId.");
				Object.Destroy(gameObject2);
			}
		}

		// Token: 0x06001727 RID: 5927 RVA: 0x00075D4C File Offset: 0x00073F4C
		public void OnEvent(EventData photonEvent)
		{
			if (photonEvent.Code == this.manualInstantiationEventCode)
			{
				object[] array = photonEvent.CustomData as object[];
				int num = (int)array[0];
				GameObject gameObject = this.PrefabsToInstantiate[num];
				Vector3 vector = (Vector3)array[1];
				Quaternion quaternion = (Quaternion)array[2];
				GameObject gameObject2;
				if (this.differentPrefabs)
				{
					gameObject2 = Object.Instantiate<GameObject>(Resources.Load(string.Format("{0}{1}", gameObject.name, this.remotePrefabSuffix)) as GameObject, vector, quaternion);
				}
				else
				{
					gameObject2 = Object.Instantiate<GameObject>(gameObject, vector, Quaternion.identity);
				}
				gameObject2.GetComponent<PhotonView>().ViewID = (int)array[3];
			}
		}

		// Token: 0x06001728 RID: 5928 RVA: 0x00075DF4 File Offset: 0x00073FF4
		protected virtual void GetSpawnPoint(out Vector3 spawnPos, out Quaternion spawnRot)
		{
			Transform spawnPoint = this.GetSpawnPoint();
			if (spawnPoint != null)
			{
				spawnPos = spawnPoint.position;
				spawnRot = spawnPoint.rotation;
			}
			else
			{
				spawnPos = new Vector3(0f, 0f, 0f);
				spawnRot = new Quaternion(0f, 0f, 0f, 1f);
			}
			if (this.UseRandomOffset)
			{
				Debug.Log("Set Seed");
				Random.InitState((int)(Time.time * 10000f));
				Vector3 vector = Random.insideUnitSphere;
				vector.y = 0f;
				vector = vector.normalized;
				spawnPos += this.PositionOffset * vector;
			}
		}

		// Token: 0x06001729 RID: 5929 RVA: 0x00075EC0 File Offset: 0x000740C0
		protected virtual Transform GetSpawnPoint()
		{
			if (this.SpawnPoints == null || this.SpawnPoints.Count == 0)
			{
				return null;
			}
			switch (this.Sequence)
			{
			case CharacterInstantiation.SpawnSequence.Connection:
			{
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				return this.SpawnPoints[(actorNumber == -1) ? 0 : (actorNumber % this.SpawnPoints.Count)];
			}
			case CharacterInstantiation.SpawnSequence.Random:
				return this.SpawnPoints[Random.Range(0, this.SpawnPoints.Count)];
			case CharacterInstantiation.SpawnSequence.RoundRobin:
				this.lastUsedSpawnPointIndex++;
				if (this.lastUsedSpawnPointIndex >= this.SpawnPoints.Count)
				{
					this.lastUsedSpawnPointIndex = 0;
				}
				return this.SpawnPoints[this.lastUsedSpawnPointIndex];
			default:
				return null;
			}
		}

		// Token: 0x0400159E RID: 5534
		public Transform SpawnPosition;

		// Token: 0x0400159F RID: 5535
		public float PositionOffset = 2f;

		// Token: 0x040015A0 RID: 5536
		public GameObject[] PrefabsToInstantiate;

		// Token: 0x040015A1 RID: 5537
		public List<Transform> SpawnPoints;

		// Token: 0x040015A2 RID: 5538
		public bool AutoSpawn = true;

		// Token: 0x040015A3 RID: 5539
		public bool UseRandomOffset = true;

		// Token: 0x040015A4 RID: 5540
		public CharacterInstantiation.SpawnSequence Sequence;

		// Token: 0x040015A6 RID: 5542
		[SerializeField]
		private byte manualInstantiationEventCode = 1;

		// Token: 0x040015A7 RID: 5543
		protected int lastUsedSpawnPointIndex = -1;

		// Token: 0x040015A8 RID: 5544
		[SerializeField]
		private bool manualInstantiation;

		// Token: 0x040015A9 RID: 5545
		[SerializeField]
		private bool differentPrefabs;

		// Token: 0x040015AA RID: 5546
		[SerializeField]
		private string localPrefabSuffix;

		// Token: 0x040015AB RID: 5547
		[SerializeField]
		private string remotePrefabSuffix;

		// Token: 0x0200052C RID: 1324
		public enum SpawnSequence
		{
			// Token: 0x04001BE0 RID: 7136
			Connection,
			// Token: 0x04001BE1 RID: 7137
			Random,
			// Token: 0x04001BE2 RID: 7138
			RoundRobin
		}

		// Token: 0x0200052D RID: 1325
		// (Invoke) Token: 0x06001DD6 RID: 7638
		public delegate void OnCharacterInstantiated(GameObject character);
	}
}
