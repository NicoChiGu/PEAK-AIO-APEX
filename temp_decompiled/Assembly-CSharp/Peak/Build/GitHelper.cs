using System;
using System.Diagnostics;
using UnityEngine;

namespace Peak.Build
{
	// Token: 0x020003DB RID: 987
	public static class GitHelper
	{
		// Token: 0x06001934 RID: 6452 RVA: 0x0007D598 File Offset: 0x0007B798
		public static string GetCommitShortHash()
		{
			if (!Application.isEditor)
			{
				return string.Empty;
			}
			string text;
			try
			{
				Process process = Process.Start(new ProcessStartInfo
				{
					FileName = "git",
					Arguments = "log -1 --pretty=format:\"%h\"",
					UseShellExecute = false,
					RedirectStandardOutput = true
				});
				text = ((process != null) ? process.StandardOutput.ReadToEnd().Trim() : null) ?? string.Empty;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				text = string.Empty;
			}
			return text;
		}
	}
}
