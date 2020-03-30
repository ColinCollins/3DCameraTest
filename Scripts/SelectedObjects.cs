using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedObjects : MonoBehaviour
{
	private Vector2 startPos;
	private Vector2 endPos;

	private Texture2D guiTexture;

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
			startPos = Vector2.zero;
			endPos = Vector2.zero;
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
}
