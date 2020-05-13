using UnityEngine;
using System.Collections;
public class Camera_follow_YogaPack_01: MonoBehaviour
{

	public GameObject player;
	public float cameraHeight = 10.0f;
	public float cameraDistance = 5.0f; //distance from charecter


	void Update() {

		//set one
		Vector3 pos = player.transform.position;

		Debug.Log (pos.x);

		pos.y = cameraHeight;
		pos.x = cameraDistance + pos.x;
		transform.position = pos;
	}
}



