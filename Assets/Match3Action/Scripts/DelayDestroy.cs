using UnityEngine;
using System.Collections;

/// <summary>
/// To destroy game object with delay time.
/// </summary>
public class DelayDestroy : MonoBehaviour {
	public float delayTime = 2f;
	void Start () {
		Destroy(gameObject, delayTime);
	}
}
