using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;

public class testFBX : MonoBehaviour
{

	public string filePath;

	// Use this for initialization
	void Start ()
	{
		LoadAttribute ("Connections");
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void LoadAttribute (string attributeName)
	{
		
		Match matchData = Regex.Match (File.ReadAllText (filePath), attributeName + ":[ ]*\\{[^\\}]*\\}");

		Debug.Log (matchData.Length);
		Debug.Log (matchData.Value);

//		;AnimCurveNode::R, AnimLayer::BaseLayer
//		C: "OO",105553119832896,140244214405232
//		string searchPattern = ";([a-z0-9]*)::([a-z0-9]*),\\s([a-z0-9]*)::([^\\n]*)\\n\\tC:\\s\"([a-z]*)\",([0-9]*),([0-9]*),\\s\"([^\"]*)\"";
//		MatchCollection matches = Regex.Matches (matchData.Value, searchPattern, RegexOptions.IgnoreCase);
//
//		for (int i = 0; i < matches.Count; i++) {
//			Match tempMatch = matches [i];
//			Debug.Log (tempMatch.Value);
//
//			for (int g = 0; g < tempMatch.Groups.Count; g++)
//				Debug.Log (tempMatch.Groups [g].Value);
//		}
	

//		string searchPattern = ";([a-z0-9]*)::([a-z0-9]*),\\s([a-z0-9]*)::([^\\n]*)\\n\\tC:\\s\"([a-z]*)\",([0-9]*),([0-9]*)([^\\n]*)";
//		MatchCollection matches = Regex.Matches (matchData.Value, searchPattern, RegexOptions.IgnoreCase);
//		
//		for (int i = 0; i < matches.Count; i++) {
//			Match tempMatch = matches [i];
//			Debug.Log (tempMatch.Value);
//		
//			for (int g = 0; g < tempMatch.Groups.Count; g++)
//				Debug.Log (tempMatch.Groups [g].Value);
//		}

		int startIndex = File.ReadAllText (filePath).IndexOf ("Objects:");
		Debug.Log (startIndex);
		int readCounter = 0;

		StreamReader reader = new StreamReader (filePath);

		for (int i = 0; i < startIndex; i++)
			reader.Read ();

		// find first '{'
		while (true) {
			char tempChar = (char)reader.Read ();
			++readCounter;

			if (tempChar == '{')
				break;
		}

		int bracketBalancer = 1;

		while (true) {
			char temp = (char)reader.Read ();
			++readCounter;

			if (temp == '{')
				bracketBalancer += 1;
			else if (temp == '}') {
				bracketBalancer -= 1;
				if (bracketBalancer == 0)
					break;
			}
		}

		Debug.Log (readCounter);
	}
}
