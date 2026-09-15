using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x020002C0 RID: 704
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ProximityVoiceTrigger : VoiceComponent
{
	// Token: 0x1700012B RID: 299
	// (get) Token: 0x06001320 RID: 4896 RVA: 0x000610BE File Offset: 0x0005F2BE
	public byte TargetInterestGroup
	{
		get
		{
			if (this.photonView != null)
			{
				return (byte)this.photonView.OwnerActorNr;
			}
			return 0;
		}
	}

	// Token: 0x06001321 RID: 4897 RVA: 0x000610DC File Offset: 0x0005F2DC
	protected override void Awake()
	{
		this.photonVoiceView = base.GetComponentInParent<PhotonVoiceView>();
		this.photonView = base.GetComponentInParent<PhotonView>();
		base.GetComponent<Collider>().isTrigger = true;
		this.IsLocalCheck();
	}

	// Token: 0x06001322 RID: 4898 RVA: 0x0006110C File Offset: 0x0005F30C
	private void ToggleTransmission()
	{
		if (this.photonVoiceView.RecorderInUse != null)
		{
			byte targetInterestGroup = this.TargetInterestGroup;
			if (this.photonVoiceView.RecorderInUse.InterestGroup != targetInterestGroup)
			{
				base.Logger.Log(3, "Setting RecorderInUse's InterestGroup to {0}", new object[] { targetInterestGroup });
				this.photonVoiceView.RecorderInUse.InterestGroup = targetInterestGroup;
			}
			this.photonVoiceView.RecorderInUse.RecordingEnabled = true;
		}
	}

	// Token: 0x06001323 RID: 4899 RVA: 0x00061188 File Offset: 0x0005F388
	private void OnTriggerEnter(Collider other)
	{
		if (this.IsLocalCheck())
		{
			ProximityVoiceTrigger component = other.GetComponent<ProximityVoiceTrigger>();
			if (component != null)
			{
				byte targetInterestGroup = component.TargetInterestGroup;
				base.Logger.Log(4, "OnTriggerEnter {0}", new object[] { targetInterestGroup });
				if (targetInterestGroup == this.TargetInterestGroup)
				{
					return;
				}
				if (targetInterestGroup == 0)
				{
					return;
				}
				if (!this.groupsToAdd.Contains(targetInterestGroup))
				{
					this.groupsToAdd.Add(targetInterestGroup);
				}
			}
		}
	}

	// Token: 0x06001324 RID: 4900 RVA: 0x000611FC File Offset: 0x0005F3FC
	private void OnTriggerExit(Collider other)
	{
		if (this.IsLocalCheck())
		{
			ProximityVoiceTrigger component = other.GetComponent<ProximityVoiceTrigger>();
			if (component != null)
			{
				byte targetInterestGroup = component.TargetInterestGroup;
				base.Logger.Log(4, "OnTriggerExit {0}", new object[] { targetInterestGroup });
				if (targetInterestGroup == this.TargetInterestGroup)
				{
					return;
				}
				if (targetInterestGroup == 0)
				{
					return;
				}
				if (this.groupsToAdd.Contains(targetInterestGroup))
				{
					this.groupsToAdd.Remove(targetInterestGroup);
				}
				if (!this.groupsToRemove.Contains(targetInterestGroup))
				{
					this.groupsToRemove.Add(targetInterestGroup);
				}
			}
		}
	}

	// Token: 0x06001325 RID: 4901 RVA: 0x0006128C File Offset: 0x0005F48C
	protected void Update()
	{
		if (!PunVoiceClient.Instance.Client.InRoom)
		{
			this.subscribedGroups = null;
			return;
		}
		if (this.IsLocalCheck())
		{
			if (this.groupsToAdd.Count > 0 || this.groupsToRemove.Count > 0)
			{
				byte[] array = null;
				byte[] array2 = null;
				if (this.groupsToAdd.Count > 0)
				{
					array = this.groupsToAdd.ToArray();
				}
				if (this.groupsToRemove.Count > 0)
				{
					array2 = this.groupsToRemove.ToArray();
				}
				base.Logger.Log(3, "client of actor number {0} trying to change groups, to_be_removed#={1} to_be_added#={2}", new object[]
				{
					this.TargetInterestGroup,
					this.groupsToRemove.Count,
					this.groupsToAdd.Count
				});
				if (PunVoiceClient.Instance.Client.OpChangeGroups(array2, array))
				{
					if (this.subscribedGroups != null)
					{
						List<byte> list = new List<byte>();
						for (int i = 0; i < this.subscribedGroups.Length; i++)
						{
							list.Add(this.subscribedGroups[i]);
						}
						for (int j = 0; j < this.groupsToRemove.Count; j++)
						{
							if (list.Contains(this.groupsToRemove[j]))
							{
								list.Remove(this.groupsToRemove[j]);
							}
						}
						for (int k = 0; k < this.groupsToAdd.Count; k++)
						{
							if (!list.Contains(this.groupsToAdd[k]))
							{
								list.Add(this.groupsToAdd[k]);
							}
						}
						this.subscribedGroups = list.ToArray();
					}
					else
					{
						this.subscribedGroups = array;
					}
					this.groupsToAdd.Clear();
					this.groupsToRemove.Clear();
				}
				else
				{
					base.Logger.Log(1, "Error changing groups", Array.Empty<object>());
				}
			}
			this.ToggleTransmission();
		}
	}

	// Token: 0x06001326 RID: 4902 RVA: 0x00061478 File Offset: 0x0005F678
	private bool IsLocalCheck()
	{
		if (this.photonView.IsMine)
		{
			return true;
		}
		if (base.enabled)
		{
			base.Logger.Log(3, "Disabling ProximityVoiceTrigger as does not belong to local player, actor number {0}", new object[] { this.TargetInterestGroup });
			base.enabled = false;
		}
		return false;
	}

	// Token: 0x040011C3 RID: 4547
	private List<byte> groupsToAdd = new List<byte>();

	// Token: 0x040011C4 RID: 4548
	private List<byte> groupsToRemove = new List<byte>();

	// Token: 0x040011C5 RID: 4549
	[SerializeField]
	private byte[] subscribedGroups;

	// Token: 0x040011C6 RID: 4550
	private PhotonVoiceView photonVoiceView;

	// Token: 0x040011C7 RID: 4551
	private PhotonView photonView;
}
