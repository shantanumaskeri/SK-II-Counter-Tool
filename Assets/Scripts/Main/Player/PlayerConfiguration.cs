using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerConfiguration : MonoBehaviour
{

	#region Custom function - Set height of character controller component

	public void setCharacterHeight()
	{
		switch (FindObjectOfType<EnvironmentData>().getID())
		{
			case 10:
				GetComponent<CharacterController>().height = 1.2f;
				break;

			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
				GetComponent<CharacterController>().height = 0.5f;
				break;

			default:
				GetComponent<CharacterController>().height = 0.8f;
				break;
		}
	}

	#endregion

	#region Custom function - Toggle player components (character controller movement)

	public void toggleComponents(bool toggle)
	{
		GetComponent<CharacterController>().enabled = toggle;
		GetComponent<FirstPersonController>().enabled = toggle;
	}

	#endregion

	#region Custom function - Reset transform of player

	public void resetPlayer()
	{
		transform.position = new Vector3(0.0f, 0.3209289f, 2.0f);
		transform.eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
	}

	#endregion

}
