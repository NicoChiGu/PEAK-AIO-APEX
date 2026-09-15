using System;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class MirageManager : MonoBehaviour
{
	// Token: 0x06000A21 RID: 2593 RVA: 0x00035938 File Offset: 0x00033B38
	public void sampleCamera()
	{
		this.cam.transform.position = Camera.main.transform.position;
		this.cam.transform.rotation = Camera.main.transform.rotation;
		this.cam.fieldOfView = Camera.main.fieldOfView;
		this.cam.Render();
	}

	// Token: 0x06000A22 RID: 2594 RVA: 0x000359A4 File Offset: 0x00033BA4
	private void Awake()
	{
		this.renderTexture = this.cam.targetTexture;
		this.renderTexture.width = Screen.width / this.rtDownscale;
		this.renderTexture.height = Screen.height / this.rtDownscale;
		MirageManager.instance = this;
	}

	// Token: 0x06000A23 RID: 2595 RVA: 0x000359F6 File Offset: 0x00033BF6
	private void setParticleData()
	{
	}

	// Token: 0x06000A24 RID: 2596 RVA: 0x000359F8 File Offset: 0x00033BF8
	private void Update()
	{
	}

	// Token: 0x0400097C RID: 2428
	public static MirageManager instance;

	// Token: 0x0400097D RID: 2429
	public Camera cam;

	// Token: 0x0400097E RID: 2430
	private RenderTexture renderTexture;

	// Token: 0x0400097F RID: 2431
	public int rtDownscale = 2;
}
