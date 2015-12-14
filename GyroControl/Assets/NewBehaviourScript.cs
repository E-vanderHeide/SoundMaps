using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnGUI() {
		GUILayout.Label("Magnetometer reading: " + Input.compass.rawVector.ToString());
	}
	// Update is called once per frame
	void Update () {
	
	}
}
