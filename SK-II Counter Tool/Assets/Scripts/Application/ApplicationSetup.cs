using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

public class ApplicationSetup : MonoBehaviour 
{

	#region Public variables

	[Header("Tool Sections")]
	public GameObject Login;

	[Header("Game Objects")]
	public GameObject minimize;
	public GameObject close;

	[Header("UI Components")]
	public Image taskPanel;
	public TextMeshProUGUI taskText;

	[Header("Monobehaviour Scripts")]
	public UserData userData;
	public FtpData ftpData;

	#endregion

	#region Private variables

	private JObject token;
	private string appStatus;

	#endregion

	#region DLL Imports

	[DllImport("user32.dll")]
	private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	private static extern IntPtr GetForegroundWindow();

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

	#endregion

	#region Monobehaviours

	private void OnEnable()
	{
		checkToolRuntimePlatform();
	}

	#endregion

	#region Custom function - Check runtime platform to toggle buttons and change window size

	private void checkToolRuntimePlatform()
	{
		if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			minimize.SetActive(false);
			close.SetActive(false);

			Screen.fullScreen = false;
		}

		if (Application.platform == RuntimePlatform.WindowsPlayer)
		{
			minimize.SetActive(true);
			close.SetActive(true);

			Screen.fullScreen = true;
		}
	}

	#endregion

	#region Custom function - Show task panel with custom message

	public void showTaskPanel(string message)
	{
		taskPanel.enabled = true;
		taskText.gameObject.SetActive(true);
		taskText.text = message;

		if (ftpData.getUploadStatus() == false)
		{
			StartCoroutine(hideTaskPanel());
		}
	}

	#endregion

	#region Custom function - Hide task panel and message

	private IEnumerator hideTaskPanel()
	{
		yield return new WaitForSeconds(2.0f);

		taskText.text = "";
		taskText.gameObject.SetActive(false);
		taskPanel.enabled = false;
	}

	#endregion

	#region Custom function - Set & Get JSON token object

	public void setJSONToken(JObject jObject)
	{
		token = jObject;
	}

	public JObject getJSONToken()
	{
		return token;
	}

	#endregion

	#region Custom function - Set & Get status of application

	public void setApplicationStatus(string status)
	{
		appStatus = status;
	}

	public string getApplicationStatus()
	{
		return appStatus;
	}

	#endregion

	#region Custom function - Log out of application

	public void Logout(GameObject go)
	{
		userData.resetUserData();

		go.SetActive(false);
		Login.SetActive(true);
	}

	#endregion

	#region Custom function - Check status of application window focus

	private void OnApplicationFocus(bool hasFocus)
	{
		hasFocus = ApplicationIsActivated();
	}

	#endregion

	#region Custom function - Check which window is activated in foreground

	public static bool ApplicationIsActivated()
	{
		var activatedHandle = GetForegroundWindow();
		if (activatedHandle == IntPtr.Zero)
		{
			return false;
		}

		var procId = Process.GetCurrentProcess().Id;
		int activeProcId;
		GetWindowThreadProcessId(activatedHandle, out activeProcId);

		return activeProcId == procId;
	}

	#endregion

	#region Custom function - Minimize the application

	public void minimizeApplication()
	{
		ShowWindow(GetActiveWindow(), 2);
	}

	#endregion

	#region Custom function - Closing the application

	public void closeApplication()
	{
		Application.Quit();
	}
	
	#endregion

}
