using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using TMPro;

public class AccountDetails : MonoBehaviour 
{

	#region Public variables

	[Header("Tool Sections")]
	public GameObject Campaigns;
	public GameObject Login;

	[Header("UI Components")]
	public Text firstNameText;
	public Text lastNameText;
	public Text roleText;
	public InputField newPassword;
	public InputField confirmPassword;
	public TextMeshProUGUI newPasswordMsg;

	[Header("Monobehaviour Scripts")]
	public UserData userData;
	public CampaignData campaignData;
	public ApplicationSetup applicationSetup;

	#endregion

	#region Private variables

	private string firstName;
	private string lastName;
	private string role;
	private string newPasswordAPI;
	private string newPasswordMessage;
	private JObject token;

	#endregion

	#region Monobehaviours

	// Use this for initialization
	private void OnEnable() 
	{
		initializeVariables();
		splitUserName();
		StartCoroutine(populateAccountData());
	}

	// Update is called once per frame
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			changePasswordFocus();
		}

		if (Input.GetKeyDown(KeyCode.Return))
		{
			saveAccountChanges();
		}
	}

	#endregion

	#region Custom function - Initialize variables

	private void initializeVariables()
	{
		newPasswordAPI = "http://ec2-3-0-13-114.ap-southeast-1.compute.amazonaws.com/user/login/resetPassword?";
		newPassword.text = confirmPassword.text = "";
	}

	#endregion

	#region Custom function - Change focus between password fields

	private void changePasswordFocus()
	{
		if (newPassword.isFocused)
		{
			confirmPassword.Select();
		}
		else
		{
			newPassword.Select();
		}
	}

	#endregion

	#region Custom function - Split users full name to get first and last name

	private void splitUserName()
	{
		string fullName = userData.GetFullName();
		string[] nameArray = fullName.Split(' ');

		firstName = nameArray[0];
		lastName = nameArray[1];
	}

	#endregion

	#region Custom function - Populate account data fields

	private IEnumerator populateAccountData()
	{
		yield return null;

		firstNameText.text = firstName;
		lastNameText.text = lastName;

		role = userData.GetRoleName();
		if (role == "Admin")
		{
			roleText.text = "Administrator";
		}
		else
		{
			roleText.text = role;
		}
	}

	#endregion

	#region Custom function - Save changes made to account

	public void saveAccountChanges()
	{
		if (newPassword.text != "" && confirmPassword.text != "")
		{
			if (newPassword.text == confirmPassword.text)
			{
				StartCoroutine(passwordCheck());
			}
			else
			{
				userData.setPasswordCode(0);
				applicationSetup.showTaskPanel("Passwords entered do not match. Please try again.");
			}
		}
		else
		{
			userData.setPasswordCode(0);
			applicationSetup.showTaskPanel("Password field(s) cannot be left empty. Please try again.");
		}
	}

	#endregion

	#region Custom function - Handle password checking functionality

	private IEnumerator passwordCheck()
	{
		// First checks for internet connection
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			// If internet is not available then it will show an error popup
			userData.setPasswordCode(0);
			applicationSetup.showTaskPanel("Please check your internet connection and try again.");
		}
		else
		{
			// If internet is available then checks for valid new password URL
			WWW newPasswordLink = new WWW(newPasswordAPI + "username="+userData.GetEmail()+"&password="+newPassword.text);
			yield return newPasswordLink;

			if (newPasswordLink.error != null)
			{
				userData.setPasswordCode(0);
				applicationSetup.showTaskPanel("Error " + newPasswordLink.error + ". Please try again.");
			}
			else
			{
				// If internet is available then checks for valid email address and password
				//Debug.Log("Found ... ==>" + newPasswordLink.text + "<==");
				token = JObject.Parse(newPasswordLink.text);
				newPasswordMessage = token["status"].ToString();
			}

			if (newPasswordMessage == "success")
			{
				newPasswordMsg.text = token["message"].ToString();

				userData.setPasswordCode(1);
				
				StartCoroutine(moveToCampaignsSection());
			}
			else
			{
				// If passowrd is wrong then it will show an error on the screen
				newPasswordMsg.text = "You have encountered an error. Please try again.";

				userData.setPasswordCode(0);
			}
		}
	}

	#endregion

	#region Custom function - Skip saving of details

	public void skipAccountSaving()
	{
		StartCoroutine(moveToCampaignsSection());
	}

	#endregion

	#region Custom function - Move to campaigns section after saving/skipping

	private IEnumerator moveToCampaignsSection()
	{
		yield return new WaitForSeconds(1.0f);

		applicationSetup.setApplicationStatus("moveToPublish");

		executeTransitionTo(Campaigns);
	}

	#endregion

	#region Custom function - Move to login section

	public void moveToLoginSection()
	{
		executeTransitionTo(Login);
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
