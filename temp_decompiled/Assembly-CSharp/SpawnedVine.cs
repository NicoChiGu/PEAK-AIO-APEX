using System;
using System.Collections;
using System.Runtime.CompilerServices;
using pworld.Scripts.Extensions;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class SpawnedVine : MonoBehaviour
{
	// Token: 0x06000D4C RID: 3404 RVA: 0x00042C8D File Offset: 0x00040E8D
	private void Start()
	{
		this.vine = base.GetComponent<JungleVine>();
		this.SpawnVine();
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x00042CA4 File Offset: 0x00040EA4
	public void SpawnVine()
	{
		if (this.startObject != null)
		{
			this.startObject.transform.position = this.vine.colliderRoot.GetChild(0).transform.position;
			Vector3 position = this.vine.GetPosition(1f, 0, 0);
			position..ctor(position.x, this.startObject.transform.position.y, position.z);
			this.startObject.transform.LookAt(position);
			if (this.endObject != null)
			{
				this.endObject.transform.forward = ExtTransform.GetLastChild(this.vine.colliderRoot).transform.up;
			}
		}
		if (this.endObject != null)
		{
			this.endObject.transform.position = this.vine.GetPosition(1f, 0, 0);
		}
		base.StartCoroutine(this.<SpawnVine>g__waveFX|7_0());
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x00042DAE File Offset: 0x00040FAE
	private void Update()
	{
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x00042DD4 File Offset: 0x00040FD4
	[CompilerGenerated]
	private IEnumerator <SpawnVine>g__waveFX|7_0()
	{
		float normalizedTime = 0f;
		while (normalizedTime < 1f)
		{
			normalizedTime += Time.deltaTime / this.vineWaveDecay;
			float num = Mathf.Lerp(100f, 0f, normalizedTime);
			this.vineRenderer.material.SetFloat(SpawnedVine.JitterAmount, num);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000B83 RID: 2947
	private static readonly int JitterAmount = Shader.PropertyToID("_JitterAmount");

	// Token: 0x04000B84 RID: 2948
	private JungleVine vine;

	// Token: 0x04000B85 RID: 2949
	public MeshRenderer vineRenderer;

	// Token: 0x04000B86 RID: 2950
	public float vineWaveDecay = 0.5f;

	// Token: 0x04000B87 RID: 2951
	public GameObject startObject;

	// Token: 0x04000B88 RID: 2952
	public GameObject endObject;
}
