using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeshOutline : MonoBehaviour 
{

	#region Public variables

	[Header("Ratio Indicator")]
	public GameObject ratio;

	#endregion

	#region Private variables

	private bool isHighlighted;
	private GameObject mainPanel;
	private GameObject secondPanel;
	private GameObject taskPanel;

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
		isHighlighted = false;

		mainPanel = GameObject.Find("Main Panel");
		secondPanel = GameObject.Find("Second Panel");
		taskPanel = GameObject.Find("Task Panel");
	}

	#endregion

	#region Custom function - Mouse click action on object

	private void OnMouseDown()
	{
		if (!mainPanel.GetComponent<Image>().enabled)
		{
			if (!secondPanel.GetComponent<Image>().enabled)
			{
				if (!taskPanel.GetComponent<Image>().enabled)
				{
					if (Cursor.visible)
					{
						if (gameObject.tag == "Applicable")
						{
							updateMeshOutline(!isHighlighted);
						}
					}
				}
			}
		}
	}

	#endregion

	#region Custom function - Update (apply/remove) outline based on click

	public void updateMeshOutline(bool highlight)
	{
		if (ratio != null)
		{
			ratio.SetActive(highlight);
		}

		setHighlight(highlight);

		GetComponent<Outline>().enabled = getHighlight();
	}

	#endregion

	#region Custom function - Set & Get outline highlight

	private void setHighlight(bool highlight)
	{
		isHighlighted = highlight;
	}

	public bool getHighlight()
	{
		return isHighlighted;
	}

	#endregion

}
