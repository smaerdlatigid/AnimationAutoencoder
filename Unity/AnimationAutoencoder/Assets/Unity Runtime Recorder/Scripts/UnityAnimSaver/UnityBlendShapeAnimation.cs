using UnityEngine;
using System.Collections;

public class UnityBlendShapeAnimation {

	public UnityCurveContainer[] curves;
	public SkinnedMeshRenderer skinMeshObj;
	string[] blendShapeNames;
	int blendShapeCount = 0;
	public string pathName = "";

	public UnityBlendShapeAnimation( string hierarchyPath, SkinnedMeshRenderer observeSkinnedMeshRenderer ) {
		pathName = hierarchyPath;
		skinMeshObj = observeSkinnedMeshRenderer;

		//int blendShapeCount = skinMeshObj.getbl
		Mesh blendShapeMesh = skinMeshObj.sharedMesh;
		blendShapeCount = blendShapeMesh.blendShapeCount;

		blendShapeNames = new string[blendShapeCount];
		curves = new UnityCurveContainer[blendShapeCount];

		// create curve objs and add names
		for (int i = 0; i < blendShapeCount; i++) {
			blendShapeNames [i] = blendShapeMesh.GetBlendShapeName (i);
			curves [i] = new UnityCurveContainer ("blendShape." + blendShapeNames [i]);
		}
	}

	public void AddFrame ( float time ) {
					
		for (int i = 0; i < blendShapeCount; i++)
			curves [i].AddValue (time, skinMeshObj.GetBlendShapeWeight (i));

	}
}
