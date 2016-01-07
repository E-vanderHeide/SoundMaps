using UnityEngine;
using System.Collections;

// To get the coordination of a map, we can just use the transformation of pin
// and correspond to the map


// To do: reset

public class DragMap : MonoBehaviour {
		
	public float ZoomSpeed = 0.5f;
	public DragBall pin;

	private float DistanceX;
	private float DistanceY;
	private float DistanceZ;
	
	void Start(){

		DistanceX = transform.position.x;
		DistanceY = transform.position.y;
		DistanceZ = transform.position.z;
	
	}

	// Update is called once per frame
	void Update () {

		//pin.GetComponent<bool>();
		if(Input.touchCount > 0)
		{
			Touch touch0 = Input.GetTouch(0);
			
			// Drag the map
			if(Input.touchCount == 1 && pin.PinMove == false)
			{

				float DeltaX = touch0.deltaPosition.x;
				float DeltaZ = touch0.deltaPosition.y;
				
				DistanceX = DistanceX - DeltaX*Time.deltaTime*0.5f;
				DistanceZ = DistanceZ - DeltaZ*Time.deltaTime*0.5f;
				
				transform.position = new Vector3(DistanceX,DistanceY,DistanceZ);

				//transform.Translate(DeltaX*Time.deltaTime*0.5f,DeltaY*Time.deltaTime*0.5f,0);
			}

			if(Input.touchCount ==2)
			{
				Touch touch1 = Input.GetTouch(1);

				Vector2 touch0PrevPos = touch0.position-touch0.deltaPosition;
				Vector2 touch1PrevPos = touch1.position-touch1.deltaPosition;

				float prevTouchDeltaMag = (touch0PrevPos-touch1PrevPos).magnitude;
				float touchDeltaMag = (touch0.position-touch1.position).magnitude;

				float diff = prevTouchDeltaMag-touchDeltaMag;

				Camera.main.fieldOfView += diff * ZoomSpeed;
				Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView,0.1f,179.9f);

			}
			
		}

	}


}

