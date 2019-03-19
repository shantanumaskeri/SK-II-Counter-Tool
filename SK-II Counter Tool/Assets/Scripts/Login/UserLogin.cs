using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class UserLogin : MonoBehaviour
{

	#region Public variables

	[Header("Tool Sections")]
    public GameObject EditAccount;
	public GameObject Campaigns;

	[Header("Login Components")]
	public GameObject LoginPanel;
	public GameObject ForgotPasswordPanel;
	public InputField loginEmail;
	public InputField loginPassword;
	public InputField forgotEmail;
	public Text loginDisplayText;
	public Text forgotDisplayText;

	[Header("Monobehaviour Scripts")]
	public UserData userData;
	public CampaignData campaignData;
	public ApplicationSetup applicationSetup;

	#endregion

	#region Private variables

	private bool toggle;
	private string loginAPI;
    private string forgotPasswordAPI;
	private string loginMessage;
    private string forgotMessage;
	private string Username;
	private string Password;

	#endregion

	#region Monobehaviours

	private void OnEnable()
	{
		initializeVariables();
	}

	// Update is called once per frame
	private void Update()
	{
		handleKeyInput();
	}

	#endregion

	#region Custom function - Initialize variables

	private void initializeVariables()
	{
		toggle = false;
		loginAPI = "http://ec2-3-0-13-114.ap-southeast-1.compute.amazonaws.com/user/login/validateLogin?";
		forgotPasswordAPI = "http://ec2-3-0-13-114.ap-southeast-1.compute.amazonaws.com/user/login/forgotPassword?username=";
	}

	#endregion

	#region Custom function - Handle key press input

	private void handleKeyInput()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (loginEmail.isFocused)
			{
				loginPassword.Select();
			}
			else
			{
				loginEmail.Select();
			}
		}

		if (Input.GetKeyDown(KeyCode.Return))
		{
			Login();
		}
	}

	#endregion

	#region Custom function - Show login panel (from forgot password screen)

	public void ShowLoginPanel()
    {
        LoginPanel.SetActive(true);
        loginDisplayText.text = "";
        loginEmail.text = "";
        loginPassword.text = "";
		forgotEmail.text = "";
		ForgotPasswordPanel.SetActive(false);
    }

	#endregion

	#region Custom function - Show forgot password panel (from login screen)

	public void ShowForgotPasswordPanel()
    {
        LoginPanel.SetActive(false);
        ForgotPasswordPanel.SetActive(true);
        forgotDisplayText.text = "";
    }

	#endregion

	#region Custom function - This will attempt to login on button/key press

	public void Login()
    {
        StartCoroutine(LoginCheck());
    }

	#endregion

	#region Custom function - Handling login functionality
	
	private IEnumerator LoginCheck()
    {
		// First checks for internet connection
		if (Application.internetReachability == NetworkReachability.NotReachable)
        {
			// If internet is not available then it will show an error popup
			applicationSetup.showTaskPanel("Please check your internet connection and try again.");
        }
        else
        {
			Username = loginEmail.text.Trim();
			Password = loginPassword.text.Trim();
			// If internet is available then checks for valid login URL
			WWW loginLink = new WWW(loginAPI + "username=" + Username + "&password=" + Password + "&sort=desc");
            yield return loginLink;

            if (loginLink.error != null)
            {
				applicationSetup.showTaskPanel("Error " + loginLink.error + ". Please try again.");
            }
            else
            {
				// If internet is available then checks for valid email address and password
				//Debug.Log("UserLogin Found ... ==>" + loginLink.text + "<==");
				applicationSetup.setJSONToken(JObject.Parse(loginLink.text));
                loginMessage = applicationSetup.getJSONToken()["msg"].ToString();
            }

            if (loginMessage == "success")
            {
                loginDisplayText.text = "";
                loginEmail.text = "";
                loginPassword.text = "";

				setUserData();
				moveToNextSection();
			}
            else
            {
				// If email and/or passowrd is wrong then it will show an error on the same screen
				loginDisplayText.text = "Invalid login credentials. Please try again.";
				StartCoroutine(hideDisplayMessage(loginDisplayText));
            }
        }
    }

	#endregion

	#region Custom function - This will attempt to reset password on button press

	public void ResetPassword()
    {
        StartCoroutine(EmailCheck());
    }

	#endregion

	#region Custom function - Handling reset password functionality
	
	private IEnumerator EmailCheck()
    {
		// First checks for internet connection
		if (Application.internetReachability == NetworkReachability.NotReachable)
        {
			// If internet is not available then it will show an error popup
			applicationSetup.showTaskPanel("Please check your internet connection and try again.");
		}
        else
        {
			// If internet is available then checks for valid forgot password URL
			WWW resPassetLink = new WWW(forgotPasswordAPI + forgotEmail.text);
            yield return resPassetLink;
            if (resPassetLink.error != null)
            {
				applicationSetup.showTaskPanel("Error " + resPassetLink.error + ". Please try again.");
            }
            else
            {
				// If internet is available then checks for valid email address
				//Debug.Log("Found ... ==>" + resPassetLink.text + "<==");
                applicationSetup.setJSONToken(JObject.Parse(resPassetLink.text));
                forgotMessage = applicationSetup.getJSONToken()["status"].ToString();
            }

            if (forgotMessage == "success")
            {
				// If email is correct then will show success message
				forgotDisplayText.text = "Your password has been reset. Please check your inbox.";
				StartCoroutine(hideDisplayMessage(forgotDisplayText));
			}
            else
            {
				// If email is wrong then will show an error on the same screen
				forgotDisplayText.text = "You have entered a wrong email address. Please try again.";
				StartCoroutine(hideDisplayMessage(forgotDisplayText));
			}
        }
    }

	#endregion
	
	#region Custom function - Stores all user data on successful log in

	private void setUserData()
	{
		userData.SetUserName(Username);
		userData.SetPassword(Password);
		userData.SetUserID((int)applicationSetup.getJSONToken()["user_detail"]["user_id"]);
		userData.SetEmail(applicationSetup.getJSONToken()["user_detail"]["email"].ToString());
		userData.SetFullName(applicationSetup.getJSONToken()["user_detail"]["full_name"].ToString());
		userData.SetRoleID((int)applicationSetup.getJSONToken()["user_detail"]["role_id"]);
		userData.SetRoleName(applicationSetup.getJSONToken()["user_detail"]["role_name"].ToString());
		userData.SetStoreID(applicationSetup.getJSONToken()["user_detail"]["store_id"].ToString());
		userData.SetStoreName(applicationSetup.getJSONToken()["user_detail"]["store_name"].ToString());
		userData.SetStoreIDs(userData.GetStoreID().Split(','));
	}

	#endregion

	#region Custom function - Toggle (show/hide) password entered by user

	public void togglePassword()
	{
		toggle = !toggle;
		switch (toggle)
		{
			case true:
				loginPassword.contentType = InputField.ContentType.Alphanumeric;
				break;

			case false:
				loginPassword.contentType = InputField.ContentType.Password;
				break;
		}

		loginPassword.ForceLabelUpdate();
	}

	#endregion

	#region Custom function - Hide display message text

	private IEnumerator hideDisplayMessage(Text message)
	{
		yield return new WaitForSeconds(2.0f);

		message.text = "";
	}

	#endregion

	#region Custom function - Move to next section

	private void moveToNextSection()
	{
		if (userData.getPasswordCode() == 1)
		{
			applicationSetup.setApplicationStatus("moveToPublish");

			executeTransitionTo(Campaigns);
		}
		else
		{
			executeTransitionTo(EditAccount);
		}
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