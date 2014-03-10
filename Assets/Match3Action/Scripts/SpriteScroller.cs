using UnityEngine;
using System.Collections;

public class SpriteScroller : MonoBehaviour {
	public Vector3 speed = new Vector3(-0.1f, 0f, 0f);

	// Use this for initialization
	void Start () {
		rigidbody.velocity = speed;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.localPosition.x<-4000f) transform.localPosition = Vector3.zero;
	}
}
