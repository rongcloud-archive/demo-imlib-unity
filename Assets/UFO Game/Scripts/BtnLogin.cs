using UnityEngine;
using System.Collections.Generic;
using RCloud;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Collections;
using System;

public class BtnLogin : MonoBehaviour, IRCRecivedMessageObserver, IRCConnectStatusObserver
{
	private string result;
	private string connectStatus;
	private GameObject sprite;
	private string deviceToken;
	Text btn; 

	void Awake() {
		sprite = GameObject.Find ("Player");
	}
	public void onClick()
	{
		btn = GameObject.Find ("Canvas/BtnLogin/Text").GetComponent<Text> ();
		btn.text= "Login";
		RCMessageListener.AddObserver (this);
		RCConnectionStatusListener.AddObserver (this);
//		ConversationType[] cons = new ConversationType[2];
//		cons[0] = ConversationType.ConversationType_PRIVATE;
//		cons[1] = ConversationType.ConversationType_GROUP;
//		RCConversationCallback callbak = new RCConversationCallback ();
//		callbak.onGetConversationList = (List<RCConversation> conversationList) => {
//			result = string.Format("onGetConversationList {0}",conversationList.Count);
//			Debug.Log(string.Format("RYM cb.conversationList{0}",conversationList.Count));
//		};
//		RongIMAPI.GetInstance ().GetConversationList (cons,callbak);
//		RCTextMessage txtMsg = new RCTextMessage ();
//		txtMsg.content =@"哈哈";
//		txtMsg.extra = @"sss";
//		int id =  RongIMAPI.GetInstance ().InsertMessage (ConversationType.ConversationType_PRIVATE,"24879","23087",MessageStatus.SentStatus_SENDING,txtMsg);
//		Debug.Log(string.Format("RYM cb.InsertMessage{0}",id));
//
//		RCGeneralCallback cb = new RCGeneralCallback ();
//		cb.onSuccess = () => {
//			Debug.Log("RYM cb.JoinChatRoom onSuccess");
//		};
//
//		cb.onFailure = (MessageErrorCode code) => {
//			Debug.Log(string.Format("RYM cb.JoinChatRoom onFailure{0}",code));
//		};
//
//		RongIMAPI.GetInstance ().JoinChatRoom ("chatroom001",10,cb);

//		Debug.Log(string.Format("RYM cb.ClearConversations{0}",RongIMAPI.GetInstance ().ClearConversations (cons)));

//		int[] ids =new int[2];
//		ids [0] = 6;
//		ids [1] = 8;
//		Debug.Log(string.Format("RYM cb.DeleteMessages{0}",RongIMAPI.GetInstance ().DeleteMessages (ids)));
//		RongIMAPI.GetInstance ().QuitChatRoom("chatroom001",cb);
		string userIdInput = GameObject.Find ("Canvas/InputUserId").GetComponent<InputField> ().text;
		string userNameInput = GameObject.Find ("Canvas/InputUserName").GetComponent<InputField> ().text;

		Debug.Log(string.Format("RYM cb. login userId {0} - username={1}",userIdInput,userNameInput));
		string postStr = string.Format ("userId={0}&name={1}&portraitUri=11", userIdInput, userNameInput);
		RongHttpClient client = new RongHttpClient ("http://api.cn.ronghub.com/user/getToken.json",postStr);
		string httpResult = client.ExecutePost ();
		Debug.Log(string.Format("RYM cb. login {0}",httpResult));

		RCJSONNode msgNode = RCJSON.Parse(httpResult);
		RCJSONClass msgJsonObj = msgNode.AsObject;
		string code = msgJsonObj["code"];
		if (code == "200") {
			string userId = msgJsonObj ["userId"];
			string token = msgJsonObj ["token"];
			result = "获取token成功";
			btn.text= "获取token成功";
			Debug.Log (string.Format ("RYM cb. login userId = {0} - token = {1}", userId, token));
			RongIMAPI.GetInstance ().InitRongCloud (RongHttpClient.appkey);
			RongIMAPI.GetInstance ().RegisterMessageContent (new SimpleMessage());
			if(this.deviceToken != null && this.deviceToken.Length>0)
				RongIMAPI.GetInstance ().SetDeviceToken (this.deviceToken);
			RongIMAPI.GetInstance ().ConnectRongCloud (token, userId);
			Invoke("ChangeText", 2);
		} else {
			result = "登录失败";
			btn.text= "登录失败";
			Invoke("ChangeText", 2);
		}
	}
		
	public void OnConnectStatusChanged(ConnectionStatus status)
	{
		connectStatus = status.ToString();
		Debug.Log (string.Format("OnConnectStatusChanged status {0}", status));
	}

	public void OnRecivedMessage(RCMessage message){
		string objName = message.content.GetObjectName ();
		Debug.Log ("OnRecivedMessage objName = " + objName);

		if (objName.Equals ("RC:TxtMsg")) {
			RCTextMessageContent msg = (RCTextMessageContent)message.content;
			result = string.Format( "接收到文本消息，内容：{0}",msg.Content);
			Debug.Log ("RC:TxtMsg");
		} else if (objName.Equals ("RC:ImgMsg")) {
			RCImageMessageContent msg = (RCImageMessageContent)message.content;
			Debug.Log ("RC:ImgMsg ThumbPath = " + msg.ThumbPath + "; RemoteUri = " + msg.RemoteUri);

			RCDownloadMediaFileCallback cb = new RCDownloadMediaFileCallback ();
			cb.onSuccess = (string localMediaPath) => {
				result = "下载成功";
				Debug.Log ("DownloadMedia onSuccess path = " + localMediaPath);
				msg.ImagePath = localMediaPath;
				StartCoroutine(readFile(msg.ImagePath));
			};
			cb.onFailure = (RCErrorCode code) => {
				result = "下载失败";
				Debug.Log ("DownloadMedia onFailure ErrorCode = " + code);
			};
			RongIMAPI.GetInstance ().DownloadMedia (message.m_conversation.Type,message.m_conversation.TargetId,MediaType.MediaType_IMAGE,msg.RemoteUri,cb);
		} else if (objName.Equals ("RC:AudMsg")) {
			result = "接收到语音消息";
			Debug.Log ("RC:AudMsg");
		} else {
			result = string.Format( "接收到未知消息，类型：{0}",objName);
			Debug.Log ("RC:UnknownMsg");
		}
	}

	IEnumerator readFile (string path)
	{
		Debug.Log ("000");
		WWW www = new WWW ("file://" + path);
		yield return www;
		Debug.Log ("111");
		if (www.isDone && www.error == null) {
			Debug.Log ("222");
			SpriteRenderer sr = sprite.GetComponent<SpriteRenderer> ();
			Sprite sp = Sprite.Create (www.texture, sr.sprite.textureRect, new Vector2 (0.5f, 0.5f));
			sr.sprite = sp;
		}
	}

	void OnGetDeviceToken(string token)
	{
		this.deviceToken = token;
		Debug.Log (string.Format("收到token啦{0}",deviceToken));
	}

	void ChangeText()
	{
		Text btn = GameObject.Find ("Canvas/BtnLogin/Text").GetComponent<Text> ();
		btn.text= "Login";
	}

	void OnGUI () {
		Text connectStatusText = GameObject.Find ("Canvas/CountText").GetComponent<Text> ();
		connectStatusText.text = connectStatus;
		Text callbackText = GameObject.Find ("Canvas/RecivedMessageText").GetComponent<Text> ();
		callbackText.text = result;
	}

}

