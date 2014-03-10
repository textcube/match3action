using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
    Transform target, tr;
	void Start () {
        tr = transform;
        target = GameObject.Find("_Target").transform;
	}
	
	void Update () {
        tr.LookAt(target);
	}
}
