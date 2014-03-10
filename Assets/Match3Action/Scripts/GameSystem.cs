using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Plugins;

/// <summary>
/// To record the location of data.
/// </summary>
public struct TilePoint {
	public int x, y;
	public TilePoint(int px, int py){
		x = px;
		y = py;
	}
	public override string ToString() {
		 return "(" + x + ", " + y + ")";
	}
}

/// <summary>
/// Default setting and type of tile..
/// </summary>
public class Data {
	public const int tileWidth = 7;
	public const int tileHeight = 7;
	public enum TileTypes  { Empty, Red, Violet, Green, Blue, Purple, Gold, Length };
}

/// <summary>
/// To record the location of tile data.
/// </summary>
public class Cell {
	public Data.TileTypes cellType;
	public bool IsEmpty { get { return cellType == Data.TileTypes.Empty; } }
	public void SetRandomTile(int total) {
		cellType = (Data.TileTypes) UnityEngine.Random.Range(0, total) + 1;
	}
}

/// <summary>
/// Main Game System.
/// </summary>
public class GameSystem : MonoBehaviour {
	string[] sprites = new string[]{"starfish", "sword", "bottle", "bird", "poison", "gold"};
	string[] sounds = new string[]{"perc", "fx", "glass", "coo", "water", "cash"};
	
	public const int cellScale = 100;
	public const int cellWidth = 100;
	public const int cellHeight = 100;

	public GameObject grid, puzzle, panel;
	public GameObject matchItemPrefab;
	public GameObject explosionPrefab;
	public Transform effectArea;
	
	public GameObject[] starEffectPrefabs;
	
	public UISprite choice;
	List<MatchItem> tiles;
	private Cell[,] cells = new Cell[Data.tileWidth, Data.tileHeight];
	public MatchItem curTile = null;

	public AudioClip[] audioMatchClip = null;
	public AudioSource[] audioMatchSource = null;

	public PcControl pcControl;
	public NpcControl npcControl;
	
	bool isDoing = false;
	
	// Setup Audio Source.
	void SetupAudioSource(){
		audioMatchClip = new AudioClip[sounds.Length];
		audioMatchSource = new AudioSource[sounds.Length];
		for (int i=0; i<sounds.Length; i++) {
			audioMatchClip[i] = Resources.Load("Sounds/"+sounds[i], typeof(AudioClip)) as AudioClip;
			audioMatchSource[i] = gameObject.AddComponent<AudioSource>();
			audioMatchSource[i].clip = audioMatchClip[i];
		}
	}

	// Init Tile Grid
	public void InitTileGrid() {
		for (int x = 0; x < Data.tileWidth; x++) {
			for (var y = 0; y < Data.tileHeight; y++) {
				cells[x, y] = new Cell();
				cells[x, y].SetRandomTile(6);
			}
		}
	}

	// Display Tile Grid
	public void DisplayTileGrid() {
		tiles = new List<MatchItem>();
		for (var x = 0; x < Data.tileWidth; x++) {
			for (var y = 0 ; y < Data.tileHeight; y++) {
				int type = (int)cells[x, y].cellType;
				string spriteName = sprites[(type-1)];
				GameObject instance = NGUITools.AddChild(grid, matchItemPrefab) as GameObject;
				instance.GetComponent<UISprite>().spriteName = spriteName;
				instance.transform.localScale = Vector3.one * cellScale;
				instance.transform.localPosition = new Vector3( x * cellWidth, y * -cellHeight, 0f);
				MatchItem tile = instance.GetComponent<MatchItem>();
				tile.target = gameObject;
				tile.cell = cells[x, y];
				tile.point = new TilePoint(x, y);
				tiles.Add(tile);
			}
		}
	}

	// Create Tile Grid
	public void CreateTileGrid() {
		for (int x = 0; x < Data.tileWidth; x++) {
			for (var y = 0; y < Data.tileHeight; y++) {
				cells[x, y] = new Cell();
			}
		}
	}

	// Find Match-3 Tile
	private Dictionary<TilePoint, Data.TileTypes> FindMatch(Cell[,] cells) {
		Dictionary<TilePoint, Data.TileTypes> stack = new Dictionary<TilePoint, Data.TileTypes>();
		for (var x = 0; x < Data.tileWidth; x++) {
			for (var y = 0; y < Data.tileHeight; y++) {
				var thiscell = cells[x, y];
				if (thiscell.IsEmpty) continue;
				int matchCount = 0;
				int y2 = Mathf.Min(Data.tileHeight - 1, y + 2);
				int y1;
				for (y1 = y + 1; y1 <= y2 ;y1++) {
					if (cells[x, y1].IsEmpty || thiscell.cellType != cells[x, y1].cellType) break;
					matchCount++;
				}
				if (matchCount >= 2) {
					y1 = Mathf.Min(Data.tileHeight - 1, y1 - 1);
					for (var y3 = y; y3 <= y1 ;y3++) {
						if (!stack.ContainsKey( new TilePoint(x, y3) ))
							stack.Add(new TilePoint(x, y3) , cells[x, y3].cellType);
					}
				}
			}
		}
		for (var y = 0; y < Data.tileHeight; y++) {
			for (var x = 0; x < Data.tileWidth; x++) {
				var thiscell = cells[x, y];
				if (thiscell.IsEmpty) continue;                    
				int matchCount = 0;
				int x2 = Mathf.Min(Data.tileWidth - 1, x + 3);
				int x1;
				for (x1 = x + 1; x1 <= x2; x1++) {
					if (cells[x1, y].IsEmpty || thiscell.cellType != cells[x1, y].cellType) break;
					matchCount++;
				}
				if (matchCount >= 2) {
					x1 = Mathf.Min(Data.tileWidth - 1, x1 - 1);
					for (var x3 = x; x3 <= x1 ;x3++) {
						if (!stack.ContainsKey( new TilePoint(x3, y) ))
							stack.Add(new TilePoint(x3, y) , cells[x3, y].cellType);
					}
				}
			}
		}
		return stack;
	}

	// Find Match-3 Tile Hint
	private Dictionary<TilePoint, Data.TileTypes> FindHint() {
		Dictionary<TilePoint, Data.TileTypes> stack = new Dictionary<TilePoint, Data.TileTypes>();
		Cell[,] clone = new Cell[Data.tileWidth, Data.tileHeight];
		for (var x = 0; x < Data.tileWidth-1; x++) {
			for (var y = 0; y < Data.tileHeight; y++) {
				System.Array.Copy(cells, clone, Data.tileWidth * Data.tileHeight);
				var thiscell = clone[x, y];
				clone[x, y] = clone[x+1,y];
				clone[x+1,y] = thiscell;
				Dictionary<TilePoint, Data.TileTypes> st = new Dictionary<TilePoint, Data.TileTypes>();
				st = FindMatch(clone);
				if (st.Count>0) {
					TilePoint tp = new TilePoint(x, y);
					if (!stack.ContainsKey(tp)) 
						stack.Add(tp , clone[x, y].cellType);
					tp = new TilePoint(x+1, y);
					if (!stack.ContainsKey(tp)) 
						stack.Add(tp , clone[x+1, y].cellType);
				}
			}
		}
		for (var x = 0; x < Data.tileWidth; x++) {
			for (var y = 0; y < Data.tileHeight-1; y++) {
				System.Array.Copy(cells, clone, Data.tileWidth * Data.tileHeight);
				var thiscell = clone[x, y];
				clone[x, y] = clone[x,y+1];
				clone[x,y+1] = thiscell;
				Dictionary<TilePoint, Data.TileTypes> st = new Dictionary<TilePoint, Data.TileTypes>();
				st = FindMatch(clone);
				if (st.Count>0) {
					TilePoint tp = new TilePoint(x, y);
					if (!stack.ContainsKey(tp)) 
						stack.Add(tp , clone[x, y].cellType);
					tp = new TilePoint(x, y+1);
					if (!stack.ContainsKey(tp)) 
						stack.Add(tp , clone[x, y+1].cellType);
				}
			}
		}
		return stack;
	}

	// Do Empty Tile Move Down
	private void DoEmptyDown() {
		for (var x = 0; x < Data.tileWidth; x++) {
			for (var y = 0; y < Data.tileHeight; y++) {
				var thiscell = cells[x, y];
				if (!thiscell.IsEmpty) continue;
				int y1;
				for (y1 = y; y1 > 0 ;y1--) {
					DoSwapTile( FindTile( new TilePoint(x, y1) ) ,  FindTile( new TilePoint(x, y1-1) ) );
				}
			}
		}
		for (var x = 0; x < Data.tileWidth; x++) {
			int y;
			for (y = Data.tileHeight-1; y >= 0; y--) {
				var thiscell = cells[x, y];
				if (thiscell.IsEmpty) break;
			}
			if (y<0) continue;
			var y1 = y;
			for (y = 0; y <= y1; y++) {
				MatchItem tile = FindTile( new TilePoint(x, y) );
				tile.transform.localPosition = new Vector3( x * cellWidth, (y-(y1+1)) * -cellHeight, 0f);
				tile.cell.SetRandomTile(6);
				string spriteName = sprites[(int)tile.cell.cellType - 1];
				UISprite sprite = tile.GetComponent<UISprite>();
				sprite.spriteName = spriteName;
				sprite.enabled = true;
			}
		}

		foreach (MatchItem tile in tiles) {
			Vector3 pos = new Vector3( tile.point.x * cellWidth, tile.point.y * -cellHeight);
			float dist = Vector3.Distance( tile.transform.localPosition , pos ) * 0.01f;
			dist = 1f;
			TweenParms parms = new TweenParms().Prop("localPosition", pos).Ease(EaseType.EaseOutQuart);
			HOTween.To(tile.transform, 0.5f * dist, parms );
		}
		StartCoroutine( CheckMatch3TileOnly(0.5f) );
	}

	// Find Match-3 Tile
	MatchItem FindTile(TilePoint point){
		foreach (MatchItem tile in tiles) {
			if (tile.point.Equals( point )) return tile;
		}
		return null;
	}

	// Find Match-3 Tile with Position
	MatchItem FindTileWithPosition(Vector2 pos){
		Vector3 pos3 = new Vector3(pos.x, pos.y, 0f) - (grid.transform.localPosition);
		MatchItem curTile = null;
		foreach(MatchItem tile in tiles) {
			if (curTile == null) {
				curTile = tile;
				continue;
			}
			float dist1 = Vector3.Distance( curTile.transform.localPosition , pos3);
			float dist2 = Vector3.Distance( tile.transform.localPosition , pos3);
			if (dist1 > dist2) {
				curTile = tile;
			}
		}
		float dist = Vector3.Distance( curTile.transform.localPosition , pos3);
		return curTile;
	}

	// Swap Motion Animation
	void DoSwapMotion(Transform a, Transform b){
		Vector3 posA = a.localPosition;
		Vector3 posB = b.localPosition;
		TweenParms parms = new TweenParms().Prop("localPosition", posB).Ease(EaseType.EaseOutQuart);
		HOTween.To(a, 0.2f, parms );
		parms = new TweenParms().Prop("localPosition", posA).Ease(EaseType.EaseOutQuart);
		HOTween.To(b, 0.2f, parms );
	}

	// Swap Two Tile
	void DoSwapTile(MatchItem a, MatchItem b) {
		TilePoint p1 = a.point;
		TilePoint p2 = b.point;

		Cell cell = cells[p1.x, p1.y];
		cells[p1.x, p1.y] = cells[p2.x, p2.y];
		cells[p2.x, p2.y] = cell;

		a.point = p2;
		b.point = p1;
	}

	// Attack NPC Monster
	IEnumerator AttackMonster(float delayTime) {
		pcControl.Attack();
		yield return new WaitForSeconds(delayTime);
		npcControl.Damage();
	}

	// Star Flash Effect
	void DoStarEffect(Vector3 pos, int type){
		if (type<1) return;
		pos += grid.transform.localPosition + puzzle.transform.localPosition;
		GameObject starEffectObject = Instantiate(starEffectPrefabs[type-1]) as GameObject;
		Transform starEffect = starEffectObject.transform;
		starEffect.parent = panel.transform;
		Vector3[] path = new Vector3[] { pos, new Vector3(0,100,0), new Vector3(-200,300,0) };
		starEffect.localPosition = pos;
		HOTween.To(starEffect, 0.8f, new TweenParms().Prop("localPosition", new PlugVector3Path(path, EaseType.Linear, true)));
	}

	// Fill Empty Tile
	IEnumerator FillEmpty(float delayTime) {
		yield return new WaitForSeconds(delayTime);
		DoEmptyDown();
	}

	// Check Match-3 Tile
	void CheckMatch3(Dictionary<TilePoint, Data.TileTypes> stack){
		List<MatchItem> destroyList = new List<MatchItem>();
		foreach (KeyValuePair<TilePoint, Data.TileTypes> item in stack){
			destroyList.Add (FindTile(item.Key));
		}
		foreach (MatchItem item in destroyList) {
			audioMatchSource[(int)(item.cell.cellType) - 1].Play();
			
			int type = (int)item.cell.cellType;

			item.cell.cellType = Data.TileTypes.Empty;
			item.GetComponent<UISprite>().enabled = false;

			GameObject instance = Instantiate(explosionPrefab) as GameObject;
			instance.transform.parent = effectArea;
			instance.transform.localPosition = new Vector3( item.point.x * cellWidth, item.point.y * -cellHeight, -5f);
			

			DoStarEffect(instance.transform.localPosition, type);
		}
		StartCoroutine( AttackMonster(0.7f) );
		StartCoroutine( FillEmpty(0.5f) );
	}

	// Find Hint Debug
	void DebugFindHint(){
		Dictionary<TilePoint, Data.TileTypes> st = FindHint();
		string str = "";
		foreach (KeyValuePair<TilePoint, Data.TileTypes> item in st){
			str += item.Key + ", ";
		}
		Debug.Log("FindHint: " + str);
	}

	// Ready Game Trun
	void ReadyGameTurn(){
		isDoing = false;
		DebugFindHint();
	}

	// Check Only Match-3 Tile
	IEnumerator CheckMatch3TileOnly(float delayTime) {
		yield return new WaitForSeconds(delayTime);
		Dictionary<TilePoint, Data.TileTypes> stack = FindMatch(cells);
		if (stack.Count>0) {
			CheckMatch3(stack);
		} else {
			ReadyGameTurn();
		}
	}

	// Check Match-3 Tile
	IEnumerator CheckMatch3Tile(float delayTime, MatchItem a, MatchItem b) {
		yield return new WaitForSeconds(delayTime);
		Dictionary<TilePoint, Data.TileTypes> stack = FindMatch(cells);
		if (stack.Count>0) {
			CheckMatch3(stack);
		} else {
			DoSwapTile(a, b);
			DoSwapMotion(a.transform, b.transform);
			ReadyGameTurn();
		}
	}

	// Click Event
	void OnClickAction(MatchItem tile){
		if (isDoing) return;

		if (tile==null) return;
		if (curTile==null) {
			curTile = tile;
			choice.transform.localPosition = curTile.transform.localPosition;
			choice.enabled = true;
		} else {
			if ( Mathf.Abs( curTile.point.x - tile.point.x ) + Mathf.Abs( curTile.point.y - tile.point.y ) != 1 ) {
				curTile = null;
				choice.enabled = false;
				return;
			}
			isDoing = true;
			DoSwapTile(curTile, tile);
			DoSwapMotion(curTile.transform, tile.transform);
			StartCoroutine( CheckMatch3Tile(0.5f, curTile, tile) );
			curTile = null;
			choice.enabled = false;
		}
	}

    void GotoMenu()
    {
        Application.LoadLevel("Menu");
    }

	// Start Game
	void Start () {
		isDoing = false;
		SetupAudioSource();
		choice.enabled = false;
		CreateTileGrid();
		while (true){
			InitTileGrid();
			Dictionary<TilePoint, Data.TileTypes> stack = FindMatch(cells);
			if (stack.Count<1) break;
		}
		DisplayTileGrid();
		ReadyGameTurn();
	}
}
