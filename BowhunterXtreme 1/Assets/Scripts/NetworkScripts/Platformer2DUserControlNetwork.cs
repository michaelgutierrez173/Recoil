﻿using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;
using UnityStandardAssets.Cameras;

namespace UnityStandardAssets._2D
{
	[RequireComponent(typeof (PlatformerCharacter2D))]
	public class Platformer2DUserControlNetwork : NetworkBehaviour
	{
		private PlatformerCharacter2D m_Character;
		private bool m_Jump;

		private void Awake()
		{
			m_Character = GetComponent<PlatformerCharacter2D> ();
		}

		private void Start(){
			
			if (!isLocalPlayer) {
				Destroy (this);
			} else {
				GameObject.FindGameObjectWithTag ("PlayerCamera").GetComponent<AutoCam> ().SetTarget (this.transform);
			}
		}


		private void Update()
		{
			if (!m_Jump)
			{
				// Read the jump input in Update so button presses aren't missed.
				m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
			}
		}


		private void FixedUpdate()
		{
			// Read the inputs.
			bool crouch = Input.GetKey(KeyCode.LeftControl);
			float h = CrossPlatformInputManager.GetAxis("Horizontal");
			// Pass all parameters to the character control script.
			m_Character.Move(h, crouch, m_Jump);
			m_Jump = false;
		}
	}
}
