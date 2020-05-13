using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System; 

public class AnimationRecorder : MonoBehaviour {

    public GameObject character;

    [Header("UI Elements")]
    public Text status;
    public Button RecordButton;

    [Header("Recording Settings")]
    public string savefile = "Assets/Train/test.txt";
    public bool isRecording = false;
    public float recordFPS = 5f;
    float recordTime = 0f;
    public List<GameObject> componentList = new List<GameObject>();
    List<string> poseList = new List<string>();

    void Start () {

        Transform[] allChildren = character.GetComponentsInChildren<Transform>();
        Debug.Log("children count:" + allChildren.Length);

        // recursively add children to list
        componentList.Add(character);
        AddDescendants(character.transform, componentList);
    }

    public void Save()
    {
        StreamWriter writer = new StreamWriter(savefile, true);
        for(int i = 0; i<poseList.Count; i++)
        {
            writer.WriteLine(poseList[i]);
        }
        writer.Close();
        poseList = new List<string>();
    }

    private void AddDescendants(Transform parent, List<GameObject> list)
    {
        foreach (Transform child in parent)
        {
            // compare names or tags if need be
            list.Add(child.gameObject);
            AddDescendants(child, list);
        }
    }

    public void RandomPose()
    {
        if(poseList.Count > 0)
        {
            Debug.Log("Disable Animator to set Random Pose");
            string posestring = poseList[(int)(UnityEngine.Random.Range(0, poseList.Count))];
            string[] joints = posestring.Split(new string[] { ")(" }, StringSplitOptions.None);

            int pi = 0;
            foreach (string floatstring in joints)
            {
                string[] floats = floatstring.Trim('(', ')').Split(',');
                if (pi==0)
                {
                    componentList[pi].transform.position = new Vector3(
                        float.Parse(floats[0]),
                        float.Parse(floats[1]),
                        float.Parse(floats[2])
                    );
                }
                else
                {
                    componentList[pi-1].transform.rotation = new Quaternion(
                        float.Parse(floats[0]),
                        float.Parse(floats[1]),
                        float.Parse(floats[2]),
                        float.Parse(floats[3])
                    );
                }
                pi += 1;

            }

        }
    }

    public void ToggleRecord()
    {
        //Debug.Log("BUTTON");
        if(isRecording)
        {
            RecordButton.GetComponentInChildren<Text>().text = "Record";
            isRecording = false;
        }
        else
        {
            RecordButton.GetComponentInChildren<Text>().text = "Stop";
            isRecording = true;
        }
    }
	
	string writestring = "";
	void Update () {

        if (isRecording & (Time.time > recordTime) )
        {
            recordTime = Time.time + 1f / recordFPS;

            writestring = componentList[0].transform.position.ToString("f3");
            for (int i = 0; i < componentList.Count; i++)
            {
                writestring += componentList[i].transform.rotation.ToString("f3");
            }
            
            poseList.Add(writestring);
        }

        status.text = poseList.Count.ToString();
    }
}
