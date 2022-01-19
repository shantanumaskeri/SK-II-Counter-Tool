using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignOptionsMenu : MonoBehaviour 
{

	#region Public variables

	[Header("Tool Sections")]
	public GameObject EditAccount;
	public GameObject Selection;
	public GameObject CreateCampaign;
	public GameObject Campaigns;

	[Header("UI Components")]
	public Text campaignName;
	public Button createCampaign;

	[Header("Monobehaviour Scripts")]
	public UserData userData;
	public EnvironmentData environmentData;
	public CampaignData campaignData;
	public ApplicationSetup applicationSetup;
	
	#endregion

	#region Private variables

	private string roleName;

	#endregion

	#region Monobehaviours

	private void OnEnable()
	{
		checkUserRole();
		populateCampaignData();
	}

	#endregion

	#region Custom function - Check role of user from login credentials

	private void checkUserRole()
	{
		roleName = userData.GetRoleName();
		switch (roleName)
		{
			case "Reviewer":
				createCampaign.interactable = false;
				break;

			default:
				createCampaign.interactable = true;
				break;
		}
	}

	#endregion

	#region Custom function - Populate name of campaign in text field

	private void populateCampaignData()
	{
		campaignName.text = campaignData.getCampaignName();
	}

	#endregion

	#region Custom function - Move to environment section

	public void moveToEnvironmentSection(string type)
	{
		environmentData.setType(type);

		executeTransitionTo(Selection);
	}

	#endregion

	#region Custom function - Move to edit account section

	public void moveToEditAccountSection()
	{
		executeTransitionTo(EditAccount);
	}

	#endregion

	#region Custom function - Move to create campaign section

	public void moveToCreateCampaignSection()
	{
		executeTransitionTo(CreateCampaign);
	}

	#endregion
	
	#region Custom function - Move to campaigns section
	
	public void moveToCampaignsSection()
	{
		applicationSetup.setApplicationStatus("moveToDraft");
		
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
