using UnityEngine;
using System.Collections;

namespace Absir
{
	public interface I_Login
	{
		void AutoLogin ();

		void AutoLoginSuccess (bool success);

		bool LoginChangeAccount (bool changeAccount);

		void LoginTypeStart();

		void LoginUUID (string uuid);

		void LoginAuthInfo (string authInfo);

		void LoginSessionId (string sessionId);

		void LoginUsername (string username);

		void Logout (bool sout);

		void LoginFail (string reason);

	}

	public class AB_Login : MonoBehaviour
	{
		private I_Login login;
		private string uuid;
		private string authInfo;
		private string sessionId;
		private bool changeAccount;
		private string c_uuid;
		private string c_authInfo;
		private string c_sessionId;

		void Start ()
		{
			login = ComponentUtils.GetComponentObject<I_Login> (gameObject);
			AB_SDK.ME.login = this;
			if (!AB_SDK.IsLoginClicked ()) {
				login.AutoLogin ();
				AB_SDK.ME.AddInitAction (AB_SDK.AutoLogin);
			}
		}
		
		void OnDisable ()
		{
			if (AB_SDK.ME.login == this) {
				AB_SDK.ME.login = null;
			}
		}

		public void LoginClick (string type)
		{
			if (string.IsNullOrEmpty (type)) {
				if (!string.IsNullOrEmpty (sessionId)) {
					login.LoginSessionId (sessionId);
					return;
				}

				if (!string.IsNullOrEmpty (authInfo)) {
					login.LoginAuthInfo (authInfo);
					return;
				}

				if (!string.IsNullOrEmpty (uuid)) {
					login.LoginUUID (uuid);
					return;
				}

				if (login.LoginChangeAccount (changeAccount)) {
					return;
				}
			}

			login.LoginTypeStart ();
			AB_SDK.ME.LoginType (type, changeAccount);
		}

		public void LogoutClick ()
		{
			uuid = null;
			authInfo = null;
			sessionId = null;
			login.Logout (AB_SDK.Logout ());
		}

		public void ChangeAccountClick (string type)
		{
			changeAccount = true;
			c_uuid = uuid;
			c_authInfo = authInfo;
			c_sessionId = sessionId;
			uuid = null;
			authInfo = null;
			sessionId = null;
			LoginClick (type);
		}

		public void ChangeAccountCancel ()
		{
			changeAccount = false;
			uuid = c_uuid;
			authInfo = c_authInfo;
			sessionId = c_sessionId;
			c_uuid = null;
			c_authInfo = null;
			c_sessionId = null;
		}

		public void LoginUUID (string uuid, bool autoLogin)
		{
			authInfo = null;
			sessionId = null;
			this.uuid = uuid;
			if (autoLogin) {
				login.AutoLoginSuccess (true);

			} else {
				login.LoginUUID (uuid);
			}
		}

		public void LoginAuthInfo (string authInfo, bool autoLogin)
		{
			sessionId = null;
			this.authInfo = authInfo;
			if (autoLogin) {
				login.AutoLoginSuccess (true);
				
			} else {
				login.LoginAuthInfo (authInfo);
			}
		}
		
		public void LoginSessionId (string sessionId, bool autoLogin)
		{
			this.sessionId = sessionId;
			if (autoLogin) {
				login.AutoLoginSuccess (true);
					
			} else {
				login.LoginSessionId (sessionId);
			}
		}
		
		public void LoginInfoSessionId (string loginInfo, string sessionId)
		{
			if (!string.IsNullOrEmpty (loginInfo)) {
				string username = AB_SDK.GetLoginUsername (loginInfo);
				if (!string.IsNullOrEmpty (username)) {
					login.LoginUsername (username);
				}
			}
			
			LoginSessionId (sessionId, false);
		}
		
		public void LoginFail (string reason, bool autoLogin)
		{
			uuid = null;
			authInfo = null;
			sessionId = null;
			if (autoLogin) {
				login.AutoLoginSuccess (false);
				
			} else {
				login.LoginFail (reason);
			}
		}

	}
}
