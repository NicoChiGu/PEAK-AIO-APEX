using System;
using UnityEngine;
using UnityEngine.Serialization;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000078 RID: 120
[Serializable]
public class CharacterCustomizationData : IBinarySerializable
{
	// Token: 0x06000548 RID: 1352 RVA: 0x0001F1E0 File Offset: 0x0001D3E0
	public void Serialize(BinarySerializer serializer)
	{
		serializer.WriteInt(this.currentSkin);
		serializer.WriteInt(this.currentAccessory);
		serializer.WriteInt(this.currentEyes);
		serializer.WriteInt(this.currentMouth);
		serializer.WriteInt(this.currentOutfit);
		serializer.WriteInt(this.currentHat);
		serializer.WriteInt(this.currentSash);
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x0001F244 File Offset: 0x0001D444
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.currentSkin = deserializer.ReadInt();
		this.currentAccessory = deserializer.ReadInt();
		this.currentEyes = deserializer.ReadInt();
		this.currentMouth = deserializer.ReadInt();
		this.currentOutfit = deserializer.ReadInt();
		this.currentHat = deserializer.ReadInt();
		this.currentSash = deserializer.ReadInt();
		this.CorrectValues();
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0001F2AC File Offset: 0x0001D4AC
	public void CorrectValues()
	{
		if (Singleton<Customization>.Instance)
		{
			if (this.currentSkin >= Singleton<Customization>.Instance.skins.Length)
			{
				this.currentSkin = 0;
			}
			if (this.currentEyes >= Singleton<Customization>.Instance.eyes.Length)
			{
				this.currentEyes = 0;
			}
			if (this.currentMouth >= Singleton<Customization>.Instance.mouths.Length)
			{
				this.currentMouth = 0;
			}
			if (this.currentAccessory >= Singleton<Customization>.Instance.accessories.Length)
			{
				this.currentAccessory = 0;
			}
			if (this.currentOutfit >= Singleton<Customization>.Instance.fits.Length)
			{
				this.currentOutfit = 0;
			}
			if (this.currentHat >= Singleton<Customization>.Instance.hats.Length)
			{
				this.currentHat = 0;
			}
			if (this.currentSash >= Singleton<Customization>.Instance.sashes.Length)
			{
				this.currentSash = Singleton<Customization>.Instance.sashes.Length - 1;
			}
		}
	}

	// Token: 0x04000590 RID: 1424
	[SerializeField]
	public int currentSkin;

	// Token: 0x04000591 RID: 1425
	[SerializeField]
	public int currentAccessory;

	// Token: 0x04000592 RID: 1426
	[SerializeField]
	public int currentEyes;

	// Token: 0x04000593 RID: 1427
	[SerializeField]
	public int currentMouth;

	// Token: 0x04000594 RID: 1428
	[FormerlySerializedAs("currentFit")]
	[SerializeField]
	public int currentOutfit;

	// Token: 0x04000595 RID: 1429
	[SerializeField]
	public int currentHat;

	// Token: 0x04000596 RID: 1430
	[SerializeField]
	public int currentSash;
}
