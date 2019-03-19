using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.IO;

public class FtpData : MonoBehaviour 
{
	
	#region Private variables

	private bool allJPGsUploaded;
	private bool allPNGsUploaded;
	private bool isUploading;
	private string campaignName;
	private string ftpFileName;
	private bool isTransitioned;
	private int jpgCount;
	private int pngCount;
	private int totalJPGs;
	private int totalPNGs;
	private string ftpUsername;
	private string ftpPassword;
	private string folderPath;
	private string campaignPath;
	private GameObject campaignInstance;

	#endregion

	#region Monobehaviours

	private void OnEnable()
	{
		initializeVariables();
		setUploadStatus(false);
	}

	// Update is called once per frame
	private void Update()
	{
		if (allJPGsUploaded && allPNGsUploaded)
		{
			if (!isTransitioned)
			{
				if (FindObjectOfType<GameManager>() != null)
				{
					StartCoroutine(FindObjectOfType<GameManager>().startTransitionToCampaigns());
				}
				else
				{
					if (Directory.Exists(campaignPath))
					{
						Directory.Delete(campaignPath, true);
					}

					if (campaignInstance != null)
					{
						Destroy(campaignInstance);

						FindObjectOfType<CampaignMenu>().updateCampaignCount("dec");
					}

					setUploadStatus(false);
					GetComponent<ApplicationSetup>().showTaskPanel("Files Uploaded!");
				}
				
				isTransitioned = true;
			}
		}
	}

	#endregion

	#region Custom function - Initialize variables

	private void initializeVariables()
	{
		ftpUsername = "rakesh";
		ftpPassword = "Raks_0723";
	}

	#endregion

	#region Custom function - Prepare asset data for ftp uploading

	public void prepareFtpData()
	{
		allJPGsUploaded = allPNGsUploaded = isTransitioned = false;

		campaignName = FindObjectOfType<CampaignData>().getCampaignName();
		campaignName = campaignName.Replace(@" ", "_");

		setFtpFileName(PlayerPrefs.GetString("ftpFile_" + campaignName));
		setFtpDirectoryPath();
		setJPGUploadCount(0);
		setPNGUploadCount(0);
	}

	#endregion

	#region Custom function - Set directory path for ftp uploading

	private void setFtpDirectoryPath()
	{
		if (FindObjectOfType<GameManager>() == null)
		{
			folderPath = GetComponent<CampaignData>().getScreenshotFolder() + "/" + GetComponent<CampaignData>().getCampaignName() + "/Drafts";
		}
		else
		{
			folderPath = GetComponent<CampaignData>().getScreenshotFolder() + "/" + GetComponent<CampaignData>().getCampaignName() + "/Published";
		}
	}

	#endregion

	#region Custom function - Set & Get name of ftp file

	public void setFtpFileName(string name)
	{
		ftpFileName = name;
	}

	public string getFtpFileName()
	{
		Debug.Log("getFtpFileName: " + ftpFileName);
		return ftpFileName;
	}

	#endregion

	#region Custom function - Add to name of ftp file

	public void addToFtpFileName(string name)
	{
		ftpFileName += name;
		PlayerPrefs.SetString("ftpFile_" + campaignName, ftpFileName);
	}

	#endregion

	#region Custom function - Remove from name of ftp file

	public void removeFromFtpFileName()
	{
		if (ftpFileName.Length > 0)
		{
			ftpFileName = ftpFileName.Remove(ftpFileName.Length - 1);
		}
	}

	#endregion

	#region Custom function - Set & Get status of file upload

	public void setUploadStatus(bool status)
	{
		isUploading = status;
	}

	public bool getUploadStatus()
	{
		return isUploading;
	}

	#endregion

	#region Custom function - Configure deleting of a draft campaign after publishing

	public void configureCampaignDeletion(string path, GameObject instance)
	{
		campaignPath = path;
		campaignInstance = instance;
	}

	#endregion

	#region Custom function - Execute protocols pertaining to FTP

	public void executeFtpProtocol()
	{
		uploadJPG();
		uploadPNG();
		GetComponent<CampaignData>().createCampaign();
	}

	#endregion

	#region Custom function - Upload a JPG to ftp server

	private void uploadJPG()
	{
		using (System.Net.WebClient client = new System.Net.WebClient())
		{
			string folderRepair = folderPath.ToString();
			folderRepair = folderRepair.Replace(@"\", "/");

			DirectoryInfo dir = new DirectoryInfo(folderRepair);
			FileInfo[] info = dir.GetFiles("*.jpg*");

			totalJPGs = info.Length;

			if (totalJPGs > 0)
			{
				string filePath = folderRepair + "/screenshot_" + GetComponent<UserData>().getUniqueUserName() + "_" + GetComponent<CampaignData>().getUniqueCampaignName() + "_3840x2160_" + jpgCount + ".jpg";

				Uri Check = new Uri("ftp://3.0.13.114/" + new FileInfo(filePath).Name);
				client.UploadFileCompleted += new System.Net.UploadFileCompletedEventHandler(OnJPGUploadCompleted);
				client.UploadProgressChanged += new System.Net.UploadProgressChangedEventHandler(OnJPGUploadProgressChanged);
				client.Credentials = new System.Net.NetworkCredential(ftpUsername, ftpPassword);
				client.UploadFileAsync(Check, "STOR", filePath);
			}
			else
			{
				allJPGsUploaded = true;
			}
		}
	}

	#endregion

	#region Custom function - Upload a PNG to ftp server

	private void uploadPNG()
	{
		using (System.Net.WebClient client = new System.Net.WebClient())
		{
			string folderRepair = folderPath.ToString();
			folderRepair = folderRepair.Replace(@"\", "/");

			DirectoryInfo dir = new DirectoryInfo(folderRepair);
			FileInfo[] info = dir.GetFiles("*.png*");

			totalPNGs = info.Length;

			if (totalPNGs > 0)
			{
				string filePath = folderRepair + "/screenshot_" + GetComponent<UserData>().getUniqueUserName() + "_" + GetComponent<CampaignData>().getUniqueCampaignName() + "_3840x2160_" + pngCount + ".png";

				Uri Check = new Uri("ftp://3.0.13.114/" + new FileInfo(filePath).Name);
				client.UploadFileCompleted += new System.Net.UploadFileCompletedEventHandler(OnPNGUploadCompleted);
				client.UploadProgressChanged += new System.Net.UploadProgressChangedEventHandler(OnPNGUploadProgressChanged);
				client.Credentials = new System.Net.NetworkCredential(ftpUsername, ftpPassword);
				client.UploadFileAsync(Check, "STOR", filePath);
			}
			else
			{
				allPNGsUploaded = true;
			}
		}
	}

	#endregion

	#region Custom function - Check JPG upload progress

	private void OnJPGUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
	{
		setUploadStatus(true);
		GetComponent<ApplicationSetup>().showTaskPanel("File Upload in Progress: " + e.ProgressPercentage + "%");
	}

	#endregion

	#region Custom function - Check PNG upload progress

	private void OnPNGUploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
	{
		setUploadStatus(true);
		GetComponent<ApplicationSetup>().showTaskPanel("File Upload in Progress: " + e.ProgressPercentage + "%");
	}

	#endregion

	#region Custom function - Check JPG upload completion

	private void OnJPGUploadCompleted(object sender, UploadFileCompletedEventArgs e)
	{
		if (totalJPGs > 0)
		{
			if (jpgCount < totalJPGs)
			{
				setJPGUploadCount(jpgCount + 1);
				uploadJPG();
			}
			else
			{
				allJPGsUploaded = true;
			}
		}
	}

	#endregion

	#region Custom function - Check PNG upload completion

	private void OnPNGUploadCompleted(object sender, UploadFileCompletedEventArgs e)
	{
		if (totalPNGs > 0)
		{
			if (pngCount < totalPNGs)
			{
				setPNGUploadCount(pngCount + 1);
				uploadPNG();
			}
			else
			{
				allPNGsUploaded = true;
			}
		}
	}

	#endregion

	#region Custom function - Set & Get JPG upload count

	private void setJPGUploadCount(int count)
	{
		jpgCount = count;
	}

	public int getJPGUploadCount()
	{
		return jpgCount;
	}

	#endregion

	#region Custom function - Set & Get PNG upload count

	private void setPNGUploadCount(int count)
	{
		pngCount = count;
	}

	public int getPNGUploadCount()
	{
		return pngCount;
	}

	#endregion

}
