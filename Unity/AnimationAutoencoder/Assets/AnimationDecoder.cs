/* Copyright 2018 The TensorFlow Authors. All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
==============================================================================*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// Simple example demonstrating use of the experimental C# bindings for TensorFlowLite.
/// </summary>
public class AnimationDecoder : MonoBehaviour {

  [Tooltip("Configurable TFLite model.")]
  [SerializeField] string fileName = "mnist.tflite";

  [Tooltip("Preprocessing Vector Mean")]
  [SerializeField] string vectorMean = "mean_pose.txt";

  [Tooltip("Configurable TFLite input tensor data.")]
  public float[] inputs;

  [Tooltip("Target Text widget for display of inference execution.")]
  public Text inferenceText;


  private Interpreter interpreter;
  private float[] outputs;
    public List<GameObject> body;
    void Awake() {
    // As the demo is extremely simple, there's no need to run at full frame-rate.
    QualitySettings.vSyncCount = 0;
    Application.targetFrameRate = 15;
  }

  float[] vmean;
  void Start () {
    interpreter = new Interpreter(FileUtil.LoadFile(fileName));
    Debug.LogFormat(
        "InputCount: {0}, OutputCount: {1}",
        interpreter.GetInputTensorCount(),
        interpreter.GetOutputTensorCount());

    // load vector from file
    string[] numbers = File.ReadAllLines(vectorMean);
    List<float> parsedNumbers = new List<float>();
    for (int i = 0; i < numbers.Length; i++)
    {
        if (float.TryParse(numbers[i], out float parsedValue))
        {
            parsedNumbers.Add(parsedValue);
        }
    }
    vmean = new float[parsedNumbers.Count];
    for (int i =0; i<parsedNumbers.Count; i++)
    {
      vmean[i] = parsedNumbers[i];
    }
    Debug.Log(vmean.Length);
  }

  void LateUpdate () {
    if (inputs == null) {
      return;
    }

    if (outputs == null || outputs.Length != inputs.Length) {
        interpreter.ResizeInputTensor(0, new int[]{inputs.Length});
        interpreter.AllocateTensors();
        outputs = new float[147];
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

    for (int i = 0; i < body.Count; i++)
    {
      body[i].transform.position = new Vector3(
          vmean[i*7] + outputs[i * 7],
          vmean[i * 7+1] + outputs[i * 7+1],
          vmean[i * 7+2] + outputs[i * 7+2]
      );

      body[i].transform.rotation = new Quaternion(
          vmean[i*7+3] + outputs[i*7+3],
          vmean[i*7+4] + outputs[i*7+4], 
          vmean[i*7+5] + outputs[i*7+5],
          vmean[i*7+6] + outputs[i*7+6]
      );
    }
  }

  void OnDestroy() {
    interpreter.Dispose();
  }

   private static string ArrayToString(float[] values) {
    return string.Join(",", values.Select(x => x.ToString()).ToArray());
  }
}
