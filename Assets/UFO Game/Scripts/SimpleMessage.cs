using System;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;
using UnityEngine;
using RCloud;

public class SimpleMessage : RCMessageContent
{
	private string m_test;

	public String Test
	{
		get { return m_test; }
		set { m_test = value; }
	}

	public SimpleMessage()
	{
	}

	public SimpleMessage(string content)
	{
		m_test = content;
	}

	public override string Serialize()
	{ 
		RCJSONClass jsonClass = new RCJSONClass ();
		jsonClass.Add ("Test", new RCJSONData (m_test));
		string json = jsonClass.ToJSON ();
		Debug.Log("SimpleMessage Serialize = " + json);
		return json;
	}

	public override RCMessageContent Unserialize(string json)
	{
		Debug.Log("SimpleMessage Serialize = " + json);
		RCJSONClass jsonClass = RCJSON.Parse (json).AsObject;
		SimpleMessage msg = new SimpleMessage (jsonClass["Test"]);
		return msg;
	}

	public override string GetObjectName()
	{
		return "XX:Test";
	}

	public override MessagePsersistent GetPersistentFlag ()
	{
		return MessagePsersistent.MessagePersistent_NONE;
	}
}

