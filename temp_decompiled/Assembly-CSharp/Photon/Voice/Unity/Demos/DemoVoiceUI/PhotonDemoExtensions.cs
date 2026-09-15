using System;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Voice.Unity.Demos.DemoVoiceUI
{
	// Token: 0x02000395 RID: 917
	public static class PhotonDemoExtensions
	{
		// Token: 0x060017B7 RID: 6071 RVA: 0x00078AEE File Offset: 0x00076CEE
		public static bool Mute(this Player player)
		{
			Hashtable hashtable = new Hashtable(1);
			hashtable.Add("mu", true);
			return player.SetCustomProperties(hashtable, null, null);
		}

		// Token: 0x060017B8 RID: 6072 RVA: 0x00078B0F File Offset: 0x00076D0F
		public static bool Unmute(this Player player)
		{
			Hashtable hashtable = new Hashtable(1);
			hashtable.Add("mu", false);
			return player.SetCustomProperties(hashtable, null, null);
		}

		// Token: 0x060017B9 RID: 6073 RVA: 0x00078B30 File Offset: 0x00076D30
		public static bool IsMuted(this Player player)
		{
			return player.HasBoolProperty("mu");
		}

		// Token: 0x060017BA RID: 6074 RVA: 0x00078B3D File Offset: 0x00076D3D
		public static bool SetPhotonVAD(this Player player, bool value)
		{
			Hashtable hashtable = new Hashtable(1);
			hashtable.Add("pv", value);
			return player.SetCustomProperties(hashtable, null, null);
		}

		// Token: 0x060017BB RID: 6075 RVA: 0x00078B5E File Offset: 0x00076D5E
		public static bool SetWebRTCVAD(this Player player, bool value)
		{
			Hashtable hashtable = new Hashtable(1);
			hashtable.Add("wv", value);
			return player.SetCustomProperties(hashtable, null, null);
		}

		// Token: 0x060017BC RID: 6076 RVA: 0x00078B7F File Offset: 0x00076D7F
		public static bool SetAEC(this Player player, bool value)
		{
			Hashtable hashtable = new Hashtable(1);
			hashtable.Add("ec", value);
			return player.SetCustomProperties(hashtable, null, null);
		}

		// Token: 0x060017BD RID: 6077 RVA: 0x00078BA0 File Offset: 0x00076DA0
		public static bool SetAGC(this Player player, bool agcEnabled, int gain, int level)
		{
			Hashtable hashtable = new Hashtable(1);
			hashtable.Add("gc", new object[] { agcEnabled, gain, level });
			return player.SetCustomProperties(hashtable, null, null);
		}

		// Token: 0x060017BE RID: 6078 RVA: 0x00078BE9 File Offset: 0x00076DE9
		public static bool SetMic(this Player player, Recorder.MicType type)
		{
			Hashtable hashtable = new Hashtable(1);
			hashtable.Add("m", type);
			return player.SetCustomProperties(hashtable, null, null);
		}

		// Token: 0x060017BF RID: 6079 RVA: 0x00078C0A File Offset: 0x00076E0A
		public static bool HasPhotonVAD(this Player player)
		{
			return player.HasBoolProperty("pv");
		}

		// Token: 0x060017C0 RID: 6080 RVA: 0x00078C17 File Offset: 0x00076E17
		public static bool HasWebRTCVAD(this Player player)
		{
			return player.HasBoolProperty("wv");
		}

		// Token: 0x060017C1 RID: 6081 RVA: 0x00078C24 File Offset: 0x00076E24
		public static bool HasAEC(this Player player)
		{
			return player.HasBoolProperty("ec");
		}

		// Token: 0x060017C2 RID: 6082 RVA: 0x00078C34 File Offset: 0x00076E34
		public static bool HasAGC(this Player player)
		{
			object[] array = player.GetObjectProperty("gc") as object[];
			return array != null && array.Length != 0 && (bool)array[0];
		}

		// Token: 0x060017C3 RID: 6083 RVA: 0x00078C64 File Offset: 0x00076E64
		public static int GetAGCGain(this Player player)
		{
			object[] array = player.GetObjectProperty("gc") as object[];
			if (array == null || array.Length <= 1)
			{
				return 0;
			}
			return (int)array[1];
		}

		// Token: 0x060017C4 RID: 6084 RVA: 0x00078C98 File Offset: 0x00076E98
		public static int GetAGCLevel(this Player player)
		{
			object[] array = player.GetObjectProperty("gc") as object[];
			if (array == null || array.Length <= 2)
			{
				return 0;
			}
			return (int)array[2];
		}

		// Token: 0x060017C5 RID: 6085 RVA: 0x00078CCC File Offset: 0x00076ECC
		public static Recorder.MicType? GetMic(this Player player)
		{
			Recorder.MicType? micType = default(Recorder.MicType?);
			try
			{
				micType = new Recorder.MicType?((Recorder.MicType)player.GetObjectProperty("m"));
			}
			catch
			{
				micType = default(Recorder.MicType?);
			}
			return micType;
		}

		// Token: 0x060017C6 RID: 6086 RVA: 0x00078D18 File Offset: 0x00076F18
		private static bool HasBoolProperty(this Player player, string prop)
		{
			object obj;
			return player.CustomProperties.TryGetValue(prop, ref obj) && (bool)obj;
		}

		// Token: 0x060017C7 RID: 6087 RVA: 0x00078D40 File Offset: 0x00076F40
		private static int? GetIntProperty(this Player player, string prop)
		{
			object obj;
			if (player.CustomProperties.TryGetValue(prop, ref obj))
			{
				return new int?((int)obj);
			}
			return default(int?);
		}

		// Token: 0x060017C8 RID: 6088 RVA: 0x00078D74 File Offset: 0x00076F74
		private static object GetObjectProperty(this Player player, string prop)
		{
			object obj;
			if (player.CustomProperties.TryGetValue(prop, ref obj))
			{
				return obj;
			}
			return null;
		}

		// Token: 0x04001620 RID: 5664
		internal const string MUTED_KEY = "mu";

		// Token: 0x04001621 RID: 5665
		internal const string PHOTON_VAD_KEY = "pv";

		// Token: 0x04001622 RID: 5666
		internal const string WEBRTC_AEC_KEY = "ec";

		// Token: 0x04001623 RID: 5667
		internal const string WEBRTC_VAD_KEY = "wv";

		// Token: 0x04001624 RID: 5668
		internal const string WEBRTC_AGC_KEY = "gc";

		// Token: 0x04001625 RID: 5669
		internal const string MIC_KEY = "m";
	}
}
