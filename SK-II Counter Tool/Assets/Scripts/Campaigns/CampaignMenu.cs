using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using System.Linq;

public class CampaignMenu : MonoBehaviour
{

	#region Public variables

	[Header("Tool Sections")]
	public GameObject CampaignOptions;
	public GameObject CreateCampaign;
	public GameObject EditAccount;

	[Header("Game Objects")]
	public GameObject PublishTab;
	public GameObject ArchivedTab;
	public GameObject DraftsTab;
	public GameObject CampaignPrefab;
	public GameObject campaignParent;
	public GameObject WaitPopUp;
	public GameObject SearchInputField;
	public GameObject ScrollBarHorizontal;
	public GameObject ScrollBarVertical;
	public GameObject CampaignScrollView;

	[Header("UI Components")]
	public Text TotalCampaignText;
	public Dropdown MarketList;
	public Dropdown DateList;
	public Image ListView;
	public Image GridView;
	public Sprite ListImage;
	public Sprite ListImageSelected;
	public Sprite GridImage;
	public Sprite GridImageSelected;
	public Button createCampaign;
	public Button search;

	[Header("Monobehaviour Scripts")]
	public UserData userData;
	public CampaignData campaignData;
	public ApplicationSetup applicationSetup;

	#endregion

	#region Private variables

	private int totalCampaigns;
	private string loginAPI;
	private string CampaignSorting;
	private string roleName;
	private string marketLabel;
	private string counterLabel;
	private string fixtureLabel;
	private DirectoryInfo rootDirectoryInfo;
	private DirectoryInfo imagesDirectoryInfo;
	private DirectoryInfo[] campaignDirectoryInfo;
	private DirectoryInfo[] draftDirectoryInfo;
	private FileInfo[] jpgFileInfo;
	private FileInfo[] pngFileInfo;
	private List<string> availabeMarketList;
	private JObject token;

	#endregion

	#region Monobehaviours

	// Use this for initialization
	private void Start()
	{
		applicationSetup.setApplicationStatus("moveToPublish");
	}

	private void OnEnable()
	{
		initializeVariables();
		checkUserRole();
		createScreenshotDirectory();
		prepareCampaignSection(applicationSetup.getJSONToken());
	}

	#endregion

	#region Custom function - Initialize variables

	private void initializeVariables()
	{
		loginAPI = "http://ec2-3-0-13-114.ap-southeast-1.compute.amazonaws.com/user/login/validateLogin?";
		jpgFileInfo = null;
		pngFileInfo = null;
		availabeMarketList = new List<string>();
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

	#region Custom function - Create root directory to store screenshots

	private void createScreenshotDirectory()
	{
		if (campaignData.getScreenshotFolder() == null || campaignData.getScreenshotFolder().Length == 0)
		{
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
			{
				campaignData.setScreenshotFolder(Application.dataPath);
			}
			if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
			{
				campaignData.setScreenshotFolder(Application.persistentDataPath);
			}

			var stringPath = campaignData.getScreenshotFolder() + "/..";
			campaignData.setScreenshotFolder(Path.GetFullPath(stringPath));
			campaignData.setScreenshotFolder(campaignData.getScreenshotFolder() + "/screenshots");

			System.IO.Directory.CreateDirectory(campaignData.getScreenshotFolder());
		}
	}

	#endregion

	#region Custom function - Prepare campaign section post successful login

	private void prepareCampaignSection(JObject ReceivedToken)
	{
		StopAllCoroutines();
		setJSONToken(ReceivedToken);

		int totalMArketAvailable = (int)getJSONToken()["market_count"];
		availabeMarketList.Add("Markets");
		for (int r = 0; r <= totalMArketAvailable - 1; r++)
		{
			availabeMarketList.Add(getJSONToken()["market_list"][r]["market_name"].ToString());
		}
		MarketList.ClearOptions();
		MarketList.AddOptions(availabeMarketList);
		CampaignSorting = "desc";
		ListView.sprite = ListImageSelected;
		GridView.sprite = GridImage;

		if (applicationSetup.getApplicationStatus() == "moveToDraft")
		{
			ShowUISelection(DraftsTab);
			StartCoroutine(OpenDraft(0));
		}
		else
		{
			ShowUISelection(PublishTab);
			initializeCampaignLoading(getJSONToken(), applicationSetup.getApplicationStatus());
		}

		addCampaignsToList();
		configureScrollBar("List");
	}

	#endregion

	#region Custom function - Set & Get JSON token object

	private void setJSONToken(JObject jsonToken)
	{
		token = jsonToken;
	}

	public JObject getJSONToken()
	{
		return token;
	}

	#endregion

	#region Custom function - Add campaign names to list

	private void addCampaignsToList()
	{
		for (int i = 0; i < (int)getJSONToken()["campaign_count"]; i++)
		{
			campaignData.setCampaignList(getJSONToken()["campaign_detail"][(i + 1).ToString()]["name"].ToString());
		}
	}

	#endregion

	#region Custom function - Start layout change based on list/grid input

	public void changeLayout(string view)
	{
		StartCoroutine(UpdateLyout(view));
	}

	#endregion

	#region Custom function - Update the layout based on view

	private IEnumerator UpdateLyout(string view)
	{
		WaitPopUp.SetActive(true);
		campaignParent.SetActive(false);
		campaignParent.transform.position = new Vector3(0, 0, 0);

		yield return new WaitForSeconds(0.5f);

		if (view == "List")
		{
			campaignParent.SetActive(true);
			CampaignScrollView.GetComponent<ScrollRect>().horizontal = true;
			CampaignScrollView.GetComponent<ScrollRect>().vertical = false;
			campaignParent.GetComponent<GridLayoutGroup>().padding.left = 50;
			campaignParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(350, 350);
			campaignParent.GetComponent<GridLayoutGroup>().spacing = new Vector2(20, 50);
			campaignParent.GetComponent<GridLayoutGroup>().startCorner = GridLayoutGroup.Corner.UpperLeft;
			campaignParent.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
			campaignParent.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedRowCount;
			campaignParent.GetComponent<GridLayoutGroup>().constraintCount = 1;
			campaignParent.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			campaignParent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
			ListView.sprite = ListImageSelected;
			GridView.sprite = GridImage;
		}
		else if (view == "Grid")
		{
			campaignParent.SetActive(true);
			campaignParent.GetComponent<GridLayoutGroup>().startAxis = GridLayoutGroup.Axis.Horizontal;
			campaignParent.GetComponent<GridLayoutGroup>().cellSize = new Vector2(350, 350);
			campaignParent.GetComponent<GridLayoutGroup>().spacing = new Vector2(95, 20);
			campaignParent.GetComponent<GridLayoutGroup>().constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			campaignParent.GetComponent<GridLayoutGroup>().constraintCount = 3;
			CampaignScrollView.GetComponent<ScrollRect>().horizontal = false;
			CampaignScrollView.GetComponent<ScrollRect>().vertical = true;
			campaignParent.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
			campaignParent.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
			ListView.sprite = ListImage;
			GridView.sprite = GridImageSelected;
		}

		WaitPopUp.SetActive(false);

		configureScrollBar(view);
	}

	#endregion

	#region Custom function - Configure the scroll bar based on view

	private void configureScrollBar(string view)
	{
		if (view == "List")
		{
			CampaignScrollView.GetComponent<ScrollRect>().verticalScrollbar = null;
			CampaignScrollView.GetComponent<ScrollRect>().horizontalScrollbar = ScrollBarHorizontal.GetComponent<Scrollbar>();
			CampaignScrollView.GetComponent<ScrollRect>().horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
			CampaignScrollView.GetComponent<ScrollRect>().horizontalScrollbarSpacing = -3;

			ScrollBarVertical.SetActive(false);
			ScrollBarHorizontal.SetActive(true);
		}

		if (view == "Grid")
		{
			CampaignScrollView.GetComponent<ScrollRect>().horizontalScrollbar = null;
			CampaignScrollView.GetComponent<ScrollRect>().verticalScrollbar = ScrollBarVertical.GetComponent<Scrollbar>();
			CampaignScrollView.GetComponent<ScrollRect>().verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
			CampaignScrollView.GetComponent<ScrollRect>().verticalScrollbarSpacing = -3;

			ScrollBarHorizontal.SetActive(false);
			ScrollBarVertical.SetActive(true);
		}
	}

	#endregion

	#region Custom function - Show published campaigns on tab selection

	public void PublishClicked()
	{
		ShowUISelection(PublishTab);
		initializeCampaignLoading(getJSONToken(), "moveToPublish");
	}

	#endregion

	#region Custom function - Show archived campaigns on tab selection

	public void ArchiveClicked()
	{
		ShowUISelection(ArchivedTab);
		initializeCampaignLoading(getJSONToken(), "moveToArchive");
	}

	#endregion

	#region Custom function - Show draft campaigns on tab selection

	public void DraftsClicked()
	{
		ShowUISelection(DraftsTab);
		StartCoroutine(OpenDraft(0));
	}

	#endregion

	#region Custom function - Show selection of UI button when clicked

	private void ShowUISelection(GameObject tab)
	{
		ResetAllTab();
		tab.GetComponentInChildren<Image>().enabled = true;
		tab.GetComponent<Text>().color = new Color32(0, 0, 0, 255);
	}

	#endregion

	#region Custom function - Reset all tabs

	public void ResetAllTab()
	{
		StopAllCoroutines();

		PublishTab.GetComponent<Text>().color = new Color32(143, 139, 140, 255);
		ArchivedTab.GetComponent<Text>().color = new Color32(143, 139, 140, 255);
		DraftsTab.GetComponent<Text>().color = new Color32(143, 139, 140, 255);
		PublishTab.GetComponentInChildren<Image>().enabled = false;
		ArchivedTab.GetComponentInChildren<Image>().enabled = false;
		DraftsTab.GetComponentInChildren<Image>().enabled = false;
	}

	#endregion

	#region Custom function - Toggle (enable/disable) interaction with campaigns in a section

	private void toggleCampaignInteraction(bool toggle, string functionName)
	{
		WaitPopUp.SetActive(toggle);
		DateList.interactable = MarketList.interactable = search.interactable = !toggle;

		if (toggle == false)
		{
			StopCoroutine(functionName);
		}
	}

	#endregion

	#region Custom function - Initialize loading of campaigns (publish/archive)

	private void initializeCampaignLoading(JObject token, string status)
	{
		applicationSetup.setApplicationStatus(status);

		string campaignStatus = applicationSetup.getApplicationStatus().Substring(6);
		campaignStatus = campaignStatus.ToLower();

		StartCoroutine(LoadCampaigns(token, campaignStatus));
	}

	#endregion

	#region Custom function - Load publish or archive campaigns

	private IEnumerator LoadCampaigns(JObject token, string status)
	{
		totalCampaigns = 0;
		TotalCampaignText.text = "No Campaigns";

		toggleCampaignInteraction(true, "");

		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			applicationSetup.showTaskPanel("Please check your internet connection and try again");
		}
		else
		{
			string commonPath;
			WWW loginLink = new WWW(loginAPI + "username=" + userData.GetUserName() + "&password=" + userData.GetPassword() + "&sort=" + CampaignSorting + "&market_id=");
			yield return loginLink;
			
			setJSONToken(JObject.Parse(loginLink.text));
			UnloadCampaigns();

			for (int i = 0; i <= (int)getJSONToken()["campaign_count"] - 1; i++)
			{
				if (status == getJSONToken()["campaign_detail"][(i + 1).ToString()]["status"].ToString())
				{
					commonPath = getJSONToken()["campaign_detail"][(i + 1).ToString()]["common_image_path"].ToString();
					WWW imageLink = new WWW(commonPath + getJSONToken()["campaign_detail"][(i + 1).ToString()]["images"][0].ToString());
					yield return imageLink;
					GameObject instance = Instantiate(CampaignPrefab, transform.position, transform.rotation) as GameObject;
					instance.transform.SetParent(campaignParent.transform, false);
					instance.transform.Find("CenterImage").gameObject.GetComponent<Image>().sprite = Sprite.Create(imageLink.texture, new Rect(0.0f, 0.0f, imageLink.texture.width, imageLink.texture.height), new Vector2(0.5f, 0.5f), 100.0f);
					instance.transform.Find("CenterImage").gameObject.GetComponent<Image>().preserveAspect = true;
					instance.transform.Find("CampaignName").gameObject.GetComponent<Text>().text = getJSONToken()["campaign_detail"][(i + 1).ToString()]["name"].ToString();
					instance.transform.Find("DaysText").gameObject.GetComponent<Text>().text = getJSONToken()["campaign_detail"][(i + 1).ToString()]["last_updated_at"].ToString();
					instance.transform.Find("Market").gameObject.GetComponent<Text>().text = getJSONToken()["campaign_detail"][(i + 1).ToString()]["market_count"] + " Market • " + getJSONToken()["campaign_detail"][(i + 1).ToString()]["counter_count"] + " Counters • " + getJSONToken()["campaign_detail"][(i + 1).ToString()]["fixture_count"] + " Fixtures";
					instance.GetComponent<CampaignsActions>().campaignStatus = status;

					TotalCampaignText.gameObject.SetActive(true);

					updateCampaignCount("inc");
				}
			}
		}

		toggleCampaignInteraction(false, "LoadCampaigns");
	}

	#endregion

	#region Custom function - Open draft section of campaigns

	private IEnumerator OpenDraft(int sort)
	{
		totalCampaigns = 0;
		TotalCampaignText.text = "No Campaigns";
		
		applicationSetup.setApplicationStatus("moveToDraft");
		
		toggleCampaignInteraction(true, "");
		UnloadCampaigns();
		
		yield return null;
		PrepareDraft(sort);
	}

	#endregion

	#region Custom function - Prepare content of draft section

	private void PrepareDraft(int sort)
	{
		if (!Directory.Exists(campaignData.getScreenshotFolder()))
		{
			TotalCampaignText.text = "No Campaigns";
		}
		else
		{
			rootDirectoryInfo = new DirectoryInfo(campaignData.getScreenshotFolder());

			if (sort == 1)
			{
				campaignDirectoryInfo = rootDirectoryInfo.GetDirectories().OrderBy(p => p.LastWriteTime).ToArray();
			}
			else
			{
				campaignDirectoryInfo = rootDirectoryInfo.GetDirectories().OrderByDescending(p => p.LastWriteTime).ToArray();
			}
			
			if (campaignDirectoryInfo.Length > 0)
			{
				for (int i = 0; i < campaignDirectoryInfo.Length; i++)
				{
					if (Directory.Exists(campaignDirectoryInfo[i].FullName))
					{
						draftDirectoryInfo = new DirectoryInfo(campaignDirectoryInfo[i].FullName).GetDirectories();
						for (int j = 0; j < draftDirectoryInfo.LongLength; j++)
						{
							if (draftDirectoryInfo[j].Name == "Drafts")
							{
								if (Directory.Exists(draftDirectoryInfo[j].FullName))
								{
									imagesDirectoryInfo = new DirectoryInfo(draftDirectoryInfo[j].FullName);
									jpgFileInfo = imagesDirectoryInfo.GetFiles("*.jpg*");
									pngFileInfo = imagesDirectoryInfo.GetFiles("*.png*");

									if (jpgFileInfo.Length == 0 && pngFileInfo.Length == 0)
									{
										TotalCampaignText.text = "No Campaigns";
										toggleCampaignInteraction(false, "OpenDraft");
		
										return;
									}

									updateCampaignCount("inc");
									populateCampaignsInDraft(i, j);
								}
								else
								{
									TotalCampaignText.text = "No Campaigns";
								}
							}
							else
							{
								TotalCampaignText.text = "No Campaigns";
							}
						}
					}
					else
					{
						TotalCampaignText.text = "No Campaigns";
					}
				}
			}
			else
			{
				TotalCampaignText.text = "No Campaigns";
			}
		}
		
		toggleCampaignInteraction(false, "OpenDraft");
	}

	#endregion
	
	#region Custom function - Get time since last modified the draft

	private string getTimeSinceLastModified(int draftDirIndex)
	{
		DateTime lastModified = Directory.GetLastWriteTime(draftDirectoryInfo[draftDirIndex].FullName);
		DateTime current = DateTime.Now;
		TimeSpan timeSpan = current - lastModified;

		int years = timeSpan.Days / 365;
		int weeks = timeSpan.Days / 7;
		string output = "";

		if (years > 0)
		{
			if (years == 1)
			{
				output = years + " year ago";
			}
			else
			{
				output = years + " years ago";
			}
		}
		else if (weeks > 0)
		{
			if (weeks == 1)
			{
				output = weeks + " week ago";
			}
			else
			{
				output = weeks + " weeks ago";
			}
		}
		else if (timeSpan.Days > 0)
		{
			if (timeSpan.Days == 1)
			{
				output = timeSpan.Days + " day ago";
			}
			else
			{
				output = timeSpan.Days + " days ago";
			}
		}
		else if (timeSpan.Hours > 0)
		{
			if (timeSpan.Hours == 1)
			{
				output = timeSpan.Hours + " hour ago";
			}
			else
			{
				output = timeSpan.Hours + " hours ago";
			}
		}
		else if (timeSpan.Minutes > 0)
		{
			if (timeSpan.Minutes == 1)
			{
				output = timeSpan.Minutes + " minute ago";
			}
			else
			{
				output = timeSpan.Minutes + " minutes ago";
			}
		}
		else
		{
			output = "just now";
		}

		return output;
	}

	#endregion

	#region Custom function - Populate campaigns in draft section

	private void populateCampaignsInDraft(int campaignDirIndex, int draftDirIndex)
	{
		if (jpgFileInfo.Length > 0 && pngFileInfo.Length == 0)
		{
			displayDraftCampaigns(jpgFileInfo, campaignDirIndex, draftDirIndex);
		}
		else if (pngFileInfo.Length > 0 && jpgFileInfo.Length == 0)
		{
			displayDraftCampaigns(pngFileInfo, campaignDirIndex, draftDirIndex);
		}
		else if (pngFileInfo.Length > 0 && jpgFileInfo.Length > 0)
		{
			int _random = UnityEngine.Random.Range(0, 2);
			if (_random == 0)
			{
				displayDraftCampaigns(jpgFileInfo, campaignDirIndex, draftDirIndex);
			}
			else
			{
				displayDraftCampaigns(pngFileInfo, campaignDirIndex, draftDirIndex);
			}
		}
	}

	#endregion

	#region Custom function - Display campaigns in draft section

	private void displayDraftCampaigns(FileInfo[] fileInfos, int campaignDirIndex, int draftDirIndex)
	{
		WWW imageLink = new WWW(fileInfos[0].FullName);
		GameObject instance = Instantiate(CampaignPrefab, transform.position, transform.rotation) as GameObject;
		instance.transform.SetParent(campaignParent.transform, false);
		instance.transform.Find("CenterImage").gameObject.GetComponent<Image>().sprite = Sprite.Create(imageLink.texture, new Rect(0.0f, 0.0f, imageLink.texture.width, imageLink.texture.height), new Vector2(0.5f, 0.5f), 100.0f); ;
		instance.transform.Find("CenterImage").gameObject.GetComponent<Image>().preserveAspect = true;
		instance.transform.Find("CampaignName").gameObject.GetComponent<Text>().text = campaignDirectoryInfo[campaignDirIndex].Name;
		instance.transform.Find("DaysText").gameObject.GetComponent<Text>().text = getTimeSinceLastModified(draftDirIndex);
		instance.GetComponent<CampaignsActions>().campaignStatus = "draft";
		TotalCampaignText.gameObject.SetActive(true);
		Array.Clear(fileInfos, 0, fileInfos.Length);

		getAssetDataFromFile(campaignDirectoryInfo[campaignDirIndex].Name, instance);
	}

	#endregion

	#region Custom function - Get asset data from text file

	private void getAssetDataFromFile(string campaignName, GameObject main)
	{
		string file = campaignData.getScreenshotFolder() + "/DO_NOT_DELETE.txt";
		
		if (!File.Exists(file))
		{
			return;
		}
		else
		{
			StreamReader reader = new StreamReader(file);

			while (!reader.EndOfStream)
			{
				string fileContents = reader.ReadLine();
				string[] lines = fileContents.Split(',');

				if (campaignName == lines[0])
				{
					if (lines[1] != main.GetComponent<CampaignsActions>().marketId)
					{
						main.GetComponent<CampaignsActions>().totalMarkets += 1;

						if (lines[2] == "counter")
						{
							main.GetComponent<CampaignsActions>().totalCounters += 1;
						}
						if (lines[2] == "fixture")
						{
							main.GetComponent<CampaignsActions>().totalFixtures += 1;
						}

						main.GetComponent<CampaignsActions>().marketId = lines[1];
					}
					else
					{
						if (lines[2] == "counter")
						{
							main.GetComponent<CampaignsActions>().totalCounters += 1;
						}
						if (lines[2] == "fixture")
						{
							main.GetComponent<CampaignsActions>().totalFixtures += 1;
						}
					}

					updateCampaignAssetCount(main);
				}
			}
			
			reader.Close();
		}
	}

	#endregion

	#region Custom function - Update total asset count in a campaign

	private void updateCampaignAssetCount(GameObject main)
	{
		if (main.GetComponent<CampaignsActions>().totalMarkets == 1)
		{
			marketLabel = "Market";
		}
		else
		{
			marketLabel = "Markets";
		}
		if (main.GetComponent<CampaignsActions>().totalCounters == 1)
		{
			counterLabel = "Counter";
		}
		else
		{
			counterLabel = "Counters";
		}
		if (main.GetComponent<CampaignsActions>().totalFixtures == 1)
		{
			fixtureLabel = "Fixture";
		}
		else
		{
			fixtureLabel = "Fixtures";
		}
		
		main.transform.Find("Market").gameObject.GetComponent<Text>().text = main.GetComponent<CampaignsActions>().totalMarkets + " " + marketLabel + " • " + main.GetComponent<CampaignsActions>().totalCounters + " " + counterLabel + " • " + main.GetComponent<CampaignsActions>().totalFixtures + " " + fixtureLabel;

	}

	#endregion

	#region Custom function - Sort campaigns by date of creation/publishing

	public void SortCampainsByDate(int sort)
	{
		if (applicationSetup.getApplicationStatus() == "moveToDraft")
		{
			StartCoroutine(OpenDraft(sort));
		}
		else
		{
			if (sort == 0)
			{
				CampaignSorting = "desc";
			}

			if (sort == 1)
			{
				CampaignSorting = "asc";
			}

			initializeCampaignLoading(token, applicationSetup.getApplicationStatus());
		}
	}

	#endregion

	#region Custom function - Sort campaigns by markets

	public void SortCampainsByMarket(int sort)
	{
		if (applicationSetup.getApplicationStatus() == "moveToDraft")
		{
			getMarketDataFromFile(sort);
		}
		else
		{
			if (sort == 1)
			{
				CampaignSorting = "desc";
			}

			if (sort == 2)
			{
				CampaignSorting = "asc";
			}

			initializeCampaignLoading(token, applicationSetup.getApplicationStatus());
		}
	}

	#endregion

	#region Custom function - Get market data from text file

	private void getMarketDataFromFile(int sort)
	{
		totalCampaigns = 0;

		int numChildren = campaignParent.transform.childCount;
		string file = campaignData.getScreenshotFolder() + "/DO_NOT_DELETE.txt";

		if (!File.Exists(file))
		{
			return;
		}
		else
		{
			StreamReader reader = new StreamReader(file);

			while (!reader.EndOfStream)
			{
				string fileContents = reader.ReadLine();
				string[] lines = fileContents.Split(',');

				toggleCampaigns(false, numChildren);

				if (sort == int.Parse(lines[1]))
				{
					StartCoroutine(filterByMarket(lines[0]));
				}
				else
				{
					if (sort > 0)
					{
						DateList.interactable = false;

						TotalCampaignText.text = "No Campaigns";
					}
					else
					{
						DateList.interactable = true;

						toggleCampaigns(true, numChildren);

						TotalCampaignText.text = numChildren + " Campaigns";
					}
				}
			}

			reader.Close();
		}
	}

	#endregion

	#region Custom function - Filter draft campaigns by market region

	private IEnumerator filterByMarket(string campaignName)
	{
		yield return null;

		DateList.interactable = false;

		totalCampaigns = 0;

		int numChildren = campaignParent.transform.childCount;
		for (int i = 0; i < numChildren; i++)
		{
			if (campaignName == campaignParent.transform.GetChild(i).gameObject.transform.Find("CampaignName").gameObject.GetComponent<Text>().text)
			{
				campaignParent.transform.GetChild(i).gameObject.SetActive(true);
			}
			
			if (campaignParent.transform.GetChild(i).gameObject.activeSelf)
			{
				updateCampaignCount("inc");
			}
		}
	}

	#endregion

	#region Custom function - Update number of campaigns

	public void updateCampaignCount(string dir)
	{
		switch (dir)
		{
			case "inc":
				totalCampaigns += 1;
				if (totalCampaigns == 1)
				{
					TotalCampaignText.text = totalCampaigns.ToString() + " Campaign";
				}
				else
				{
					TotalCampaignText.text = totalCampaigns.ToString() + " Campaigns";
				}
				break;

			case "dec":
				totalCampaigns--;
				if (totalCampaigns > 0)
				{
					if (totalCampaigns > 1)
					{
						TotalCampaignText.text = totalCampaigns.ToString() + " Campaigns";
					}
					else
					{
						TotalCampaignText.text = totalCampaigns.ToString() + " Campaign";
					}
				}
				else
				{
					totalCampaigns = 0;

					TotalCampaignText.text = "No Campaigns";

					if (applicationSetup.getApplicationStatus() != "moveToDraft")
					{
						campaignData.getCampaignList().Clear();
					}
				}
				break;
		}
	}

	#endregion

	#region Custom function - Toggle (show/hide) search input field

	public void SearchButtonClicked()
    {
        if (!SearchInputField.activeSelf)
        {
            SearchInputField.SetActive(true);
        }
        else
        {
            SearchInputField.SetActive(false);
        }
    }

	#endregion

	#region Custom function - Search for a campaign

	public void searchCampaign(string searchedCharacters)
    {
        StartCoroutine(showSearchResult(searchedCharacters));
    }

	#endregion

	#region Custom function - Show results of campaign search

	public IEnumerator showSearchResult(string character)
    {
        totalCampaigns = 0;
        string commonPath;

		toggleCampaignInteraction(true, "");

		if (Application.internetReachability == NetworkReachability.NotReachable)
        {
			applicationSetup.showTaskPanel("Please check your internet connection and try again");
		}
        else
        {
            if (applicationSetup.getApplicationStatus() != "moveToPublish" || applicationSetup.getApplicationStatus() == "moveToArchive")
            {
				WWW loginLink = new WWW(loginAPI + "username=" + userData.GetUserName() + "&password=" + userData.GetPassword() + "&sort=" + CampaignSorting);
				yield return loginLink;

				setJSONToken(JObject.Parse(loginLink.text));
				UnloadCampaigns();

				for (int i = 0; i <= (int)getJSONToken()["campaign_count"] - 1; i++)
				{
					if (applicationSetup.getApplicationStatus() == getJSONToken()["campaign_detail"][(i + 1).ToString()]["status"].ToString())
					{
						if (getJSONToken()["campaign_detail"][(i + 1).ToString()]["name"].ToString().Contains(character))
						{
							commonPath = getJSONToken()["campaign_detail"][(i + 1).ToString()]["common_image_path"].ToString();
							WWW imageLink = new WWW(commonPath + getJSONToken()["campaign_detail"][(i + 1).ToString()]["images"][0].ToString());
							yield return imageLink;

							GameObject instance = Instantiate(CampaignPrefab, transform.position, transform.rotation) as GameObject;
							instance.transform.SetParent(campaignParent.transform, false);
							instance.transform.Find("CenterImage").gameObject.GetComponent<Image>().sprite = Sprite.Create(imageLink.texture, new Rect(0.0f, 0.0f, imageLink.texture.width, imageLink.texture.height), new Vector2(0.5f, 0.5f), 100.0f);
							instance.transform.Find("CenterImage").gameObject.GetComponent<Image>().preserveAspect = true;
							instance.transform.Find("CampaignName").gameObject.GetComponent<Text>().text = getJSONToken()["campaign_detail"][(i + 1).ToString()]["name"].ToString();
							instance.transform.Find("DaysText").gameObject.GetComponent<Text>().text = getJSONToken()["campaign_detail"][(i + 1).ToString()]["last_updated_at"].ToString();
							instance.transform.Find("Market").gameObject.GetComponent<Text>().text = getJSONToken()["campaign_detail"][(i + 1).ToString()]["market_count"] + " Market • " + getJSONToken()["campaign_detail"][(i + 1).ToString()]["counter_count"] + " Counters • " + getJSONToken()["campaign_detail"][(i + 1).ToString()]["fixture_count"] + " Fixtures";
							TotalCampaignText.gameObject.SetActive(true);

							updateCampaignCount("inc");
						}
					}
				}
			}
			else
			{
				filterBySearch(campaignParent, character);
			}
        }

		toggleCampaignInteraction(false, "showSearchResult");
    }

	#endregion

	#region Custom function - Filter draft campaigns by search results

	private void filterBySearch(GameObject main, string input)
	{
		totalCampaigns = 0;

		int numChildren = main.transform.childCount;
		for (int i = 0; i < numChildren; i++)
		{
			if (main.transform.GetChild(i).gameObject.transform.Find("CampaignName").gameObject.GetComponent<Text>().text.ToLower().Contains(input))
			{
				main.transform.GetChild(i).gameObject.SetActive(true);
			}
			else
			{
				main.transform.GetChild(i).gameObject.SetActive(false);
			}

			if (main.transform.GetChild(i).gameObject.activeSelf)
			{
				updateCampaignCount("inc");
			}
			else
			{
				if (totalCampaigns == 0)
				{
					TotalCampaignText.text = "No Campaigns";
				}
			}
		}
	}

	#endregion
	
	#region Custom function - Toggle (show/hide) campaigns in a section

	private void toggleCampaigns(bool toggle, int count)
	{
		for (int i = 0; i < count; i++)
		{
			if (campaignParent.transform.GetChild(i).gameObject.activeSelf == !toggle)
			{
				campaignParent.transform.GetChild(i).gameObject.SetActive(toggle);
			}
		}
	}

	#endregion
	
	#region Custom function - Unload campaigns in a section

	private void UnloadCampaigns()
	{
		foreach (Transform child in campaignParent.transform)
		{
			Destroy(child.gameObject);
		}
	}

	#endregion
	
	#region Custom function - Move to campaign options section

	public void moveToCampaignOptionsSection()
	{
		executeTransitionTo(CampaignOptions);
	}

	#endregion

	#region Custom function - Move to create campaign section

	public void moveToCreateCampaignSection()
	{
		executeTransitionTo(CreateCampaign);
	}

	#endregion

	#region Custom function - Move to edit account section

	public void moveToEditAccountSection()
	{
		executeTransitionTo(EditAccount);
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
