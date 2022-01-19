using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignDetails : MonoBehaviour 
{

	#region Public variables

	[Header("Tool Sections")]
	public GameObject Selection2;
	public GameObject EditAccount;
	public GameObject Campaigns;

	[Header("UI Components")]
	public InputField campaignNameField;

	[Header("Monobehavior Scripts")]
	public EnvironmentData environmentData;
	public CampaignData campaignData;
	public ApplicationSetup applicationSetup;

	#endregion

	#region Private variables

	private string storeType;
	private string[] specialCharacters = {"!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "=", "+", "/", "{", "}", "|", ":", ";", "<", ",", ">", ".", "?", "~", "`", @"\"};

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
		campaignNameField.text = "";
	}

	#endregion

	#region Custom function - Perform validation of campaign before creating

	public void performCampaignValidation(string type)
	{
		storeType = type;

		if (campaignNameField.text == "")
		{
			applicationSetup.showTaskPanel("Please enter a campaign name.");
		}
		else
		{
			checkCampaignName();
		}
	}

	#endregion

	#region Custom function - Check name of campaign entered

	private void checkCampaignName()
	{
		for (int i = 0; i < campaignData.getCampaignList().Count; i++)
		{
			if (campaignNameField.text == campaignData.getCampaignList()[i].ToString())
			{
				applicationSetup.showTaskPanel("Campaign already exists. Please enter a different name.");

				return;
			}
		}

		for (int j = 0; j < specialCharacters.Length; j++)
		{
			if (campaignNameField.text.Contains(specialCharacters[j]))
			{
				applicationSetup.showTaskPanel("Please enter a valid campaign name.");

				return;
			}
		}

		moveToEnvironmentSection();
	}

	#endregion

	#region Custom function - Move to environment section

	private void moveToEnvironmentSection()
	{
		campaignData.setDraftEdit(false);
		campaignData.setCampaignName(campaignNameField.text);
		environmentData.setType(storeType);

		executeTransitionTo(Selection2);
	}

	#endregion

	#region Custom function - Move to edit account section

	public void moveToEditAccountSection()
	{
		executeTransitionTo(EditAccount);
	}

	#endregion

	#region Custom function - Move to campaigns section

	public void moveToCampaignsSection()
	{
		applicationSetup.setApplicationStatus("moveToPublish");
		
		executeTransitionTo(Campaigns);
	}

	#endregion

	#region Custom function - Execute transition to a section

	private void executeTransitionTo(GameObject section)
	{
		StopAllCoroutines();

		gameObject.SetActive(false);
		section.SetActive(true);
	}

	#endregion

}
