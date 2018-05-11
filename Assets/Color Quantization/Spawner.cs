using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
		InvokeRepeating("SpawnCube", 0, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SpawnCube()
	{
		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		cube.AddComponent(typeof(Rigidbody));
		cube.GetComponent<Renderer>().material.SetColor("_Color", Random.ColorHSV());
	}
}
