using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

public class MenuManager : MonoBehaviour {
    public Camera uiCam;
    Transform tr;
    Rect screenRect;
    Menu[] nodes;
    int seq = 0;
    bool okDrag = false;
    bool isRoll = false;
    Vector3 mousePos;

    void Start()
    {
        tr = transform;
        nodes = GetComponentsInChildren<Menu>();
        screenRect = new Rect(0, 0, Screen.width, Screen.height * 2f / 3f);
	}

    void OnDoneRollEffect()
    {
        isRoll = false;
    }
    void Roll()
    {
        if (isRoll) return;
        isRoll = true;
        float dist = mousePos.x - Input.mousePosition.x;
        float sign = Mathf.Sign(dist);
        if (Mathf.Abs(dist) < 1f) Application.LoadLevel("Game" + seq);
        int t = seq + (int)sign;
        seq = (t + 3) % 3;
        Vector3 rot = new Vector3(0f, 120f * t, 0f);
        TweenParms tparms = new TweenParms().Prop("eulerAngles", rot).Ease(EaseType.EaseInOutQuad).Id("myTween");
        tparms.OnComplete(OnDoneRollEffect);
        HOTween.Kill("myTween");
        HOTween.To(tr, 0.5f, tparms);
    }

    public bool IsCursorOnUI(Vector3 point)
    {
        if (uiCam != null)
        {
            Ray inputRay = uiCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(inputRay, out hit, Mathf.Infinity, 1 << uiCam.gameObject.layer))
            {
                return true;
            }
        }
        return false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsCursorOnUI(Input.mousePosition))
        {
            okDrag = true;
            mousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0) && !IsCursorOnUI(Input.mousePosition))
        {
            if (okDrag)
            {
                Roll();
                okDrag = false;
            }
        }
    }
}
