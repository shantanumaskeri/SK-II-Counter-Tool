using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{

	#region Public variables

	[Header("Camera Orbit Settings")]
	public Transform target;
	public float xSpeed = 120.0f;
	public float ySpeed = 120.0f;
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	
	[Header("Monobehaviour Scripts")]
	public GameManager gameManager;
	
	#endregion

	#region Private variables

	private float x, y;
	private float distance;
	
	#endregion

	#region Monobehaviours

	// Use this for initialization
	private void OnEnable()
	{
		initializeVariables();
		setupCameraOrbit();	
	}
	
	// Update is called once per frame
	private void Update()
	{
		updateCameraOrbit();	
	}

	#endregion

	#region Custom function - Initialize variables

	private void initializeVariables()
	{
		x = y = 0.0f;
		
		if (gameManager.environmentData.getType() == "counter")
		{
			distance = 7.5f;
		}
		else
		{
			distance = 2.5f;
		}
	}

	#endregion

	#region Custom functions - Orbit camera around object

	private void setupCameraOrbit()
	{
		Vector3 angles = transform.eulerAngles;
		x = angles.y;
		y = angles.x;
	}

	private void updateCameraOrbit()
	{
		if (target)
		{
			x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

			y = ClampAngle(y, yMinLimit, yMaxLimit);

			Quaternion rotation = Quaternion.Euler(y, x, 0);

			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 position = rotation * negDistance + target.position;

			transform.rotation = rotation;
			transform.position = position;
		}
	}

	#endregion

	#region Custom function - Clamp camera angle between minimum and maximum limits

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp(angle, min, max);
	}

	#endregion

}
