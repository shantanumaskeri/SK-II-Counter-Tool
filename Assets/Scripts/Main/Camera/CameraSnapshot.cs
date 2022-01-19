using UnityEngine;
using System.Collections;
using System.IO;

public class CameraSnapshot : MonoBehaviour
{

	#region Public variables

	[Header("Camera Snapshot Settings")]
	public bool optimizeForManyScreenshots = true;
	public int captureWidth = 3840;
    public int captureHeight = 2160;
	public GameObject hideGameObject;

	[Header("Monobehaviour Scripts")]
	public GameManager gameManager;

	#endregion

	#region Private variables

	private bool snapshotCapturedAndSaved;
	private Rect rect;
    private RenderTexture renderTexture;
	private Texture2D screenShot;

	#endregion

	#region Monobehaviours

	// Use this for initialization
	private void OnEnable()
	{
		initializeVariables();
		getUniqueFileNames();
		createCampaignDirectory();
	}

	#endregion

	#region Custom function - Initialize variables

	private void initializeVariables()
	{
		snapshotCapturedAndSaved = false;
		gameManager.campaignData.setCampaignRootFolder("");
		gameManager.campaignData.setPublishFolder("");
		gameManager.campaignData.setDraftFolder("");
	}

	#endregion

	#region Custom function - Get unique file names from login credentials

	private void getUniqueFileNames()
	{
		string full_name = gameManager.userData.GetFullName();
		gameManager.userData.setUniqueUserName(full_name.Replace(@" ", "_"));

		string campaign_name = gameManager.campaignData.getCampaignName();
		gameManager.campaignData.setUniqueCampaignName(campaign_name.Replace(@" ", "_"));
	}

	#endregion

	#region  Custom function - Create the different campaign directories

	private void createCampaignDirectory()
	{
		if (gameManager.campaignData.getCampaignRootFolder() == null || gameManager.campaignData.getCampaignRootFolder().Length == 0)
		{
			gameManager.campaignData.setCampaignRootFolder(gameManager.campaignData.getCampaignRootFolder() + gameManager.campaignData.getScreenshotFolder() + "/" + gameManager.campaignData.getCampaignName());

			System.IO.Directory.CreateDirectory(gameManager.campaignData.getCampaignRootFolder());
		}

		obtainDirectoryFiles();
	}

	#endregion

	#region Custom function - Obtain number of files in a campaigns directory

	private void obtainDirectoryFiles()
	{
		string jpgMask = string.Format("screenshot_{0}_{1}_{2}x{3}*.{4}", gameManager.userData.getUniqueUserName(), gameManager.campaignData.getUniqueCampaignName(), captureWidth, captureHeight, "jpg");
		string pngMask = string.Format("screenshot_{0}_{1}_{2}x{3}*.{4}", gameManager.userData.getUniqueUserName(), gameManager.campaignData.getUniqueCampaignName(), captureWidth, captureHeight, "png");

		gameManager.setJPGCount(Directory.GetFiles(gameManager.campaignData.getCampaignRootFolder(), jpgMask, SearchOption.TopDirectoryOnly).Length);
		gameManager.setPNGCount(Directory.GetFiles(gameManager.campaignData.getCampaignRootFolder(), pngMask, SearchOption.TopDirectoryOnly).Length);
	}

	#endregion

	#region Custom function - Create unique file name for each snapshot

	private string uniqueFilename(string folder, int width, int height)
    {
		string fileLocation;
		string filename;

		if (gameManager.getSnapshotFormat() == "jpg")
		{
			fileLocation = string.Format("{0}/screenshot_{1}_{2}_{3}x{4}_{5}.{6}", folder, gameManager.userData.getUniqueUserName(), gameManager.campaignData.getUniqueCampaignName(), width, height, gameManager.getJPGCount(), gameManager.getSnapshotFormat());
			filename = string.Format("screenshot_{0}_{1}_{2}x{3}_{4}.{5}", gameManager.userData.getUniqueUserName(), gameManager.campaignData.getUniqueCampaignName(), width, height, gameManager.getJPGCount(), gameManager.getSnapshotFormat());

			if (folder == gameManager.campaignData.getCampaignRootFolder())
			{
				gameManager.setJPGCount(gameManager.getJPGCount() + 1);
			}
		}
		else
		{
			fileLocation = string.Format("{0}/screenshot_{1}_{2}_{3}x{4}_{5}.{6}", folder, gameManager.userData.getUniqueUserName(), gameManager.campaignData.getUniqueCampaignName(), width, height, gameManager.getPNGCount(), gameManager.getSnapshotFormat());
			filename = string.Format("screenshot_{0}_{1}_{2}x{3}_{4}.{5}", gameManager.userData.getUniqueUserName(), gameManager.campaignData.getUniqueCampaignName(), width, height, gameManager.getPNGCount(), gameManager.getSnapshotFormat());

			if (folder == gameManager.campaignData.getCampaignRootFolder())
			{
				gameManager.setPNGCount(gameManager.getPNGCount() + 1);
			}
		}

		gameManager.ftpData.addToFtpFileName(filename + "|");

		return fileLocation;
     }

	#endregion

	#region Custom function - Capture snapshot of specified resolution

	public void captureSnapshot()
    {
		if (!snapshotCapturedAndSaved)
		{
			gameManager.executePreSnapshotActions();

			if (hideGameObject != null)
			{
				hideGameObject.SetActive(false);
			}

			if (renderTexture == null)
			{
				rect = new Rect(0, 0, captureWidth, captureHeight);
				renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
				screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
			}

			Camera camera = this.GetComponent<Camera>();
			camera.targetTexture = renderTexture;
			camera.Render();

			RenderTexture.active = renderTexture;
			screenShot.ReadPixels(rect, 0, 0);

			camera.targetTexture = null;
			RenderTexture.active = null;

			gameManager.setDefaultFileName(uniqueFilename(gameManager.campaignData.getCampaignRootFolder(), (int)rect.width, (int)rect.height));
			
			byte[] fileHeader = null;
			byte[] fileData = null;

			if (gameManager.getSnapshotFormat() == "png")
			{
				fileData = screenShot.EncodeToPNG();
			}
			else if (gameManager.getSnapshotFormat() == "jpg")
			{
				fileData = screenShot.EncodeToJPG();
			}

			new System.Threading.Thread(() =>
			{
				var f1 = System.IO.File.Create(gameManager.getDefaultFileName());
				if (fileHeader != null) f1.Write(fileHeader, 0, fileHeader.Length);
				f1.Write(fileData, 0, fileData.Length);
				f1.Close();
			}).Start();

			if (hideGameObject != null)
			{
				hideGameObject.SetActive(true);
			}

			if (optimizeForManyScreenshots == false)
			{
				Destroy(renderTexture);
				renderTexture = null;
				screenShot = null;
			}

			snapshotCapturedAndSaved = true;

			StartCoroutine(enableNextCapture());
		}
	}

	#endregion

	#region Custom function - Wait for specified time before allowing to capture next snapshot

	private IEnumerator enableNextCapture()
	{
		yield return new WaitForSeconds(1.0f);

		gameManager.executePostSnapshotActions();

		snapshotCapturedAndSaved = false;

		StopCoroutine(enableNextCapture());
	}

	#endregion

}