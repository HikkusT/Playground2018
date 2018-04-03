using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flow : MonoBehaviour {

	public float scaleFactor = 0.3f;
	public float vel = 1.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = new Vector3(1f + scaleFactor * Mathf.Sin(vel * Time.time), 1f + scaleFactor * Mathf.Sin(vel * Time.time), 1f + scaleFactor * Mathf.Sin(vel * Time.time));
		transform.eulerAngles = new Vector3(-12f + 3f * Mathf.Sin(vel * Time.time), 15f - 4f * Mathf.Cos(vel * Time.time), 0);
	}
}
