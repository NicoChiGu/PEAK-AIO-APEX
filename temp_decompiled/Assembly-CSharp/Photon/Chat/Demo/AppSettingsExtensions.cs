using System;
using Photon.Realtime;

namespace Photon.Chat.Demo
{
	// Token: 0x02000398 RID: 920
	public static class AppSettingsExtensions
	{
		// Token: 0x060017F0 RID: 6128 RVA: 0x000793A4 File Offset: 0x000775A4
		public static ChatAppSettings GetChatSettings(this AppSettings appSettings)
		{
			return new ChatAppSettings
			{
				AppIdChat = appSettings.AppIdChat,
				AppVersion = appSettings.AppVersion,
				FixedRegion = (appSettings.IsBestRegion ? null : appSettings.FixedRegion),
				NetworkLogging = appSettings.NetworkLogging,
				Protocol = appSettings.Protocol,
				EnableProtocolFallback = appSettings.EnableProtocolFallback,
				Server = (appSettings.IsDefaultNameServer ? null : appSettings.Server),
				Port = (ushort)appSettings.Port,
				ProxyServer = appSettings.ProxyServer
			};
		}
	}
}
