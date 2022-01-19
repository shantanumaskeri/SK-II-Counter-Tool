using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeshDetails : MonoBehaviour
{

	#region Private variables

	private bool isTextureSaved = false;
	private float targetRatio;
	private string textureName;
	private string componentName;

	#endregion

	#region Monobehaviours

	// Use this for initialization
	private void OnEnable()
	{
		setRatio();
	}

	#endregion

	#region Custom function - Set & Get ratio of mesh

	private void setRatio()
	{
		if (gameObject.GetComponent<MeshRenderer>() != null)
		{
			Material[] materials = gameObject.GetComponent<MeshRenderer>().materials;
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i].mainTexture != null)
				{
					float width = materials[i].mainTexture.width;
					float height = materials[i].mainTexture.height;

					targetRatio = (float)(width / height);
				}
			}
		}
	}

	public float getRatio()
	{
		return targetRatio;
	}

	#endregion

	#region Custom function - Set & Get texture save status

	public void setTextureSave(bool saved)
	{
		isTextureSaved = saved;
	}

	public bool getTextureSave()
	{
		return isTextureSaved;
	}

	#endregion

	#region Custom function - Set & Get name of texture

	public void setTextureName(string name)
	{
		textureName = name;
	}

	public string getTextureName()
	{
		return textureName;
	}

	#endregion

	#region Custom function - Set & Get name of component

	public void setComponentName(string name)
	{
		componentName = name;
	}

	public string getComponentName()
	{
		return componentName;
	}

	#endregion

}
