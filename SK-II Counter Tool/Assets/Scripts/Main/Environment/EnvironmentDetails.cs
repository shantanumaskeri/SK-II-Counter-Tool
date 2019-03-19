using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnvironmentDetails : MonoBehaviour 
{

	#region Private variables

	private bool isTextureChanged = false;
	private int storeId;
	private List<Texture> textureList = new List<Texture>();
	private string storeType;
	private string campaignName;

	#endregion

	#region Monobehaviours

	private void OnEnable()
	{
		initializeVariables();
	}

	#endregion

	#region Custom function - Initialize variables

	private void initializeVariables()
	{
		storeId = FindObjectOfType<EnvironmentData>().getID();
		storeType = FindObjectOfType<EnvironmentData>().getType();
		campaignName = FindObjectOfType<CampaignData>().getCampaignName();
		campaignName = campaignName.Replace(@" ", "_");
	}

	#endregion

	#region Custom function - Set & Get change in environment texture

	public void setTextureChange(bool change)
	{
		isTextureChanged = change;
	
		PlayerPrefs.SetInt("textureChange_" + campaignName + "_" + storeType + "_" + storeId, System.Convert.ToInt32(isTextureChanged));
	}

	public bool getTextureChange()
	{
		return System.Convert.ToBoolean(PlayerPrefs.GetInt("textureChange_" + campaignName + "_" + storeType + "_" + storeId));
	}

	#endregion

	#region Custom function - Set & Get environment texture list

	public void setListTextures(Texture texture)
	{
		textureList.Add(texture);
	}

	public List<Texture> getListTextures()
	{
		return textureList;
	}

	#endregion

}
