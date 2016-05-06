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
using System.Collections.Generic;

#endregion // Namespaces

// ######################################################################
// GE2DSky_CloudFlow class handles cloud sprite, it moves sprite along x direction.
// Cloud sprites will be returned at opposite side of which side where they float-off.
// ######################################################################

public class GE2DSky_CloudFlow : GE2DSky_Base2DSky
{

	// ########################################
	// Variables
	// ########################################

	#region Variables

	// Behavior of cloud sprite
	public enum eCloudFlowBehavior
	{
		FlowMixedLeftRight,
		FlowToLeft,
		FlowToRight
	}
	
	// Cloud information
	[System.Serializable]			// Embed this class with sub properties in the inspector. http://docs.unity3d.com/ScriptReference/Serializable.html
	public class Cloud
	{
		public float m_MoveSpeed;					// Move speed of cloud
		public List<GameObject> m_Clouds;  			// Handle of cloud's gameObject
		public List<Vector3> m_OriginalLocalPos;	// LocalPosition before first Update() is called
	}

	[HideInInspector]								// Remark this line if you want to see each cloud details on inspector tab
	public List<Cloud> m_CloudList = null; 			// Array of cloud

	public bool m_Tile = false;						// Set to true if this cloud is fit or larger than the screen width
	public eCloudFlowBehavior m_Behavior = eCloudFlowBehavior.FlowMixedLeftRight;	// Flow behavior of cloud

	public float m_MinSpeed = 0.05f;				// Minimum speed of cloud flow
	public float m_MaxSpeed = 0.3f;					// Maximum speed of cloud flow

	public float m_SpeedMultiplier = 1.0f;

	Vector3 LeftEdgeOfScreen;  						// World position at middle-left of the camera view
	Vector3 RightEdgeOfScreen;                      // World position at middle-right most position at the edge of the camera view
	
	#endregion // Variables

	// ########################################
	// MonoBehaviour Functions
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.html
	// ########################################

	#region Monobehavior

	// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
	void Start()
	{
		// Init cloud list
		m_CloudList = new List<Cloud>();

		int index = 0;

		// Collect all children and put them to m_CloudList
		int SwitchDirectionCount = 0;
		foreach (Transform child in transform)
		{
			// Focus only the active GameObject.
			if (child.gameObject.activeSelf == true)
			{
				// Add new Cloud class to m_CloudList
				m_CloudList.Add(new Cloud());

				// Random speed
				m_CloudList[index].m_MoveSpeed = 0;
				while (m_CloudList[index].m_MoveSpeed == 0)
					m_CloudList[index].m_MoveSpeed = Random.Range(m_MinSpeed, m_MaxSpeed);

				// Cloud flow to left direction
				if (m_Behavior == eCloudFlowBehavior.FlowToLeft)
				{
					m_CloudList[index].m_MoveSpeed = -Mathf.Abs(m_CloudList[index].m_MoveSpeed);
				}
				// Cloud flow to right direction
				else if (m_Behavior == eCloudFlowBehavior.FlowToRight)
				{
					m_CloudList[index].m_MoveSpeed = Mathf.Abs(m_CloudList[index].m_MoveSpeed);
				}
				// Cloud flow left or right direction
				else
				{
					// Random direction
					int randomDirection = Random.Range(0, 2);
					if (randomDirection == 0)
						randomDirection = -1;

					// Switch direction when SwitchDirectionCount is even number
					if (SwitchDirectionCount % 2 == 0)
					{
						m_CloudList[index].m_MoveSpeed = -(randomDirection) * Mathf.Abs(m_CloudList[index].m_MoveSpeed);
					}
					else
					{
						m_CloudList[index].m_MoveSpeed = (randomDirection) * Mathf.Abs(m_CloudList[index].m_MoveSpeed);
					}

					// Increase SwitchDirectionCount
					SwitchDirectionCount++;
				}

				// Init cloud information
				m_CloudList[index].m_Clouds = new List<GameObject>();
				m_CloudList[index].m_Clouds.Add(child.gameObject);

				// Set this GameObject to current m_CloudList.m_Cloud
				m_CloudList[index].m_Clouds[0] = child.gameObject;

				// Keep original LocalPosition to use later when we have to pool this cloud when it move off the screen edge
				m_CloudList[index].m_OriginalLocalPos = new List<Vector3>();
				m_CloudList[index].m_OriginalLocalPos.Add(new Vector3());
				m_CloudList[index].m_OriginalLocalPos[0] = child.gameObject.transform.localPosition;

				// Increase index
				index++;
			}
		}

		// Init cloud position
		RepositionClouds(false);
	}

	// Update is called every frame, if the MonoBehaviour is enabled.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
	void Update()
	{
		// Make sure we have Orthographic Camera
		if (m_Camera == null)
		{
			FindOrthographicCamera();
		}

		// Log warning if there is no Orthographic Camera in the scene
		if (m_Camera == null)
		{
			Debug.LogWarning("There is no Orthographic camera in the scene.");

			// Update nothing
			return;
		}

		// Check all cloud
		int index = 0;
		foreach (Cloud cloudlist in m_CloudList)
		{
			// Checked all GameObjects in current cloud
			foreach (GameObject cloud in cloudlist.m_Clouds)
			{
				// Focus only active cloud
				if (cloud.activeSelf == true)
				{
					// Move current cloud
					cloud.transform.localPosition = new Vector3(cloud.transform.localPosition.x + ((m_CloudList[index].m_MoveSpeed * m_SpeedMultiplier) * Time.deltaTime),
															cloud.transform.localPosition.y,
															cloud.transform.localPosition.z);

					// Check left and right edges of screen
					FindLeftAndRightEdgesOfScreen();

					// If cloud is moving from left to right
					if (m_CloudList[index].m_MoveSpeed > 0)
					{
						// If this cloud move off from right edge of screen
						if (cloud.transform.localPosition.x > RightEdgeOfScreen.x + cloud.GetComponent<Renderer>().bounds.size.x / 2)
						{
							// If this cloud is Large tile cloud
							if (m_Tile == true)
							{
								// Find the left-most cloud in cloud list
								//int MostLeftIndex = 0;
								float MinPosX = Screen.width;
								for (int i = 0; i < cloudlist.m_Clouds.Count; i++)
								{
									if (cloudlist.m_Clouds[i].transform.localPosition.x < MinPosX)
									{
										MinPosX = cloudlist.m_Clouds[i].transform.localPosition.x;
										//MostLeftIndex = i;
									}
								}
                                
								// Bring this cloud back to next to left-most cloud
								//cloud.transform.localPosition = new Vector3(MinPosX - cloud.GetComponent<Renderer>().bounds.size.x,
								//cloud.transform.localPosition.y,
								//cloud.transform.localPosition.z);
							}
							// Otherwise it is small cloud
							else
							{
								// Random new speed
								m_CloudList[index].m_MoveSpeed = Random.Range(m_MinSpeed, m_MaxSpeed);
								//while(m_CloudList[index].m_MoveSpeed==0)
								//	m_CloudList[index].m_MoveSpeed = Random.Range(m_MinSpeed, m_MaxSpeed);

								// Bring this cloud back to left-edge of screen
								//cloud.transform.localPosition = new Vector3(LeftEdgeOfScreen.x - (cloud.GetComponent<Renderer>().bounds.size.x * 0.55f),
								//										Random.Range(-m_Camera.orthographicSize / 2, m_Camera.orthographicSize / 2),
								//										cloud.GetComponent<Renderer>().bounds.size.z);
							}
						}
					}
					// If clod is moving from right to left
					else
					{
						// If this cloud move off from left edge of screen
						if (cloud.transform.localPosition.x < LeftEdgeOfScreen.x - cloud.GetComponent<Renderer>().bounds.size.x / 2)
						{
							// If this cloud is Large tile cloud
							if (m_Tile == true)
							{
								// Find the right-most cloud in cloud list
								//int MostRightIndex = 0;
								float MaxPosX = -Screen.width;
								for (int i = 0; i < cloudlist.m_Clouds.Count; i++)
								{
									if (cloudlist.m_Clouds[i].transform.localPosition.x > MaxPosX)
									{
										MaxPosX = cloudlist.m_Clouds[i].transform.localPosition.x;
										//MostRightIndex = i;
									}
								}

								// Bring this cloud back to next to right-most cloud
								//cloud.transform.localPosition = new Vector3(MaxPosX + cloud.GetComponent<Renderer>().bounds.size.x,
								//cloud.transform.localPosition.y,
								//cloud.transform.localPosition.z);
							}
							// Otherwise it is small cloud
							else
							{
								// Random new speed
								m_CloudList[index].m_MoveSpeed = -Random.Range(m_MinSpeed, m_MaxSpeed);
								//while (m_CloudList[index].m_MoveSpeed == 0)
								//	m_CloudList[index].m_MoveSpeed = Random.Range(m_MinSpeed, m_MaxSpeed);

								// Bring this cloud back to right-edge of screen
								//cloud.transform.localPosition = new Vector3(RightEdgeOfScreen.x + (cloud.GetComponent<Renderer>().bounds.size.x * 0.55f),
								//										Random.Range(-m_Camera.orthographicSize / 2, m_Camera.orthographicSize / 2),
								//										cloud.GetComponent<Renderer>().bounds.size.z);
							}
						}
					}
				}
			}
			index++;
		}
	}


	// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html
	void FixedUpdate()
	{
		// Reposition sprites when screen size is changed
		if (m_PreviousScreenWidth != Screen.width || m_PreviousScreenHeight != Screen.height)
		{
			RepositionClouds(true);

			// Keep current screen width and height
			KeepCurrentScreenSizeInfo();
		}
	}

	#endregion // MonoBehaviour

	// ########################################
	// Utility functions
	// ########################################

	#region Utility Functions

	// Find world positions at left and right edges of Orthographic view
	void FindLeftAndRightEdgesOfScreen()
	{
		// Make sure we have Orthographic Camera
		FindOrthographicCamera();

		// Calculate positions
		if (m_Camera != null)
		{
			LeftEdgeOfScreen = m_Camera.ScreenToWorldPoint(new Vector3(0, 0, 0));
			RightEdgeOfScreen = m_Camera.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
		}
	}

	// Reposition clouds
	void RepositionClouds(bool RemoveOldCloud)
	{

		// Find world positions at left and right edges of Orthographic view
		FindLeftAndRightEdgesOfScreen();

		// Make tile if it is large cloud
		if (m_Tile == true)
		{
			for (int index = 0; index < m_CloudList.Count; index++)
			{
				// Find the nearest center of screen cloud
				int MiddleCloud = 0;
				float MinX = (RightEdgeOfScreen.x * 2);
				for (int i = 0; i < m_CloudList[index].m_Clouds.Count; i++)
				{
					float absX = Mathf.Abs(m_CloudList[index].m_Clouds[i].transform.position.x);
					if (absX < MinX)
					{
						MinX = m_CloudList[index].m_Clouds[i].transform.position.x;
						MiddleCloud = i;
					}
				}

				// Remove all cloud but one at the MiddleCloud index
				/*for (int i = m_CloudList[index].m_Clouds.Count - 1; i >= 0; i--)
				{
					if (i != MiddleCloud)
					{
						Destroy(m_CloudList[index].m_Clouds[i]);
						m_CloudList[index].m_Clouds.RemoveAt(i);
						m_CloudList[index].m_OriginalLocalPos.RemoveAt(i);
					}
				}*/

				// Get SpriteRenderer
				SpriteRenderer sr = m_CloudList[index].m_Clouds[0].GetComponent<SpriteRenderer>();
				if (sr)
				{
					// Get width and height of sprite
					float SpriteWidth = sr.sprite.bounds.size.x;
					//float SpriteHeight = sr.sprite.bounds.size.y;

					// If sprite width is larger than 0
					if (SpriteWidth > 0/* && SpriteHeight > 0*/)
					{
						// Calculate tile count
						int TileCount = (((int)(RightEdgeOfScreen.x * 2) / (int)SpriteWidth) + 8);

						// If cloudlist count is less than TileCount, we do add new cloud
						if (m_CloudList[index].m_Clouds.Count < TileCount)
						{
							int DuplicateCount = 0;

							// Add new cloud
							for (int i = 1; i < TileCount; i++)
							{
								// Set this gameObject to current m_CloudList.m_Cloud
								m_CloudList[index].m_Clouds.Add((GameObject)Instantiate(m_CloudList[index].m_Clouds[0]));

								// Move duplicated clouds as children of this gameObject
								m_CloudList[index].m_Clouds[i].transform.parent = m_CloudList[index].m_Clouds[0].transform.parent;

								// Prepare x position
								float x = 0;
								if (i % 2 == 0)
								{
									x = m_CloudList[index].m_Clouds[0].transform.position.x - (SpriteWidth * (int)((DuplicateCount / 2) + 1));
								}
								else
								{
									x = m_CloudList[index].m_Clouds[0].transform.position.x + (SpriteWidth * (int)((DuplicateCount / 2) + 1));
								}

								// Set position to new cloud
								m_CloudList[index].m_Clouds[i].transform.position = new Vector3(x,
																					m_CloudList[index].m_Clouds[0].transform.position.y,
																					m_CloudList[index].m_Clouds[0].transform.position.z);

								// Keep original LocalPosition for later for bringing it back when it moves out from the screen edge
								m_CloudList[index].m_OriginalLocalPos.Add(new Vector3());
								m_CloudList[index].m_OriginalLocalPos[i] = m_CloudList[index].m_Clouds[i].transform.localPosition;

								// Increase duplicate count
								DuplicateCount++;
							}
						}
					}
				}
			}
		}
	}

	#endregion // Utility Functions
}