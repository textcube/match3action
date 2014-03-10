using UnityEngine;
using System.Collections;

public class DietScroller : MonoBehaviour {
    public Transform panel;
    public int total = 100;
    public int bundle = 3;
    public GameObject itemPrefab;
    public float posX = 0f;
    public float cellWidth = 102;
    public float cellHeight = 102;

    GameObject[] itemList;

    string[] spriteList = new string[] { "bird", "bottle", "gold", "poison", "starfish", "sword" };

    void Start()
    {
        BoxCollider col = collider as BoxCollider;
        col.size = new Vector3(cellWidth * total, cellHeight, 1f);
        col.center = new Vector3(cellWidth * (total - 1) / 2f, 0f, 0f);
        itemList = new GameObject[total];
        AddItem(0);
        AddItem(total - 1);
    }

    void AddItem(int seq)
    {
        if (seq < 0 || seq >= total) return;
        if (itemList[seq]) return;
        GameObject go = NGUITools.AddChild(gameObject, itemPrefab);
        go.transform.localPosition = Vector3.right * seq * cellWidth;
        string str = (seq + 1).ToString("0000");
        UILabel label = go.GetComponentInChildren<UILabel>();
        label.text = str;
        UISprite sprite = go.GetComponentInChildren<UISprite>();
        sprite.spriteName = spriteList[seq%spriteList.Length];
        go.name = "item_" + str;
        Item item = go.GetComponent<Item>();
        item.min = -cellWidth;
        item.max = (bundle) * cellWidth;
        if (seq == 0 || seq == total - 1) item.on = true;
        itemList[seq] = go;
    }

    void Update()
    {
        posX = panel.localPosition.x;
        int pos = Mathf.Abs(Mathf.FloorToInt(posX / cellWidth));
        for (int i = -1; i < bundle; i++)
        {
            int seq = i + pos;
            AddItem(i + pos);
        }
    }
}
