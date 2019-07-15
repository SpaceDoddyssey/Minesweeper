using UnityEngine;
using System.Collections;

public enum Sprites {
	BLANK,
	ONECLOSEMINE,
	TWOCLOSEMINES,
	THREECLOSEMINES,
	FOURCLOSEMINES,
	FIVECLOSEMINES,
	SIXCLOSEMINES,
	SEVENCLOSEMINES,
	EIGHTCLOSEMINES,
	NINECLOSEMINES,
	COVERED,
	MINE,
	FLAG,
	MAX_SPRITE_TYPES
}

public class GridSquareController : MonoBehaviour {

	//private TileType tileType;

	// Use this for initialization
	//private TileType tileType;

	public int xIndex, yIndex;
	private bool isRevealed;
	private bool hasFlag = false;
	private bool isMine;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SetSprite(Sprite spr){
		//tileType = type;
		var renderer = GetComponent<SpriteRenderer> ();
		renderer.sprite = spr;
	}

	public void SetIsRevealed(bool revealState){isRevealed = revealState;}
	public void SetIsMine(bool mineState)      {isMine     = mineState;}
	public void SetFlag(bool flagState)        {hasFlag    = flagState;}

	public bool IsRevealed()  {return isRevealed;}
	public bool HasFlag()     {return hasFlag;}
	public bool IsMine()      {return isMine;}
}
