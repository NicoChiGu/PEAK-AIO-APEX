using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x0200008E RID: 142
public class CSVReader
{
	// Token: 0x060005B1 RID: 1457 RVA: 0x00020AD8 File Offset: 0x0001ECD8
	public static void DebugOutputGrid(string[,] grid)
	{
		string text = "";
		for (int i = 0; i < grid.GetUpperBound(1); i++)
		{
			for (int j = 0; j < grid.GetUpperBound(0); j++)
			{
				text += grid[j, i];
				text += "|";
			}
			text += "\n";
		}
	}

	// Token: 0x060005B2 RID: 1458 RVA: 0x00020B38 File Offset: 0x0001ED38
	public static Dictionary<string, List<string>> SplitCsvDict(string csvText, int overrideColumnCount = 0, bool debug = false)
	{
		string[,] array = CSVReader.SplitCsvGrid(csvText, true, overrideColumnCount);
		Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
		if (debug && Application.isEditor)
		{
			CSVReader.DebugOutputGrid(array);
		}
		for (int i = 0; i < array.GetLength(1); i++)
		{
			List<string> list = new List<string>();
			if (array[0, i] != null)
			{
				string text = array[0, i].ToUpperInvariant();
				for (int j = 1; j < array.GetLength(0); j++)
				{
					string.IsNullOrEmpty(array[j, i]);
					list.Add(array[j, i]);
				}
				if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text) && !dictionary.ContainsKey(text))
				{
					dictionary.Add(text, list);
				}
			}
		}
		return dictionary;
	}

	// Token: 0x060005B3 RID: 1459 RVA: 0x00020BFC File Offset: 0x0001EDFC
	public static string[,] SplitCsvGrid(string csvText, bool cutQuotes = true, int overrideColumnCount = 0)
	{
		csvText = csvText.Replace("\n", "");
		csvText = csvText.Replace("\\n", "\n");
		string[] array = csvText.Replace("\r", "").Split(new string[] { ",ENDLINE,", ",ENDLINE", "ENDLINE,", "ENDLINE" }, 0);
		CSVReader.cachedCsvLineArray = new string[CSVReader.GetCsvLineLength(array[0])];
		int num = Mathf.Max(overrideColumnCount, CSVReader.cachedCsvLineArray.Length);
		string[,] array2 = new string[num + 1, array.Length + 1];
		for (int i = 0; i < array.Length; i++)
		{
			CSVReader.SplitCsvLine(array[i], CSVReader.cachedCsvLineArray, cutQuotes);
			for (int j = 0; j < num; j++)
			{
				array2[j, i] = CSVReader.cachedCsvLineArray[j];
			}
		}
		return array2;
	}

	// Token: 0x060005B4 RID: 1460 RVA: 0x00020CD8 File Offset: 0x0001EED8
	public static void SplitCsvLine(string line, string[] output, bool cutQuotes = true)
	{
		int num = 0;
		int num2 = -1;
		line = CSVReader.ReplaceUnQuotedChars(line, ',', '|');
		int num3 = 0;
		while (num3 < line.Length && num != output.Length)
		{
			if (line.get_Chars(num3).Equals('|'))
			{
				if (num3 - num2 - 1 == 0)
				{
					output[num] = "";
				}
				else
				{
					output[num] = line.Substring(num2 + 1, num3 - num2 - 1);
				}
				num++;
				num2 = num3;
			}
			num3++;
		}
		int num4 = output.Length - 1;
		if (num <= num4)
		{
			output[num4] = line.Substring(num2 + 1, line.Length - num2 - 1);
			num++;
		}
		if (cutQuotes)
		{
			for (int i = 0; i < output.Length; i++)
			{
				output[i] = output[i].Trim('"');
			}
		}
	}

	// Token: 0x060005B5 RID: 1461 RVA: 0x00020D90 File Offset: 0x0001EF90
	public static int GetCsvLineLength(string line)
	{
		line = CSVReader.ReplaceUnQuotedChars(line, ',', '|');
		return line.Split('|', 0).Length;
	}

	// Token: 0x060005B6 RID: 1462 RVA: 0x00020DAC File Offset: 0x0001EFAC
	public static string[] SplitCsvLine(string line, bool cutQuotes = true)
	{
		line = CSVReader.ReplaceUnQuotedChars(line, ',', '|');
		string[] array = line.Split('|', 0);
		if (cutQuotes)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim('"');
			}
		}
		return array;
	}

	// Token: 0x060005B7 RID: 1463 RVA: 0x00020DF0 File Offset: 0x0001EFF0
	private static string ReplaceUnQuotedChars(string line, char original, char replace)
	{
		CSVReader.staticStringBuilder.Clear();
		bool flag = false;
		for (int i = 0; i < line.Length; i++)
		{
			CSVReader.currentParsedCharacter = line.get_Chars(i);
			if (CSVReader.currentParsedCharacter.Equals('"'))
			{
				flag = !flag;
			}
			else if (!flag && CSVReader.currentParsedCharacter.Equals(original))
			{
				CSVReader.staticStringBuilder.Append(replace);
			}
			else
			{
				CSVReader.staticStringBuilder.Append(CSVReader.currentParsedCharacter);
			}
		}
		return CSVReader.staticStringBuilder.ToString();
	}

	// Token: 0x040005C1 RID: 1473
	public static string[] cachedCsvLineArray = null;

	// Token: 0x040005C2 RID: 1474
	private static char currentParsedCharacter;

	// Token: 0x040005C3 RID: 1475
	private const char quoteChar = '"';

	// Token: 0x040005C4 RID: 1476
	private static StringBuilder staticStringBuilder = new StringBuilder();
}
