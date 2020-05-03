using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TensorFlowLite;


public class AnimationDecoder : MonoBehaviour {

    [Tooltip("Rigged character to map output to")]
    public GameObject character;
    List<GameObject> componentList = new List<GameObject>();

    [Tooltip("Configurable TFLite model.")]
    public string tfliteFileName = "mnist.tflite";
    private Interpreter interpreter;

    [Tooltip("Preprocessing mean feature vector")]
    public string vectorFileName = "mean_pose.txt";
    float[] vmean;
    float[] outputs;

    [Tooltip("Configurable TFLite input tensor data.")]
    public float[] inputs; // Length = input

    [Tooltip("Target Text widget for display of inference execution.")]
    public Text inferenceText;

    private void AddDescendants(Transform parent, List<GameObject> list)
    {
        foreach (Transform child in parent)
        {
            // compare names or tags if need be
            list.Add(child.gameObject);
            AddDescendants(child, list);
        }
    }

    void Awake() {
        // As the demo is extremely simple, there's no need to run at full frame-rate.
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 15;
    }

    void Start () {
        // recursively add children to list
        AddDescendants(character.transform, componentList);

        // load decoder
        string path = Path.Combine(Application.streamingAssetsPath, tfliteFileName);
        interpreter = new Interpreter(FileUtil.LoadFile(path));
                
          Debug.LogFormat( // TODO make UI
            "InputCount: {0}, OutputCount: {1}",
            interpreter.GetInputTensorCount(), // always 1,1
            interpreter.GetOutputTensorCount()
        );

        // TODO public 
        inputs = new float[2];

        // load mean features from file
        path = Path.Combine(Application.streamingAssetsPath, vectorFileName);
        string[] numbers = File.ReadAllLines(path);
        vmean = new float[componentList.Count * 4];

        for (int i = 0; i < numbers.Length; i++)
        {
            if (float.TryParse(numbers[i], out float parsedValue))
            {
                vmean[i] = parsedValue;
            }
        }

        // make sure the character and inference model are compatible
        // Assert.AreEqual(componentList.Count * 4, interpreter.GetOutputTensorCount());
        // interpreter doesn't know input/output sizes... must code manually for now
    }


    void LateUpdate () {
        if (inputs == null) {
            return;
        }

        if (outputs == null) {
            interpreter.ResizeInputTensor(0, new int[]{inputs.Length});
            interpreter.AllocateTensors();
            outputs = new float[componentList.Count * 4];
        }

        float startTimeSeconds = Time.realtimeSinceStartup;
        interpreter.SetInputTensorData(0, inputs);
        interpreter.Invoke();
        interpreter.GetOutputTensorData(0, outputs);
        float inferenceTimeSeconds = Time.realtimeSinceStartup - startTimeSeconds;

        inferenceText.text = string.Format(
            "Inference took {0:0.0000} ms\nInput(s): {1}\n",
            inferenceTimeSeconds * 1000.0,
            ArrayToString(inputs)
        );

        for (int i = 0; i < componentList.Count; i++)
        {
            componentList[i].transform.rotation = new Quaternion(
                vmean[i * 4 + 0] + outputs[i * 4 + 0],
                vmean[i * 4 + 1] + outputs[i * 4 + 1],
                vmean[i * 4 + 2] + outputs[i * 4 + 2],
                vmean[i * 4 + 3] + outputs[i * 4 + 3]
            );
        }
    }

    void OnDestroy() 
    {
        interpreter.Dispose();
    }

    private static string ArrayToString(float[] values) {
        return string.Join(",", values.Select(x => x.ToString()).ToArray());
    }
}
