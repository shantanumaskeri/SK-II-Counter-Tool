using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{

	#region Public variables

	[Header("Camera Zoom Settings")]
	public float zoomSpeed;
	public float orthographicSizeMin;
	public float orthographicSizeMax;
	public float fovMin;
	public float fovMax;

	#endregion

	#region Private variables

	private Camera myCamera;

	#endregion

	#region Monobehaviours

	// Use this for initialization
	private void OnEnable()
	{
		setupCameraZoom();
	}

	// Update is called once per frame
	private void Update()
	{
		updateCameraZoom();
	}

	#endregion

	#region Custom function - Get camera component for zoom

	private void setupCameraZoom()
	{
		myCamera = GetComponent<Camera>();
	}

	#endregion

	#region Custom function - Zoom camera in and out

	private void updateCameraZoom()
	{
		if (myCamera.orthographic)
		{
			if (Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				myCamera.orthographicSize += zoomSpeed;
			}
			if (Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				myCamera.orthographicSize -= zoomSpeed;
			}
			myCamera.orthographicSize = Mathf.Clamp(myCamera.orthographicSize, orthographicSizeMin, orthographicSizeMax);
		}
		else
		{
			if (Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				myCamera.fieldOfView += zoomSpeed;
			}
			if (Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				myCamera.fieldOfView -= zoomSpeed;
			}
			myCamera.fieldOfView = Mathf.Clamp(myCamera.fieldOfView, fovMin, fovMax);
		}
	}

	#endregion

}
