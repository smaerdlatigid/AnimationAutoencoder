using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FbxObjectNode {
	
	public string nodeName;
	public string nodeValue;

	string headerString = "";

	List<FbxObjectSubNode> subNodes;

	public FbxObjectNode (string nodeType, string nodeId, string nodeName, string subType) {
		headerString = nodeType + ": " + nodeId + ", \"" + nodeName + "\", \"" + subType + "\" {\n";
		subNodes = new List<FbxObjectSubNode> ();
	}

	public void AddSubnode ( string inputName, string inputValue ) {
		FbxObjectSubNode newSubNode = new FbxObjectSubNode ();
		newSubNode.SetupData (inputName, inputValue);

		subNodes.Add (newSubNode);
	}

	public void AddSubnode ( string inputName, float[] inputFloatData ) {
		FbxObjectSubNode newSubNode = new FbxObjectSubNode ();
		newSubNode.SetupData (inputName, inputFloatData);

		subNodes.Add (newSubNode);
	}

	public void AddSubnode ( string inputName, string[] inputStringData ) {
		FbxObjectSubNode newSubNode = new FbxObjectSubNode ();
		newSubNode.SetupData (inputName, inputStringData);

		subNodes.Add (newSubNode);
	}

	public void AddSubnode ( string inputName, int[] inputIntData ) {
		FbxObjectSubNode newSubNode = new FbxObjectSubNode ();
		newSubNode.SetupData (inputName, inputIntData);

		subNodes.Add (newSubNode);
	}

	public string GetResultString () {
		string resultStr = "\t" + headerString;

		for (int i = 0; i < subNodes.Count; i++)
			resultStr += "\t\t" + subNodes [i].GetResultString ();

		resultStr += "\t}\n";

		return resultStr;
	}
}
