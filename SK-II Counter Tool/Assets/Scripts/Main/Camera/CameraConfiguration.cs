using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConfiguration : MonoBehaviour
{
	
	#region Public variables
	
	[Header("Monobehaviour Scripts")]
	public GameManager gameManager;
	
	#endregion
	
	#region Custom function - Toggle camera components (orbit/zoom)

	public void toggleComponents(bool toggle)
	{
		GetComponent<CameraOrbit>().enabled = toggle;
		GetComponent<CameraZoom>().enabled = toggle;
	}

	#endregion

	#region Custom function - Reset cameras components

	public void resetCameras(GameObject cam1, GameObject cam2)
	{
		cam1.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
		cam1.transform.localRotation = Quaternion.identity;
		cam1.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
		
		if (gameManager.environmentData.getType() == "counter")
		{
			cam2.transform.position = new Vector3(0.0f, 4.084792f, 6.29003f);
		}
		else
		{
			cam2.transform.position = new Vector3(-0.05311462f, 1.335143f, 2.112953f);
		}
		
		cam2.transform.eulerAngles = new Vector3(33.0f, 180.0f, 0.0f);
		cam2.GetComponent<Camera>().fieldOfView = 60.0f;
	}

	#endregion

}
