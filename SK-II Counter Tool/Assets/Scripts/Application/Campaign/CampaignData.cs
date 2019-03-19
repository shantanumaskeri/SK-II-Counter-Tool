using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class CampaignData : MonoBehaviour 
{

	#region Private variables

	private bool isEditingDraft;
	private string campaignName;
	private string screenshotFolder;
	private string campaignFolder;
	private string publishFolder;
	private string draftFolder;
	private List<string> allCampaigns = new List<string>();
	private string uniqueCampaignName;
	private string createCampaignAPI = "http://ec2-3-0-13-114.ap-southeast-1.compute.amazonaws.com/cms/createCampaign?";
	private string campaignMessage;
	private JObject token;

	#endregion

	#region Monobehaviours

	private void OnEnable()
	{
		setDraftEdit(false);
	}

	#endregion

	#region Custom function - Set & Get name of campaign

	public void setCampaignName(string name)
	{
		campaignName = name;
	}

	public string getCampaignName()
	{
		return campaignName;
	}

	#endregion

	#region Custom function - Set & Get draft editing status

	public void setDraftEdit(bool editing)
	{
		isEditingDraft = editing;
	}

	public bool getDraftEdit()
	{
		return isEditingDraft;
	}

	#endregion

	#region Custom function - Set & Get screenshot folder

	public void setScreenshotFolder(string folder)
	{
		screenshotFolder = folder;
	}

	public string getScreenshotFolder()
	{
		return screenshotFolder;
	}

	#endregion

	#region Custom function - Set & Get campaign root folder

	public void setCampaignRootFolder(string folder)
	{
		campaignFolder = folder;
	}

	public string getCampaignRootFolder()
	{
		return campaignFolder;
	}

	#endregion

	#region Custom function - Set & Get publish folder

	public void setPublishFolder(string folder)
	{
		publishFolder = folder;
	}

	public string getPublishFolder()
	{
		return publishFolder;
	}

	#endregion

	#region Custom function - Set & Get draft folder

	public void setDraftFolder(string folder)
	{
		draftFolder = folder;
	}

	public string getDraftFolder()
	{
		return draftFolder;
	}

	#endregion

	#region Custom function - Set & Get list of all campaigns

	public void setCampaignList(string list)
	{
		allCampaigns.Add(list);
	}

	public List<string> getCampaignList()
	{
		return allCampaigns;
	}

	#endregion

	#region Custom function - Set & Get unique name of campaign

	public void setUniqueCampaignName(string name)
	{
		uniqueCampaignName = name;
	}

	public string getUniqueCampaignName()
	{
		return uniqueCampaignName;
	}

	#endregion

	#region Custom function - Create campaign in backend to publish

	public void createCampaign()
	{
		StartCoroutine(checkCampaignPublishing());
	}

	#endregion

	#region Custom function - Handle campaign creating functionality

	private IEnumerator checkCampaignPublishing()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			// If internet is not available then it will show an error popup
			GetComponent<ApplicationSetup>().showTaskPanel("Please check your internet connection and try again.");
		}
		else
		{
			GetComponent<FtpData>().removeFromFtpFileName();

			WWW campaignLink = new WWW(createCampaignAPI + "campaign_name=" + getCampaignName() + "&created_by=" + GetComponent<UserData>().GetUserID() + "&images=" + GetComponent<FtpData>().getFtpFileName() + "&market_id=" + GetComponent<EnvironmentData>().getMarketID() + "&market_name=" + GetComponent<EnvironmentData>().getMarketName() + "&counter_name=" + GetComponent<EnvironmentData>().getCounterName() + "&fixture_name=" + GetComponent<EnvironmentData>().getFixtureName());
			yield return campaignLink;

			if (campaignLink.error != null)
			{
				GetComponent<ApplicationSetup>().showTaskPanel(campaignLink.error + ". Please try again.");
			}
			else
			{
				token = JObject.Parse(campaignLink.text);
				campaignMessage = token["msg"].ToString();

				if (campaignMessage == "Success")
				{
					GetComponent<ApplicationSetup>().showTaskPanel("Published successfully!");
					GetComponent<ApplicationSetup>().setApplicationStatus("moveToPublish");
				}
				else
				{
					GetComponent<ApplicationSetup>().showTaskPanel("Failed to publish. Please try again.");
					GetComponent<ApplicationSetup>().setApplicationStatus("");
				}
			}
		}
	}

	#endregion

}
