using UnityEngine;
using System.Collections;

public class Drag : MonoBehaviour {
	
	private float DistanceX;
	private float DistanceY;
	
	// Use this for initialization
	void Start () {
		
		DistanceX = transform.position.x;
		DistanceY = transform.position.y;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.touchCount > 0)
		{
			Touch theTouch = Input.GetTouch(0);
			
			// Drag the map
			if(Input.touchCount == 1)
			{
				
				float DeltaX = theTouch.deltaPosition.x;
				float DeltaY = theTouch.deltaPosition.y;
				
				DistanceX = DistanceX + DeltaX*Time.deltaTime*1f;
				DistanceY = DistanceY + DeltaY*Time.deltaTime*1f;
				
				transform.position = new Vector3(DistanceX,DistanceY,0);
				
				//transform.Translate(DeltaX*Time.deltaTime*0.5f,DeltaY*Time.deltaTime*0.5f,0);
			}
			
			// Scale the map
			//if(Input.touchCount == 2)
			
		}
		
		
	}
}
