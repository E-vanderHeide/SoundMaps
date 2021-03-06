﻿using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
	[Serializable]
	public class MobileLook
	{
		public float XSensitivity = 2f;
		public float YSensitivity = 2f;
		public bool clampVerticalRotation = true;
		public float MinimumX = -90F;
		public float MaximumX = 90F;
		public bool smooth;
		public float smoothTime = 5f;
		
		
		private Quaternion m_CharacterTargetRot;
		private Quaternion m_CameraTargetRot;
		
		
		public void Init(Transform character, Transform camera)
		{
			// initialize with the initial compass value of mobile phone
			
			m_CharacterTargetRot = character.localRotation;
			m_CameraTargetRot = camera.localRotation;
		}
		
		//-------------------------------------------------------------------------------------------------------------------------------------
		//By Xu: Create a new LookRotation funtion to control facing with gyro
		//In the remote mode, value of compass cannot be returned
		public void LookRotation(Transform character, Transform camera)
		{
			
			//Benifit of gyro: Calibrate once at the beginning, it won't get disturbed by other magnetic field
			Gyroscope heading = Input.gyro;
			
			m_CharacterTargetRot *= Quaternion.Euler (0f,-1*heading.rotationRateUnbiased.z,0f);//(z,x,y),change the direction of walking
			//m_CharacterTargetRot *= Quaternion.Euler (-2*heading.rotationRateUnbiased.x,0f,0f);
			
			if(clampVerticalRotation)
				m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);
			
			if(smooth)
			{
				character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
				                                            smoothTime * Time.deltaTime);
				camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
				                                         smoothTime * Time.deltaTime);
			}
			else
			{
				character.localRotation = m_CharacterTargetRot;
				camera.localRotation = m_CameraTargetRot;
			}
			
		}
		
		//		public void LookRotation(Transform character, Transform camera)
		//		{
		//			float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
		//			float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
		//			
		//			m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
		//			m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);
		//			
		//			if(clampVerticalRotation)
		//				m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);
		//			
		//			if(smooth)
		//			{
		//				character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
		//				                                            smoothTime * Time.deltaTime);
		//				camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
		//				                                         smoothTime * Time.deltaTime);
		//			}
		//			else
		//			{
		//				character.localRotation = m_CharacterTargetRot;
		//				camera.localRotation = m_CameraTargetRot;
		//			}
		//		}
		
		
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
		
	}
}