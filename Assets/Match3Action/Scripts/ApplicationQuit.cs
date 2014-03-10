using UnityEngine;
using System.Collections;

/// <summary>
/// To quit game with Escape Key.
/// </summary>
public class ApplicationQuit : MonoBehaviour {
	void Update () {
		if (Input.GetKey(KeyCode.Escape)) {
			Application.Quit();
		}
	}
}
