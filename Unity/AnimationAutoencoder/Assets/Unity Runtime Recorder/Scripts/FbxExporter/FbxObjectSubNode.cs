using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FbxObjectSubNode {

	public string nodeName;
	public string nodeValue;

	public FbxObjectSubNode () {
		nodeName = "";
		nodeValue = "";
	}

	public void SetupData ( string inputName, string inputValue ) {
		nodeName = inputName + ": ";
		nodeValue = inputValue;
	}

	public void SetupData ( string inputName, float[] inputData ) {
		nodeName = inputName + ": *" + inputData.Length.ToString() + " ";
		nodeValue = "{\n\t\t\ta: ";

		for( int i=0; i< inputData.Length; i++ )
		{
			if (i == 0)
				nodeValue += inputData[i].ToString ();
			else
				nodeValue += "," + inputData[i].ToString ();
		}
		nodeValue += "\n\t\t}";
	}

	public void SetupData ( string inputName, int[] inputData ) {
		nodeName = inputName + ": *" + inputData.Length.ToString() + " ";
		nodeValue = "{\n\t\t\ta: ";

		for( int i=0; i< inputData.Length; i++ )
		{
			if (i == 0)
				nodeValue += inputData[i].ToString ();
			else
				nodeValue += "," + inputData[i].ToString ();
		}
		nodeValue += "\n\t\t}";
	}

	public void SetupData ( string inputName, string[] inputData ) {
		nodeName = inputName + ": *" + inputData.Length.ToString() + " ";
		nodeValue = "{\n\t\t\ta: ";

		for( int i=0; i< inputData.Length; i++ )
		{
			if (i == 0)
				nodeValue += inputData[i];
			else
				nodeValue += "," + inputData[i];
		}
		nodeValue += "\n\t\t}";
	}

	public string GetResultString () {
		return nodeName + nodeValue + "\n";
	}
}
