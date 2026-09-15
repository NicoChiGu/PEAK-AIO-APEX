using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

// Token: 0x02000135 RID: 309
[ExecuteAlways]
public class LocalizedText : MonoBehaviour
{
	// Token: 0x1700009B RID: 155
	// (get) Token: 0x060009B6 RID: 2486 RVA: 0x00033CF4 File Offset: 0x00031EF4
	public static Dictionary<string, List<string>> mainTable
	{
		get
		{
			if (LocalizedText.MAIN_TABLE == null)
			{
				LocalizedText.LoadMainTable(true);
			}
			return LocalizedText.MAIN_TABLE;
		}
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x00033D08 File Offset: 0x00031F08
	public static void TryInitTables()
	{
		if (LocalizedText.MAIN_TABLE == null)
		{
			LocalizedText.LoadMainTable(true);
		}
		if (LocalizedText.DIALOGUE_TABLE == null)
		{
			LocalizedText.InitDialogueTable(false);
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x060009B8 RID: 2488 RVA: 0x00033D24 File Offset: 0x00031F24
	public static Dictionary<string, List<string>> dialogueTable
	{
		get
		{
			if (LocalizedText.DIALOGUE_TABLE == null)
			{
				LocalizedText.InitDialogueTable(false);
			}
			return LocalizedText.DIALOGUE_TABLE;
		}
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x00033D38 File Offset: 0x00031F38
	private void OnEnable()
	{
		if (string.IsNullOrEmpty(this.index))
		{
			this.index = this.row.ToString();
		}
		this.TryFindTextAsset();
		if (Application.isPlaying)
		{
			this.InitDisplayType();
		}
		this.RefreshText();
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x00033D71 File Offset: 0x00031F71
	public void DebugReload()
	{
		LocalizedText.LoadMainTable(true);
		this.OnEnable();
		LocalizedText.RefreshAllText();
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00033D84 File Offset: 0x00031F84
	public static void ReloadAll()
	{
		LocalizedText.LoadMainTable(true);
		LocalizedText.RefreshAllText();
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x00033D91 File Offset: 0x00031F91
	private void InitDisplayType()
	{
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x00033D93 File Offset: 0x00031F93
	[ContextMenu("Debug Serialization")]
	private void DebugSerialization()
	{
		LocalizedText.SerializeMainTable();
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x00033D9C File Offset: 0x00031F9C
	private static string SerializeMainTable()
	{
		string text = JsonConvert.SerializeObject(LocalizedText.mainTable);
		File.WriteAllText("Assets/Resources/Localization/SerializedTermsData.txt", text);
		return text;
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x00033DC0 File Offset: 0x00031FC0
	private static string SerializeDialogueTable()
	{
		string text = JsonConvert.SerializeObject(LocalizedText.dialogueTable);
		File.WriteAllText("Assets/Resources/Localization/SerializedDialogueData.txt", text);
		return text;
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x00033DE4 File Offset: 0x00031FE4
	public static void LoadMainTable(bool forceSerialization = true)
	{
		if (Application.isEditor && forceSerialization)
		{
			LocalizedText.MAIN_TABLE = CSVReader.SplitCsvDict((Resources.Load("Localization/Localized_Text") as TextAsset).text, 0, false);
			TextAsset textAsset = Resources.Load("Localization/Unlocalized_Text") as TextAsset;
			LocalizedText.MAIN_TABLE = Enumerable.ToDictionary<KeyValuePair<string, List<string>>, string, List<string>>(Enumerable.Concat<KeyValuePair<string, List<string>>>(LocalizedText.MAIN_TABLE, CSVReader.SplitCsvDict(textAsset.text, 0, false)), (KeyValuePair<string, List<string>> x) => x.Key, (KeyValuePair<string, List<string>> x) => x.Value);
			LocalizedText.SerializeMainTable();
		}
		else
		{
			LocalizedText.MAIN_TABLE = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>((Resources.Load("Localization/SerializedTermsData") as TextAsset).text);
		}
		if (!Application.isPlaying)
		{
			using (Dictionary<string, List<string>>.KeyCollection.Enumerator enumerator = LocalizedText.MAIN_TABLE.Keys.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string text = enumerator.Current;
					LocalizedText.lineLength = LocalizedText.MAIN_TABLE[text].Count;
				}
			}
		}
		LocalizedText.InitPlatformSpecificTables();
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x00033F18 File Offset: 0x00032118
	public static void InitDialogueTable(bool forceSerialization = false)
	{
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x00033F1C File Offset: 0x0003211C
	private static void InitPlatformSpecificTables()
	{
		LocalizedText.TABLE_PC = new Dictionary<string, List<string>>();
		LocalizedText.TABLE_XB = new Dictionary<string, List<string>>();
		LocalizedText.TABLE_SW = new Dictionary<string, List<string>>();
		LocalizedText.TABLE_PS = new Dictionary<string, List<string>>();
		List<string> list = new List<string>();
		foreach (string text in LocalizedText.MAIN_TABLE.Keys)
		{
			if (LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_PC, text, LocalizedText.MAIN_TABLE[text], "_PC") || LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_XB, text, LocalizedText.MAIN_TABLE[text], "_XB") || LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_SW, text, LocalizedText.MAIN_TABLE[text], "_SW") || LocalizedText.TryInsertIntoTable(LocalizedText.TABLE_PS, text, LocalizedText.MAIN_TABLE[text], "_PS"))
			{
				list.Add(text);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			LocalizedText.MAIN_TABLE.Remove(list[i]);
		}
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x00034040 File Offset: 0x00032240
	private static bool TryInsertIntoTable(Dictionary<string, List<string>> table, string i, List<string> contents, string refToRemove)
	{
		if (i.EndsWith(refToRemove))
		{
			string text = i.Substring(0, i.LastIndexOf(refToRemove));
			table.Add(text.ToUpperInvariant(), contents);
			return true;
		}
		return false;
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x00034077 File Offset: 0x00032277
	private void TryFindTextAsset()
	{
		this.tmp = base.GetComponent<TMP_Text>();
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x00034088 File Offset: 0x00032288
	public void RefreshText()
	{
		if (this.useDebugLanguage && !Application.isPlaying)
		{
			LocalizedText.CURRENT_LANGUAGE = this.debugLanguage;
		}
		if (!this.tmp)
		{
			this.TryFindTextAsset();
		}
		if (this.autoSet)
		{
			this.currentText = this.GetText();
			this.currentText += this.addendum;
			if (this.tmp)
			{
				this.tmp.text = this.currentText;
				if (!Application.isPlaying && this.tripleIt)
				{
					this.tmp.text = this.tmp.text + this.tmp.text + this.tmp.text;
				}
			}
		}
		this.UpdateSpriteAsset();
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x00034154 File Offset: 0x00032354
	private void UpdateSpriteAsset()
	{
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x00034156 File Offset: 0x00032356
	private static string FailsafeParsing(string s)
	{
		s = s.Replace("\"\"", "\"");
		LocalizedText.Language current_LANGUAGE = LocalizedText.CURRENT_LANGUAGE;
		return s;
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x00034174 File Offset: 0x00032374
	private static string Frenchify(string s)
	{
		for (int i = 0; i < s.Length; i++)
		{
			if (LocalizedText.unbreakableSpaceRequiredChars.Contains(s.get_Chars(i)))
			{
				if (i == 0)
				{
					s = s.Insert(i, '\u00a0'.ToString());
				}
				else
				{
					char c = s.get_Chars(i - 1);
					if (c == ' ')
					{
						s = s.Remove(i - 1, 1).Insert(i - 1, '\u00a0'.ToString());
					}
					else if (c != '\u00a0')
					{
						s = s.Insert(i - 1, '\u00a0'.ToString());
					}
				}
			}
		}
		return s;
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x00034219 File Offset: 0x00032419
	private static string ReplaceCustomValues(string s)
	{
		return s;
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x060009CA RID: 2506 RVA: 0x0003421C File Offset: 0x0003241C
	public static bool languageSupportsAllCaps
	{
		get
		{
			return LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Russian && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Ukrainian && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.SimplifiedChinese && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.TraditionalChinese && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Japanese && LocalizedText.CURRENT_LANGUAGE != LocalizedText.Language.Korean;
		}
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x00034258 File Offset: 0x00032458
	public string GetText()
	{
		string text = LocalizedText.GetText(this.index, true);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		return "";
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x00034281 File Offset: 0x00032481
	public void SetText(string text)
	{
		if (!this.tmp)
		{
			this.TryFindTextAsset();
		}
		if (this.tmp)
		{
			this.tmp.text = text;
		}
		this.index = "";
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x000342BA File Offset: 0x000324BA
	public void SetTextLocalized(string id)
	{
		this.SetText(LocalizedText.GetText(id, true));
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x000342C9 File Offset: 0x000324C9
	public void SetIndex(string index)
	{
		if (!this.index.Equals(index))
		{
			this.index = index;
			this.RefreshText();
		}
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x000342E6 File Offset: 0x000324E6
	private static string GetText(int intid)
	{
		return LocalizedText.GetText(intid.ToString(), true);
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x000342F8 File Offset: 0x000324F8
	public static string GetText(string id, bool printDebug = true)
	{
		List<string> list = null;
		id = id.ToUpperInvariant();
		string text2;
		try
		{
			List<string> list2;
			if (LocalizedText.mainTable.TryGetValue(id, ref list2))
			{
				list = list2;
			}
			if (list != null)
			{
				string text = list[(int)LocalizedText.CURRENT_LANGUAGE];
				text = LocalizedText.FailsafeParsing(text);
				if (Ext.IsNullOrEmpty(text))
				{
					text = list[0];
					text = LocalizedText.FailsafeParsing(text);
					text = LocalizedText.ReplaceCustomValues(text);
				}
				text2 = text;
			}
			else if (printDebug)
			{
				text2 = "LOC: " + id;
			}
			else
			{
				Debug.LogError("Failed to load text: " + id);
				text2 = "";
			}
		}
		catch (Exception ex)
		{
			string text3 = "Failed to load text: ";
			string text4 = id;
			string text5 = "\n";
			Exception ex2 = ex;
			Debug.LogError(text3 + text4 + text5 + ((ex2 != null) ? ex2.ToString() : null));
			text2 = "";
		}
		return text2;
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x000343C0 File Offset: 0x000325C0
	public static string GetText(string id, LocalizedText.Language language)
	{
		string text;
		try
		{
			text = LocalizedText.mainTable[id.ToUpperInvariant()][(int)language];
		}
		catch (Exception)
		{
			text = "";
		}
		return text;
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00034400 File Offset: 0x00032600
	public static string GetText(string id, TextMeshProUGUI text)
	{
		if (text != null && text.GetComponent<LocalizedText>() == null)
		{
			text.gameObject.AddComponent<LocalizedText>().autoSet = false;
		}
		return LocalizedText.GetText(id, true);
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x00034431 File Offset: 0x00032631
	public static string GetText(string id, TextMeshPro text)
	{
		if (text != null && text.GetComponent<LocalizedText>() == null)
		{
			text.gameObject.AddComponent<LocalizedText>().autoSet = false;
		}
		return LocalizedText.GetText(id, true);
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00034462 File Offset: 0x00032662
	public static string GetText(string id, Text text)
	{
		if (text != null && text.GetComponent<LocalizedText>() == null)
		{
			text.gameObject.AddComponent<LocalizedText>().autoSet = false;
		}
		return LocalizedText.GetText(id, true);
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x00034494 File Offset: 0x00032694
	public static LocalizedText.Language GetSystemLanguage()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		if (systemLanguage <= 14)
		{
			if (systemLanguage == 10)
			{
				return LocalizedText.Language.English;
			}
			if (systemLanguage == 14)
			{
				return LocalizedText.Language.French;
			}
		}
		else
		{
			if (systemLanguage == 15)
			{
				return LocalizedText.Language.German;
			}
			switch (systemLanguage)
			{
			case 21:
				return LocalizedText.Language.Italian;
			case 22:
				return LocalizedText.Language.Japanese;
			case 23:
				return LocalizedText.Language.Korean;
			case 27:
				return LocalizedText.Language.Polish;
			case 28:
				return LocalizedText.Language.BRPortuguese;
			case 30:
				return LocalizedText.Language.Russian;
			case 34:
				return LocalizedText.Language.SpanishSpain;
			case 37:
				return LocalizedText.Language.Turkish;
			case 38:
				return LocalizedText.Language.Ukrainian;
			case 40:
				return LocalizedText.Language.SimplifiedChinese;
			case 41:
				return LocalizedText.Language.SimplifiedChinese;
			}
		}
		return LocalizedText.Language.English;
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x00034542 File Offset: 0x00032742
	public static void SetLanguageToSystemLanguage()
	{
		LocalizedText.CURRENT_LANGUAGE = LocalizedText.GetSystemLanguage();
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x0003454E File Offset: 0x0003274E
	public static void SetLanguage(int languageInt)
	{
		Debug.Log("Setting language to" + languageInt.ToString());
		if (languageInt == -1)
		{
			LocalizedText.SetLanguageToSystemLanguage();
		}
		else
		{
			LocalizedText.CURRENT_LANGUAGE = (LocalizedText.Language)languageInt;
		}
		LocalizedText.RefreshAllText();
	}

	// Token: 0x060009D8 RID: 2520 RVA: 0x0003457C File Offset: 0x0003277C
	public static void RefreshAllText()
	{
		LocalizedText[] array = Object.FindObjectsByType<LocalizedText>(0, 0);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RefreshText();
		}
		Action onLangugageChanged = LocalizedText.OnLangugageChanged;
		if (onLangugageChanged == null)
		{
			return;
		}
		onLangugageChanged.Invoke();
	}

	// Token: 0x060009D9 RID: 2521 RVA: 0x000345B6 File Offset: 0x000327B6
	public static void AppendCSVLine(string line, string basicPath, string fullPath)
	{
	}

	// Token: 0x060009DA RID: 2522 RVA: 0x000345B8 File Offset: 0x000327B8
	public static string GetNameIndex(string displayName)
	{
		return "NAME_" + displayName;
	}

	// Token: 0x060009DB RID: 2523 RVA: 0x000345C5 File Offset: 0x000327C5
	public static string GetDescriptionIndex(string displayName)
	{
		return "DESC_" + displayName;
	}

	// Token: 0x060009DD RID: 2525 RVA: 0x000345E4 File Offset: 0x000327E4
	// Note: this type is marked as 'beforefieldinit'.
	static LocalizedText()
	{
		List<char> list = new List<char>();
		list.Add('?');
		list.Add('!');
		list.Add(':');
		list.Add(';');
		list.Add('"');
		list.Add('%');
		LocalizedText.unbreakableSpaceRequiredChars = list;
	}

	// Token: 0x0400092A RID: 2346
	private static Dictionary<string, List<string>> MAIN_TABLE;

	// Token: 0x0400092B RID: 2347
	private static Dictionary<string, List<string>> TABLE_PC = new Dictionary<string, List<string>>();

	// Token: 0x0400092C RID: 2348
	private static Dictionary<string, List<string>> TABLE_SW = new Dictionary<string, List<string>>();

	// Token: 0x0400092D RID: 2349
	private static Dictionary<string, List<string>> TABLE_XB = new Dictionary<string, List<string>>();

	// Token: 0x0400092E RID: 2350
	private static Dictionary<string, List<string>> TABLE_PS = new Dictionary<string, List<string>>();

	// Token: 0x0400092F RID: 2351
	private static Dictionary<string, List<string>> DIALOGUE_TABLE;

	// Token: 0x04000930 RID: 2352
	public static Action OnLangugageChanged;

	// Token: 0x04000931 RID: 2353
	public const string MAIN_PATH = "Localization/Localized_Text";

	// Token: 0x04000932 RID: 2354
	public const string FULL_PATH = "Assets/Resources/Localization/Localized_Text.csv";

	// Token: 0x04000933 RID: 2355
	public const string UNLOCALIZED_PATH = "Localization/Unlocalized_Text";

	// Token: 0x04000934 RID: 2356
	public const string FULL_PATH_UNLOCALIZED = "Assets/Resources/Localization/Unlocalized_Text.csv";

	// Token: 0x04000935 RID: 2357
	public const string SERIALIZED_TERMS_PATH = "Localization/SerializedTermsData";

	// Token: 0x04000936 RID: 2358
	public const string SERIALIZED_DIALOGUE_PATH = "Localization/SerializedDialogueData";

	// Token: 0x04000937 RID: 2359
	public const string SERIALIZED_TERMS_PATH_FULL = "Assets/Resources/Localization/SerializedTermsData.txt";

	// Token: 0x04000938 RID: 2360
	public const string SERIALIZED_DIALOGUE_PATH_FULL = "Assets/Resources/Localization/SerializedDialogueData.txt";

	// Token: 0x04000939 RID: 2361
	public const int LANGUAGE_COUNT = 14;

	// Token: 0x0400093A RID: 2362
	public static LocalizedText.Language CURRENT_LANGUAGE = LocalizedText.Language.English;

	// Token: 0x0400093B RID: 2363
	public string index;

	// Token: 0x0400093C RID: 2364
	public TMP_Text tmp;

	// Token: 0x0400093D RID: 2365
	public bool autoSet = true;

	// Token: 0x0400093E RID: 2366
	private int row;

	// Token: 0x0400093F RID: 2367
	[SerializeField]
	private string currentText;

	// Token: 0x04000940 RID: 2368
	public bool useDebugLanguage;

	// Token: 0x04000941 RID: 2369
	public LocalizedText.Language debugLanguage;

	// Token: 0x04000942 RID: 2370
	public bool tripleIt;

	// Token: 0x04000943 RID: 2371
	private const string defaultHeaderName = "Muli";

	// Token: 0x04000944 RID: 2372
	private static int lineLength;

	// Token: 0x04000945 RID: 2373
	public string addendum;

	// Token: 0x04000946 RID: 2374
	public LocalizedText.FontStyle fontStyle;

	// Token: 0x04000947 RID: 2375
	public const char UNBREAKABLE_SPACE = '\u00a0';

	// Token: 0x04000948 RID: 2376
	private static List<char> unbreakableSpaceRequiredChars;

	// Token: 0x0200045E RID: 1118
	public enum Language
	{
		// Token: 0x040018D4 RID: 6356
		English,
		// Token: 0x040018D5 RID: 6357
		French,
		// Token: 0x040018D6 RID: 6358
		Italian,
		// Token: 0x040018D7 RID: 6359
		German,
		// Token: 0x040018D8 RID: 6360
		SpanishSpain,
		// Token: 0x040018D9 RID: 6361
		SpanishLatam,
		// Token: 0x040018DA RID: 6362
		BRPortuguese,
		// Token: 0x040018DB RID: 6363
		Russian,
		// Token: 0x040018DC RID: 6364
		Ukrainian,
		// Token: 0x040018DD RID: 6365
		SimplifiedChinese,
		// Token: 0x040018DE RID: 6366
		TraditionalChinese,
		// Token: 0x040018DF RID: 6367
		Japanese,
		// Token: 0x040018E0 RID: 6368
		Korean,
		// Token: 0x040018E1 RID: 6369
		Polish,
		// Token: 0x040018E2 RID: 6370
		Turkish
	}

	// Token: 0x0200045F RID: 1119
	public enum FontStyle
	{
		// Token: 0x040018E4 RID: 6372
		Normal,
		// Token: 0x040018E5 RID: 6373
		Shadow,
		// Token: 0x040018E6 RID: 6374
		Fuzzy,
		// Token: 0x040018E7 RID: 6375
		Outline,
		// Token: 0x040018E8 RID: 6376
		Custom
	}
}
