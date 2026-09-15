using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;
using Zorro.Core.Editor;

// Token: 0x02000137 RID: 311
[CreateAssetMenu(menuName = "Peak/MapBaker")]
public class MapBaker : SingletonAsset<MapBaker>
{
	// Token: 0x1700009E RID: 158
	// (get) Token: 0x060009E2 RID: 2530 RVA: 0x0003486F File Offset: 0x00032A6F
	[Obsolete("Renamed to ScenePaths.")]
	public string[] AllLevels
	{
		get
		{
			return this.ScenePaths;
		}
	}

	// Token: 0x1700009F RID: 159
	// (get) Token: 0x060009E3 RID: 2531 RVA: 0x00034877 File Offset: 0x00032A77
	private static bool IsMidBuild
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060009E4 RID: 2532 RVA: 0x0003487A File Offset: 0x00032A7A
	public string GetBiomeID(int levelIndex)
	{
		if (this.BiomeIDs.Count == 0)
		{
			return "";
		}
		levelIndex %= this.BiomeIDs.Count;
		return this.BiomeIDs[levelIndex];
	}

	// Token: 0x060009E5 RID: 2533 RVA: 0x000348AA File Offset: 0x00032AAA
	public string GetLevel(int levelIndex)
	{
		if (this.ScenePaths.Length == 0)
		{
			Debug.LogError("No levels found, using WilIsland...");
			return "";
		}
		levelIndex %= this.ScenePaths.Length;
		string text = PathUtil.WithoutExtensions(PathUtil.GetFileName(this.ScenePaths[levelIndex]));
		bool isEditor = Application.isEditor;
		return text;
	}

	// Token: 0x04000954 RID: 2388
	public string[] ScenePaths;

	// Token: 0x04000955 RID: 2389
	public List<string> BiomeIDs;
}
