using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{

	#region Public variables

	[Header("Tool Sections")]
	public GameObject CampaignOptions;
	public GameObject Selection;
	public GameObject Campaigns;

	[Header("Game Objects")]
	public GameObject fpsCam;
	public GameObject beCam;
	public GameObject player;
	public GameObject libraryPanel;
	public GameObject mainPanel;
	public GameObject secondPanel;
	public GameObject snapshotPanel;
	public GameObject libraryImage;
	public GameObject libraryCampaign;
	public GameObject librarySubCategory;
	public GameObject libraryParent;
	public GameObject titleCounter;
	public GameObject titleFixture;
	public GameObject fixtureProducts;
	public GameObject counterDetails;
	public GameObject fixtureDetails;

	[Header("UI Components")]
	public Button publish;
	public Button saveDraft;
	public Button clearAll;
	public Button refresh;
	public Button back;
	public Button counter;
	public Button fixture;
	public Button downloadC1;
	public Button downloadC2;
	public Button hideAssets;
	public Button showAssets;
	public TextMeshProUGUI uiText;
	public GridLayoutGroup gridLayout;
	public Scrollbar scrollbar;
	public Dropdown formatList;
	public Dropdown ratioList;
	public Image mainImage;
	public Image secondImage;
	public Image iCounter;
	public Image iFixture;
	public Sprite counterEnabled;
	public Sprite counterDisabled;
	public Sprite fixtureEnabled;
	public Sprite fixtureDisabled;
	public Text counterName;
	public Text counterMarketCity;
	public Text fixtureName;
	public Text fixtureMarketCity;

	[Header("Monobehaviour Scripts")]
	public PlayerConfiguration playerConfiguration;
	public CameraConfiguration cameraConfiguration;
	public ImageLibrary imageLibrary;
	public UserData userData;
	public EnvironmentData environmentData;
	public CampaignData campaignData;
	public FtpData ftpData;
	public ApplicationSetup applicationSetup;

	#endregion

	#region Private variables
	
	private GameObject fixtureSubAssets;
	private bool isEditModeActive;
	private bool isCameraSwitched;
	private int jpgCount = 0;
	private int pngCount = 0;
	private string roleName;
	private string snapshotFormat;
	private string filename;
	private List<string> availableRatioList = new List<string>();
	private string[,] counterDetailsArray = new string[13, 3] { { "Global Counter", "Domestic - China", "China" }, { "Iwataya", "Domestic - Japan", "Japan" }, { "Lotte Bandung", "Domestic - Korea", "Korea" }, { "Global Counter", "Domestic - Korea", "Korea" }, { "Global Counter (Inbound)", "Domestic - Japan", "Japan" }, { "Global Counter (RV)", "Travel Retail - Hong Kong", "Hong Kong" }, { "Haitang Bay", "Travel Retail - China", "China" }, { "Everich Kinmen Island", "Travel Retail - Taiwan", "Taiwan" }, { "Keio Shinjuku", "Domestic - Japan", "Japan" }, { "Shinsegae Kyungki", "Domestic - Korea", "Korea" }, { "Lotte Downtown", "Domestic - Korea", "Korea" }, { "Tian Shan Bai Huo (Sparkle)", "Domestic - China", "China" }, { "Shanghai New World (RV)", "Domestic - China", "China" } };
	private string[,] fixtureDetailsArray = new string[3, 3] { { "Initiative Highlighter", "Market", "City" }, { "POM", "Market", "City" }, { "POM450", "Market", "City" } };

	#endregion

	#region Monobehaviours

	// Use this for initialization
	private void OnEnable()
	{
		initializeVariables();
		prepareAssetsByType();
		togglePanels(mainPanel, true);
		togglePanels(secondPanel, true);
		togglePanels(snapshotPanel, false);
		StartCoroutine(setAllMeshTextures());
		StartCoroutine(checkNumSnapshots());
		StartCoroutine(assignProductTags());
		playerConfiguration.resetPlayer();
		cameraConfiguration.resetCameras(fpsCam, beCam);
		ftpData.prepareFtpData();
	}

	// Update is called once per frame
	private void Update()
	{
		toggleEditMode();
		manualSwitchCameraView();
	}

	#endregion

	#region Custom function - Initialize variables

	private void initializeVariables()
	{
		isCameraSwitched = false;
		isEditModeActive = true;
		roleName = userData.GetRoleName();
	}

	#endregion

	#region Custom function - Prepare tool assets based on type selected

	private void prepareAssetsByType()
	{
		if (environmentData.getType() == "counter")
		{
			titleCounter.SetActive(true);
			titleFixture.SetActive(false);
			fixtureProducts.SetActive(false);
			counterDetails.SetActive(true);
			fixtureDetails.SetActive(false);

			int id = environmentData.getID();
			counterName.text = counterDetailsArray[id, 0];
			counterMarketCity.text = counterDetailsArray[id, 1] + "   •   " + counterDetailsArray[id, 2];

			environmentData.setCounterName(counterDetailsArray[id, 0]);
			environmentData.setMarketName(counterDetailsArray[id, 1]);
		}
		else
		{
			titleFixture.SetActive(true);
			titleCounter.SetActive(false);
			fixtureProducts.SetActive(true);
			showAssets.gameObject.SetActive(false);
			fixtureDetails.SetActive(true);
			counterDetails.SetActive(false);

			int id = environmentData.getID();
			fixtureName.text = fixtureDetailsArray[id, 0];
			fixtureMarketCity.text = fixtureDetailsArray[id, 1] + "   •   " + fixtureDetailsArray[id, 2];

			environmentData.setFixtureName(fixtureDetailsArray[id, 0]);
			environmentData.setMarketName(fixtureDetailsArray[id, 1]);
		}
	}

	#endregion
	
	#region Custom function - Assign tag to the products of fixtures
	
	private IEnumerator assignProductTags()
	{
		yield return null;
		
		if (environmentData.getType() == "fixture")
		{
			fixtureSubAssets = GameObject.FindGameObjectWithTag("Fixture Products");
		}
	}
	
	#endregion
	
	#region Custom function - Set textures of all meshes

	private IEnumerator setAllMeshTextures()
	{
		yield return null;

		MeshOutline[] meshes = FindObjectsOfType<MeshOutline>();
		for (int i = 0; i < meshes.Length; i++)
		{
			if (meshes[i].gameObject.GetComponent<MeshDetails>().getTextureSave() == false)
			{
				Material[] materials = meshes[i].gameObject.GetComponent<MeshRenderer>().materials;
				for (int j = 0; j < materials.Length; j++)
				{
					if (materials[j].mainTexture != null)
					{
						environmentData.getEnvironment().GetComponent<EnvironmentDetails>().setListTextures(materials[j].mainTexture);
					}
				}
			}
		}

		toggleButtons(clearAll, environmentData.getEnvironment().GetComponent<EnvironmentDetails>().getTextureChange());
	}

	#endregion

	#region Custom function - Reset textures of all meshes

	public void resetAllMeshTextures()
	{
		MeshOutline[] meshes = FindObjectsOfType<MeshOutline>();
		for (int i = 0; i < meshes.Length; i++)
		{
			Material[] materials = meshes[i].gameObject.GetComponent<MeshRenderer>().materials;
			for (int j = 0; j < materials.Length; j++)
			{
				if (materials[j].mainTexture != null)
				{
					materials[j].mainTexture = environmentData.getEnvironment().GetComponent<EnvironmentDetails>().getListTextures()[j];
					materials[j].SetTexture("_EmissionMap", environmentData.getEnvironment().GetComponent<EnvironmentDetails>().getListTextures()[j]);
				}
			}

			meshes[i].gameObject.GetComponent<MeshDetails>().setTextureSave(false);
		}

		environmentData.getEnvironment().GetComponent<EnvironmentDetails>().setTextureChange(false);
		toggleButtons(clearAll, environmentData.getEnvironment().GetComponent<EnvironmentDetails>().getTextureChange());
	}

	#endregion

	#region Custom function - Toggle edit mode on key press

	private void toggleEditMode()
	{
		if (!mainImage.enabled)
		{
			if (!snapshotPanel.activeSelf)
			{
				if (Input.GetKeyDown(KeyCode.E))
				{
					isEditModeActive = !isEditModeActive;
				}

				configureActiveCamera();
				toggleCursor();
			}
		}
	}

	#endregion

	#region Custom function - Configure componenets on active camera

	private void configureActiveCamera()
	{
		if (player.activeSelf)
		{
			playerConfiguration.toggleComponents(!isEditModeActive);
		}
		else if (beCam.activeSelf)
		{
			cameraConfiguration.toggleComponents(!isEditModeActive);
		}
	}

	#endregion

	#region Custom function - Toggle cursor based on active camera

	private void toggleCursor()
	{
		Cursor.visible = isEditModeActive;

		switch (isEditModeActive)
		{
			case true:
				Cursor.lockState = CursorLockMode.None;
				break;

			case false:
				Cursor.lockState = CursorLockMode.Locked;
				break;
		}
	}

	#endregion

	#region Custom function - Switch manually between 2 camera views

	private void manualSwitchCameraView()
	{
		if (!mainImage.enabled)
		{
			if (Input.GetKeyDown(KeyCode.C))
			{
				isCameraSwitched = !isCameraSwitched;

				beCam.SetActive(isCameraSwitched);
				player.SetActive(!isCameraSwitched);

				configureActiveCamera();
			}
		}
	}

	#endregion

	#region Custom function - Auto switch between 2 camera views

	private void autoSwitchCameraView()
	{
		if (!mainImage.enabled)
		{
			if (!player.activeSelf)
			{
				isCameraSwitched = !isCameraSwitched;

				beCam.SetActive(isCameraSwitched);
				player.SetActive(!isCameraSwitched);

				configureActiveCamera();
			}
		}
	}

	#endregion

	#region Custom function - Toggle (show/hide) panels

	private void togglePanels(GameObject panel, bool toggle)
	{
		if (panel.name == "Main Panel")
		{
			mainImage.enabled = toggle;
		}
		else if (panel.name == "Second Panel")
		{
			secondImage.enabled = toggle;
		}
		else
		{
			panel.SetActive(toggle);
		}
	}

	#endregion

	#region Custom function - Toggle (enable/disable) buttons

	private void toggleButtons(Button button, bool toggle)
	{
		button.interactable = toggle;
	}

	#endregion

	#region Custom function - Toggle (enable/disable) menus

	private void toggleMenus(Dropdown dropdown, bool toggle)
	{
		dropdown.interactable = toggle;
	}

	#endregion

	#region Custom function - Toggle text to display

	private void toggleTexts(string message)
	{
		uiText.text = message;
	}

	#endregion

	#region Custom function - Swap sprites of images based on type loaded

	private void swapSpritesBySelection(string type)
	{
		switch (type)
		{
			case "counter":
				iCounter.sprite = counterDisabled;
				iFixture.sprite = fixtureEnabled;
				break;

			case "fixture":
				iCounter.sprite = counterEnabled;
				iFixture.sprite = fixtureDisabled;
				break;

			default:
				iCounter.sprite = counterDisabled;
				iFixture.sprite = fixtureDisabled;
				break;
		}
	}
	#endregion

	#region Custom function - Set & Get number of JPGs

	public void setJPGCount(int count)
	{
		jpgCount = count;
	}

	public int getJPGCount()
	{
		return jpgCount;
	}

	#endregion

	#region Custom function - Set & Get number of PNGs

	public void setPNGCount(int count)
	{
		pngCount = count;
	}

	public int getPNGCount()
	{
		return pngCount;
	}

	#endregion

	#region Custom function - Check number of snapshots taken

	private IEnumerator checkNumSnapshots()
	{
		yield return null;
		
		switch (roleName)
		{
			case "Reviewer":
				toggleButtons(saveDraft, false);
				toggleButtons(publish, false);
				break;

			default:
				int totalCount = jpgCount + pngCount;
				
				toggleButtons(saveDraft, System.Convert.ToBoolean(totalCount));
				toggleButtons(publish, System.Convert.ToBoolean(totalCount));
				break;
		}
	}

	#endregion

	#region Custom function - Configure (show/hide) snapshot options panel

	public void configureSnapshotOptionsPanel(bool toggle)
	{
		formatList.value = 0;

		downloadC1.gameObject.SetActive(player.activeSelf);
		downloadC2.gameObject.SetActive(beCam.activeSelf);

		setSnapshotFormat("");
		togglePanels(mainPanel, toggle);
		togglePanels(secondPanel, toggle);
		togglePanels(snapshotPanel, toggle);
		toggleButtons(downloadC1, false);
		toggleButtons(downloadC2, false);
		toggleTexts("");
	}

	#endregion

	#region Custom function - Set & Get format of snapshot

	private void setSnapshotFormat(string format)
	{
		snapshotFormat = format;
	}

	public string getSnapshotFormat()
	{
		return snapshotFormat;
	}

	#endregion

	#region Custom function - Update format of snapshot to be taken

	public void updateSnapshotFormat(int index)
	{
		switch (index)
		{
			case 0:
				setSnapshotFormat("");
				toggleButtons(downloadC1, false);
				toggleButtons(downloadC2, false);
				break;

			case 1:
				setSnapshotFormat("jpg");
				toggleButtons(downloadC1, true);
				toggleButtons(downloadC2, true);
				break;

			case 2:
				setSnapshotFormat("png");
				toggleButtons(downloadC1, true);
				toggleButtons(downloadC2, true);
				break;
		}
	}

	#endregion

	#region Custom function - Set & Get default file name

	public void setDefaultFileName(string name)
	{
		filename = name;
	}

	public string getDefaultFileName()
	{
		return filename;
	}

	#endregion

	#region Custom function - Save user activity to draft

	public void saveAsDraft()
	{
		createDraftDirectory();

		DirectoryInfo dirInfo;
		FileInfo[] fileInfo;
		if (Directory.Exists(campaignData.getCampaignRootFolder()))
		{
			dirInfo = new DirectoryInfo(campaignData.getCampaignRootFolder());
			fileInfo = dirInfo.GetFiles();
			for (int i = 0; i < fileInfo.Length; i++)
			{
				File.Copy(fileInfo[i].FullName, campaignData.getDraftFolder() + "/" + fileInfo[i].Name, true);
			}

			applicationSetup.showTaskPanel("Saving as Draft...");
			applicationSetup.setApplicationStatus("moveToDraft");

			saveAssetInfoForEditingDrafts();
			saveCampaignInfoForAssetSelection();
			StartCoroutine(startTransitionToCampaigns());
		}
	}

	#endregion

	#region Custom function - Create draft directory

	private void createDraftDirectory()
	{
		if (campaignData.getDraftFolder() == null || campaignData.getDraftFolder().Length == 0)
		{
			campaignData.setDraftFolder(campaignData.getDraftFolder() + campaignData.getCampaignRootFolder() + "/Drafts");

			System.IO.Directory.CreateDirectory(campaignData.getDraftFolder());
		}
	}

	#endregion

	#region Custom function - Save information of asset for editing drafts

	private void saveAssetInfoForEditingDrafts()
	{
		string file = campaignData.getScreenshotFolder() + "/" + campaignData.getCampaignName() + "/Drafts/DO_NOT_DELETE.txt";

		MeshOutline[] meshes = FindObjectsOfType<MeshOutline>();
		for (int i = 0; i < meshes.Length; i++)
		{
			writeDataToFileForEditingDrafts(meshes[i].gameObject, file);
		}
	}

	#endregion

	#region Custom function - Write environment data to a text file for editing drafts

	private void writeDataToFileForEditingDrafts(GameObject main, string file)
	{
		string data = campaignData.getCampaignName() + "," + environmentData.getType() + "," + environmentData.getID().ToString() + "," + main.GetComponent<MeshDetails>().getComponentName() + "," + main.GetComponent<MeshDetails>().getTextureName();

		using (StreamWriter writer = new StreamWriter(file, true))
		{
			writer.WriteLine(data);
		}
	}

	#endregion

	#region Custom function - Save campaign info for asset selection

	private void saveCampaignInfoForAssetSelection()
	{
		string file = campaignData.getScreenshotFolder() + "/DO_NOT_DELETE.txt";

		writeDataForAssetSelection(file);
	}

	#endregion

	#region Custom function - Write data to text file for asset selection

	private void writeDataForAssetSelection(string file)
	{
		bool uniqueDataEntry = false;
		string data = campaignData.getCampaignName() + "," + environmentData.getMarketID() + "," + environmentData.getType() + "," + environmentData.getID().ToString();

		if (!File.Exists(file))
		{
			uniqueDataEntry = true;
		}
		else
		{
			using (StreamReader reader = new StreamReader(file))
			{
				while (!reader.EndOfStream)
				{
					string fileContents = reader.ReadLine();
					
					if (fileContents != data)
					{
						uniqueDataEntry = true;
					}
				}
			}
		}
		
		if (uniqueDataEntry)
		{
			using (StreamWriter writer = new StreamWriter(file, true))
			{
				writer.WriteLine(data);
			}
		}
	}

	#endregion

	#region Custom function - Publish user activity

	public void Publish()
	{
		createPublishDirectory();

		DirectoryInfo dirInfo;
		FileInfo[] fileInfo;
		if (Directory.Exists(campaignData.getCampaignRootFolder()))
		{
			dirInfo = new DirectoryInfo(campaignData.getCampaignRootFolder());
			fileInfo = dirInfo.GetFiles();
			for (int i = 0; i < fileInfo.Length; i++)
			{
				File.Copy(fileInfo[i].FullName, campaignData.getPublishFolder() + "/" + fileInfo[i].Name, true);
			}

			ftpData.executeFtpProtocol();

			applicationSetup.showTaskPanel("Publishing...");
		}
	}

	#endregion

	#region Custom function - Create publish directory

	private void createPublishDirectory()
	{
		if (campaignData.getPublishFolder() == null || campaignData.getPublishFolder().Length == 0)
		{
			campaignData.setPublishFolder(campaignData.getPublishFolder() + campaignData.getCampaignRootFolder() + "/Published");

			System.IO.Directory.CreateDirectory(campaignData.getPublishFolder());
		}
	}

	#endregion

	#region Custom function - Execute all actions before loading campaigns

	public void executePreCampaignLoadActions()
	{
		clearRatioList();
		setRatioList("All Ratios");

		ratioList.ClearOptions();
		ratioList.AddOptions(getRatioList());
		
		gridLayout.cellSize = new Vector2(198.0f, 150.0f);

		swapSpritesBySelection("");
		togglePanels(mainPanel, true);
		togglePanels(secondPanel, true);
		toggleButtons(refresh, false);
		toggleButtons(counter, false);
		toggleButtons(fixture, false);
		toggleMenus(ratioList, false);
		toggleTexts("Loading Campaign Library...");
	}

	#endregion

	#region Custom function - Execute all actions before loading sub-categories

	public void executePreSubCategoryLoadActions()
	{
		clearRatioList();
		setRatioList("All Ratios");

		ratioList.ClearOptions();
		ratioList.AddOptions(getRatioList());

		gridLayout.cellSize = new Vector2(198.0f, 150.0f);

		togglePanels(mainPanel, true);
		togglePanels(secondPanel, true);
		toggleButtons(refresh, false);
		toggleMenus(ratioList, false);
		toggleTexts("Loading Sub-Category Library...");
	}

	#endregion

	#region Custom function - Execute all actions before loading images

	public void executePreImageLoadActions()
	{
		gridLayout.cellSize = new Vector2(150.0f, 150.0f);

		togglePanels(mainPanel, true);
		togglePanels(secondPanel, true);
		toggleButtons(refresh, false);
		toggleTexts("Loading Image Library...");
	}

	#endregion

	#region Custom function - Execute all actions after loading (campaigns/sub-categories/images)

	public void executePostLoadActions(string type)
	{
		togglePanels(mainPanel, false);
		togglePanels(secondPanel, false);
		togglePanels(snapshotPanel, false);
		toggleTexts("");
		resetScrollbar();
		playerConfiguration.setCharacterHeight();
		
		if (type != "campaigns")
		{
			executeTypeSelectionActions(imageLibrary.getLoadType());
		}
		if (type == "images")
		{
			toggleMenus(ratioList, true);
		}
	}

	#endregion

	#region Custom function - Execute all actions before taking snapshot

	public void executePreSnapshotActions()
	{
		togglePanels(mainPanel, true);
		togglePanels(secondPanel, true);
		togglePanels(snapshotPanel, false);
		toggleTexts("Saving Snapshot to folder...");
	}

	#endregion

	#region Custom function - Execute all actions after taking snapshot

	public void executePostSnapshotActions()
	{
		setSnapshotFormat("");
		togglePanels(mainPanel, false);
		togglePanels(secondPanel, false);
		togglePanels(snapshotPanel, false);
		toggleTexts("");
		StartCoroutine(checkNumSnapshots());
	}

	#endregion

	#region Custom function - Execute actions when library loading fails

	public void executeLoadFailActions(string message)
	{
		togglePanels(mainPanel, true);
		togglePanels(secondPanel, true);
		toggleButtons(refresh, true);
		toggleTexts(message);
	}

	#endregion

	#region Custom function - Execute all actions after drag and drop

	public void executeDragAndDropActions()
	{
		toggleButtons(clearAll, true);
	}

	#endregion

	#region Custom function - Execute all actions after selecting counter/fixture

	private void executeTypeSelectionActions(string type)
	{
		switch (type)
		{
			case "counter":
				toggleButtons(counter, false);
				toggleButtons(fixture, true);
				break;

			case "fixture":
				toggleButtons(counter, true);
				toggleButtons(fixture, false);
				break;
		}
		
		swapSpritesBySelection(type);
	}

	#endregion
	
	#region Custom function - Reset scrollbar position on load

	private void resetScrollbar()
	{
		scrollbar.value = 1;
	}

	#endregion
	
	#region Custom function - Set & Get ratio list

	public void setRatioList(string list)
	{
		availableRatioList.Add(list);
	}

	public List<string> getRatioList()
	{
		return availableRatioList;
	}

	public void clearRatioList()
	{
		availableRatioList.Clear();
	}

	#endregion
	
	#region Custom function - Hide products on fixtures
	
	public void hideFixtureProducts()
	{
		hideAssets.gameObject.SetActive(false);
		showAssets.gameObject.SetActive(true);
		
		if (environmentData.getType() == "fixture")
		{
			fixtureSubAssets.SetActive(false);
		}
	}
	
	#endregion
	
	#region Custom function - Show products on fixtures
	
	public void showFixtureProducts()
	{
		showAssets.gameObject.SetActive(false);
		hideAssets.gameObject.SetActive(true);
		
		if (environmentData.getType() == "fixture")
		{
			fixtureSubAssets.SetActive(true);
		}
	}

	#endregion

	#region Custom function - Check state of the image library

	public void checkImageLibraryState(GameObject button)
	{
		switch (button.name)
		{
			case "Back":
				switch (imageLibrary.getLibraryState())
				{
					case "images":
						imageLibrary.createSubCategoryLibrary();
						break;

					case "subcategories":
						imageLibrary.createCampaignLibrary();
						break;

					case "campaigns":
						moveToEnvironmentSection();
						break;
				}
				break;

			case "Refresh":
				switch (imageLibrary.getLibraryState())
				{
					case "images":
						imageLibrary.createSubCategoryLibrary();
						break;

					case "subcategories":
						imageLibrary.createCampaignLibrary();
						break;
				}
				break;
		}	
	}

	#endregion

	#region Custom function - Start transition to move to campaigns

	public IEnumerator startTransitionToCampaigns()
	{
		if (applicationSetup.getApplicationStatus() == "moveToPublish")
		{
			environmentData.getEnvironment().GetComponent<EnvironmentDetails>().setTextureChange(false);

			ftpData.setUploadStatus(false);
			applicationSetup.showTaskPanel("Files Uploaded!");
		}

		yield return new WaitForSeconds(2.0f);

		moveToCampaignsSection();
	}

	#endregion

	#region Custom function - Move to campaigns section

	public void moveToCampaignsSection()
	{
		executeTransitionTo(Campaigns);
	}

	#endregion

	#region Custom function - Move to campaign options section

	public void moveToCampaignOptionsSection()
	{
		environmentData.getEnvironment().GetComponent<EnvironmentDetails>().setTextureChange(false);

		executeTransitionTo(CampaignOptions);
	}

	#endregion

	#region Custom function - Move to environment section

	private void moveToEnvironmentSection()
	{
		environmentData.getEnvironment().GetComponent<EnvironmentDetails>().setTextureChange(false);

		executeTransitionTo(Selection);
	}

	#endregion

	#region Custom function - Do a clean up of all assets

	private void cleanUp()
	{
		StopAllCoroutines();

		autoSwitchCameraView();
		imageLibrary.destroyLibrary();

		Destroy(environmentData.getEnvironment());
	}

	#endregion

	#region Custom function - Execution transition to a section

	private void executeTransitionTo(GameObject section)
	{
		cleanUp();

		gameObject.SetActive(false);
		section.SetActive(true);
	}

	#endregion

}
