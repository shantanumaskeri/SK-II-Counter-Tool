using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class SelectionMenu : MonoBehaviour
{

	#region Public variables

	[Header("Tool Sections")]
	public GameObject EditAccount;
	public GameObject Campaigns;
	public GameObject CreateCampaign;
	public GameObject CampaignOptions;
	public GameObject Main;

	[Header("Game Objects")]
	public GameObject allCounters;
	public GameObject countersParent;
	public GameObject allFixtures;
	public GameObject fixturesParent;

	[Header("Game Object Arrays")]
	public GameObject[] counterPrefabs;
	public GameObject[] fixturePrefabs;

	[Header("UI Components")]
	public Scrollbar scrollbarCounter;
	public Scrollbar scrollbarFixture;
	public Dropdown markets;
	public Dropdown city;
	public Dropdown fixtureTypes;
	public InputField searchBox;
	public Button search;
	public Button createCampaign;

	[Header("Monobehaviour Scripts")]
	public UserData userData;
	public EnvironmentData environmentData;
	public CampaignData campaignData;
	public ApplicationSetup applicationSetup;

	#endregion

	#region Private variables

	private bool isVisible;
	private GameObject[] objArray;
	private string roleName;
	private string storeType;
	private List<int> environmentIDs;

	#endregion

	#region Monobehaviours

	private void OnEnable()
	{
		initializeVariables();
		checkUserRole();
		resetEnvironment();
		loadEnvironmentType();
		prepareEnvironmentSelection();
		checkCampaignStatus();
		StartCoroutine(resetScrollbar());
	}

	#endregion

	#region Custom function - Initialize variables

	private void initializeVariables()
	{
		isVisible = false;	
		searchBox.gameObject.SetActive(isVisible);

		environmentIDs = new List<int>();

		storeType = environmentData.getType();
	}

	#endregion

	#region Custom function - Check role of user from login credentials

	private void checkUserRole()
	{
		if (createCampaign != null)
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
	}

	#endregion

	#region Custom function - Load selected type of environment

	private void loadEnvironmentType()
	{
		if (environmentData.getType() == "counter")
		{
			allCounters.SetActive(true);
			allFixtures.SetActive(false);

			city.gameObject.SetActive(true);
		}
		else
		{
			allFixtures.SetActive(true);
			allCounters.SetActive(false);

			city.gameObject.SetActive(false);
		}
	}

	#endregion

	#region Custom function - Prepare environments for selection

	private void prepareEnvironmentSelection()
	{
		if (environmentData.getType() == "counter")
		{
			removeEnvironmentsFromSelection(countersParent);
			addEnvironmentsToSelection(counterPrefabs, countersParent);
		}
		else
		{
			removeEnvironmentsFromSelection(fixturesParent);
			addEnvironmentsToSelection(fixturePrefabs, fixturesParent);
		}
	}

	#endregion

	#region Custom function - Add environments to selection menu

	private void addEnvironmentsToSelection(GameObject[] environmentPrefab, GameObject parent)
	{
		for (int i = 0; i < environmentPrefab.Length; i++)
		{
			GameObject environment = Instantiate(environmentPrefab[i], environmentPrefab[i].transform.position, Quaternion.identity) as GameObject;
			environment.transform.SetParent(parent.transform, false);
			environment.GetComponent<Button>().onClick.AddListener(delegate { setupEnvironment(environment.GetComponent<SelectionDetails>().environmentID); });
		}
	}

	#endregion

	#region Custom function - Remove environments from selection menu

	private void removeEnvironmentsFromSelection(GameObject parent)
	{
		int numChildren = parent.transform.childCount;
		for (int i = 0; i < numChildren; i++)
		{
			GameObject environment = parent.transform.GetChild(i).gameObject;
			Destroy(environment);
		}
	}

	#endregion

	#region Custom function - Check status of campaign

	private void checkCampaignStatus()
	{
		if (campaignData.getDraftEdit() == true)
		{
			readDataFromFile();
		}
	}

	#endregion

	#region Custom function - Read data from text file

	private void readDataFromFile()
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

				if (campaignData.getCampaignName() == lines[0])
				{
					if (storeType == lines[2])
					{
						if (environmentData.getType() == "counter")
						{
							updateEnvironmentMenu(counterPrefabs, countersParent, lines);
						}
						else
						{
							updateEnvironmentMenu(fixturePrefabs, fixturesParent, lines);
						}
					}
				}
			}

			reader.Close();
		}
	}

	#endregion

	#region Custom function - Update the menu of environment selection

	private void updateEnvironmentMenu(GameObject[] environmentPrefab, GameObject parent, string[] data)
	{
		removeEnvironmentsFromSelection(parent);

		environmentIDs.Add(int.Parse(data[3]));
		for (int j = 0; j < environmentIDs.Count; j++)
		{
			GameObject environment = Instantiate(environmentPrefab[environmentIDs[j]], environmentPrefab[environmentIDs[j]].transform.position, Quaternion.identity) as GameObject;
			environment.transform.SetParent(parent.transform, false);
			environment.GetComponent<Button>().onClick.AddListener(delegate { setupEnvironment(environment.GetComponent<SelectionDetails>().environmentID); });
		}
	}

	#endregion

	#region Custom function - Reset scrollbars position on load

	private IEnumerator resetScrollbar()
	{
		yield return null;

		scrollbarCounter.value = scrollbarFixture.value = 0;
	}

	#endregion

	#region Custom function - Filter environment by markets

	public void filterByMarkets(int index)
	{
		applyFilterToMenu(index, markets, city, fixtureTypes);
	}

	#endregion

	#region Custom function - Filter environment by city

	public void filterByCity(int index)
	{
		applyFilterToMenu(index, city, markets, fixtureTypes);
	}

	#endregion

	#region Custom function - Filter environment by fixture types

	public void filterByFixtureTypes(int index)
	{
		applyFilterToMenu(index, fixtureTypes, markets, city);
	}

	#endregion

	#region Custom function - Apply filter to environment menu

	private void applyFilterToMenu(int index, Dropdown menu1, Dropdown menu2, Dropdown menu3)
	{
		if (environmentData.getType() == "counter")
		{
			toggleEnvironments(countersParent, true);
		}
		else
		{
			toggleEnvironments(fixturesParent, true);
		}
		
		if (index > 0)
		{
			getEnvironmentTag(menu1.options[index].text);
		}

		if (environmentData.getType() == "counter")
		{
			toggleEnvironments(countersParent, false);
		}
		else
		{
			toggleEnvironments(fixturesParent, false);
		}
		
		if (index > 0)
		{
			showEnvironmentByTag();
		}
		else
		{
			if (environmentData.getType() == "counter")
			{
				toggleEnvironments(countersParent, true);
			}
			else
			{
				toggleEnvironments(fixturesParent, true);
			}
		}

		toggleMenus(menu2, !System.Convert.ToBoolean(index));
		toggleMenus(menu3, !System.Convert.ToBoolean(index));
		toggleButtons(search, !System.Convert.ToBoolean(index));
	}

	#endregion

	#region Custom function - Toggle (show/hide) environments

	private void toggleEnvironments(GameObject parent, bool toggle)
	{
		int numChildren = parent.transform.childCount;
		for (int i = 0; i < numChildren; i++)
		{
			parent.transform.GetChild(i).gameObject.SetActive(toggle);
		}
	}

	#endregion

	#region Custom function - Get environment tag

	private void getEnvironmentTag(string tag)
	{
		objArray = GameObject.FindGameObjectsWithTag(tag);
	}

	#endregion

	#region Custom function - Show environment based on tag selected

	private void showEnvironmentByTag()
	{
		for (int i = 0; i < objArray.Length; i++)
		{
			objArray[i].transform.parent.gameObject.SetActive(true);
		}
	}

	#endregion

	#region Custom function - Toggle (enable/disable) drop down menus

	private void toggleMenus(Dropdown menu, bool toggle)
	{
		menu.interactable = toggle;
	}

	#endregion

	#region Custom function - Toggle (enable/disable) buttons

	private void toggleButtons(Button button, bool toggle)
	{
		button.interactable = toggle;
	}

	#endregion

	#region Custom function - Toggle (show/hide) search field box

	public void toggleSearchBox()
	{
		isVisible = !isVisible;
		searchBox.gameObject.SetActive(isVisible);
	}

	#endregion

	#region Custom function - Get options from search field

	public void getSearchOptions(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			if (environmentData.getType() == "counter")
			{
				toggleEnvironments(countersParent, true);
			}
			else
			{
				toggleEnvironments(fixturesParent, true);
			}
			
			toggleMenus(markets, true);
			toggleMenus(city, true);
			toggleMenus(fixtureTypes, true);
		}
		else
		{
			if (environmentData.getType() == "counter")
			{
				filterBySearch(countersParent, input);
			}
			else
			{
				filterBySearch(fixturesParent, input);
			}
			
			toggleMenus(markets, false);
			toggleMenus(city, false);
			toggleMenus(fixtureTypes, false);
		}
	}

	#endregion

	#region Custom function - Filter environment by search field results

	private void filterBySearch(GameObject parent, string input)
	{
		int numChildren = parent.transform.childCount;
		for (int i = 0; i < numChildren; i++)
		{
			if (parent.transform.GetChild(i).gameObject.transform.Find("Stall Name").gameObject.GetComponent<Text>().text.ToLower().Contains(input))
			{
				parent.transform.GetChild(i).gameObject.SetActive(true);
			}
			else
			{
				parent.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
	}

	#endregion

	#region Custom function - Filter environment by date

	public void filterByDate(int index)
	{
		if (environmentData.getType() == "counter")
		{
			sortChildrenComponents(countersParent);
		}
		else
		{
			sortChildrenComponents(fixturesParent);
		}
	}

	#endregion

	#region Custom function - Sort children components in order given by date

	private void sortChildrenComponents(GameObject parent)
	{
		int numChildren = parent.transform.childCount;
		int[] indexArray = new int[numChildren];

		for (int i = 0; i < numChildren; i++)
		{
			indexArray[i] = i;

			parent.transform.GetChild(i).gameObject.transform.SetSiblingIndex(parent.transform.GetChild(i).gameObject.transform.GetSiblingIndex() - indexArray[i]);
		}
	}

	#endregion

	#region Custom function - Set id and select environment

	public void setupEnvironment(int id)
	{
		environmentData.setID(id);
		environmentData.setMarketID((id + 1).ToString());

		if (environmentData.getType() == "counter")
		{
			hideAllCheckboxes(countersParent);
		}
		else
		{
			hideAllCheckboxes(fixturesParent);
		}

		showCheckbox(EventSystem.current.currentSelectedGameObject);
	}

	#endregion

	#region Custom function - Reset id and deselect environment

	private void resetEnvironment()
	{
		environmentData.setID(-1);

		if (environmentData.getType() == "counter")
		{
			hideAllCheckboxes(countersParent);
		}
		else
		{
			hideAllCheckboxes(fixturesParent);
		}
	}

	#endregion

	#region Custom function - Show selection checkbox

	private void showCheckbox(GameObject target)
	{
		int numChildren = target.transform.childCount;
		for (int i = 0; i < numChildren; i++)
		{
			if (target.transform.GetChild(i).gameObject.tag == "Checkmark")
			{
				target.transform.GetChild(i).gameObject.SetActive(true);
			}
		}
	}

	#endregion

	#region Custom function - Hide selection checkbox

	private void hideAllCheckboxes(GameObject parent)
	{
		int numChildren = parent.transform.childCount;
		for (int i = 0; i < numChildren; i++)
		{
			if (parent.transform.GetChild(i).gameObject.transform.Find("Check").gameObject.tag == "Checkmark")
			{
				parent.transform.GetChild(i).gameObject.transform.Find("Check").gameObject.SetActive(false);
			}
		}
	}

	#endregion

	#region Custom function - Move to main section

	public void moveToMainSection()
	{
		if (environmentData.getID() != -1)
		{
			executeTransitionTo(Main);
		}
		else
		{
			applicationSetup.showTaskPanel("Please select a " + environmentData.getType() + " to proceed further.");
		}
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
		applicationSetup.setApplicationStatus("moveToPublish");

		executeTransitionTo(Campaigns);
	}

	#endregion

	#region Custom function - Move to campaign options section

	public void moveToCampaignOptionsSection()
	{
		executeTransitionTo(CampaignOptions);
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
