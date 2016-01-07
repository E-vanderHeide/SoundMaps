using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class firstperson2D : MonoBehaviour {

	float speed = 1.0f;

	public float XSensitivity = 2f;
	public float YSensitivity = 2f;
	public bool clampVerticalRotation = true;
	public float MinimumX = -90F;
	public float MaximumX = 90F;
	public bool smooth;
	public float smoothTime = 5f;

	// Use this for initialization
	void Start () {
	
	}

	private Quaternion m_CharacterTargetRot;
	
	public void Init(Transform character, Transform camera)
	{
		// initialize with the initial compass value of mobile phone
		
		m_CharacterTargetRot = transform.localRotation;

		Input.gyro.enabled = true;
		Input.location.Start ();
	}

	// Update is called once per frame
	void Update () {
	
	}

	private void FixedUpdate()
	{
		// read inputs
		float h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");
		Vector3 move = new Vector3 (h, v, 0);

		transform.position += move * speed * Time.deltaTime;

	}

	public void LookRotation(Transform character, Transform camera)
	{

		float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
		float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

		//m_CharacterTargetRot *= Quaternion.Euler (0f,yRot,0f);//(z,xx,y),change the direction of walking
		transform.rotation *= Quaternion.Euler (-xRot,0f,0f);
		
		//Benifit of gyro: Calibrate once at the beginning, it won't get disturbed by other magnetic field
//		Gyroscope heading = Input.gyro;
//		
//		m_CharacterTargetRot *= Quaternion.Euler (0f,-2*heading.rotationRateUnbiased.z,0f);//(z,xx,y),change the direction of walking
//		m_CharacterTargetRot *= Quaternion.Euler (-2*heading.rotationRateUnbiased.x,0f,0f);// 

		
		if(smooth)
		{
			transform.localRotation = Quaternion.Slerp (transform.localRotation, m_CharacterTargetRot,
			                                            smoothTime * Time.deltaTime);
		}
		else
		{
			transform.localRotation = m_CharacterTargetRot;
		}
		
	}

	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;
		
		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
		
		angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
		
		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
		
		return q;
	}

	private void OnGUI()
	{
		GUILayout.Label ("Magnetometer reading: " + Input.gyro.attitude);
	}

}
