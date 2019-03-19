using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.IO;
using Crosstales.FB;

public class CampaignsActions : MonoBehaviour
{

	#region Public variables

	[Header("UI Components")]
	public Image OpenBtn;
    public Image SaveAllBtn;
	public Image PublishAllBtn;
    public Sprite OpenBtnHoverSprite;
    public Sprite OpenBtnNormalSprite;
    public Sprite SaveAllBtnHoverSprite;
    public Sprite SaveAllBtnNormalSprite;
	public Sprite PublishAllBtnHoverSprite;
	public Sprite PublishAllBtnNormalSprite;
	public Sprite MoreBg;
	public Sprite MoreBg2;
	public Button ExtraOptions;

	[Header("Game Objects")]
	public GameObject CenterPanel;
    public GameObject MoreMenu;
	public GameObject Archive;

	[HideInInspector]
	public int totalCounters = 0;
	[HideInInspector]
	public int totalFixtures = 0;
	[HideInInspector]
	public int totalMarkets = 0;
	[HideInInspector]
	public string campaignStatus;
	[HideInInspector]
	public string marketId;

	#endregion

	#region Private variables

	private JObject token;
	private bool mouseOnCenterPanel;
	private string roleName;
	private GameObject application;
	private GameObject campaigns;
	private string CampaignStatusUpdate;

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
		application = GameObject.Find("/Application");
		campaigns = GameObject.Find("/Campaigns");
		roleName = application.GetComponent<UserData>().GetRoleName();
		marketId = "";
		CampaignStatusUpdate = "http://ec2-3-0-13-114.ap-southeast-1.compute.amazonaws.com/cms/createCampaign/edit/?campaign_name=";
	}

	#endregion

	#region Custom function - Mouse hover actions on button (open)

	public void OpenBtnMouseHover()
    {
        OpenBtn.sprite = OpenBtnHoverSprite;
    }

    public void OpenBtnMouseBackFromHover()
    {
        OpenBtn.sprite = OpenBtnNormalSprite;
    }

	#endregion

	#region Custom function - Mouse hover actions on button (save all)

	public void SaveAllBtnMouseHover()
    {
        SaveAllBtn.sprite = SaveAllBtnHoverSprite;
    }

    public void SaveAllBtnMouseBackFromHover()
    {
        SaveAllBtn.sprite = SaveAllBtnNormalSprite;
    }

	#endregion

	#region Custom function - Mouse hover actions on button (publish all)

	public void PublishAllBtnMouseHover()
	{
		PublishAllBtn.sprite = PublishAllBtnHoverSprite;
	}

	public void PublishAllBtnMouseBackFromHover()
	{
		PublishAllBtn.sprite = PublishAllBtnNormalSprite;
	}

	#endregion

	#region Custom function - Mouse hover actions on center panel

	public void CenterPanelMouseHover()
    {
		mouseOnCenterPanel = true;
        CenterPanel.SetActive(true);

		checkCampaignStatusForUI();
    }

    public void CenterPanelBackFromHover()
    {
        mouseOnCenterPanel = false;
        CenterPanel.SetActive(false);
        MoreMenu.SetActive(false);
    }

	#endregion

	#region Custom function - Mouse hover actions on center image

	public void CenterImageMouseHover()
    {
		if (!mouseOnCenterPanel)
        {
            CenterPanel.SetActive(true);
		}

		checkCampaignStatusForUI();
	}

	public void CenterImageBackFromMouseHover()
	{
		CenterPanel.SetActive(false);
		MoreMenu.SetActive(false);
	}

	#endregion

	#region Custom function - Check status of a campaign and show UI

	private void checkCampaignStatusForUI()
	{
		if (campaignStatus == "draft")
		{
			SaveAllBtn.enabled = false;
			OpenBtn.enabled = true;
			PublishAllBtn.enabled = true;
		}
		else
		{
			OpenBtn.enabled = false;
			PublishAllBtn.enabled = false;
			SaveAllBtn.enabled = true;
		}

		if (roleName == "Reviewer")
		{
			OpenBtn.gameObject.GetComponent<Button>().interactable = false;
			PublishAllBtn.gameObject.GetComponent<Button>().interactable = false;
			ExtraOptions.interactable = false;
		}
		else
		{
			OpenBtn.gameObject.GetComponent<Button>().interactable = true;
			PublishAllBtn.gameObject.GetComponent<Button>().interactable = true;
			ExtraOptions.interactable = true;
		}
	}

	#endregion

	#region Custom function - Toggle (show/hide) more options

	public void ShowHideMoreOption()
    {
		if (!MoreMenu.activeSelf)
		{
			MoreMenu.SetActive(true);
		}
		else
		{
			MoreMenu.SetActive(false);
		}

		if (campaignStatus == "publish")
		{
			Archive.SetActive(true);
			MoreMenu.transform.localPosition = new Vector3(128.0f, 67.8f, 0.0f);
			MoreMenu.GetComponent<Image>().sprite = MoreBg;
			MoreMenu.GetComponent<Image>().preserveAspect = true;
			MoreMenu.GetComponent<Image>().SetNativeSize();
			MoreMenu.GetComponent<GridLayoutGroup>().padding.top = 30;
		}
		else
		{
			Archive.SetActive(false);
			MoreMenu.transform.localPosition = new Vector3(128.0f, 79.8f, 0.0f);
			MoreMenu.GetComponent<Image>().sprite = MoreBg2;
			MoreMenu.GetComponent<Image>().preserveAspect = true;
			MoreMenu.GetComponent<Image>().SetNativeSize();
			MoreMenu.GetComponent<GridLayoutGroup>().padding.top = 22;
		}
	}

	#endregion

	#region Custom function - Open asset in draft campaign to edit

	public void OpenAssetToEdit()
    {
        string campaignName = transform.Find("CampaignName").gameObject.GetComponent<Text>().text;
        
        if (campaignStatus == "draft")
        {
			application.GetComponent<CampaignData>().setCampaignName(campaignName);
			application.GetComponent<CampaignData>().setDraftEdit(true);

			campaigns.GetComponent<CampaignMenu>().moveToCampaignOptionsSection();
		}
    }

	#endregion

	#region Custom function - Update status of campaign

	public void updateCampaignStatus(string status)
    {
		StartCoroutine(changeStateTo(status));
    }

	#endregion

	#region Custom function - Change state of campaign to (archive/delete)

	private IEnumerator changeStateTo(string status)
    {
		string campaignName = transform.Find("CampaignName").gameObject.GetComponent<Text>().text;
		
		if (campaignStatus == "draft")
		{
			//string path = campaigns.GetComponent<CampaignMenu>().getScreenshotPath();
			string path = application.GetComponent<CampaignData>().getScreenshotFolder();
			string fullPath = path + "/" + campaignName;

			if (Directory.Exists(fullPath))
			{
				Directory.Delete(fullPath, true);
			}

			if (gameObject != null)
			{
				Destroy(gameObject);

				campaigns.GetComponent<CampaignMenu>().updateCampaignCount("dec");
			}
		}
		else
		{
			WWW statusUpdate = new WWW(CampaignStatusUpdate + campaignName + "&status=" + status);
			yield return statusUpdate;
			token = JObject.Parse(statusUpdate.text);
			
			if (token["error"].ToString() == "false" && token["msg"].ToString() == "Success")
			{
				Destroy(gameObject);
				
				campaigns.GetComponent<CampaignMenu>().updateCampaignCount("dec");
			}
			
			if (status == "delete")
			{
				for (int i = 0; i < application.GetComponent<CampaignData>().getCampaignList().Count; i++)
				{
					if (application.GetComponent<CampaignData>().getCampaignList()[i] == campaignName)
					{
						application.GetComponent<CampaignData>().getCampaignList().RemoveAt(i);
					}
				}
			}
		}
	}

	#endregion

	#region Custom function - Save all snapshots in a campaign

	public void SaveAllSnapshots()
	{
		StartCoroutine(SaveImagesToDirectory());
	}

	#endregion

	#region Custom function - Store snapshot data in image and save to directory

	private IEnumerator SaveImagesToDirectory()
	{
		string campaignName = transform.Find("CampaignName").gameObject.GetComponent<Text>().text;
		string path = Crosstales.FB.FileBrowser.OpenSingleFolder("Save Snapshots for" + campaignName);

		campaigns.GetComponent<CampaignMenu>().WaitPopUp.SetActive(true);
		token = campaigns.GetComponent<CampaignMenu>().getJSONToken();
		string commonPath;

		for (int i = 0; i <= (int)token["campaign_count"] - 1; i++)
		{
			commonPath = token["campaign_detail"][(i + 1).ToString()]["common_image_path"].ToString();
			if (campaignName.Trim() == token["campaign_detail"][(i + 1).ToString()]["name"].ToString())
			{
				//string camapignPath = path + "/" + campaignName;
				for (int m = 0; m <= (int)token["campaign_detail"][(i + 1).ToString()]["market_count"] - 1; m++)
				{
					if ((int)token["campaign_detail"][(i + 1).ToString()]["market"][m]["counter_count"] >= 1)
					{
						for (int c = 0; c <= (int)token["campaign_detail"][(i + 1).ToString()]["market"][m]["counter_count"] - 1; c++)
						{
							System.IO.Directory.CreateDirectory(path + "/" + campaignName + "/" + token["campaign_detail"][(i + 1).ToString()]["market"][m]["market_name"].ToString()
								+ "/" + token["campaign_detail"][(i + 1).ToString()]["market"][m]["counter"][c]["counter_name"].ToString());
							string[] images = token["campaign_detail"][(i + 1).ToString()]["market"][m]["counter"][c]["image_path"].ToString().Split(',');
							for (int ti = 0; ti <= images.Length - 1; ti++)
							{
								campaigns.GetComponent<CampaignMenu>().WaitPopUp.GetComponentInChildren<Text>().text = "Downloading Image   " + (ti + 1) + "   of   " + images.Length + "...";
								WWW imageLink = new WWW(commonPath + images[ti]);
								yield return imageLink;
								Texture2D image = imageLink.texture;
								string fileName = images[ti];
								byte[] bytes = image.EncodeToPNG();
								Destroy(image);
								File.WriteAllBytes(path + "/" + campaignName + "/" + token["campaign_detail"][(i + 1).ToString()]["market"][0]["market_name"].ToString()
									+ "/" + token["campaign_detail"][(i + 1).ToString()]["market"][m]["counter"][c]["counter_name"].ToString() + "/" + fileName, bytes);
							}
						}
					}

					if ((int)token["campaign_detail"][(i + 1).ToString()]["market"][m]["fixture_count"] >= 1)
					{
						for (int c = 0; c <= (int)token["campaign_detail"][(i + 1).ToString()]["market"][m]["fixture_count"] - 1; c++)
						{
							System.IO.Directory.CreateDirectory(path + "/" + campaignName + "/" + token["campaign_detail"][(i + 1).ToString()]["market"][m]["market_name"].ToString()
								+ "/" + token["campaign_detail"][(i + 1).ToString()]["market"][m]["fixture"][c]["fixture_name"].ToString());
							string[] images = token["campaign_detail"][(i + 1).ToString()]["market"][m]["fixture"][c]["image_path"].ToString().Split(',');
							for (int ti = 0; ti <= images.Length - 1; ti++)
							{
								campaigns.GetComponent<CampaignMenu>().WaitPopUp.GetComponentInChildren<Text>().text = "Downloading Image   " + (ti + 1) + "   of   " + images.Length + "...";
								WWW imageLink = new WWW(commonPath + images[ti]);
								yield return imageLink;
								Texture2D image = imageLink.texture;
								string fileName = images[ti];
								byte[] bytes = image.EncodeToPNG();
								Destroy(image);
								File.WriteAllBytes(path + "/" + campaignName + "/" + token["campaign_detail"][(i + 1).ToString()]["market"][0]["market_name"].ToString()
									+ "/" + token["campaign_detail"][(i + 1).ToString()]["market"][m]["fixture"][c]["fixture_name"].ToString() + "/" + fileName, bytes);
							}
						}
					}
				}
			}
		}

		campaigns.GetComponent<CampaignMenu>().WaitPopUp.GetComponentInChildren<Text>().text = "Please Wait...";
		campaigns.GetComponent<CampaignMenu>().WaitPopUp.SetActive(false);
	}

	#endregion

	#region Custom function - Publish all assets in a campaign

	public void PublishAll()
	{
		string campaignName = transform.Find("CampaignName").gameObject.GetComponent<Text>().text;
		string fullName = application.GetComponent<UserData>().GetFullName();
		//string path = campaigns.GetComponent<CampaignMenu>().getScreenshotPath();
		string path = application.GetComponent<CampaignData>().getScreenshotFolder();
		string fullPath = path + "/" + campaignName;

		application.GetComponent<ApplicationSetup>().showTaskPanel("Publishing...");
		application.GetComponent<UserData>().setUniqueUserName(fullName.Replace(@" ", "_"));
		application.GetComponent<CampaignData>().setCampaignName(campaignName);
		application.GetComponent<FtpData>().configureCampaignDeletion(fullPath, gameObject);
		application.GetComponent<FtpData>().prepareFtpData();
		application.GetComponent<FtpData>().executeFtpProtocol();
	}

	#endregion

}
