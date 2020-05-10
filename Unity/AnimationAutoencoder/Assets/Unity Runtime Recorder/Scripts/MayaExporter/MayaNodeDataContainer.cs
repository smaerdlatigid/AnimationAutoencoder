using UnityEngine;
using System.Collections;
using System.IO;

public class MayaNodeDataContainer {

	TransformTracker tracker;

	public Transform observeObj;
	string objNodeName;		// name for maya to recgnize obj
	string saveFileName;	// replace '/' with '-', or would cause errors
	string fileFolderPath;

	bool recordTranslation = false;
	bool recordRotation = false;
	bool recordScale = false;

	string objFinalFilePath = "";

	public MayaNodeDataContainer ( Transform inputObj, string namePath, string inputPath, bool recordT, bool recordR, bool recordS ) {

		objNodeName = namePath;
		saveFileName = objNodeName.Replace ('/', '-');
		observeObj = inputObj;
		fileFolderPath = inputPath;

		recordTranslation = recordT;
		recordRotation = recordR;
		recordScale = recordS;

		// setup tracker
		tracker = new TransformTracker (inputObj, recordT, recordR, recordS);

	}

	public void recordFrame ( int frameIndex ) {
		tracker.recordFrame ();
	}

	public void WriteIntoFile () {

		string filePathX = fileFolderPath + saveFileName + "_dataX";
		string filePathY = fileFolderPath + saveFileName + "_dataY";
		string filePathZ = fileFolderPath + saveFileName + "_dataZ";
		StreamWriter dataWriterX = new StreamWriter (filePathX);
		StreamWriter dataWriterY = new StreamWriter (filePathY);
		StreamWriter dataWriterZ = new StreamWriter (filePathZ);


		// write positions
		if (recordTranslation) {
			dataWriterX.Write (getMayaCurveHeadContent ("animCurveTL", "translateX", tracker.posDataList.Count));
			dataWriterY.Write (getMayaCurveHeadContent ("animCurveTL", "translateY", tracker.posDataList.Count));
			dataWriterZ.Write (getMayaCurveHeadContent ("animCurveTL", "translateZ", tracker.posDataList.Count));

			Vector3 mayaPos = Vector3.zero;

			// write datas
			for (int i = 0; i < tracker.posDataList.Count; i++) {
				mayaPos = ExportHelper.UnityToMayaPosition (tracker.posDataList [i]);

				dataWriterX.Write (" " + i + " " + mayaPos.x);
				dataWriterY.Write (" " + i + " " + mayaPos.y);
				dataWriterZ.Write (" " + i + " " + mayaPos.z);
			}

			// end file data
			dataWriterX.Write (";\n");
			dataWriterY.Write (";\n");
			dataWriterZ.Write (";\n");

			// write ending content
			dataWriterX.Write (getMayaCurveFootContent ("translateX", "tx"));
			dataWriterY.Write (getMayaCurveFootContent ("translateY", "ty"));
			dataWriterZ.Write (getMayaCurveFootContent ("translateZ", "tz"));
		}



		// write rotations
		if (recordRotation) {
			
			dataWriterX.Write (getMayaCurveHeadContent ("animCurveTA", "rotateX", tracker.rotDataList.Count));
			dataWriterY.Write (getMayaCurveHeadContent ("animCurveTA", "rotateY", tracker.rotDataList.Count));
			dataWriterZ.Write (getMayaCurveHeadContent ("animCurveTA", "rotateZ", tracker.rotDataList.Count));

			Vector3 mayaRot = Vector3.zero;

			// write datas
			for (int i = 0; i < tracker.rotDataList.Count; i++) {
				mayaRot = ExportHelper.UnityToMayaRotation (tracker.rotDataList [i]);

				dataWriterX.Write (" " + i + " " + mayaRot.x);
				dataWriterY.Write (" " + i + " " + mayaRot.y);
				dataWriterZ.Write (" " + i + " " + mayaRot.z);
			}

			// end file data
			dataWriterX.Write (";\n");
			dataWriterY.Write (";\n");
			dataWriterZ.Write (";\n");

			// write ending content
			dataWriterX.Write (getMayaCurveFootContent ("rotateX", "rx"));
			dataWriterY.Write (getMayaCurveFootContent ("rotateY", "ry"));
			dataWriterZ.Write (getMayaCurveFootContent ("rotateZ", "rz"));
		}



		// write scales
		if (recordScale) {
			
			dataWriterX.Write (getMayaCurveHeadContent ("animCurveTU", "scaleX", tracker.scaleDataList.Count));
			dataWriterY.Write (getMayaCurveHeadContent ("animCurveTU", "scaleY", tracker.scaleDataList.Count));
			dataWriterZ.Write (getMayaCurveHeadContent ("animCurveTU", "scaleZ", tracker.scaleDataList.Count));

			Vector3 mayaScale = Vector3.zero;

			// write datas
			for (int i = 0; i < tracker.rotDataList.Count; i++) {
				mayaScale = tracker.scaleDataList [i];

				dataWriterX.Write (" " + i + " " + mayaScale.x);
				dataWriterY.Write (" " + i + " " + mayaScale.y);
				dataWriterZ.Write (" " + i + " " + mayaScale.z);
			}

			// end file data
			dataWriterX.Write (";\n");
			dataWriterY.Write (";\n");
			dataWriterZ.Write (";\n");

			// write ending content
			dataWriterX.Write (getMayaCurveFootContent ("scaleX", "sx"));
			dataWriterY.Write (getMayaCurveFootContent ("scaleY", "sy"));
			dataWriterZ.Write (getMayaCurveFootContent ("scaleZ", "sz"));
		}



		// end writing
		dataWriterX.Close ();
		dataWriterY.Close ();
		dataWriterZ.Close ();




		// process final file
		objFinalFilePath = fileFolderPath + saveFileName + "_all";

		File.AppendAllText (objFinalFilePath, File.ReadAllText (filePathX));
		File.AppendAllText (objFinalFilePath, File.ReadAllText (filePathY));
		File.AppendAllText (objFinalFilePath, File.ReadAllText (filePathZ));

		// clean x y z files
		File.Delete (filePathX);
		File.Delete (filePathY);
		File.Delete (filePathZ);

	}

	public string getFinalFilePath () {

		if (objFinalFilePath == "")
			return null;
		else
			return objFinalFilePath;
	}

	public bool cleanFile () {
		if (objFinalFilePath == "")
			return false;
		else
		{
			File.Delete( objFinalFilePath );
			objFinalFilePath = "";

			return true;
		}
	}

	// write header part
	string getMayaCurveHeadContent (string curveName, string propertyName, int animCount) {

		string fileContent = "";
		fileContent += "createNode " + curveName + " -n \"" + objNodeName + "_" + propertyName + "\";\n";
		fileContent += "\tsetAttr \".tan\" 18;\n";
		fileContent += "\tsetAttr \".wgt\" no;\n";
		fileContent += "\tsetAttr -s "+animCount.ToString()+" \".ktv[0:"+(animCount-1).ToString()+"]\" ";

		return fileContent;
	}

	// write connectAttr part
	string getMayaCurveFootContent (string propertyName, string footPropertyName) {
		string fileContent = "";
		fileContent += "connectAttr \"" + objNodeName + "_" + propertyName + ".o\" \"" + objNodeName + "." + footPropertyName + "\";\n";

		return fileContent;
	}
}
