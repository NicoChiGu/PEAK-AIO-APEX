using System;
using UnityEngine;

// Token: 0x020002A0 RID: 672
public class MirrorScript : MonoBehaviour
{
	// Token: 0x04001114 RID: 4372
	[Tooltip("Maximum number of per pixel lights that will show in the mirrored image")]
	public int MaximumPerPixelLights = 2;

	// Token: 0x04001115 RID: 4373
	[Tooltip("Texture size for the mirror, depending on how close the player can get to the mirror, this will need to be larger")]
	public int TextureSize = 768;

	// Token: 0x04001116 RID: 4374
	[Tooltip("Subtracted from the near plane of the mirror")]
	public float ClipPlaneOffset = 0.07f;

	// Token: 0x04001117 RID: 4375
	[Tooltip("Far clip plane for mirro camera")]
	public float FarClipPlane = 1000f;

	// Token: 0x04001118 RID: 4376
	[Tooltip("What layers will be reflected?")]
	public LayerMask ReflectLayers = -1;

	// Token: 0x04001119 RID: 4377
	[Tooltip("Add a flare layer to the reflection camera?")]
	public bool AddFlareLayer;

	// Token: 0x0400111A RID: 4378
	[Tooltip("For quads, the normal points forward (true). For planes, the normal points up (false)")]
	public bool NormalIsForward = true;

	// Token: 0x0400111B RID: 4379
	[Tooltip("Aspect ratio (width / height). Set to 0 to use default.")]
	public float AspectRatio;

	// Token: 0x0400111C RID: 4380
	[Tooltip("Set to true if you have multiple mirrors facing each other to get an infinite effect, otherwise leave as false for a more realistic mirror effect.")]
	public bool MirrorRecursion;
}
