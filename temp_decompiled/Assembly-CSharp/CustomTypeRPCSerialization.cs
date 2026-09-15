using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using Unity.Collections;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x02000147 RID: 327
public static class CustomTypeRPCSerialization
{
	// Token: 0x06000A8E RID: 2702 RVA: 0x000387EC File Offset: 0x000369EC
	public static void Initialize()
	{
		PhotonPeer.RegisterType(typeof(PhotonView), byte.MaxValue, new SerializeMethod(CustomTypeRPCSerialization.SerializePhotonView), new DeserializeMethod(CustomTypeRPCSerialization.DeserializePhotonView));
		PhotonPeer.RegisterType(typeof(ItemInstanceData), 254, new SerializeMethod(CustomTypeRPCSerialization.SerializeItemData), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeItemData));
		PhotonPeer.RegisterType(typeof(BackpackReference), 253, new SerializeMethod(CustomTypeRPCSerialization.SerializeBackpackRef), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeBackpackRef));
		PhotonPeer.RegisterType(typeof(ReconnectData), 252, new SerializeMethod(CustomTypeRPCSerialization.SerializeReconnectData), new DeserializeMethod(CustomTypeRPCSerialization.DeserializeReconnectData));
	}

	// Token: 0x06000A8F RID: 2703 RVA: 0x000388AD File Offset: 0x00036AAD
	private static object DeserializeReconnectData(byte[] buffer)
	{
		return ReconnectData.Deserialize(buffer);
	}

	// Token: 0x06000A90 RID: 2704 RVA: 0x000388BC File Offset: 0x00036ABC
	private static byte[] SerializeReconnectData(object customObject)
	{
		if (customObject is ReconnectData)
		{
			return ((ReconnectData)customObject).Serialize();
		}
		throw new Exception("Could not serialize reconnect data, type: " + customObject.GetType().Name);
	}

	// Token: 0x06000A91 RID: 2705 RVA: 0x000388FA File Offset: 0x00036AFA
	private static object DeserializeBackpackRef(byte[] serializedcustomobject)
	{
		return IBinarySerializable.GetFromManagedArray<BackpackReference>(serializedcustomobject);
	}

	// Token: 0x06000A92 RID: 2706 RVA: 0x00038907 File Offset: 0x00036B07
	private static byte[] SerializeBackpackRef(object customobject)
	{
		return IBinarySerializable.ToManagedArray<BackpackReference>((BackpackReference)customobject);
	}

	// Token: 0x06000A93 RID: 2707 RVA: 0x00038914 File Offset: 0x00036B14
	private static object DeserializeItemData(byte[] serializedcustomobject)
	{
		NativeArray<byte> nativeArray = ArrayExtensions.ToNativeArray<byte>(serializedcustomobject, 2);
		BinaryDeserializer binaryDeserializer = new BinaryDeserializer(nativeArray);
		Guid guid = binaryDeserializer.ReadGuid();
		ItemInstanceData itemInstanceData;
		if (!ItemInstanceDataHandler.TryGetInstanceData(guid, out itemInstanceData))
		{
			itemInstanceData = new ItemInstanceData(guid);
			ItemInstanceDataHandler.AddInstanceData(itemInstanceData);
		}
		itemInstanceData.Deserialize(binaryDeserializer);
		nativeArray.Dispose();
		return itemInstanceData;
	}

	// Token: 0x06000A94 RID: 2708 RVA: 0x00038960 File Offset: 0x00036B60
	private static byte[] SerializeItemData(object d)
	{
		ItemInstanceData itemInstanceData = (ItemInstanceData)d;
		BinarySerializer binarySerializer = new BinarySerializer(24, 2);
		binarySerializer.WriteGuid(itemInstanceData.guid);
		itemInstanceData.Serialize(binarySerializer);
		byte[] array = NativeArrayExtensions.ToByteArray(binarySerializer.buffer);
		binarySerializer.Dispose();
		return array;
	}

	// Token: 0x06000A95 RID: 2709 RVA: 0x000389A1 File Offset: 0x00036BA1
	public static object DeserializePhotonView(byte[] data)
	{
		return PhotonView.Find(BitConverter.ToInt32(data));
	}

	// Token: 0x06000A96 RID: 2710 RVA: 0x000389B3 File Offset: 0x00036BB3
	public static byte[] SerializePhotonView(object customType)
	{
		return BitConverter.GetBytes(((PhotonView)customType).ViewID);
	}
}
