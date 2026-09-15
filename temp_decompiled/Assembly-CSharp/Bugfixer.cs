using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class Bugfixer : MonoBehaviour
{
	// Token: 0x06001001 RID: 4097 RVA: 0x0004F769 File Offset: 0x0004D969
	private void Start()
	{
	}

	// Token: 0x06001002 RID: 4098 RVA: 0x0004F76C File Offset: 0x0004D96C
	private void Update()
	{
		if (Input.GetKey(304) && Input.GetKeyDown(46))
		{
			Character target = this.GetTarget();
			if (target != null)
			{
				PhotonNetwork.Instantiate("BugfixOnYou", Vector3.zero, Quaternion.identity, 0, null).GetComponent<PhotonView>().RPC("AttachBug", 0, new object[] { target.photonView.ViewID });
			}
		}
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x0004F7E0 File Offset: 0x0004D9E0
	private Character GetTarget()
	{
		if (this.useLocalCharacter)
		{
			return Character.localCharacter;
		}
		Character character = null;
		float num = float.MaxValue;
		foreach (Character character2 in Character.AllCharacters)
		{
			float num2 = Vector3.Angle(MainCamera.instance.transform.forward, character2.Center - MainCamera.instance.transform.position);
			if (num2 < num)
			{
				num = num2;
				character = character2;
			}
		}
		return character;
	}

	// Token: 0x04000E68 RID: 3688
	public bool useLocalCharacter;
}
