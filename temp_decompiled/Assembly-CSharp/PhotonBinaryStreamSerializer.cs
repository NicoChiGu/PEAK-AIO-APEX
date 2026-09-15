using System;
using Photon.Pun;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Serizalization;

// Token: 0x0200014B RID: 331
public abstract class PhotonBinaryStreamSerializer<T> : MonoBehaviourPunCallbacks, IPunObservable where T : struct, IBinarySerializable
{
	// Token: 0x06000AB2 RID: 2738
	public abstract T GetDataToWrite();

	// Token: 0x06000AB3 RID: 2739 RVA: 0x00039504 File Offset: 0x00037704
	protected virtual void Awake()
	{
		this.photonView = base.GetComponent<PhotonView>();
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x00039514 File Offset: 0x00037714
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			if (this.ShouldSendData())
			{
				if (IBinarySerializable.shouldLog)
				{
					Debug.Log(base.gameObject.name + " sending data in type " + base.GetType().Name);
				}
				T dataToWrite = this.GetDataToWrite();
				BinarySerializer binarySerializer = new BinarySerializer(UnsafeUtility.SizeOf<T>(), 2);
				dataToWrite.Serialize(binarySerializer);
				byte[] array = NativeArrayExtensions.ToByteArray(binarySerializer.buffer);
				NetworkStats.RegisterBytesSent<T>((ulong)((long)array.Length));
				stream.SendNext(array);
				binarySerializer.Dispose();
				return;
			}
		}
		else
		{
			if (IBinarySerializable.shouldLog)
			{
				Debug.Log(base.gameObject.name + " received data in type " + base.GetType().Name);
			}
			BinaryDeserializer binaryDeserializer = new BinaryDeserializer((byte[])stream.ReceiveNext(), 2);
			T t = new T();
			t.Deserialize(binaryDeserializer);
			binaryDeserializer.Dispose();
			this.RemoteValue = Optionable<T>.Some(t);
			this.OnDataReceived(t);
		}
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x00039612 File Offset: 0x00037812
	public virtual void OnDataReceived(T data)
	{
		this.sinceLastPackage = 0f;
	}

	// Token: 0x06000AB6 RID: 2742 RVA: 0x0003961F File Offset: 0x0003781F
	public virtual bool ShouldSendData()
	{
		return true;
	}

	// Token: 0x040009FC RID: 2556
	protected Optionable<T> RemoteValue;

	// Token: 0x040009FD RID: 2557
	protected float sinceLastPackage;

	// Token: 0x040009FE RID: 2558
	protected PhotonView photonView;
}
