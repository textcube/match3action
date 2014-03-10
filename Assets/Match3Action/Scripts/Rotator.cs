using UnityEngine;
using System.Collections;

/// <summary>
/// To rotate game object with random.
/// </summary>
public class Rotator : MonoBehaviour {
	float counter;
	Quaternion rot, nextRot;

	void DoRandRotate(){
		nextRot = Random.rotation;
		rot = transform.rotation;
		counter = 0;
	}

	void Start () {
		InvokeRepeating("DoRandRotate", 0f, 1f);
	}
	
	void Update () {
		transform.rotation = Quaternion.Lerp(transform.rotation, nextRot, counter*Time.deltaTime);
		counter++;
	}
}
