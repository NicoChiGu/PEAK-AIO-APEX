using System;
using UnityEngine;

// Token: 0x0200001C RID: 28
[Serializable]
public class FeedData
{
	// Token: 0x06000237 RID: 567 RVA: 0x00010A1D File Offset: 0x0000EC1D
	public void PrintDescription()
	{
		Debug.Log(this.GetDescription());
	}

	// Token: 0x06000238 RID: 568 RVA: 0x00010A2C File Offset: 0x0000EC2C
	public string GetDescription()
	{
		Character character;
		bool characterWithPhotonID = Character.GetCharacterWithPhotonID(this.giverID, out character);
		Character character2;
		Character.GetCharacterWithPhotonID(this.receiverID, out character2);
		Item item;
		bool flag = ItemDatabase.TryGetItem(this.itemID, out item);
		string text = (characterWithPhotonID ? character.characterName : "An unknown scout");
		string text2 = (characterWithPhotonID ? character.characterName : "an unknown scout");
		string text3 = (flag ? item.GetItemName(null) : "an unknown item");
		return string.Concat(new string[] { text, " is feeding ", text2, " a ", text3, "..." });
	}

	// Token: 0x04000211 RID: 529
	public int giverID;

	// Token: 0x04000212 RID: 530
	public int receiverID;

	// Token: 0x04000213 RID: 531
	public ushort itemID;

	// Token: 0x04000214 RID: 532
	public float totalItemTime;
}
