using System;
using Zorro.Core;

// Token: 0x02000079 RID: 121
public class ConnectionService : GameService
{
	// Token: 0x0600054C RID: 1356 RVA: 0x0001F39C File Offset: 0x0001D59C
	public ConnectionService()
	{
		this.StateMachine = new ConnectionService.ConnectionServiceStateMachine();
		this.StateMachine.RegisterState(new DefaultConnectionState());
		this.StateMachine.RegisterState(new JoinSpecificRoomState());
		this.StateMachine.RegisterState(new InRoomState());
		this.StateMachine.RegisterState(new HostState());
		this.StateMachine.RegisterState(new KickedState());
		this.StateMachine.SwitchState<DefaultConnectionState>(false);
	}

	// Token: 0x04000597 RID: 1431
	public ConnectionService.ConnectionServiceStateMachine StateMachine;

	// Token: 0x0200041C RID: 1052
	public class ConnectionServiceStateMachine : StateMachine<ConnectionState>
	{
	}
}
