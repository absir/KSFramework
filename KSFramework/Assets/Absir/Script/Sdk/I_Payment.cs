using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public class Payment
	{
		public string platform;
		public string orderId;
		public string id;
		public int number;
		public string data;
		public string moreInfo;
	}

	public interface I_Payment
	{
		void PayOrder (string id, string name, string desc, int originPrice, int currentPrice, int number, string data, string prepare);

		void PaySuccess (string orderId, string id, int currentPrice, int number, string data, int syncType);

		void PayFail (string reason);

		Payment decodePayment (string payment);

		IList<Payment> decodePayments (string payments);

		string encodePayments (IList<Payment> payments);

		IEnumerator validatePayment (Payment payment);
	}
}
