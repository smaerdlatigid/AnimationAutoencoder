using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTracker {

	Transform targetTransform;

	bool recordPos = false;
	bool recordRot = false;
	bool recordScale = false;

	public List<Vector3> posDataList;
	public List<Quaternion> rotDataList;
	public List<Vector3> scaleDataList;

	public TransformTracker (Transform targetObj, bool trackPos, bool trackRot, bool trackScale) {

		targetTransform = targetObj;

		if (trackPos) {
			posDataList = new List<Vector3> ();
			recordPos = trackPos;
		}

		if (trackRot) {
			rotDataList = new List<Quaternion> ();
			recordRot = trackRot;
		}

		if (trackScale) {
			scaleDataList = new List<Vector3> ();
			recordScale = trackScale;
		}
	}

	public void recordFrame() {
		if( recordPos )
			posDataList.Add (targetTransform.localPosition);

		if (recordRot)
			rotDataList.Add (targetTransform.localRotation);

		if (recordScale)
			scaleDataList.Add (targetTransform.localScale);
	}
}