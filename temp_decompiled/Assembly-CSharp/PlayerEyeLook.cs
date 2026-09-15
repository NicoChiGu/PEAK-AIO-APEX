using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200015A RID: 346
public class PlayerEyeLook : MonoBehaviour
{
	// Token: 0x06000B18 RID: 2840 RVA: 0x0003B187 File Offset: 0x00039387
	private void Start()
	{
		this.localCharacter = base.GetComponent<Character>();
	}

	// Token: 0x06000B19 RID: 2841 RVA: 0x0003B198 File Offset: 0x00039398
	private void Update()
	{
		this.characters = Character.AllCharacters;
		this.distance = float.PositiveInfinity;
		for (int i = 0; i < this.characters.Count; i++)
		{
			float num = Vector3.Distance(this.characters[i].Center, this.localCharacter.Center);
			if (num < this.distance && this.characters[i] != this.localCharacter)
			{
				this.distance = num;
				this.character = this.characters[i];
			}
			AnimatedMouth component = this.characters[i].GetComponent<AnimatedMouth>();
			if (num < this.listenRange && component.isSpeaking && this.characters[i] != this.localCharacter)
			{
				this.distance = num;
				this.character = this.characters[i];
			}
		}
		if (this.character != null)
		{
			this.lookDir = (this.character.Head - this.localCharacter.Head).normalized;
			this.lookDelta = this.localCharacter.GetBodypart(BodypartType.Head).transform.forward - this.lookDir;
			base.transform.InverseTransformDirection(this.lookDelta);
			this.UpDelta = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.up, this.lookDelta);
			this.RightDelta = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.right, this.lookDelta);
			this.lookAngle = Vector3.Angle(this.localCharacter.data.lookDirection, this.lookDir);
		}
		if (this.character != null && this.distance < this.lookRange && this.lookAngle < this.lookAngleMax)
		{
			this.eyeTarget = new Vector2(this.RightDelta * -this.XMax, this.UpDelta * this.YMax);
			this.lookingAtCharacter = true;
		}
		else
		{
			this.lookingAtCharacter = false;
			Vector3 forward = this.localCharacter.GetBodypart(BodypartType.Hip).transform.forward;
			forward.y = 0f;
			Vector3 vector = this.localCharacter.data.lookDirection - forward;
			float num2 = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.right, vector);
			float num3 = Vector3.Dot(this.localCharacter.GetBodypart(BodypartType.Head).transform.up, vector);
			this.eyeTarget = new Vector2(num2 * this.XMax, num3 * -this.YMax);
		}
		float num4 = 1f;
		if (this.character != this.lastCharacter)
		{
			num4 = 0.3f;
		}
		this.eyePos = Vector2.Lerp(this.eyePos, this.eyeTarget, Time.deltaTime * this.lookSmoothing * num4);
		for (int j = 0; j < this.eyeRenderers.Length; j++)
		{
			this.eyeRenderers[j].material.SetVector("_EyePosition", this.eyePos);
		}
		if (Vector3.Distance(this.lastViewDir, this.localCharacter.GetBodypart(BodypartType.Head).transform.forward) > this.xLookThreshold)
		{
			this.lastViewDir = this.localCharacter.GetBodypart(BodypartType.Head).transform.forward;
		}
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x0003B52C File Offset: 0x0003972C
	private void OnDrawGizmosSelected()
	{
		if (this.localCharacter == null)
		{
			return;
		}
		if (this.lookingAtCharacter)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(this.localCharacter.Head, this.lookDir * this.lookRange);
		}
		else
		{
			Gizmos.color = Color.yellow;
			Vector3 forward = this.localCharacter.GetBodypart(BodypartType.Head).transform.forward;
			forward.y = 0f;
			Gizmos.DrawRay(this.localCharacter.Head, forward * this.lookRange);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawRay(this.localCharacter.Head, this.localCharacter.GetBodypart(BodypartType.Head).transform.forward * this.lookRange);
	}

	// Token: 0x04000A50 RID: 2640
	public bool lookingAtCharacter;

	// Token: 0x04000A51 RID: 2641
	private List<Character> characters = new List<Character>();

	// Token: 0x04000A52 RID: 2642
	public float distance;

	// Token: 0x04000A53 RID: 2643
	public float lookRange;

	// Token: 0x04000A54 RID: 2644
	public float listenRange;

	// Token: 0x04000A55 RID: 2645
	private Character lastCharacter;

	// Token: 0x04000A56 RID: 2646
	public float lookSmoothing;

	// Token: 0x04000A57 RID: 2647
	public Character character;

	// Token: 0x04000A58 RID: 2648
	public Renderer[] eyeRenderers;

	// Token: 0x04000A59 RID: 2649
	private Vector3 lookDir;

	// Token: 0x04000A5A RID: 2650
	public float lookAngleMax;

	// Token: 0x04000A5B RID: 2651
	private Vector3 lookDelta;

	// Token: 0x04000A5C RID: 2652
	private float RightDelta;

	// Token: 0x04000A5D RID: 2653
	private float UpDelta;

	// Token: 0x04000A5E RID: 2654
	public float lookAngle;

	// Token: 0x04000A5F RID: 2655
	public float xLookThreshold;

	// Token: 0x04000A60 RID: 2656
	[FormerlySerializedAs("leftRightMax")]
	public float XMax;

	// Token: 0x04000A61 RID: 2657
	[FormerlySerializedAs("upDownMax")]
	public float YMax;

	// Token: 0x04000A62 RID: 2658
	private Character localCharacter;

	// Token: 0x04000A63 RID: 2659
	private Vector2 eyePos = Vector2.zero;

	// Token: 0x04000A64 RID: 2660
	private Vector2 eyeTarget = Vector2.zero;

	// Token: 0x04000A65 RID: 2661
	private Vector3 lastViewDir;
}
