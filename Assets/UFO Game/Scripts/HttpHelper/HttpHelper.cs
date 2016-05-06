﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace RCloud  
{  
	class RongHttpClient
	{
		String methodUrl = null;
		String postStr = null;
//		static public String appkey = "z3v5yqkbv8v30";
//		String appSecret = "aL1VbnQdzNII8v";
		static public String appkey = "p5tvi9dst19k4";
		String appSecret = "nDkhVnhcemXU";
		public RongHttpClient(String methodUrl,String postStr)
		{
			this.methodUrl = methodUrl;
			this.postStr = postStr;
		}

		public String ExecutePost()
		{
			System.Random rd = new System.Random();
			int rd_i = rd.Next();
			String nonce = Convert.ToString(rd_i);

			String timestamp = Convert.ToString(ConvertDateTimeInt(DateTime.Now)); 

			String signature = GetHash(this.appSecret + nonce + timestamp);

			//ServicePointManager.ServerCertificateValidationCallback += ValidateRemoteCertificate;
			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

			HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(this.methodUrl);

			myRequest.Method = "POST";
			myRequest.ContentType = "application/x-www-form-urlencoded";

			myRequest.Headers.Add("App-Key", RongHttpClient.appkey);
			myRequest.Headers.Add("Nonce", nonce);
			myRequest.Headers.Add("Timestamp", timestamp);
			myRequest.Headers.Add("Signature", signature);
			myRequest.ReadWriteTimeout = 30 * 1000;

			byte[] data = Encoding.UTF8.GetBytes(this.postStr);
			myRequest.ContentLength = data.Length;

			Stream newStream = myRequest.GetRequestStream();

			// Send the data.
			newStream.Write(data, 0, data.Length);
			newStream.Close();

			HttpWebResponse myResponse = null;
			try
			{
				myResponse = (HttpWebResponse)myRequest.GetResponse();
				StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);

				string content = reader.ReadToEnd();

				return content;
			}
			//异常请求
			catch (WebException e)
			{
				myResponse = (HttpWebResponse)e.Response;
				using (Stream errData = myResponse.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(errData))
					{
						string text = reader.ReadToEnd();

						return text;
					}
				}
			}

		}
		/// <summary>  
		/// DateTime时间格式转换为Unix时间戳格式  
		/// </summary>  
		/// <param name="time"> DateTime时间格式</param>  
		/// <returns>Unix时间戳格式</returns>  
		public int ConvertDateTimeInt(System.DateTime time)
		{
			System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
			return (int)(time - startTime).TotalSeconds;
		}

		public String GetHash(String input)
		{
			//建立SHA1对象
			SHA1 sha = new SHA1CryptoServiceProvider(); 

			//将mystr转换成byte[]
			UTF8Encoding enc = new UTF8Encoding();
			byte[] dataToHash = enc.GetBytes(input);

			//Hash运算
			byte[] dataHashed = sha.ComputeHash(dataToHash);

			//将运算结果转换成string
			string hash = BitConverter.ToString(dataHashed).Replace("-", "");

			return hash;
		}

		/// <summary>
		/// Certificate validation callback.
		/// </summary>
		private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors error)
		{
			// If the certificate is a valid, signed certificate, return true.
			if (error == System.Net.Security.SslPolicyErrors.None)
			{
				return true;
			}

			Console.WriteLine("X509Certificate [{0}] Policy Error: '{1}'",
				cert.Subject,
				error.ToString());

			return false;
		}

	}
} 