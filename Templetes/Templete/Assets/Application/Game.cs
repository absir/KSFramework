using UnityEngine;
using System.Collections;
using Absir;

public class TestConf : BeanConf
{
	public readonly string name1 = "222";

	private int down2 = 3;

	public string GetName1() {
		return name1;
	}

	public int GetDown2() {
		return down2;
	}
}

public class TestConfRes : BeanConfRes
{
	public string name = "123";

	public int down = 1;
}


public class Game : AB_Init
{
	protected override void InitStart ()
	{
		Debug.Log ("Game InitStart");

		Debug.Log (ABKernel.Config("appname"));

		TestConf conf1 = new TestConf ();

		Debug.Log (conf1.GetName1());
		Debug.Log (conf1.GetDown2());

		conf1.InitConf ();

		TestConfRes conf2 = new TestConfRes ();
		conf2.InitConf ();

		Debug.Log (JsonUtility.ToJson(conf1));
		Debug.Log (conf1.GetName1());
		Debug.Log (conf1.GetDown2());
		Debug.Log (JsonUtility.ToJson(conf2));
	}
}
