using System;
using pworld.Scripts.Extensions;
using pworld.Scripts.PPhys;
using UnityEngine;

namespace Knot
{
	// Token: 0x020003A6 RID: 934
	public class KnotTieText : MonoBehaviour
	{
		// Token: 0x0600184E RID: 6222 RVA: 0x0007B1FC File Offset: 0x000793FC
		private void Awake()
		{
			this.spring = base.GetComponent<PPhysSpringBase>();
		}

		// Token: 0x0600184F RID: 6223 RVA: 0x0007B20A File Offset: 0x0007940A
		private void Start()
		{
		}

		// Token: 0x06001850 RID: 6224 RVA: 0x0007B20C File Offset: 0x0007940C
		private void Update()
		{
			this.timeAlive += Time.deltaTime;
			if (this.timeAlive > this.lifeTime)
			{
				Object.Destroy(base.gameObject);
			}
			if (this.timeAlive > this.lifeTime - 1f)
			{
				this.spring.Target = ExtDataTypes.ToVec(0);
			}
			base.transform.position += Vector3.up * (Time.deltaTime * this.velocity);
		}

		// Token: 0x04001681 RID: 5761
		public float velocity;

		// Token: 0x04001682 RID: 5762
		public PPhysSpringBase spring;

		// Token: 0x04001683 RID: 5763
		public float lifeTime;

		// Token: 0x04001684 RID: 5764
		private float timeAlive;
	}
}
