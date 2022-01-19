using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentData : MonoBehaviour
{

	#region Private variables

	private int selectedId;
	private string selectedType;
	private string marketId;
	private string marketName;
	private string counterName;
	private string fixtureName;
	private GameObject selectedEnvironment;

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
		marketName = counterName = fixtureName = "";
	}

	#endregion

	#region Custom function - Set & Get id of environment

	public void setID(int id)
	{
		selectedId = id;
	}

	public int getID()
	{
		return selectedId;
	}

	#endregion

	#region Custom function - Set & Get type of environment

	public void setType(string type)
	{
		selectedType = type;
	}

	public string getType()
	{
		return selectedType;
	}

	#endregion

	#region Custom function - Set & Get market id of environment

	public void setMarketID(string id)
	{
		marketId = id;
	}

	public string getMarketID()
	{
		return marketId;
	}

	#endregion

	#region Custom function - Set & Get name of market

	public void setMarketName(string name)
	{
		marketName = name;
	}

	public string getMarketName()
	{
		return marketName;
	}

	#endregion

	#region Custom function - Set & Get name of counter

	public void setCounterName(string name)
	{
		counterName = name;
	}

	public string getCounterName()
	{
		return counterName;
	}

	#endregion

	#region Custom function - Set & Get name of fixture

	public void setFixtureName(string name)
	{
		fixtureName = name;
	}

	public string getFixtureName()
	{
		return fixtureName;
	}

	#endregion

	#region Custom function - Set & Get environment object

	public void setEnvironment(GameObject environment)
	{
		selectedEnvironment = environment;
	}

	public GameObject getEnvironment()
	{
		return selectedEnvironment;
	}

	#endregion

}
