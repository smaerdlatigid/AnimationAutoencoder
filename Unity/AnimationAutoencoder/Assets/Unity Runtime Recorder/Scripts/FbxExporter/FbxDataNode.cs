using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class FbxDataNode
{
	public static FbxDataNode[] FetchNodes (string inputData, int level)
	{
		List<FbxDataNode> nodes = new List<FbxDataNode> ();

		StringReader reader = new StringReader (inputData);

		bool foundBracket = false;

		string[] tempNodeData = new string[2];

		while (reader.Peek () != -1) {
			
			string strLine = reader.ReadLine ();

			// found bracket, fetch subNodes
			if (strLine.IndexOf ("{") != -1) {

				int bracketNum = 1;
				string[] nodeData = FbxDataNode.GetNodeData (strLine);
				string contentInsideBracket = "";

				// search for bracket end
				while (reader.Peek () != -1) {
					string insideBracketLine = reader.ReadLine ();

					if (insideBracketLine.IndexOf ("{") != -1) {
						contentInsideBracket += insideBracketLine + "\n";
						++bracketNum;
					} else if (insideBracketLine.IndexOf ("}") != -1) {
						--bracketNum;

						if (bracketNum == 0) {
							break;
						} else {
							contentInsideBracket += insideBracketLine + "\n";
						}
					} else
						contentInsideBracket += insideBracketLine + "\n";
				}

				FbxDataNode newNode = new FbxDataNode (nodeData [0], nodeData [1], level);
				newNode.hasSubNode = true;
				FbxDataNode[] subNodes = FbxDataNode.FetchNodes (contentInsideBracket, level + 1);

				for (int i = 0; i < subNodes.Length; i++)
					newNode.addSubNode (subNodes [i]);

				nodes.Add (newNode);
			}
			// found attribute, add to a subnode
			else if (strLine.IndexOf (":") != -1) {

				string[] nodeData = FbxDataNode.GetNodeData (strLine);
				FbxDataNode newNode = new FbxDataNode (nodeData [0], nodeData [1], level);

				nodes.Add (newNode);
			}
			// found nothing, just skip
			else {
				// some data might be last data's
				if (level > 1) {
					int lastIndex = nodes.Count;
					nodes [lastIndex - 1].nodeData += strLine;
				}
			}
		}

		return nodes.ToArray ();
	}

	public static string[] GetNodeData (string strLine)
	{

		string searchPattern = "";

		if (strLine.IndexOf ("{") != -1) {
			searchPattern = "([^:]*):\\s([^\\{]*)\\{";
		} else {
			searchPattern = "([^:]*):\\s([^\\n]*)\\n";
			strLine += "\n";
		}
		Match matchData = Regex.Match (strLine, searchPattern);

		string[] resultData = new string[2];
		if (matchData.Success) {
			resultData [0] = matchData.Groups [1].Value;
			resultData [1] = matchData.Groups [2].Value;

			// clear \t
			resultData [0] = resultData [0].Replace ("\t", "");

			// clear spaces
			if (resultData [1] [resultData[1].Length - 1] == ' ')
				resultData [1] = resultData [1].Substring (0, resultData [1].Length - 1);
			
		} else {
			Debug.Log ("ERROR :: Cant Get Node Data");
		}

		return resultData;
	}

	public static int nowDataId = 0;

	public static int GetNextDataId ()
	{
		return FbxDataNode.nowDataId++;
	}


	public string nodeName;
	public string nodeData;

	int level = 0;

	public List<FbxDataNode> subNodes;

	// for file store
	string dataFilePath = "";
	bool isDataInFile = false;

	public bool hasSubNode = false;

	// create new data
	public FbxDataNode (string nodeName, string nodeData, int level)
	{
		subNodes = new List<FbxDataNode> ();
		this.nodeName = nodeName;
		this.nodeData = nodeData;
		this.level = level;
	}

	public void addSubNode (string newNodeName, string newNodeData)
	{
		FbxDataNode newSubNode = new FbxDataNode (newNodeName, newNodeData, level + 1);
		subNodes.Add (newSubNode);
		hasSubNode = true;
	}

	public void addSubNode (FbxDataNode newNode)
	{
		subNodes.Add (newNode);
		hasSubNode = true;
	}

	public string getResultData ()
	{
		string resultString = "";
		string levelBlanks = "";
		
		for (int i = 0; i < level; i++)
			levelBlanks += "\t";

		resultString += levelBlanks + nodeName + ": " + nodeData;

		// if still some subnodes
		if (hasSubNode) {
			resultString += " {\n";

			// if data stored, load data
			if (isDataInFile)
				resultString += File.ReadAllText (dataFilePath);

			// load other subNode datas
			for (int i = 0; i < subNodes.Count; i++) {
				resultString += subNodes [i].getResultData ();
			}

			resultString += levelBlanks + "}\n";
		} else {
			resultString += "\n";
		}

		return resultString;
	}

	public void saveDataOnDisk (string saveFolder)
	{
		if (!isDataInFile) {
			isDataInFile = true;
			dataFilePath = saveFolder + nodeName + "-Data-" + FbxDataNode.GetNextDataId ().ToString ();

			if (File.Exists (dataFilePath))
				File.Delete (dataFilePath);
		}

		StreamWriter writer = new StreamWriter (dataFilePath, true);
		for (int i = 0; i < subNodes.Count; i++) {
			writer.Write (subNodes [i].getResultData ());
		}

		subNodes.Clear ();
		writer.Close ();
	}

	public void clearSavedData () {
		if (isDataInFile)
			File.Delete (dataFilePath);

		for (int i = 0; i < subNodes.Count; i++)
			subNodes [i].clearSavedData ();
	}
}
