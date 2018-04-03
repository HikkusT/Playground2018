using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplexScale : MonoBehaviour {

	float amplitude = 1.7f;
	float vel = 2f;
	float scale = 1f;
	// Use this for initialization
	void Start () {
		vel = vel/scale;
	}
	
	// Update is called once per frame
	void Update () {
		double noise = Simplex3D.calcNoise((transform.position.x + Time.time/vel)/scale, (transform.position.y + Time.time/vel)/scale, (transform.position.z + Time.time/vel)/scale);
		transform.localScale = new Vector3((float)(1 + noise/amplitude), (float)(1 + noise/amplitude), (float)(1 + noise/amplitude));
	}
}
