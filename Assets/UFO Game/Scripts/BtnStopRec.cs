using UnityEngine;
using System.Collections;
using RCloud;
using UnityEngine.UI;

public class BtnStopRec : MonoBehaviour
{
	public void onClick ()
	{
		Debug.Log ("BtnStopRec Clicked");
		RongIMAPI.GetInstance ().StopRecordVoice ();
	}
}

