using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Absir
{
	public class AB_WWW
	{
		public static readonly Dictionary<string, string> CertHashDictTrust = new Dictionary<string, string> ();

		public static readonly AB_WWW ME = new AB_WWW ();

		protected AB_WWW ()
		{
			ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback (ValidateServerCertificate);
			ServicePointManager.Expect100Continue = true;
			CertHashDictTrust.Add ("A230A940127D5253AA85A122C583D93586694DDC", "");
		}

		public static bool ValidateServerCertificate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{
			//Debug.Log (certificate.GetPublicKeyString ());
			//Debug.Log (certificate.GetCertHashString());

			string trust = null;
			if (CertHashDictTrust.TryGetValue (certificate.GetCertHashString (), out trust)) {
				return true;
			}

			return false;
		}

		public void Request (string url, string contentType, byte[] postData, bool gzip, Action<WebResponse, Stream> callback)
		{
			WebRequest request = WebRequest.Create (url);
			if (postData != null) {
				request.ContentType = contentType;
				request.GetRequestStream ().Write (postData, 0, postData.Length);
			}

			if (gzip) {
				request.Headers.Add (HttpRequestHeader.AcceptEncoding, "gzip,deflate"); 
			}

			request.BeginGetResponse ((result) => {
				WebResponse response = request.EndGetResponse (result);
				try {
					Stream stream = response.GetResponseStream ();
					string encoding = response.Headers.Get ("Content-Encoding");
					if (!string.IsNullOrEmpty (encoding) && encoding.ToLower ().Contains ("gzip")) {
						stream = new GZipStream (stream, CompressionMode.Decompress);
					}

					result = null;
					callback (response, stream);

				} catch (System.Exception e) { 
					Debug.LogError (e);
					
				} finally {
					if (result != null) {
						callback (response, null);
					}
				}

			}, request);
		}

		public const string JSON_CONTENT_TYPE = "application/json;charset=utf-8";

		public void RequestJson<T> (string url, object postData, bool gzip, Action<WebResponse, T> callback)
		{
			Request (url, JSON_CONTENT_TYPE, postData == null ? null : UTF8Encoding.UTF8.GetBytes (postData is string ? (string)postData : JsonUtility.ToJson (postData)), gzip, (response, stream) => {
				System.Type t = typeof(T);
				try {
					if (t == typeof(Stream)) {
						t = null;
						callback (response, (T)(object)stream);

					} else {
						string json = new StreamReader (stream, UTF8Encoding.UTF8).ReadToEnd ();
						if (t == typeof(string)) {
							t = null;
							callback (response, (T)(object)json);

						} else {
							T jsonObject = JsonUtility.FromJson<T> (json);
							t = null;
							callback (response, jsonObject);
						}
					}

				} catch (System.Exception e) { 
					Debug.LogError (e);

				} finally {
					if (t != null) {
						callback (response, default(T));
					}
				}
			});
		}
	}
}
