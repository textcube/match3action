using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

/// <summary>
/// To shake pc/npc game object.
/// </summary>
public class Shaker : MonoBehaviour {
	public float time = 1f;
	void Start () {
		Vector3 pos = transform.position - Vector3.up * 0.005f;
		TweenParms parms = new TweenParms().Prop("position", pos).Ease(EaseType.Linear).Loops(-1, LoopType.Yoyo);
		HOTween.To(transform, time, parms );
		Quaternion rot = Quaternion.Euler(0f, 0f, 1f);
		parms = new TweenParms().Prop("rotation", rot).Ease(EaseType.Linear).Loops(-1, LoopType.Yoyo);
		HOTween.To(transform, time, parms );
	}
}
