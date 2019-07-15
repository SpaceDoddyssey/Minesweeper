using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using System;


public class MapAreaController : MonoBehaviour {

	public int mapWidth;
	public int mapHeight;
	public int startX;
	public int startY;
	public int frameCounter;
	public int updateFreq;
	public int curScore;
	public int numMines;

	private bool GameOver;

	public GameObject [,] objectGrid;
	public GameObject gridPrefab;
	public Text ScoreText;

	private Sprite [] tilePool = new Sprite[(int)Sprites.MAX_SPRITE_TYPES];

	// Use this for initialization
	void Start () {

		tilePool[(int)Sprites.BLANK]           = Resources.Load ("Sprites/BlankGridSquare", typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.ONECLOSEMINE]    = Resources.Load ("Sprites/1CloseMine",	    typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.TWOCLOSEMINES]   = Resources.Load ("Sprites/2CloseMines",     typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.THREECLOSEMINES] = Resources.Load ("Sprites/3CloseMines",	    typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.FOURCLOSEMINES]  = Resources.Load ("Sprites/4CloseMines",	    typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.FIVECLOSEMINES]  = Resources.Load ("Sprites/5CloseMines",	    typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.SIXCLOSEMINES]   = Resources.Load ("Sprites/6CloseMines",	    typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.SEVENCLOSEMINES] = Resources.Load ("Sprites/7CloseMines",	    typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.EIGHTCLOSEMINES] = Resources.Load ("Sprites/8CloseMines",	    typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.NINECLOSEMINES]  = Resources.Load ("Sprites/9CloseMines",	    typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.COVERED]         = Resources.Load ("Sprites/FilledGridSquare",typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.MINE]            = Resources.Load ("Sprites/Mine",            typeof(Sprite)) as Sprite;
		tilePool[(int)Sprites.FLAG]            = Resources.Load ("Sprites/Flag",            typeof(Sprite)) as Sprite;

		//gridData = GridSquareData.createDefaultGrid(mapWidth, mapHeight);
		CreateGrid ();
		DisplayGrid ();
		Reset();
	}

	void Reset (){
		ScoreText = GameObject.Find("ScoreText").GetComponent<Text>();
		ScoreText.text = curScore.ToString();

		GameOver = false;
		curScore = 0;
		resetBoard();
		GenerateNewMines();
		RevealStart ();
	}

	void resetBoard (){
		if (objectGrid != null) {
			for (int x = 0; x < mapWidth; x++) {
				for (int y = 0; y < mapHeight; y++) {
					var go = objectGrid [x, y];
					var gridSquare = go.GetComponent<GridSquareController>();
					gridSquare.SetIsRevealed (false);
					gridSquare.SetIsMine (false);
					gridSquare.SetFlag (false);
					gridSquare.SetSprite (tilePool [(int)Sprites.COVERED]);
				}
			}
		}
	}

	public void CreateGrid () {
		//resetSprites ();

		objectGrid = new GameObject [mapWidth, mapHeight]; 
		for (int x = 0; x < mapWidth; x++) {
			for (int y = 0; y < mapHeight; y++) {
				var go = Instantiate(gridPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				go.name = string.Format ("GridSquare{0},{1}", x, y);
				go.transform.parent = gameObject.transform;
				objectGrid [x, y] = go;

				var gridSquare = go.GetComponent<GridSquareController>();
				gridSquare.SetIsRevealed (false);
				gridSquare.xIndex = x;
				gridSquare.yIndex = y;

				EventTrigger trigger = go.AddComponent<EventTrigger> ();

				EventTrigger.Entry entry = new EventTrigger.Entry ();
				entry.eventID = EventTriggerType.PointerClick;
				entry.callback.AddListener ((eventData) => {OnPointerClick(eventData);});
				trigger.triggers.Add (entry);

				BoxCollider box = go.GetComponent <BoxCollider> ();
				box.size = new Vector3 (1.0f, 1.0f, 1.0f);
				box.enabled = true;
			}
		}
	}

	public void DisplayGrid () {
		float displaySize = 1.0f;
		float xStart = 0.0f;
		float yStart = 0.0f;
		float stepSize = 1.0f;

		for (int x = 0; x < mapWidth; x++) {
			float xPos = xStart + (x * stepSize);
			for (int y = 0; y < mapHeight; y++) {
				float yPos = yStart - (y * stepSize);
				objectGrid [x, y].transform.localPosition = new Vector3 (xPos, yPos, 0.0f);
				objectGrid [x, y].transform.localScale = new Vector3 (displaySize, displaySize, 1.0f);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (!GameOver) {
			ScoreText.text = curScore.ToString ();
		} else {
			ScoreText.text = "Game Over!";
		}

		if (Input.GetKey (KeyCode.R)) {
			Reset ();
		}
	}

	void GenerateNewMines (){
		for (int mine = 0; mine < numMines; mine++) {
			int hangPrevent = 0;
			while (hangPrevent < 10000) {
				int mineX = (int)UnityEngine.Random.Range (0.0f, mapWidth);
				int mineY = (int)UnityEngine.Random.Range (0.0f, mapHeight);
				var targetSquare = objectGrid [mineX, mineY].GetComponent<GridSquareController> ();
				if (targetSquare.IsMine()==false) {
					targetSquare.SetIsMine (true);
					break;
				}
				hangPrevent++;
				if (hangPrevent == 10000) {
					Debug.Log ("Mine generation took too long! Whoah Nelly!");
				}
			}
		}
	}

	void RevealStart(){
		int hangPrevent = 0;
		while (hangPrevent < 10000) {
			int startX = (int)UnityEngine.Random.Range (0.0f, mapWidth);
			int startY = (int)UnityEngine.Random.Range (0.0f, mapHeight);
			var targetSquare = objectGrid [startX, startY].GetComponent<GridSquareController> ();
			if (CountNearbyMines (startX, startY) == 0) {
				TileReveal (startX, startY);
				break;
			}
			hangPrevent++;
			if (hangPrevent == 10000) {
				Debug.Log ("Safe start generation took too long! Whoah Nelly!");
			}
		}
	}

	void TileReveal(int x, int y){
		if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight){
		var targetSquare = objectGrid [x, y].GetComponent<GridSquareController> ();
			if (targetSquare.IsRevealed () == false) {
				targetSquare.SetIsRevealed (true);
				int closeMines = (CountNearbyMines (targetSquare.xIndex, targetSquare.yIndex));
				if (targetSquare.IsMine ()) {
					Debug.Log ("Game Over!");
					GameOver = true;
					targetSquare.SetSprite (tilePool [(int)Sprites.MINE]);
				} else {
                    curScore++;
					if (closeMines == 0) {
						targetSquare.SetSprite (tilePool [(int)Sprites.BLANK]);
						// If the tile has no mines adjacent to it, reveal all adjacent tiles 
						TileReveal (x - 1, y + 1);
						TileReveal (x,     y + 1);
						TileReveal (x + 1, y + 1);
						TileReveal (x - 1, y); 
						/* No need to reveal the same tile again */
						TileReveal (x + 1, y);
						TileReveal (x - 1, y - 1);
						TileReveal (x    , y - 1);
						TileReveal (x + 1, y - 1);
					} else if (closeMines == 1) {
						targetSquare.SetSprite (tilePool [(int)Sprites.ONECLOSEMINE]);
					} else if (closeMines == 2) {
						targetSquare.SetSprite (tilePool [(int)Sprites.TWOCLOSEMINES]);
					} else if (closeMines == 3) {
						targetSquare.SetSprite (tilePool [(int)Sprites.THREECLOSEMINES]);
					} else if (closeMines == 4) {
						targetSquare.SetSprite (tilePool [(int)Sprites.FOURCLOSEMINES]);
					} else if (closeMines == 5) {
						targetSquare.SetSprite (tilePool [(int)Sprites.FIVECLOSEMINES]);
					} else if (closeMines == 6) {
						targetSquare.SetSprite (tilePool [(int)Sprites.SIXCLOSEMINES]);
					} else if (closeMines == 7) {
						targetSquare.SetSprite (tilePool [(int)Sprites.SEVENCLOSEMINES]);
					} else if (closeMines == 8) {
						targetSquare.SetSprite (tilePool [(int)Sprites.EIGHTCLOSEMINES]);
					} else if (closeMines == 9) {
						targetSquare.SetSprite (tilePool [(int)Sprites.NINECLOSEMINES]);
					}
				}
			}
		}
	}

	public bool checkForMine(int x, int y){
		var curSquare = objectGrid [x, y];
		var gridSquare = curSquare.GetComponent<GridSquareController>();
		if (gridSquare.IsMine()) {
			return true;
		} else {return false;}
	}

	public int CountNearbyMines (int x, int y) {
		//resetSprites ();
		int closeMines = 0;
		//Check the current tile. If it's a mine, the surrounding mines are irrelevant.
		if (checkForMine (x, y)) {
			return 10;
		}
		//Check the 8 surrounding tiles
		//Check the top row
		if (x - 1 >= 0 && y - 1 >= 0) {if (checkForMine (x - 1, y - 1)) {closeMines++;}}
		if (			  y - 1 >= 0) {if (checkForMine (x, y - 1)) {closeMines++;}}
		if (x + 1 < mapWidth && y - 1 >= 0) {if (checkForMine (x + 1, y - 1)) {closeMines++;}}
		//Check the middle row
		if (x - 1 >= 0)        {if (checkForMine (x - 1, y    )) {closeMines++;}}
		if (x + 1 < mapWidth)  {if (checkForMine (x + 1, y    )) {closeMines++;}}
		//Check the bottom row
		if (x - 1 >= 0 && y + 1 < mapHeight) {if (checkForMine (x - 1, y + 1)) {closeMines++;}}
		if (		      y + 1 < mapHeight) {if (checkForMine (x    , y + 1)) {closeMines++;}}
		if (x + 1 < mapWidth && y + 1 < mapHeight) {if (checkForMine (x + 1, y + 1)) {closeMines++;}}
		return closeMines;
	}

	public void OnPointerClick(BaseEventData eventData){
		if (GameOver != true) {
			PointerEventData data = (PointerEventData)eventData;
			var go = data.pointerPress;
			GridSquareController grid = go.GetComponent<GridSquareController> ();
			if (data.button == PointerEventData.InputButton.Left) {
				if (grid != null && grid.HasFlag () == false) {
					TileReveal (grid.xIndex, grid.yIndex);
				}
			} else if (data.button == PointerEventData.InputButton.Right) {
				if (grid != null && grid.IsRevealed () == false) {
					if (grid.HasFlag() == true) {
						grid.SetFlag (false);
						grid.SetSprite(tilePool [(int)Sprites.COVERED]);
					} else if (grid.HasFlag() == false) {
						grid.SetFlag (true);
						grid.SetSprite(tilePool [(int)Sprites.FLAG]);
					}
					//Debug.Log ("Indices: " + grid.xIndex + " " + grid.yIndex);
				}
			}	
		}
	}
}