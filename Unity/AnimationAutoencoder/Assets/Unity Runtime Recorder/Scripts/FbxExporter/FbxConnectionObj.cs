using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class FbxConnectionObj {

	/*
	 * ;AnimCurveNode::T, Model::pCube1
	 * C: "OP",105553124109952,140364338281984, "Lcl Translation"
	 * 
	 */

	public string type1;
	public string name1;
	public string id1;

	public string type2;
	public string name2;
	public string id2;

	public string relation;
	public string relationDesc;
	bool hasRelationDesc = false;

	public bool isSucceed = false;

	public FbxConnectionObj (string type1, string name1, string id1, string type2, string name2, string id2, string relation, string relationDesc = "")
	{
		this.type1 = type1;
		this.name1 = name1;
		this.id1 = id1;
		this.type2 = type2;
		this.name2 = name2;
		this.id2 = id2;
		this.relation = relation;
		this.relationDesc = relationDesc;

		if (relationDesc == "")
			hasRelationDesc = false;
		else
			hasRelationDesc = true;
	}

	public FbxConnectionObj ( string connectionStr ) {
		string pattern = "\\t;([^:]*)::([^,]*),\\s([^:]*)::([^\\n]*)\\n\\tC:\\s\"([^\"]*)\",([0-9]*),([0-9]*)([^\n]*\n)";
		Match regMatch = Regex.Match (connectionStr, pattern);

		if (regMatch.Success) {
			//regMatch.Groups[0].Value : all text
			type1 = regMatch.Groups [1].Value;
			name1 = regMatch.Groups [2].Value;
			type2 = regMatch.Groups [3].Value;
			name2 = regMatch.Groups [4].Value;
			relation = regMatch.Groups [5].Value;
			id1 = regMatch.Groups [6].Value;
			id2 = regMatch.Groups [7].Value;
			string relationDescText = regMatch.Groups [8].Value;

			// get relation desc
			string relationDescPattern = ",\\s\"([^\"]*)\"";
			Match relationDescMatch = Regex.Match (relationDescText, relationDescPattern);

			if (relationDescMatch.Success) {
				hasRelationDesc = true;
				relationDesc = relationDescMatch.Groups [1].Value;
			} else {
				hasRelationDesc = false;
				relationDesc = "";
			}

			// got data
			isSucceed = true;
		} else
			isSucceed = false;
	}

	public string getOutputData () {
		if( hasRelationDesc )
			return "\t;"+type1+"::"+name1+", "+type2+"::"+name2+"\n\tC: \""+relation+"\","+id1+","+id2+", \""+relationDesc+"\"\n";
		else
			return "\t;"+type1+"::"+name1+", "+type2+"::"+name2+"\n\tC: \""+relation+"\","+id1+","+id2+"\n";
	}

	public void Log () {
		Debug.Log (type1 + " " + name1 + " " + type2 + " " + name2 + " " + relation + " " + id1 + " " + id2 + " " + relationDesc);
	}
}
