using System;
using UnityEngine;

// Token: 0x0200030D RID: 781
public class RemoveMusic : MonoBehaviour
{
	// Token: 0x06001435 RID: 5173 RVA: 0x000665DE File Offset: 0x000647DE
	private void Start()
	{
		this.musics = GameObject.FindGameObjectsWithTag("Music");
	}

	// Token: 0x06001436 RID: 5174 RVA: 0x000665F0 File Offset: 0x000647F0
	private void Update()
	{
		for (int i = 0; i < this.musics.Length; i++)
		{
			if (this.musics[i] != null)
			{
				this.musics[i].GetComponent<AudioSource>().volume /= 1.01f;
			}
		}
	}

	// Token: 0x040012B8 RID: 4792
	public GameObject[] musics;
}
