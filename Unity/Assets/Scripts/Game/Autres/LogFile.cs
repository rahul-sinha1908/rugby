using UnityEngine;
using System.Collections;
using System.IO;

public class LogFile {

	string nameFile;
	StreamWriter writer;
	long length;

	public LogFile()
	{
		nameFile = "";
		writer = null;
	}

	public void SetName(string sName)
	{
		if (sName != "")
			nameFile = sName;
	}

	public string GetName()
	{
		return nameFile;
	}
	
	public long GetLength()
	{
		return length;
	}

	public void WriteLine(string line, bool onlyCreate = false)
	{
		try 
		{
			if (line == "")
				return;

            if (!File.Exists(nameFile + ".txt"))
            {
                writer = File.CreateText(nameFile + ".txt");
            }

            else if (writer != null)
            {
                writer.WriteLine(line);
            }

            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
			
			FileInfo info = new FileInfo(nameFile + ".txt");
			length = info.Length;
		}
		catch(System.Exception e) 
		{
			Debug.LogWarning(e.Message); // Ex : Directory not found ;)
		}
	}
	
	public void ClearFile()
	{
		if (!File.Exists(nameFile + ".txt"))
			return;
		
		File.WriteAllText(nameFile + ".txt", "");
	}
}
