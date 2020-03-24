using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceTest : MonoBehaviour
{
	private static AndroidJavaObject jo;
	public string[] keywords = { "寒冰箭", "开始吧", "哒哒哒哒哒哒", "剑来"};
	public Button RecordBtn;
	public Text ResultArea;

	void Start()
	{
		//#if UNITY_ANDROID && !UNITY_EDITOR
		//		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		//		 "currentActivity" 固定写法
		//		jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

		//		jo.CallStatic("bindUnityObject", "Manager"); 
		//#endif

		//		RecordBtn.onClick.AddListener(Test);
		//		ResultArea.text = "";


	}


	void SpeechRecognition()
	{

	}

	public void Test()
	{
		jo.CallStatic("startRecord");
	}

	public void ReceiveVoiceResult(string result)
	{
		ResultArea.text = result;
	}
}
