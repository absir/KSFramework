namespace Absir
{
	public delegate void Action ();

	public delegate void Action<T1> (T1 arg1);

	public delegate void Action<T1, T2> (T1 arg1, T2 arg2);

	public delegate void Action<T1, T2, T3> (T1 arg1, T2 arg2, T3 arg3);

	public delegate void Action<T1, T2, T3, T4> (T1 arg1, T2 arg2, T3 arg3, T4 arg4);

	public delegate TResult Func<TResult> ();

	public delegate TResult Func<TResult, T> (T arg);

	public delegate TResult Func<TResult, T1, T2> (T1 arg1, T2 arg2);

	public delegate TResult Func<TResult, T1, T2, T3> (T1 arg1, T2 arg2, T3 arg3);

	public delegate TResult Func<TResult, T1, T2, T3, T4> (T1 arg1, T2 arg2, T3 arg3, T4 arg4);

	public class ActionObj<T1, T2>
	{
		public T1 t1;

		public T2 t2;

		public static ActionObj<T1, T2> newObj (T1 t1, T2 t2)
		{
			ActionObj<T1, T2> obj = new ActionObj<T1, T2> ();
			obj.t1 = t1;
			obj.t2 = t2;
			return obj;
		}
	}

	public class ActionObj<T1, T2, T3>
	{

		public T1 t1;

		public T2 t2;

		public T3 t3;

		public static ActionObj<T1, T2, T3> newObj (T1 t1, T2 t2, T3 t3)
		{
			ActionObj<T1, T2, T3> obj = new ActionObj<T1, T2, T3> ();
			obj.t1 = t1;
			obj.t2 = t2;
			obj.t3 = t3;
			return obj;
		}
	}
}

