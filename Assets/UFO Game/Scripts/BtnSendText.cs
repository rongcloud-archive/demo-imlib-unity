using UnityEngine;
using System.Collections.Generic;
using RCloud;
using UnityEngine.Events;
using UnityEngine.UI;

public class BtnSendText : MonoBehaviour
{
	private string result;
	private string targetId;
	Text btn; 
	public void onClick()
	{
		btn = GameObject.Find ("Canvas/BtnSendText/Text").GetComponent<Text> ();
		btn.text= result;
		Debug.Log("Button Clicked");
		result = "SendTextMessage";
		RCTextMessageContent msg = new RCTextMessageContent ("veasdf", "asdfasdf");
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

	void Update () {
		if(btn != null)
			btn.text= result;
		targetId = GameObject.Find ("Canvas/InputTargetId").GetComponent<InputField> ().text;
	}

	void ChangeText()
	{
		Text btn = GameObject.Find ("Canvas/BtnSendText/Text").GetComponent<Text> ();
		result = "SendTextMessage";
		btn.text= result;
	}
	 
}

