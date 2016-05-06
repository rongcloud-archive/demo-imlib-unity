using UnityEngine;
using System;
using System.Collections;
using RCloud;
using UnityEngine.UI;

public class BtnStartRec : MonoBehaviour
{
	private string targetId;
	private string result;
	Text btn; 
	public void onClick ()
	{
		btn = GameObject.Find ("Canvas/BtnStartRec/Text").GetComponent<Text> ();
		btn.text= "Start";
		Debug.Log ("BtnStartRec Clicked");
		RCVoiceCaptureCallback cb = new RCVoiceCaptureCallback ();
		cb.onVoiceVolume = (float volume) => {
			Debug.Log (string.Format ("RYM cb.VoiceCaptureVolumeCallback volume = {0}", volume));
		};
		cb.onVoiceCaptureFinished = (Boolean timeout, string voiceUri, int duration) => {
			Debug.Log (string.Format ("RYM cb.VoiceCaptureFinishedCallback volume = {0}, {1}, {2}", timeout, voiceUri, duration));

			RCAudioMessageContent msg = new RCAudioMessageContent (voiceUri, duration);

			RCSendMessageCallback callback = new RCSendMessageCallback ();
			callback.onSendSuccessCallback = () => {
				result = "发送语音消息成功";
				Debug.Log("RYM cb.sendSuccessCallback");
				Invoke("ChangeText", 2);
			};
			callback.onSendFailureCallback = (RCErrorCode code) => {
				result = string.Format("发送自定义消息失败 code = {0}",code);
				Debug.Log(string.Format("发送自定义消息失败 code = {0}",code));
				result = "发送语音消息失败";
				Invoke("ChangeText", 2);
			};
			Debug.Log("RYM RongIMAPI.GetInstance ().SendMessage");
			RongIMAPI.GetInstance ().SendMessage (ConversationType.ConversationType_PRIVATE, targetId, msg, null, null, callback);
		};
		RongIMAPI.GetInstance ().StartRecordVoice (cb);
	}

	void Update () {
		if(btn != null)
			btn.text= result;
		targetId = GameObject.Find ("Canvas/InputTargetId").GetComponent<InputField> ().text;
	}

	void ChangeText()
	{
		Text btn = GameObject.Find ("Canvas/BtnSendText/Text").GetComponent<Text> ();
		result = "Record Voice";
		btn.text= result;
	}
}

