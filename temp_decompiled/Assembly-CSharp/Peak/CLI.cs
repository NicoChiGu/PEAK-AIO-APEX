using System;
using System.Collections.Generic;
using UnityEngine;

namespace Peak
{
	// Token: 0x020003B3 RID: 947
	public static class CLI
	{
		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06001863 RID: 6243 RVA: 0x0007C021 File Offset: 0x0007A221
		private static Dictionary<string, string> ParsedArgs
		{
			get
			{
				if (CLI._cmdLineArgs == null)
				{
					CLI._cmdLineArgs = CLI.ParseCommandLineArgs();
				}
				return CLI._cmdLineArgs;
			}
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06001864 RID: 6244 RVA: 0x0007C03C File Offset: 0x0007A23C
		public static string ForceVersionArg
		{
			get
			{
				string text;
				if (CLI.TryGetArg("forceVersion", out text))
				{
					return text;
				}
				return string.Empty;
			}
		}

		// Token: 0x06001865 RID: 6245 RVA: 0x0007C060 File Offset: 0x0007A260
		private static Dictionary<string, string> ParseCommandLineArgs()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				string text = commandLineArgs[i];
				if (text.StartsWith('-'))
				{
					string text2 = text;
					text = text2.Substring(1, text2.Length - 1);
					if (i + 1 < commandLineArgs.Length)
					{
						string text3 = commandLineArgs[i + 1];
						if (!text3.StartsWith('-'))
						{
							dictionary[text] = text3;
						}
						else
						{
							dictionary[text] = "";
						}
					}
					else
					{
						dictionary[text] = "";
					}
				}
			}
			return dictionary;
		}

		// Token: 0x06001866 RID: 6246 RVA: 0x0007C0EA File Offset: 0x0007A2EA
		public static bool TryGetArg(string arg, out string result)
		{
			return CLI.ParsedArgs.TryGetValue(arg, ref result);
		}

		// Token: 0x06001867 RID: 6247 RVA: 0x0007C0F8 File Offset: 0x0007A2F8
		public static bool TryGetArg(string arg, out bool result)
		{
			result = false;
			string text;
			return CLI.TryGetArg(arg, out text) && bool.TryParse(text, ref result);
		}

		// Token: 0x06001868 RID: 6248 RVA: 0x0007C11C File Offset: 0x0007A31C
		public static bool HasArg(string arg)
		{
			string text;
			bool flag = CLI.ParsedArgs.TryGetValue(arg, ref text);
			if (text != string.Empty)
			{
				Debug.LogWarning(string.Concat(new string[] { "Arg ", arg, " has a value (", text, ") which we're ignoring." }));
			}
			return flag;
		}

		// Token: 0x04001696 RID: 5782
		private static Dictionary<string, string> _cmdLineArgs;

		// Token: 0x02000538 RID: 1336
		public static class Args
		{
			// Token: 0x04001BF5 RID: 7157
			public const string ForceVersion = "forceVersion";

			// Token: 0x04001BF6 RID: 7158
			public const string BuildPath = "buildPath";

			// Token: 0x04001BF7 RID: 7159
			public const string CleanBuild = "cleanBuild";

			// Token: 0x04001BF8 RID: 7160
			public const string GPUCulling = "gpuCulling";
		}
	}
}
