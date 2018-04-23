using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraBZone : MonoBehaviour {
	void OnEnable()
	{
		GetComponent<Camera>().depthTextureMode =DepthTextureMode.DepthNormals;
	}
	// Use this for initialization
}
