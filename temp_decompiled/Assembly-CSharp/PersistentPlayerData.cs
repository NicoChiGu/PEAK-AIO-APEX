using System;
using Zorro.Core.Serizalization;

// Token: 0x0200008A RID: 138
public class PersistentPlayerData : IBinarySerializable
{
	// Token: 0x0600058D RID: 1421 RVA: 0x0001FC55 File Offset: 0x0001DE55
	public void Serialize(BinarySerializer serializer)
	{
		this.customizationData.Serialize(serializer);
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x0001FC63 File Offset: 0x0001DE63
	public void Deserialize(BinaryDeserializer deserializer)
	{
		this.customizationData = IBinarySerializable.DeserializeClass<CharacterCustomizationData>(deserializer);
	}

	// Token: 0x040005B3 RID: 1459
	public CharacterCustomizationData customizationData = new CharacterCustomizationData();
}
