using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float stepSize;
	public float cameraZoom = 10.0f;
	private bool isRightMouseDown = false;
	private Vector3 mouseDownRefLoc;
	private Vector3 gridRefLoc;

	// Use this for initialization
	void Start () {
		var camera = GetComponent<Camera> ();
		camera.orthographicSize = cameraZoom;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 delta = new Vector3(0.0f, 0.0f, 0.0f);
		float step = stepSize;
		if (Input.GetKey (KeyCode.LeftControl) || Input.GetKey (KeyCode.RightControl)) {
			step = stepSize * 2.0f;
		}
		if (Input.GetKey(KeyCode.UpArrow)) {
			delta.y += step;
		} else  if (Input.GetKey(KeyCode.DownArrow)) {
			delta.y -= step;
		}
		if (Input.GetKey(KeyCode.RightArrow)) {
			delta.x += step;
		} else  if (Input.GetKey(KeyCode.LeftArrow)) {
			delta.x -= step;
		}
		gameObject.transform.position += delta;
		var zoom = Input.GetAxis ("Mouse ScrollWheel");
		if (zoom != 0) {
			cameraZoom -= zoom*4.0f;
			var camera = GetComponent<Camera> ();
			camera.orthographicSize = cameraZoom;
		}

		if (!isRightMouseDown && Input.GetMouseButton(1)){
			isRightMouseDown = true;
			mouseDownRefLoc = Input.mousePosition;
			gridRefLoc = gameObject.transform.position;
		} else if (isRightMouseDown && !Input.GetMouseButton(1)){
			isRightMouseDown = false;
		}
		if (Input.GetMouseButton(1)) {
			Vector3 dragDelta = Input.mousePosition - mouseDownRefLoc;
			gameObject.transform.position = gridRefLoc - dragDelta*0.05f;
		}
	}
}