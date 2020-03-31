using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HighlightingSystem;


public class SelectedObjects : MonoBehaviour
{
	private Vector2 startPos;
	private Vector2 endPos;

	private Texture2D guiTexture;

	public List<GameObject> FreedomObjs;
	private List<GameObject> selectedObjs;

	private void Start()
	{
		ColorUtility.TryParseHtmlString("#AED6F1", out Color color);
		color.a = 0.6f;

		guiTexture = new Texture2D(1, 1);
		guiTexture.SetPixel(0, 0, color);
		guiTexture.Apply();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			startPos = Input.mousePosition;
			endPos = startPos;
		}

		if (Input.GetMouseButton(0))
		{
			endPos = Input.mousePosition;
		}

		if (Input.GetMouseButtonUp(0))
		{
			selectAllObjects();
			startPos = Vector2.zero;
			endPos = Vector2.zero;
		}

	}

	private void selectAllObjects()
	{
		Vector2 lbP = Vector2.zero;
		Vector2 rtP = Vector2.zero;

		if (startPos.x > endPos.x)
		{
			lbP.x = endPos.x;
			rtP.x = startPos.x;
		}
		else {
			lbP.x = startPos.x;
			rtP.x = endPos.x;
		}

		if (startPos.y > endPos.y)
		{
			lbP.y = endPos.y;
			rtP.y = startPos.y;
		}
		else {
			rtP.y = endPos.y;
			lbP.y = startPos.y;
		}

		// Debug.Log("left bottom: " + lbP + "right top: " + rtP);

		Camera camera = Camera.main;
		for (int i = 0; i < FreedomObjs.Count; i++)
		{
			Vector3 location = camera.WorldToScreenPoint(FreedomObjs[i].transform.position);

			// Debug.Log("物体位置：" + location);

			if (location.x < lbP.x || location.x > rtP.x || location.y < lbP.y || location.y > rtP.y
				|| location.z < camera.nearClipPlane || location.z > camera.farClipPlane)
			{
				// false
				FreedomObjs[i].GetComponent<Highlighter>().enabled = false;
			}
			else {
				// true
				FreedomObjs[i].GetComponent<Highlighter>().enabled = true;
			}
		}

	}


	// GUI Canver 左上角为中心点
	private void OnGUI()
	{
		Vector2 size = endPos - startPos;
		Rect boxRect = new Rect(startPos.x, Screen.height - startPos.y, size.x, -size.y);

		GUI.skin.box.normal.background = guiTexture;
		GUI.skin.box.border = new RectOffset();

		GUI.Box(boxRect, GUIContent.none);
	}

	private void OnPostRener() 
	{ 
		
	}
}
