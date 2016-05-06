// 2D Sky 1.0.5
//
// Author:	Gold Experience Team (http://www.ge-team.com)
// Details:	https://www.assetstore.unity3d.com/en/#!/content/21555
// Support:	geteamdev@gmail.com
//
// Please direct any bugs/comments/suggestions to support e-mail.

#region Namespaces

using UnityEngine;
using System.Collections;

#endregion // Namespaces

// ######################################################################
// GE2DSky_SkyBG class does strength sprite to fit the orthographic camera view
// ######################################################################

public class GE2DSky_SkyBG : GE2DSky_Base2DSky
{

	// ########################################
	// MonoBehaviour Functions
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.html
	// ########################################

	#region Monobehavior

	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
	void Start () {

		// Resize background sprite
		Resize();
	}

	// Update is called every frame, if the MonoBehaviour is enabled.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
	void Update () {
	}

	// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html
	void FixedUpdate()
	{
		// If screen resolution is changed
		if (m_PreviousScreenWidth != Screen.width || m_PreviousScreenHeight != Screen.height)
		{
			// Resize background sprite
			Resize();

			// Keep current screen width and height information
			KeepCurrentScreenSizeInfo();
		}
	}

	#endregion // MonoBehaviour

	// ########################################
	// Utility functions
	// ########################################

	#region Utility Functions

	// Resize background sprite
	void Resize()
	{
		// Get SpriteRenderer
		SpriteRenderer sr = GetComponent<SpriteRenderer>();
		transform.localScale = new Vector3(1, 1, 1);

		// Get width and height of sprite
		float SpriteWidth = sr.sprite.bounds.size.x;
		float SpriteHeight = sr.sprite.bounds.size.y;

		// Get worldScreenWidth and worldScreenHeight
		float ScreenHeight = m_Camera.orthographicSize * 2f;
		float ScreenWidth = ScreenHeight / Screen.height * Screen.width;
		
		Vector3 DesiredScale = new Vector3(1f, 1f, 1f);

		// Stretch image to fit on the screen
		DesiredScale.x = ScreenWidth / SpriteWidth;
		DesiredScale.y = ScreenHeight / SpriteHeight;

		// Apply change to localScale
		transform.localScale = DesiredScale;
	}

	#endregion // Utility Functions
}
