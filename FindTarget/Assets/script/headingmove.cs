using UnityEngine;
using System.Collections;

public class headingmove : MonoBehaviour {
	

	public float movementSpeed = 2.0f;
	public float counterClockwise = 1000.0f;
	public GameObject target;

	public AudioSource RightDirection;
	public AudioSource ReachTarget;

	//connect to GPS
	private float deltaLat;
	private float deltaLon;
	private LocationInfo lastLocation;
	private LocationInfo currentLocation;
	private int i;

	private Vector3 direction;
	private bool playSound; // To control sound doesn't play like a virus
	private bool playDing;

	void Start () {

		Input.gyro.enabled = true;
		Input.location.Start ();
		i = 0;
		playSound = false;
		playDing = false;

	}
		
	void Update () {

		//use GPS to control
//		currentLocation = Input.location.lastData;
//		if ((currentLocation.latitude != lastLocation.latitude || 
//		    currentLocation.latitude != lastLocation.latitude) && 
//		    i != 0) {
//			deltaLat = currentLocation.latitude - lastLocation.latitude;
//			deltaLon = currentLocation.longitude - lastLocation.longitude;
//	
//			Vector3 m_Move = deltaLon * Vector3.forward + deltaLat * Vector3.right;
//			transform.Translate (m_Move * Time.deltaTime * movementSpeed);
//		}
//
//		lastLocation = currentLocation;
//		i++;

		Gyroscope heading = Input.gyro;
		transform.rotation *= Quaternion.Euler (0f,0f, 1.8f*heading.rotationRateUnbiased.z);//(z,xx,y),change the direction of walking
		//transform.rotation *= Quaternion.Euler (-2*heading.rotationRateUnbiased.x,0f,0f);// 

		Vector3 up = new Vector3 (transform.up.x, 0, transform.up.z);
		direction = target.transform.position - transform.position;
		if (Vector3.Dot (up, direction) < 0) {
			// Play a wall sound
			// RightDirection.Play();

		}

		float alfa = Mathf.Acos (Vector3.Dot (up, direction) / (transform.up.magnitude * direction.magnitude));
		//if (new Vector3(transform.up.x, 0,transform.up.z).normalized == direction.normalized) {
		if (alfa < Mathf.PI / 6 && playDing == false) {
			// Play a DING sound
			RightDirection.Play ();
			playDing = true;
		} else if (alfa >= Mathf.PI / 6) {
			playDing =false;
		}


		if (direction.magnitude < 5f && playSound == false) {
			// if the distance is less than 5m, play a successful sound
			ReachTarget.Play ();
			//ReachTarget.loop = true;
			playSound = true;
		} else if (direction.magnitude >= 5f) {
			// if the distance is larger than 5m, stop playing
			ReachTarget.Stop();
			playSound = false;
		}


		//use keys to control
		if(Input.GetKey(KeyCode.W)) {
			transform.position += new Vector3(transform.up.x, 0,transform.up.z) * Time.deltaTime * movementSpeed;
		}
		else if(Input.GetKey(KeyCode.S)) {
			transform.position -= new Vector3(transform.up.x, 0,transform.up.z) * Time.deltaTime * movementSpeed;
		}
		else if(Input.GetKey(KeyCode.A)) {
			transform.position -= new Vector3(transform.right.x, 0,transform.right.z) * Time.deltaTime * movementSpeed;
		}
		else if(Input.GetKey(KeyCode.D)) {
			transform.position += new Vector3(transform.right.x, 0,transform.right.z) * Time.deltaTime * movementSpeed;
		}

//		// this is counterClockwise rotation
//		if(Input.GetKey(KeyCode.E)) {
//			transform.Rotate(0, 0, Time.deltaTime * counterClockwise);
//		}
//
//		// this is Clockwise rotation
//		else if(Input.GetKey(KeyCode.Q)) {
//			transform.Rotate(0, 0,  - Time.deltaTime * counterClockwise);
//		}
	}

	private void OnGUI()
	{
		GUILayout.Label ("GPS reading: " + Input.location.lastData.longitude + "," + Input.location.lastData.latitude);
		//GUILayout.Label ("Magnetometer reading: " + Input.gyro);
	}
}

