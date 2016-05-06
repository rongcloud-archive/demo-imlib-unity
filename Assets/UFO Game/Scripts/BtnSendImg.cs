using UnityEngine;
using System.Collections;
using RCloud;
using UnityEngine.Events;
using UnityEngine.UI;

public class BtnSendImg : MonoBehaviour
{
	private string result;
	private string targetId;

	Text btn; 
	public void onClick()
	{
		btn = GameObject.Find ("Canvas/BtnSendImg/Text").GetComponent<Text> ();
		btn.text= result;
		Debug.Log ("BtnSendImg Clicked");
		RCImageMessageContent msg = new RCImageMessageContent ("/sdcard/location.png");
		RCSendMessageCallback cb = new RCSendMessageCallback ();
		cb.onSendSuccessCallback = () => {
			result = string.Format("发送消息成功 targetId = {0}",targetId);
			Debug.Log("RYM cb.sendSuccessCallback");
			Invoke("ChangeText", 2);
		};
		cb.onSendFailureCallback = (RCErrorCode code ) => {
			result = string.Format("发送消息失败 code = {0}",code);
			Debug.Log("RYM cb.onSendFailureCallback");
			Invoke("ChangeText", 2);
		};

		RongIMAPI.GetInstance ().SendMessage (ConversationType.ConversationType_PRIVATE, targetId, msg,null, null, cb);
	}

	void ChangeText()
	{
		Text btn = GameObject.Find ("Canvas/BtnSendText/Text").GetComponent<Text> ();
		result = "Send Image";
		btn.text= result;
	}
	void Update () 
	{
		if(btn != null)
			btn.text= result;
		targetId = GameObject.Find ("Canvas/InputTargetId").GetComponent<InputField> ().text;
	}
}
