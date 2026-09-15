using System;
using UnityEngine;

// Token: 0x020002FE RID: 766
public interface IValidationConstraint
{
	// Token: 0x1700013A RID: 314
	// (get) Token: 0x060013EE RID: 5102
	bool Muted { get; }

	// Token: 0x060013EF RID: 5103
	bool Validate(GameObject spawnedProp, PropSpawner.SpawnData spawnData);
}
