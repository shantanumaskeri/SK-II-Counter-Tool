using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour
{

	#region Private variables

	private float deltaTime = 0.0f;

	#endregion

	#region Monobehaviours

	// Update is called once per frame
	private void Update()
	{
		incrementTime();
	}

	private void OnGUI()
	{
		showFPS();
	}

	#endregion

	#region Custom functions - Display FPS on screen

	private void incrementTime()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
	}

	private void showFPS()
	{
		int w = Screen.width, h = Screen.height;
 
		GUIStyle style = new GUIStyle();
 
		Rect rect = new Rect(0, 0, w, h * 0.02f);
		style.alignment = TextAnchor.UpperCenter;
		style.font = (Font)Resources.Load("Fonts/LiberationSans-BoldItalic");
		style.fontSize = h * 2 / 50;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.0f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label(rect, text, style);
	}

	#endregion
}