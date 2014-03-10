using UnityEngine;
using System.Collections;

/// <summary>
/// To draw texture scroll animation.
/// </summary>

public class Scroller : MonoBehaviour {
	public Vector2 speed = new Vector2(0f, 0.4f);
	void Update () {
		renderer.material.mainTextureOffset += speed * Time.deltaTime;
	}
}
