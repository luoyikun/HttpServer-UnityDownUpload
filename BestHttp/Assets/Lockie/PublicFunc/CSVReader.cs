using UnityEngine;
using System.Linq;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class CSV
{
	public string[] header;
	public List<string> lines;

	public List<string[]> Search(string[] keywords)
	{
		var rows = new List<string[]>();
		if(keywords.Count() == 0)
		{
			lines.ForEach(line => rows.Add(CSVReader.ParseLine(line)));
			return rows;
		}

		foreach(var line in lines)
		{
			bool match = true;
			foreach(var keyword in keywords)
			{
				match = MatchKeyword(line, keyword);
				if(!match) break;
			}
			if(match)
			{
				rows.Add(CSVReader.ParseLine(line));
			}
		}
		return rows;
	}

	bool MatchKeyword(string line_, string keyword_)
	{
		string line = line_.ToLower();
		string keyword = keyword_.ToLower();
		if(keyword.Contains("=="))      return Match(line, keyword, "==", (a, b) => { return a == b; });
		else if(keyword.Contains("!=")) return Match(line, keyword, "!=", (a, b) => { return a != b; });
		else if(keyword.Contains(">=")) return Match(line, keyword, ">=", (a, b) => { return TryParse(a) >= TryParse(b); });
		else if(keyword.Contains("<=")) return Match(line, keyword, "<=", (a, b) => { return TryParse(a) <= TryParse(b); });
		else if(keyword.Contains(">"))  return Match(line, keyword, ">",  (a, b) => { return TryParse(a) > TryParse(b); });
		else if(keyword.Contains("<"))  return Match(line, keyword, "<",  (a, b) => { return TryParse(a) < TryParse(b); });
		else if(keyword.Contains("="))  return Match(line, keyword, "=",  (a, b) => { return a == b; });
		else
		{
			// simple search
			return line.Contains(keyword);
		}
	}

	double TryParse(string value)
	{
		double result;
		if(!Double.TryParse(value, out result)) return default(double);
		return result;
	}

	bool Match(string line, string keyword, string separator, Func<string, string, bool> judge)
	{
		var elements = keyword.Split(new string[]{separator}, StringSplitOptions.None);
		if(elements.Count() != 2) return false;

		var columnName = elements[0];
		var columnValue = elements[1];
		if(string.IsNullOrEmpty(columnValue)) return false;

		return judge(GetColumnValue(line, columnName), columnValue);
	}

	string GetColumnValue(string line, string columnName)
	{
		var columnIndex = Array.IndexOf(header, columnName);
		if(columnIndex == -1) return "";

		var cells = CSVReader.ParseLine(line);
		if(cells.Count() <= columnIndex) return "";

		return cells[columnIndex];
	}
}

public static class CSVReader {
	private static readonly string splitter = "[wafer]";
	private static readonly string[] splitters = new string[]{splitter};
	public static CSV Read(string text)
	{
		CSV csv = new CSV();
		text = text.Trim().Replace("\r", "") + "\n";

		// read cells
		csv.lines = new List<string>();
		bool startCell = false;
		StringBuilder line = new StringBuilder();
		for(int i=0; i<text.Length; ++i)
		{
			char c = text[i];
			if(c == '"')
			{
				if(text[i+1] == '"') i++;
				else
				{
					startCell = !startCell;
					continue;
				}
			}
			else if(!startCell && c == ',')
			{
				line.Append(splitter);
				continue;
			}
			else if(!startCell && c == '\n')
			{
				csv.lines.Add(line.ToString());
				line.Length = 0;
				continue;
			}
			line.Append(c);
		}
		string lastLine = line.ToString().Trim();
		if(!string.IsNullOrEmpty(lastLine)) csv.lines.Add(lastLine);

		// add line number
		csv.lines = csv.lines.Select((t, index) => string.Format("{0}{2}{1}", (index > 0 ? (index+1).ToString() : "line"), t, splitter)).ToList();

		// get header
		csv.header = ParseLine(csv.lines[0]);
		csv.lines.RemoveAt(0);

		return csv;
	}

	public static string[] ParseLine(string line)
	{
		return line.Split(splitters, StringSplitOptions.None);
	}

    public static string[] ParseLineZeroStart(string line)
    {
        string[] strAll =  line.Split(splitters, StringSplitOptions.None);
        string[] strRet = new string[strAll.Length-1];
        for (int i = 1; i < strAll.Length; i++)
        {
            strRet[i - 1] = strAll[i];
        }
        return strRet;
    }
}

public class CSVMgr
{
    public static List<string[]> GetData(string path)
    {
        string tipString = FileIoMgr.GetString(Application.streamingAssetsPath + "/" + path);

        List<string[]> rows = new List<string[]>();
        CSV csv = CSVReader.Read(tipString);
        csv.lines.ForEach(line => rows.Add(CSVReader.ParseLineZeroStart(line)));
        return rows;
    }
}
