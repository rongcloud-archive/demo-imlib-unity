using UnityEngine;
using System.Collections;
using RCloud;
using UnityEngine.UI;

public class BtnSendSimpleMessage : MonoBehaviour {

	private string result;
	private string targetId;
	Text btn; 
	public void onClick()
	{
	    btn = GameObject.Find ("Canvas/BtnSendSimpleMessage/Text").GetComponent<Text> ();
		btn.text= result;
		Debug.Log("Button Clicked");
		result = "SendSimpleMessage";
		SimpleMessage msg = new SimpleMessage ("veasdf");
		RCSendMessageCallback cb = new RCSendMessageCallback ();
		cb.onSendSuccessCallback = () => {
			result = "发送自定义消息成功";
			Debug.Log("RYM cb.sendSuccessCallback");
			Invoke("ChangeText", 2);
		};
		cb.onSendFailureCallback = (RCErrorCode code) => {
			result = string.Format("发送自定义消息失败 code = {0}",code);
			Debug.Log(result);
			Invoke("ChangeText", 2);
		};
		RongIMAPI.GetInstance ().SendMessage (ConversationType.ConversationType_PRIVATE, targetId, msg,null, null, cb);
	}

	void Update () {
//		btn = GameObject.Find ("Canvas/BtnSendSimpleMessage/Text").GetComponent<Text> ();
		if(btn != null)
			btn.text= result;
		targetId = GameObject.Find ("Canvas/InputTargetId").GetComponent<InputField> ().text;
	}

	void ChangeText()
	{
		Text btn = GameObject.Find ("Canvas/BtnSendSimpleMessage/Text").GetComponent<Text> ();
		result = "SendSimpleMessage";
		btn.text= result;
	}
}
