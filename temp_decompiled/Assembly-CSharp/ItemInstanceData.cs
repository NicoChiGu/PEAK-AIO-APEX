using System;
using System.Collections.Generic;
using System.Linq;
using Zorro.Core.Serizalization;

// Token: 0x020000CE RID: 206
public class ItemInstanceData : IBinarySerializable
{
	// Token: 0x060007E3 RID: 2019 RVA: 0x0002C64D File Offset: 0x0002A84D
	public ItemInstanceData()
	{
	}

	// Token: 0x060007E4 RID: 2020 RVA: 0x0002C660 File Offset: 0x0002A860
	public ItemInstanceData(Guid guid)
	{
		this.guid = guid;
	}

	// Token: 0x060007E5 RID: 2021 RVA: 0x0002C67C File Offset: 0x0002A87C
	public void Serialize(BinarySerializer serializer)
	{
		List<KeyValuePair<DataEntryKey, DataEntryValue>> list = Enumerable.ToList<KeyValuePair<DataEntryKey, DataEntryValue>>(this.data);
		byte b = (byte)list.Count;
		serializer.WriteByte(b);
		foreach (KeyValuePair<DataEntryKey, DataEntryValue> keyValuePair in list)
		{
			DataEntryKey key = keyValuePair.Key;
			DataEntryValue value = keyValuePair.Value;
			serializer.WriteByte((byte)key);
			serializer.WriteByte(DataEntryValue.GetTypeValue(value.GetType()));
			value.Serialize(serializer);
		}
	}

	// Token: 0x060007E6 RID: 2022 RVA: 0x0002C710 File Offset: 0x0002A910
	public void Deserialize(BinaryDeserializer deserializer)
	{
		byte b = deserializer.ReadByte();
		this.data = new Dictionary<DataEntryKey, DataEntryValue>((int)b);
		for (int i = 0; i < (int)b; i++)
		{
			DataEntryKey dataEntryKey = (DataEntryKey)deserializer.ReadByte();
			DataEntryValue newFromValue = DataEntryValue.GetNewFromValue(deserializer.ReadByte());
			newFromValue.Init();
			newFromValue.Deserialize(deserializer);
			this.data.Add(dataEntryKey, newFromValue);
		}
	}

	// Token: 0x060007E7 RID: 2023 RVA: 0x0002C769 File Offset: 0x0002A969
	public bool HasData(DataEntryKey key)
	{
		return this.data.ContainsKey(key);
	}

	// Token: 0x060007E8 RID: 2024 RVA: 0x0002C778 File Offset: 0x0002A978
	public bool TryGetDataEntry<T>(DataEntryKey key, out T value) where T : DataEntryValue
	{
		DataEntryValue dataEntryValue;
		bool flag = this.data.TryGetValue(key, ref dataEntryValue);
		if (flag)
		{
			value = (T)((object)dataEntryValue);
			return flag;
		}
		value = default(T);
		return flag;
	}

	// Token: 0x060007E9 RID: 2025 RVA: 0x0002C7AC File Offset: 0x0002A9AC
	public T RegisterNewEntry<T>(DataEntryKey key) where T : DataEntryValue, new()
	{
		T t = new T();
		t.Init();
		this.data.Add(key, t);
		return t;
	}

	// Token: 0x060007EA RID: 2026 RVA: 0x0002C7DD File Offset: 0x0002A9DD
	public T RegisterEntry<T>(DataEntryKey key, T value) where T : DataEntryValue, new()
	{
		value.Init();
		this.data.Add(key, value);
		return value;
	}

	// Token: 0x040007AF RID: 1967
	public Guid guid;

	// Token: 0x040007B0 RID: 1968
	public Dictionary<DataEntryKey, DataEntryValue> data = new Dictionary<DataEntryKey, DataEntryValue>();
}
