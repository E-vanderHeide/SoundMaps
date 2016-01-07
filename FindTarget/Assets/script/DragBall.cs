using UnityEngine;
using System.Collections;

// To get the coordination of a map, we can just use the transformation of pin
// and correspond to the map


// To do: how to prevent moving the map when the pin is moved
//        reset

public class DragBall : MonoBehaviour {
	
	public float horizontallimit = 24.0f;
	public float verticallimit = 12.0f;
	public Renderer rend;
	public bool PinMove;

	private float DistanceX;
	private float DistanceZ;

	private bool RunOnce;
	private float t; // the begin time of a touch

	// Use this for initialization
	void Start () {

		transform.position = new Vector3 (-10000, -10000, -10000);
		rend = GetComponent<Renderer> ();
		rend.enabled = false; //invisablize the pin first
		RunOnce = false;
		PinMove = false;

	}


	// Update is called once per frame
	void Update () {

		if(Input.touchCount > 0)
		{
			PinMove = false;
			Touch theTouch = Input.GetTouch(0);

			// Enable the pin with long press
			if (!RunOnce && Input.touchCount == 1 && LongPress(theTouch)) 
			{
				Vector3 p = Camera.main.ScreenToWorldPoint (new Vector3(theTouch.position.x,theTouch.position.y, Camera.main.transform.position.y));
				RunOnce = true;
				transform.position = p;
				rend.enabled = true;
				DistanceX = p.x;
				DistanceZ = p.z;
			}

			// Drag the pin when the finger is on the pin
			if(Input.touchCount == 1 && OnPin(theTouch))
			{

				PinMove = true;
				float DeltaX = theTouch.deltaPosition.x;
				float DeltaZ = theTouch.deltaPosition.y;
			
				DistanceX = Mathf.Clamp(DistanceX + DeltaX*Time.deltaTime*2.5f, -horizontallimit, horizontallimit);
				DistanceZ = Mathf.Clamp(DistanceZ + DeltaZ*Time.deltaTime*2.5f, -verticallimit, verticallimit);

				transform.position = new Vector3(DistanceX,0,DistanceZ);
				//transform.Translate(DeltaX*Time.deltaTime*0.5f,DeltaY*Time.deltaTime*0.5f,0);
			}


		}

	}


	// Need to substract half of the height of quad, with pin image
	private void OnGUI()
	{
		GUILayout.Label ("Geographic coordication: " + transform.position.x + (transform.position.z-0.25f*transform.localScale.z));
	}

	bool LongPress( Touch touch ){

		if (touch.phase == TouchPhase.Began) {

			t = Time.time;
			return false;
		} 
		else if (touch.phase == TouchPhase.Stationary) {

			if (Time.time - t > 1.5) {
				return true;
			}
			return false;
		} 
		else {
			return false;
		}
	}

	bool OnPin(Touch touch){

		float range = 8f;
		Vector3 pos = Camera.main.ScreenToWorldPoint (new Vector3(touch.position.x,touch.position.y, Camera.main.transform.position.y));
		if (transform.position.x > pos.x - range && transform.position.x < pos.x + range &&
			transform.position.y > pos.y - range && transform.position.y < pos.y + range &&
			transform.position.z > pos.z - range && transform.position.z < pos.z + range) 
		{
			return true;
		}

		return false;
	}

}

