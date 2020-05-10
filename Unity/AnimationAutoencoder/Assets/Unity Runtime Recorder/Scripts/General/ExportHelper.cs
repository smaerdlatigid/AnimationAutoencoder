using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExportHelper {
	
	// convert unity translation to maya translation
	public static Vector3 UnityToMayaPosition (Vector3 t)
	{
		return new Vector3(-t.x, t.y, t.z);
	}

	// convert unity rotation to maya rotation
	//	Vector3 Export2MayaRotation (Vector3 r)
	//	{
	//		
	//		//return new Vector3 (r.z, r.y, r.x);
	//		// return new Vector3(r.z, r.x, r.y);
	//		//return new Vector3(r.x, -r.y, -r.z);
	//
	//		Vector3 flippedRot = new Vector3 (r.x, -r.y, -r.z);
	//		Quaternion qx = Quaternion.AngleAxis (flippedRot.x, Vector3.right);
	//		Quaternion qy = Quaternion.AngleAxis (flippedRot.y, Vector3.up);
	//		Quaternion qz = Quaternion.AngleAxis (flippedRot.z, Vector3.forward);
	//		Quaternion unityRotationQ = qx * qy * qz;
	//
	//		return WikiQuaternionToRotation (unityRotationQ);
	//	}

	public static Vector3 UnityToMayaRotation (Quaternion q) {
		// x,y,z is Maya's rotation order
		// and due to right hand / left hand coordinate, y & z should *-1
		return WikiQuaternionToRotation(q, new Vector3(1,-1,-1));
	}

	public static Vector3 WikiQuaternionToRotation (Quaternion q, Vector3 axisMultiplier) {

		/*
		 * This function is a convertion of python sample code in a wiki page
		 * 
		 * https://en.wikipedia.org/wiki/Conversion_between_quaternions_and_Euler_angles
		 * 
		 * Thanks to the GREAT INTERNET
		 * 
		 */

		float x = 0.0f;
		float y = 0.0f;
		float z = 0.0f;

		// roll (x-axis rotation)
		float ysqr = q.y * q.y;

		float t0 = 2.0f * (q.w * q.x + q.y * q.z);
		float t1 = 1.0f - 2.0f * (q.x * q.x + ysqr);
		x = Mathf.Atan2 (t0, t1) * Mathf.Rad2Deg;

		float t2 = 2.0f * (q.w * q.y - q.z * q.x);
		if (t2 > 1.0f)
			t2 = 1.0f;
		else if (t2 < -1.0f)
			t2 = -1.0f;
		y = Mathf.Asin (t2) * Mathf.Rad2Deg;

		float t3 = 2.0f * (q.w * q.z + q.x * q.y);
		float t4 = 1.0f - 2.0f * (ysqr + q.z * q.z);
		z = Mathf.Atan2 (t3, t4) * Mathf.Rad2Deg;

		return new Vector3(x * axisMultiplier.x, y * axisMultiplier.y, z * axisMultiplier.z);
	}

	// not used
	//	Vector3 UnityRot2Maya(Quaternion q)
	//	{
	//		float x =  180f / Mathf.PI *Mathf.Atan2(2f * q.x * q.w + 2f * q.y * q.z, 1 - 2f * (q.z*q.z  + q.w*q.w));     // Yaw 
	//		float y =  180f / Mathf.PI *Mathf.Asin(2f * ( q.x * q.z - q.w * q.y ) );                             // Pitch 
	//		float z =  180f / Mathf.PI *Mathf.Atan2(2f * q.x * q.y + 2f * q.z * q.w, 1 - 2f * (q.y*q.y + q.z*q.z));      // Roll 
	//		return new Vector3( (180f-x), y , -z);
	//	}
}
