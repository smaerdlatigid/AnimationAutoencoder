using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System; 

public class AnimationRecorder : MonoBehaviour {

	public string savefile;
	public bool saving;
    public float saveFPS = 5f;
	StreamWriter sw;
	// Use this for initialization
	void Start () {
		saving = false;
       // sw = new StreamWriter(savefile);
	}
	public List<GameObject> body; 

	float saveTime = 0f;
	string mystring;
	void Update () {
        if (Input.GetKey("space"))
        {
			saving = true;
        }
		else if(Input.GetKeyUp("space"))
		{
			saving = false;
		}

        if (saving & (Time.time > saveTime+1f/saveFPS) )
        {
            mystring = "";
            for (int i = 0; i < body.Count; i++)
            {
                mystring += body[i].transform.position.ToString("f3");
                mystring += body[i].transform.rotation.ToString("f3");
            }
            mystring += transform.position.ToString("f3");
            File.AppendAllText(savefile, mystring + transform.rotation.ToString("f3") + Environment.NewLine);
			saveTime = Time.time;
        }

    }
}
