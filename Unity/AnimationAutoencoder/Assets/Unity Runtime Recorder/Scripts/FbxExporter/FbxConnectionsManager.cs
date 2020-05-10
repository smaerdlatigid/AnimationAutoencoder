using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class FbxConnectionsManager {

	string fbxStrData = "";
	string originalConnectionData = "";
	List<FbxConnectionObj> connObjs;

	public FbxConnectionsManager (string inputData) {
		fbxStrData = inputData;

		LoadObjectsData ();
	}

	public void EditTargetFile (string targetFilePath) {
		string outputData = "Connections:  {\n";
		for (int i = 0; i < connObjs.Count; i++) {
			outputData += "\t\n";
			outputData += connObjs [i].getOutputData ();
		}
		outputData += "}\n";

		string sourceData = File.ReadAllText (targetFilePath);
		sourceData = Regex.Replace (sourceData, "Connections:\\s\\s\\{[^\\}]*\\}\\n", outputData);

		File.WriteAllText (targetFilePath, sourceData);
	}

	void LoadObjectsData () {

		// find start of the Objects
		Match matchResult = Regex.Match(fbxStrData, "Connections:\\s\\s\\{([^\\}]*)\\}");
		string connAll = matchResult.Groups [0].Value;
		string allConnAttributes = matchResult.Groups [1].Value;

		Debug.Log (connAll);
		Debug.Log (allConnAttributes);

		// load all attributes
		connObjs = new List<FbxConnectionObj>();

		string[] splliter = { "\t\n" };
		string[] splittedData = allConnAttributes.Split (splliter, System.StringSplitOptions.RemoveEmptyEntries);

		for (int i = 0; i < splittedData.Length; i++) {
			FbxConnectionObj tempObj = new FbxConnectionObj (splittedData [i]);
			if (tempObj.isSucceed)
				connObjs.Add (tempObj);
		}

		// debug
		Debug.Log("found attributes: " + connObjs.Count);
		for (int i = 0; i < connObjs.Count; i++) {
			connObjs [i].Log ();
		}


		// test

//
//		int startIndex = fbxStrData.IndexOf ("Objects:  {\n");
//		startIndex += ("Objects:  {\n").Length;
//
//		StringReader reader = new StringReader (fbxStrData);
//
//		// skip to start index
//		for (int i = 0; i < startIndex; i++)
//			reader.Read ();
//
//
//		// find the end of the Objects {}
//		int bracketBalancer = 1;
//		int readCounter = 0;
//
//		while (true) {
//			char temp = (char)reader.Read ();
//			++readCounter;
//
//			if (temp == '{')
//				bracketBalancer += 1;
//			else if (temp == '}') {
//				bracketBalancer -= 1;
//				if (bracketBalancer == 0)
//					break;
//			}
//		}
//
//		// save other data in string
//		originalObjsData = fbxStrData.Substring(startIndex, readCounter-1);
//		fbxStrData = fbxStrData.Remove (startIndex, readCounter - 1);
	}

//	void SearchAttribute () {
//		string searchPattern = "\\t;[^:]*::[^,]*,\\s[^:]*::[^\\n]*\\n\\tC:\\s\"[^\"]*\",[0-9]*,[0-9]*[^\\n]*\\n";
//	}

	public void AddConnectionItem (string type1, string name1, string id1, string type2, string name2, string id2, string relation, string relationDesc = "") {
		FbxConnectionObj tempConnObj = new FbxConnectionObj (type1, name1, id1, type2, name2, id2, relation, relationDesc);
		connObjs.Add (tempConnObj);
	}

	public string searchObjectId (string objectName) {

		for (int i = 0; i < connObjs.Count; i++) {
			if (connObjs [i].name2 == objectName)
				return connObjs [i].id2;
		}

		return "";
	}

	public string getAnimBaseLayerId () {
		for (int i = 0; i < connObjs.Count; i++) {
			if (connObjs [i].type1 == "AnimLayer") {
				if (connObjs [i].name1 == "BaseLayer")
					return connObjs [i].id1;
			}
		}
		return "";
	}


//	string searchPattern = ";([a-z0-9]*)::([a-z0-9]*),\\s([a-z0-9]*)::([^\\n]*)\\n\\tC:\\s\"([a-z]*)\",([0-9]*),([0-9]*),\\s\"([^\"]*)\"";
	//		MatchCollection matches = Regex.Matches (matchData.Value, searchPattern, RegexOptions.IgnoreCase);
	//
	//		for (int i = 0; i < matches.Count; i++) {
	//			Match tempMatch = matches [i];
	//			Debug.Log (tempMatch.Value);
	//
	//			for (int g = 0; g < tempMatch.Groups.Count; g++)
	//				Debug.Log (tempMatch.Groups [g].Value);
	//		}
}
