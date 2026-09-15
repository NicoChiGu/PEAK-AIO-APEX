using System;
using Zorro.PhotonUtility;

// Token: 0x02000087 RID: 135
public class InRoomState : ConnectionState
{
	// Token: 0x06000589 RID: 1417 RVA: 0x0001FBE8 File Offset: 0x0001DDE8
	public override void Enter()
	{
		base.Enter();
		this.verifiedLobby = null;
		this.hasLoadedCustomization = false;
		CommandListener commandListener = CustomCommands<CustomCommandType>.SpawnCommandListener<CommandListener>();
		commandListener.RegisterPackage<SyncPersistentPlayerDataPackage>(new SyncPersistentPlayerDataPackage());
		commandListener.RegisterPackage<SyncMapHandlerDebugCommandPackage>(new SyncMapHandlerDebugCommandPackage());
		commandListener.RegisterPackage<SyncLavaRisingPackage>(new SyncLavaRisingPackage());
		GameHandler.ClearAllStatuses();
		GameHandler.GetService<RichPresenceService>().Dirty();
	}

	// Token: 0x040005AF RID: 1455
	public bool hasLoadedCustomization;

	// Token: 0x040005B0 RID: 1456
	public string verifiedLobby;
}
