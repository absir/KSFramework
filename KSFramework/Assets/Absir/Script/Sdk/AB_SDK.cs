using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public interface I_SDK
	{
		bool IsInitAsync ();

		void AutoLogin ();
		
		void Login (string type, bool changeAccount);
		
		bool Logout ();
		
		string GetLoginUsername (string loginInfo);

		/*
		 * public static final int TYPE_SELECT_SERVER = 1; //选择服务器 public static
		 * final int TYPE_CREATE_ROLE = 2; //创建角色 public static final int
		 * TYPE_ENTER_GAME = 3; //进入游戏 public static final int TYPE_LEVEL_UP = 4;
		 * //等级提升 public static final int TYPE_EXIT_GAME = 5; //退出游戏 private int
		 * dataType; //信息类型，以上5种参数中选择 必填 private String roleID; //角色id 必填 private
		 * String roleName; //角色名称 必填 private String roleLevel; //角色等级 必填 private
		 * int serverID; //服务器id 必填 private String serverName; //服务器区服 必填 private
		 * int moneyNum; //玩家金钱数量 选填 private String vip; //玩家的VIP等级 必填 private long
		 * roleCTime; //游戏角色创建时间 ——毫秒值(10位数) 必填
		 */
		void SubmitPlayInfo (int dataType, long roleID, string roleName, int roleLevel, long serverID, string serverName, int moneyNum, int vip, long roleCTime);
		
		string PreparePay (string id, string name, string desc, int originPrice, int currentPrice, int number, string data);
		
		void PayOrderInfo (string orderId, string payInfo, string id, string name, string desc, int originPrice, int currentPrice, int number, string data);

	}

	public class AB_SDK : MonoBehaviour
	{
		public static AB_SDK ME {
			get {
				if (_ME == null) {
				    GameObject sdk = new GameObject {name = "AB_SDK"};
				    _ME = sdk.AddComponent<AB_SDK> ();
					GameObject.DontDestroyOnLoad(sdk);
				}				
				return _ME;
			}
		}

		private static AB_SDK _ME;
		private static bool inited;
		private static List<Action> initActons;
		private static I_SDK _sdk;
		private static I_Payment _payment;
		private static bool loginClicked;

		public static void Init (I_SDK sdk, I_Payment payment)
		{
			_sdk = sdk;
			_payment = payment;
			if (IsInitAsync ()) {
				// create AB_SDK gameobject
				AB_SDK.ME.AddInitAction (null);

			} else {
				AB_SDK.ME.Inited ();
			}
		}

		public static bool IsLoginClicked ()
		{
			return loginClicked;
		}

		void OnEnable ()
		{
			_ME = this;
		}
		
		void OnDisable ()
		{
			if (_ME == this) {
				_ME = null;
			}
		}

		public void Inited ()
		{
			inited = true;
			if (initActons != null) {
				foreach (var action in initActons) {
					try {
						action ();

					} catch (System.Exception e) {
						Debug.LogError ("AB_SDK init action error " + e);
					}
				}

				initActons = null;
			}
		}

		public void AddInitAction (Action action)
		{
			if (action == null) {
				return;
			}

			if (inited) {
				action ();

			} else {
				if (initActons == null) {
					initActons = new List<Action> ();
				}

				initActons.Add (action);
			}
		}

		public AB_Login login;

		public void LoginType (string type, bool changeAccount)
		{
			loginClicked = true;
			Login (type, changeAccount);
		}

		public void LoginUUID (string uuid)
		{
			login.LoginUUID (uuid, !loginClicked);
		}

		public void LoginAuthInfo (string authInfo)
		{
			login.LoginAuthInfo (authInfo, !loginClicked);
		}

		public void LoginSessionId (string sessionId)
		{
			login.LoginSessionId (sessionId, !loginClicked);
		}

		public void LoginFail (string reason)
		{
			login.LoginFail (reason, !loginClicked);
		}

		public void PayValidate (string payment)
		{ 
			AddPayment (_payment.decodePayment (payment));
		}

		//orderId,id,currentPrice,number,data,syncType
		public void PaySuccess (string data)
		{
			string[] datas = data.Split (',');
			_payment.PaySuccess (datas [0], datas [1], int.Parse (datas [2]), int.Parse (datas [3]), datas [4], int.Parse (datas [5]));
		}

		public void PayFail (string reason)
		{
			_payment.PayFail (reason);
		}

		private int roleId;
		private string paymentsKey;
		private IList<Payment> rolePayments;
		private bool paymentValidating;
		
		public void LoginRoleId (int roleId)
		{
			if (_payment == null) {
				throw new UnityException ("must set payment first");
			}
			
			this.roleId = roleId;
			paymentsKey = "AB_payments@" + roleId;
			string payments = PlayerPrefs.GetString (paymentsKey);
			if (!string.IsNullOrEmpty (payments)) {
				rolePayments = _payment.decodePayments (payments);
				AddPayment (null);
			}
		}

		public void AddPayment (Payment payment)
		{
			if (payment != null) {
				if (rolePayments == null) {
					rolePayments = new List<Payment> ();
					rolePayments.Add (payment);
				}

				string payments = _payment.encodePayments (rolePayments);
				PlayerPrefs.SetString (paymentsKey, payments);
			}

			if (!paymentValidating && rolePayments != null && rolePayments.Count > 0) {
				paymentValidating = true;
				StartCoroutine (_payment.validatePayment (rolePayments [0]));
			}
		}

		public void CompletePayment (Payment payment, bool remove)
		{
			if (remove && rolePayments != null && rolePayments.Count > 0) {
				if (rolePayments.Remove (payment)) {
					string payments = _payment.encodePayments (rolePayments);
					PlayerPrefs.SetString (paymentsKey, payments);
				}

				if (rolePayments.Count > 0) {
					StartCoroutine (_payment.validatePayment (rolePayments [0]));
					return;
				}
			}

			paymentValidating = false;
		}

		public static void PayOrder (string id, string name, string desc, int originPrice, int currentPrice, int number, string data)
		{
			string prepare = PreparePay (id, name, desc, originPrice, currentPrice, number, data);
			if (prepare != null) {
				_payment.PayOrder (id, name, desc, originPrice, currentPrice, number, data, prepare);
			} 
		}

#if( true || !(UNITY_ANDROID || UNITY_IOS)) || SDK_IN_UNITY
		public static bool IsInitAsync ()
		{
			return _sdk.IsInitAsync ();
		}

		public static void AutoLogin ()
		{
			_sdk.AutoLogin ();
		}
		
		protected static void Login (string type, bool changeAccount)
		{
			_sdk.Login (type, changeAccount);
		}
		
		public static bool Logout ()
		{
			return _sdk.Logout ();
		}
		
		public static string GetLoginUsername (string loginInfo)
		{
			return _sdk.GetLoginUsername (loginInfo);
		}
		
		public static void SubmitPlayInfo (int dataType, long roleID, string roleName, int roleLevel, long serverID, string serverName, int moneyNum, int vip, long roleCTime)
		{
			_sdk.SubmitPlayInfo (dataType, roleID, roleName, roleLevel, serverID, serverName, moneyNum, vip, roleCTime);
		}
		
		public static string PreparePay (string id, string name, string desc, int originPrice, int currentPrice, int number, string data)
		{
			return _sdk.PreparePay (id, name, desc, originPrice, currentPrice, number, data);
		}
		
		public static void PayOrderInfo (string orderId, string payInfo, string id, string name, string desc, int originPrice, int currentPrice, int number, string data)
		{
			_sdk.PayOrderInfo (orderId, payInfo, id, name, desc, originPrice, currentPrice, number, data);
		}

#elif UNITY_ANDROID

		private static bool _loaded;

		private static AndroidJavaObject _adapter;

		public static AndroidJavaObject Adapter
		{
			get
			{
				if (!_loaded)
				{
					_loaded = true;
					_adapter = new AndroidJavaObject("com.absir.sdk.UnityAdapter");
				}

				return _adapter;
			}
		}

		public static bool IsInitAsync ()
		{
			return Adapter.Call<bool>("isInitAsync");
		}

		public static void AutoLogin ()
		{
			Adapter.Call("autoLogin");
		}

		protected static void Login (string type, bool changeAccount)
		{
			Adapter.Call("login", type, changeAccount);
		}

		public static bool Logout ()
		{
			return Adapter.Call<bool>("logout");
		}

		public static string GetLoginUsername (string loginInfo)
		{
			return Adapter.Call<string>("getLoginUsername", loginInfo);
		}

		public static void SubmitPlayInfo(int dataType, long roleID, string roleName, int roleLevel, long serverID, string serverName, int moneyNum, int vip, long roleCTime)
		{
			Adapter.Call("submitPlayInfo", dataType, roleID, roleName, roleLevel, serverID, serverName, moneyNum, vip, roleCTime);
		}

		public static string PreparePay(string id, string name, string desc, int originPrice, int currentPrice, int number, string data)
		{
			return Adapter.Call<string>("preparePay", id, name, desc, originPrice, currentPrice, number, data);
		}

		public static void PayOrderInfo(string orderId, string payInfo, string id, string name, string desc, int originPrice, int currentPrice, int number, string data)
		{
			Adapter.Call("payOrderInfo", orderId, payInfo, id, name, desc, originPrice, currentPrice, number, data);
		}

#elif UNITY_IOS

		[System.Runtime.InteropServices.DllImport("__Internal")]
		public static extern bool IsInitAsync();

		[System.Runtime.InteropServices.DllImport("__Internal")]
		public static extern void AutoLogin();

		[System.Runtime.InteropServices.DllImport("__Internal")]
		protected static extern void Login(string type, bool changeAccount);

		[System.Runtime.InteropServices.DllImport("__Internal")]
		public static extern bool Logout();

		[System.Runtime.InteropServices.DllImport("__Internal")]
		public static extern string GetLoginUsername(string loginInfo);

		// public static final int TYPE_SELECT_SERVER = 1;     //选择服务器
		// public static final int TYPE_CREATE_ROLE = 2;       //创建角色
		// public static final int TYPE_ENTER_GAME = 3;        //进入游戏
		// public static final int TYPE_LEVEL_UP = 4;          //等级提升
		// public static final int TYPE_EXIT_GAME = 5;         //退出游戏
		// public static final int TYPE_EXIT_OTHER = 6;         //other change
		// private int dataType;       //信息类型，以上5种参数中选择  必填
		// private String roleID;      //角色ID     必填
		// private String roleName;    //角色名称   必填
		// private String roleLevel;   //角色等级   必填
		// private int serverID;       //服务器id   必填
		// private String serverName;  //服务器区服     必填
		// private int moneyNum;       //玩家金钱数量    选填
		// private String vip;    	//玩家的VIP等级  必填
		// private long roleCTime;  //游戏角色创建时间 ——毫秒值(10位数)   必填
		[System.Runtime.InteropServices.DllImport("__Internal")]
		public static extern void SubmitPlayInfo(int dataType, long roleID, string roleName, int roleLevel, long serverID, string serverName, int moneyNum, int vip, long roleCTime);

		[System.Runtime.InteropServices.DllImport("__Internal")]
		public static extern string PreparePay(string id, string name, string desc, int originPrice, int currentPrice, int number, string data);

		[System.Runtime.InteropServices.DllImport("__Internal")]
		public static extern void PayOrderInfo(string orderId, string payInfo, string id, string name, string desc, int originPrice, int currentPrice, int number, string data);

#endif

	}
}
