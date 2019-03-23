using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EnvironmentSetup : MonoBehaviour
{

	#region Public variables

	[Header("Game Object Arrays")]
	public GameObject[] counterPrefabs;
	public GameObject[] fixturePrefabs;

	[Header("Monobehaviour Scripts")]
	public GameManager gameManager;

	#endregion

	#region Private variables

	private int storeId;
	private string storeType;
	
	#endregion

	#region Monobehaviours

	// Use this for initialization
	private void OnEnable()
	{
		initializeVariables();
		loadEnvironment();
		checkCampaignStatus();
	}

	#endregion

	#region Custom function - Initialize variables

	private void initializeVariables()
	{
		storeId = gameManager.environmentData.getID();
		storeType = gameManager.environmentData.getType();
	}

	#endregion

	#region Custom function - Load the environment

	private void loadEnvironment()
	{
		switch (storeType)
		{
			case "counter":
				gameManager.environmentData.setEnvironment(Instantiate(counterPrefabs[storeId]) as GameObject);
				break;

			case "fixture":
				gameManager.environmentData.setEnvironment(Instantiate(fixturePrefabs[storeId]) as GameObject);
				break;
		}

		gameManager.environmentData.getEnvironment().transform.position = Vector3.zero;
		gameManager.environmentData.getEnvironment().transform.rotation = Quaternion.identity;
		gameManager.environmentData.getEnvironment().transform.parent = gameObject.transform;
	}

	#endregion

	#region Custom function - Check status of campaign

	private void checkCampaignStatus()
	{
		if (gameManager.campaignData.getDraftEdit() == true)
		{
			if (gameManager.environmentData.getEnvironment().GetComponent<EnvironmentDetails>().getTextureChange() == true)
			{
				readDataFromFile();
			}
		}
	}

	#endregion

	#region Custom function - Read data from text file

	private void readDataFromFile()
	{
		string file = gameManager.campaignData.getScreenshotFolder() + "/" + gameManager.campaignData.getCampaignName() + "/Drafts/DO_NOT_DELETE.txt";

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

				if (gameManager.campaignData.getCampaignName() == lines[0])
				{
					if (storeType == lines[1])
					{
						if (storeId == int.Parse(lines[2]))
						{
							updateEnvironment(lines);
						}
					}
				}
			}

			reader.Close();
		}
	}

	#endregion

	#region Custom function - Update the environment

	private void updateEnvironment(string[] data)
	{
		MeshOutline[] meshes = FindObjectsOfType<MeshOutline>();
		for (int i = 0; i < meshes.Length; i++)
		{
			if (meshes[i].gameObject.name == data[3])
			{
				StartCoroutine(loadTextures(meshes[i].gameObject, data[4]));
			}
		}
	}

	#endregion

	#region Custom function - Load textures from server into mesh

	private IEnumerator loadTextures(GameObject main, string url)
	{
		WWW www = new WWW(url);
		while (!www.isDone)
		{
			yield return www;

			Material[] materials = main.GetComponent<MeshRenderer>().materials;
			for (int i = 0; i < materials.Length; i++)
			{
				if (materials[i].mainTexture != null)
				{
					materials[i].mainTexture = www.texture;
					materials[i].SetTexture("_EmissionMap", www.texture);
				}
			}
		}
	}

	#endregion

}
