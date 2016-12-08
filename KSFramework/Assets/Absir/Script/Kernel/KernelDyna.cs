using System;

namespace Absir
{
	public interface DateFormat
	{
		string format (DateTime dateTime);

		DateTime parse (string dateTime);
	}

	public class DateFormatDefault : DateFormat
	{
		public static readonly DateFormat Instance = new DateFormatDefault ();

		protected DateFormatDefault ()
		{
		}

		public string format (DateTime dateTime)
		{
			return dateTime.ToString ();
		}

		public DateTime parse (string dateTime)
		{
			return DateTime.Parse (dateTime);
		}
	}

	public class SimpleDateFormat : DateFormat
	{
		public static readonly System.Globalization.CultureInfo cultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture ("en-US");

		private string _format;

		public SimpleDateFormat (string format)
		{
			_format = format;
		}

		public string format (DateTime dateTime)
		{
			return dateTime.ToString (_format);
		}

		public DateTime parse (string dateTime)
		{
			return DateTime.ParseExact (dateTime, _format, cultureInfo);
		}
	}

	public abstract class KernelDyna
	{
		public static readonly byte? BYTE_ZERO = (byte)0;

		public static readonly short? SHORT_ZERO = (short)0;

		public static readonly int? INTEGER_ZERO = (int)0;

		public static readonly long? LONG_ZERO = (long)0;

		public static readonly float? FLOAT_ZERO = (float)0;

		public static readonly double? DOUBLE_ZERO = (double)0;

		public static readonly bool? BOOLEAN_ZERO = false;

		public static readonly char? CHARACTER_ZERO = (char)0;

		public static readonly char[] DATE_CHARS = "-: /".ToCharArray ();

		public static readonly DateTime DATE_ZERO = new DateTime ((long)0);

		public static readonly DateFormat DATE_FORMAT = new SimpleDateFormat ("yyyy-MM-dd HH:mm:ss");

		public static readonly DateFormat DATE_FORMAT_DAY = new SimpleDateFormat ("yyyy-MM-dd");

		public static readonly DateFormat DATE_FORMAT_TIME = new SimpleDateFormat ("HH:mm:ss");

		public static readonly DateFormat DATE_FORMAT_FULL = new SimpleDateFormat ("yyyyMMddHHmmss");

		public static readonly DateFormat[] DATE_FORMAT_ARRAY = new DateFormat[] {
			DATE_FORMAT,
			DATE_FORMAT_DAY,
			DATE_FORMAT_TIME,
			DATE_FORMAT_FULL,
			DateFormatDefault.Instance,
			//DateFormat.getDateTimeInstance (DateFormat.LONG, DateFormat.LONG),
			//DateFormat.getDateTimeInstance (DateFormat.MEDIUM, DateFormat.MEDIUM),
			new SimpleDateFormat ("EEE MMM d hh:mm:ss a z yyyy"),
			new SimpleDateFormat ("EEE MMM d HH:mm:ss z yyyy"),
			new SimpleDateFormat ("MM/dd/yy hh:mm:ss a"),
			new SimpleDateFormat ("MM/dd/yy")
		};

		public static object to (object obj, Type t)
		{
			if (obj == null) {
				return nullTo (t);
			}

			try {
				return Convert.ChangeType (obj, t);

			} catch (Exception) {
			}
				
			if (obj is DateTime) {
				return dateTo ((DateTime)obj, t);

			} else if (obj is Enum) {
				return enumTo ((Enum)obj, t);

			} else if (obj is string) {
				return stringTo ((string)obj, t);
			}

			try {
				double num = (double)obj;
				return numberTo (num, t);

			} catch (Exception) {
			}

			return nullTo (t);
		}

		public static object nullTo (Type t)
		{
			if (t == typeof(byte)) {
				return BYTE_ZERO;

			} else if (t == typeof(short)) {
				return SHORT_ZERO;

			} else if (t == typeof(int)) {
				return INTEGER_ZERO;

			} else if (t == typeof(long)) {
				return LONG_ZERO;

			} else if (t == typeof(float)) {
				return FLOAT_ZERO;

			} else if (t == typeof(double)) {
				return DOUBLE_ZERO;

			} else if (t == typeof(bool)) {
				return BOOLEAN_ZERO;

			} else if (t == typeof(char)) {
				return CHARACTER_ZERO;
			}

			return null;
		}

		public static object numberTo (double? num, Type t)
		{
			try {
				return num;

			} catch (Exception) {
			}
				
			if (t == typeof(DateTime)) {
				return new DateTime ((long)num);

			} else if (t.IsAssignableFrom (typeof(string))) {
				return num.ToString ();

			} else if (t.IsEnum) {
				return toEnumNumber (num, t);
			}

			return null;
		}

		public static object dateTo (DateTime date, Type t)
		{
			if (t == typeof(bool?) || t == typeof(bool)) {
				return date.Ticks != 0;

			} else if (t.IsAssignableFrom (typeof(string))) {
				return ToString (date, t);
			}

			try {
				return Convert.ChangeType (date.Ticks, t);

			} catch (Exception) {
			}

			return null;
		}

		public static string ToString (DateTime date, Type t)
		{
			return ToString (date, 0);
		}

		public static string ToString (DateTime date, int type)
		{
			try {
				return DATE_FORMAT_ARRAY [type].format (date);

			} catch (Exception) {
				return null;
			}
		}

		public static object enumTo (object em, Type t)
		{
			if (t == typeof(bool?) || t == typeof(bool)) {
				return (((int)em) != 0);

			} else if (t.IsAssignableFrom (typeof(string))) {
				return em.ToString ();
			}

			try {
				return Convert.ChangeType (em, t);

			} catch (Exception) {
			}

			return null;
		}

		public static object stringTo (string str, Type t)
		{
			if (String.IsNullOrEmpty (str)) {
				return nullTo (t);
			}

			return stringTo (str, null);
		}

		public static object stringTo (string str, Type t, bool[] dynas)
		{
			if (t == typeof(byte)) {
				return toByte (str);

			} else if (t == typeof(byte?)) {
				return toByte (str, null);

			} else if (t == typeof(short)) {
				return toShort (str);

			} else if (t == typeof(short?)) {
				return toShort (str, null);

			} else if (t == typeof(int)) {
				return toInteger (str);

			} else if (t == typeof(int?)) {
				return toInteger (str, null);

			} else if (t == typeof(long)) {
				return toLong (str);

			} else if (t == typeof(long?)) {
				return toLong (str, null);

			} else if (t == typeof(float)) {
				return toFloat (str);

			} else if (t == typeof(float?)) {
				return toFloat (str, null);

			} else if (t == typeof(double)) {
				return toDouble (str);

			} else if (t == typeof(double?)) {
				return toDouble (str, null);

			} else if (t == typeof(bool)) {
				return toBoolean (str);

			} else if (t == typeof(bool?)) {
				return toBoolean (str, null);

			} else if (t == typeof(char)) {
				return toCharacter (str);

			} else if (t == typeof(char?)) {
				return toBoolean (str, null);

			} else if (t == typeof(DateTime)) {
				return toDate (str, DATE_ZERO);

			} else if (t.IsEnum) {
				return toEnum (str, t);
			}

			try {
				return Convert.ChangeType (str, t);

			} catch (Exception) {
			}

			if (dynas != null && dynas.Length > 0) {
				dynas [0] = !dynas [0];
			}

			return null;
		}

		public static byte? toByte (string str)
		{
			return toByte (str, BYTE_ZERO);
		}

		public static byte? toByte (string str, byte? defaultValue)
		{
			try {
				return Convert.ToByte (str);

			} catch (Exception) {
				try {
					return (byte)Convert.ToDouble (str);

				} catch (Exception) {
					return defaultValue;
				}
			}
		}

		public static short? toShort (string str)
		{
			return toShort (str, SHORT_ZERO);
		}

		public static short? toShort (string str, short? defaultValue)
		{
			try {
				return Convert.ToInt16 (str);

			} catch (Exception) {
				try {
					return (short)Convert.ToDouble (str);

				} catch (Exception) {
					return defaultValue;
				}
			}
		}

		public static int? toInteger (string str)
		{
			return toInteger (str, INTEGER_ZERO);
		}

		public static int? toInteger (string str, int? defaultValue)
		{
			try {
				return Convert.ToInt32 (str);

			} catch (Exception) {
				try {
					return (int)Convert.ToDouble (str);

				} catch (Exception) {
					return defaultValue;
				}
			}
		}

		public static long? toLong (string str)
		{
			return toLong (str, LONG_ZERO);
		}

		public static long? toLong (string str, long? defaultValue)
		{
			try {
				if (str.IndexOfAny (DATE_CHARS) > 0) {
					DateTime? date = toDate (str, null);
					return date == null ? 0 : ((DateTime)date).Ticks;
				}

				return Convert.ToInt64 (str);

			} catch (Exception) {
				try {
					return (long)Convert.ToDouble (str);

				} catch (Exception) {
					return defaultValue;
				}
			}
		}

		public static float? toFloat (string str)
		{
			return toFloat (str, FLOAT_ZERO);
		}

		public static float? toFloat (string str, float? defaultValue)
		{
			try {
				return (float)Convert.ToDouble (str);

			} catch (Exception) {
				return defaultValue;
			}
		}

		public static double? toDouble (string str)
		{
			return toDouble (str, DOUBLE_ZERO);
		}

		public static double? toDouble (string str, double? defaultValue)
		{
			try {
				return Convert.ToDouble (str);

			} catch (Exception) {
				return defaultValue;
			}
		}

		public static bool? toBoolean (string str)
		{
			return toBoolean (str, BOOLEAN_ZERO);
		}

		public static bool? toBoolean (string str, bool? defaultValue)
		{
			try {
				return Convert.ToDouble (str) != 0;

			} catch (Exception) {
				try {
					return Convert.ToBoolean (str);

				} catch (Exception) {
					return defaultValue;
				}
			}
		}

		public static char? toCharacter (string str)
		{
			return toCharacter (str, CHARACTER_ZERO);
		}

		public static char? toCharacter (string str, char? defaultValue)
		{
			try {
				return Convert.ToChar (str [0]);

			} catch (Exception) {
				try {
					return (char)Convert.ToDouble (str);

				} catch (Exception) {
					return defaultValue;
				}
			}
		}

		public static DateTime toDate (object obj)
		{
			return (DateTime)(object)to (obj, typeof(DateTime));
		}

		public static DateTime toDate (string str)
		{
			return (DateTime)toDate (str, DATE_ZERO);
		}

		public static DateTime? toDate (string str, DateTime? defaultValue)
		{
			foreach (DateFormat dateFormat in DATE_FORMAT_ARRAY) {
				try {
					return dateFormat.parse (str);

				} catch (Exception) {
				}
			}

			try {
				return new DateTime ((long)Convert.ToDouble (str));

			} catch (Exception) {
				return defaultValue;
			}
		}

		public static object toEnum (string str, Type enumType)
		{
			try {
				return Enum.Parse (enumType, str);

			} catch (Exception) {
				try {
					return toEnumNumber (Convert.ToDouble (str), enumType);

				} catch (Exception) {
					return null;
				}
			}
		}

		public static object toEnumNumber (double? num, Type enumType)
		{
			try {
				return Enum.ToObject (enumType, (int)num);

			} catch (Exception) {
				return null;
			}
		}
	}

}