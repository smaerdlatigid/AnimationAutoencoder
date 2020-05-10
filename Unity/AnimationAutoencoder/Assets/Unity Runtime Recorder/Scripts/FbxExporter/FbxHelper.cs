using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScottGarland;

public static class FbxHelper {

	public static string getFbxSeconds ( int frameIndex, int frameRate ) {
		BigInteger result = new BigInteger("46186158000");
		result = BigInteger.Multiply (result, frameIndex);
		result = BigInteger.Divide (result, frameRate);

		return result.ToString ();
	}

}
