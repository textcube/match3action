using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
    Transform tr;
    public float min = -100, max = 500f;

    [HideInInspector]
    public bool on = false;
    Transform panel;

    void Start()
    {
        tr = transform;
        panel = transform.parent.parent;
    }

    void OnOutside()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        Vector3 pos = tr.localPosition + panel.localPosition;
        if (pos.x > max || pos.x < min)
        {
            if (!on) OnOutside();
        }
    }
}
