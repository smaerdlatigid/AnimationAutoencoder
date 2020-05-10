using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class MayaAnimationRecorder : MonoBehaviour {

	Transform[] observeObjs;
	MayaNodeDataContainer[] objAnims;

	// the folder path
	public string saveFolderPath;
	public string saveFileName;

	// would copy and paste animation on the another file
	public string originalMaFilePath;

	// control keys
	public KeyCode startKey = KeyCode.Q;
	public KeyCode endKey = KeyCode.W;

	// other settings
	public bool changeTimeScale = false;
	public float startGameWithTimeScale = 0.0f;
	public float startRecordWithTimeScale = 1.0f;

	// data record settings
	public bool recordPosition = true;
	public bool recordRotation = true;
	public bool recordScale = true;

	// save path name or not
	public bool includePathName = false;

	public bool showLogGUI = false;
	string logMessage = "";

	public bool recordLimitFrames = false;
	public int recordFrames = 1000;

	// prevent feeling like crash
	public int processPerFrame = 20;


	public int frameIndexToStartTransform = 1500;
	bool isTransformStart = false;

	int objNums = 0;

	public int frameIndex = 0;

	bool isStart = false;
	bool isEnd = false;

	void Start () {
		if (changeTimeScale)
			Time.timeScale = startGameWithTimeScale;
		
		SettingRecordItems ();
	}

	// Use this for initialization
	void SettingRecordItems () {

		// get all record objs
		observeObjs = gameObject.GetComponentsInChildren<Transform> ();
		objAnims = new MayaNodeDataContainer[ observeObjs.Length ];

		objNums = objAnims.Length;

		for (int i=0; i< observeObjs.Length; i++) {

			string namePath = observeObjs [i].name;

			// if there are some nodes with same names, include path
			if (includePathName) {
				namePath = AnimationRecorderHelper.GetTransformPathName (transform, observeObjs [i]);
				Debug.Log ("get name: " + namePath);
			}
			objAnims[i] = new MayaNodeDataContainer( observeObjs[i], namePath, saveFolderPath, recordPosition, recordRotation, recordScale );
		}

		ShowLog ("Setting complete");
	}

	void Update () {

		if (Input.GetKeyDown (startKey)) {
			StartRecording ();
		}

		if (Input.GetKeyDown (endKey)) {
			StopRecording ();
		}
	}

	void OnGUI () {
		if (showLogGUI)
			GUILayout.Label (logMessage);
	}


	public void StartRecording () {
		ShowLog ("Recording start");

		if (changeTimeScale)
			Time.timeScale = startRecordWithTimeScale;

		isStart = true;
	}


	public void StopRecording () {
		ShowLog( "End recording, wait for file process ... " );
		isEnd = true;
		StartCoroutine( "EndRecord" );
	}


	// Update is called once per frame
	void LateUpdate () {

		if( isStart )
		{
			if( !isEnd ) {

				// split all curveContainer into many parts
				// only record parts of objects per frame
				//				partIndex = (partIndex+1) % splitPartNum;
				//
				//				int from = amountPerPart * partIndex;
				//				int to = amountPerPart * ( partIndex+1 );
				//
				//				if( to > objAnims.Length )
				//					to = objAnims.Length;

				for( int i=0; i< objNums; i++ )
				{
					objAnims[i].recordFrame( frameIndex );
				}

				frameIndex++;

				// if only record limited frames
				if (recordLimitFrames) {
					
					if (frameIndex > recordFrames) {
						ShowLog ("Recording complete, processing ...");
						isEnd = true;
						StartCoroutine ("EndRecord");
					}
				}
			}
		}

	}

	IEnumerator EndRecord () {

		ShowLog ("Writing Anim Data into Files ...");

		for (int i = 0; i < objAnims.Length; i++) {
			objAnims [i].WriteIntoFile ();

			// prevent lag
			if (i % 10 == 0)
				yield return null;
		}
		
		// save into ma file
		StartCoroutine ("exportToMayaAnimation");
	}


	IEnumerator exportToMayaAnimation () {

		// duplicate originalMaFile
		string newFilePath = saveFolderPath + saveFileName;

		/* 2017-09-26
		 * 
		 * set all spines' Joint Orient to (0,0,0)
		 * this can prevent SkinnedMesh animation export fail
		 * 
		 */
		ShowLog ("Adjusting Spine Joint Orient Values ...");

		string maFileData = File.ReadAllText (originalMaFilePath);
		maFileData = System.Text.RegularExpressions.Regex.Replace (
			maFileData, 
			"\".jo\" -type \"double3\" [^ ]* [^ ]* [^ ]*", 
			"\".jo\" -type \"double3\" 0 0 0"
		);


		// Combine ma file with animation data
		ShowLog ("Combining File into one ...");

		StreamWriter writer = new StreamWriter ( newFilePath );

		// write ma file
		writer.Write (maFileData);

		// copy all file content into one single ma file
		for( int i=0; i< objAnims.Length; i++ )
		{
			StreamReader reader = new StreamReader( objAnims[i].getFinalFilePath() );
			writer.Write( reader.ReadToEnd() );
			reader.Close();
			objAnims[i].cleanFile ();

			if( i % processPerFrame == 0 )
				yield return 0;
		}

		// all files loaded
		writer.Close ();

		ShowLog ("Succeed export animation to : " + newFilePath);
	}

	void ShowLog ( string logStr ) {
		if (showLogGUI)
			logMessage = logStr;
		else
			Debug.Log (logStr);
	}
}
