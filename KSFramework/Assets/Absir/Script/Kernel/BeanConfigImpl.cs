using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Absir
{
	public enum Environment
	{
		DEVELOP,

		DEBUG,

		TEST,

		PRODUCT
	}

	public static class EnvConfig
	{
		private static Environment environment = Environment.DEVELOP;

		private static bool active = true;

		private static bool started = true;

		public static Environment getEnvironment ()
		{
			return environment;
		}

		public static void setEnvironment (Environment environment)
		{
			EnvConfig.environment = environment;
		}

		public static bool isActive ()
		{
			return active && started;
		}

		public static void setActive (bool active)
		{
			EnvConfig.active = active;
		}

		public static bool isStarted ()
		{
			return started;
		}

		public static void setStarted (bool started)
		{
			EnvConfig.started = started;
		}

		public static void throwable (Exception e)
		{
			if (EnvConfig.getEnvironment () == Environment.DEVELOP) {
				Console.WriteLine (e.ToString ());
				Console.Write (e.StackTrace);
			}
		}

		public static bool isDevelop ()
		{
			return environment == Environment.DEVELOP;
		}
	}

	public class BreakException : Exception
	{
	}

	public interface CallbackBreak<T>
	{
		//throw BreakException
		void doWith (T template);
	}

	public delegate void CallbackTemplate<T> (T arg1);

	public static class BeanConfigIO
	{
		public static readonly char[] SEPARATOR_CHARS = "/\\".ToCharArray ();

		public static bool isStartSeparator (string filename)
		{
			if (filename.Length > 0) {
				char chr = filename [0];
				foreach (char ch in SEPARATOR_CHARS) {
					if (ch == chr) {
						return true;
					}
				}
			}

			return false;
		}

		public static string unTransferred (string value)
		{
			value = value.Trim ();
			int length = value.Length;
			if (length <= 1) {
				return value;
			}

			StringBuilder stringBuilder = new StringBuilder ();
			int quotation = 0;
			char chr = value [0];
			if (chr == '"') {
				quotation = 1;

			} else {
				stringBuilder.Append (chr);
			}

			length--;
			bool transferred = false;
			for (int i = 1; i < length; i++) {
				chr = value [i];
				if (transferred) {
					transferred = false;
					appendTransferred (stringBuilder, chr);

				} else if (chr == '\\') {
					transferred = true;

				} else {
					stringBuilder.Append (chr);
				}
			}

			chr = value [length];
			if (transferred) {
				appendTransferred (stringBuilder, chr);

			} else {
				// "" quotation
				if (quotation == 1 && chr == '"') {
					quotation = 2;

				} else {
					stringBuilder.Append (chr);
				}
			}

			return quotation == 1 ? '"' + stringBuilder.ToString () : stringBuilder.ToString ();
		}

		public static void appendTransferred (StringBuilder stringBuilder, char chr)
		{
			switch (chr) {
			case 't':
				stringBuilder.Append ("\t");
				break;
			case 'r':
				stringBuilder.Append ("\r");
				break;
			case 'n':
				stringBuilder.Append ("\n");
				break;
			case '"':
				stringBuilder.Append ('"');
				break;
			case '\'':
				stringBuilder.Append ('\'');
				break;

			default:
				stringBuilder.Append ('\\');
				stringBuilder.Append (chr);
				break;
			}
		}

		public static void doWithReadLine (System.IO.Stream input, CallbackBreak<string> callback)
		{
			doWithReadLine (input, Encoding.UTF8, callback);
		}

		public static void doWithReadLine (System.IO.Stream input, string encoding, CallbackBreak<string> callback)
		{
			doWithReadLine (input, Encoding.GetEncoding (encoding), callback);
		}

		public static void doWithReadLine (System.IO.Stream input, Encoding encoding, CallbackBreak<string> callback)
		{
			System.IO.StreamReader reader = null;
			try {
				reader = new System.IO.StreamReader (input, encoding);
				doWithReadLine (reader, callback);

			} finally {
				if (reader != null) {
					reader.Close ();
				}
			}
		}

		public static void doWithReadLine (System.IO.StreamReader reader, CallbackBreak<string> callback)
		{
			while (true) {
				try {
					string line = reader.ReadLine ();
					if (string.ReferenceEquals (line, null)) {
						break;
					}

					callback.doWith (line);

				} catch (BreakException) {
					break;
				}
			}
		}
	}

	public class BeanConfigImpl
	{
		public static T Get<T> (IDictionary<string, T> dictionary, string name)
		{
			T value = default(T);
			dictionary.TryGetValue (name, out value);
			return value;
		}

		private BeanConfigImpl beanConfig;
		private string classPath;
		private string resourcePath;
		protected internal bool outEnvironmentDenied = true;
		private Environment environment = Environment.DEVELOP;
		private IDictionary<string, object> configMap = new Dictionary<string, object> ();

		public  string getClassPath (string filename)
		{
			return getResourcePath (filename, classPath);
		}

		public  string getResourcePath (string filename)
		{
			return getResourcePath (filename, resourcePath);
		}

		public string getResourcePath (string filename, string nullPrefix)
		{
			filename = filename.Replace ("classpath:", classPath);
			filename = filename.Replace ("resourcePath:", resourcePath);
			if (nullPrefix != null && !BeanConfigIO.isStartSeparator (filename)) {
				filename = nullPrefix + filename;
			}

			return filename;
		}

		public virtual string ClassPath {
			get {
				return classPath;
			}
			set {
				this.classPath = value;
				configMap.Remove ("classPath");
				configMap ["classPath"] = value;
				//System.setProperty ("classPath", value);
			}
		}

		public virtual string ResourcePath {
			get {
				return resourcePath;
			}
			set {
				this.resourcePath = value;
				configMap.Remove ("resourcePath");
				configMap ["resourcePath"] = value;
				//System.setProperty ("resourcePath", value);
			}
		}

		public virtual Environment Environment {
			get {
				return environment;
			}
			set {
				this.environment = value;
				EnvConfig.setEnvironment (value);
			}
		}

		public virtual bool OutEnvironmentDenied {
			get {
				return outEnvironmentDenied;
			}
		}

		public virtual object getValue (string name)
		{
			return getValue (this, name);
		}

		public virtual void setValue (string name, object obj)
		{
			configMap.Remove (name);
			configMap [name] = obj;
		}

		public BeanConfigImpl (BeanConfigImpl BeanConfig) : this (BeanConfig, null)
		{
		}

		public BeanConfigImpl (BeanConfigImpl BeanConfig, string classPath)
		{
			this.beanConfig = BeanConfig;
			if (string.IsNullOrEmpty (classPath)) {
				classPath = System.Environment.CurrentDirectory;
			}

			ClassPath = classPath;
			ResourcePath = classPath;
			List<string> propertyFilenames = new List<string> ();
			HashSet<string> loadedPropertyFilenames = new HashSet<string> ();
			IDictionary<string, CallbackTemplate<string>> beanConfigTemplates = new Dictionary<string, CallbackTemplate<string>> ();
			loadBeanConfig (propertyFilenames, loadedPropertyFilenames, beanConfigTemplates);
			readProperties (resourcePath + "/config.properties", propertyFilenames, loadedPropertyFilenames, beanConfigTemplates);
			readProperties (resourcePath + "/" + environment.ToString ().ToLower () + ".properties", propertyFilenames, loadedPropertyFilenames, beanConfigTemplates);
			readProperties (resourcePath + "/properties", propertyFilenames, loadedPropertyFilenames, beanConfigTemplates);
			for (int i = 0; i < propertyFilenames.Count; i++) {
				readProperties (propertyFilenames [i], propertyFilenames, loadedPropertyFilenames, beanConfigTemplates);
			}
		}

		public void readProperties (string filename, List<string> propertyFilenames, HashSet<string> loadedPropertyFilenames, IDictionary<string, CallbackTemplate<string>> beanConfigTemplates)
		{
			if (!loadedPropertyFilenames.Contains (filename)) {
				loadedPropertyFilenames.Add (filename);
				if (System.IO.Directory.Exists (filename)) {
					readPropertiesDir (this, configMap, new System.IO.DirectoryInfo (filename), beanConfigTemplates);

				} else {
					readPropertiesFile (this, configMap, new System.IO.FileInfo (filename), beanConfigTemplates);
				}
			}
		}

		public static bool isConfigEnvironments (BeanConfigImpl beanConfig, string[] environments)
		{
			if (environments == null) {
				return true;
			}

			foreach (string environment in environments) {
				if (string.IsNullOrEmpty (environment)) {
					return true;
				}

				if (beanConfig == null || beanConfig.Environment.ToString ().Equals (environment)) {
					return true;
				}
			}

			return false;
		}

		public static void readProperties (BeanConfigImpl beanConfig, IDictionary<string, object> configMap, System.IO.Stream inputStream, IDictionary<string, CallbackTemplate<string>> beanConfigTemplates)
		{
			try {
				BeanConfigIO.doWithReadLine (inputStream, new CallbackBreakAnonymousInnerClass (beanConfig, configMap, beanConfigTemplates));

			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
				Console.Write (e.StackTrace);
			}
		}

		private class CallbackBreakAnonymousInnerClass : CallbackBreak<string>
		{
			private BeanConfigImpl beanConfig;
			private IDictionary<string, object> configMap;
			private IDictionary<string, CallbackTemplate<string>> beanConfigTemplates;

			public CallbackBreakAnonymousInnerClass (BeanConfigImpl beanConfig, IDictionary<string, object> configMap, IDictionary<string, CallbackTemplate<string>> beanConfigTemplates)
			{
				this.beanConfig = beanConfig;
				this.configMap = configMap;
				this.beanConfigTemplates = beanConfigTemplates;
			}

			private StringBuilder blockBuilder;

			private int blockAppending;

			public void doWith (string template)
			{
				int length = template.Length;
				if (length < 1) {
					return;
				}

				char chr = template [0];
				if (blockBuilder == null) {
					if (chr == '#') {
						return;

					} else if (chr == '{' && length == 2 && template [1] == '"') {
						blockBuilder = new StringBuilder ();
						blockAppending = 1;
						return;
					}

				} else if (blockAppending > 0) {
					if (chr == '"' && length == 2 && template [1] == '}') {
						blockAppending = 0;

					} else {
						if (blockAppending > 1) {
							blockBuilder.Append ("\r\n");

						} else {
							blockAppending = 2;
						}

						blockBuilder.Append (beanConfig == null ? template : beanConfig.getExpression (template));
					}

					return;
				}

				if (length < 3) {
					return;
				}

				int index = template.IndexOf ('=');
				if (index > 0 && index < length) {
					string name;
					chr = template [index - 1];
					if (chr == '.' || chr == '#' || chr == '+') {
						if (index < 1) {
							return;
						}

						name = template.Substring (0, index - 1);

					} else {
						chr = (char)0;
						name = template.Substring (0, index);
					}

					length = name.Length;
					if (length == 0) {
						return;
					}

					template = template.Substring (index + 1);
					if (beanConfig == null) {
						template = BeanConfigIO.unTransferred (template);
						if (blockBuilder != null) {
							if (template.Length > 0) {
								blockBuilder.Append ("\r\n");
								blockBuilder.Append (template);
							}

							template = blockBuilder.ToString ();
							blockBuilder = null;
							blockAppending = 0;
						}

						configMap.Remove (name);
						configMap [name] = template;

					} else {
						name = name.Trim ();
						if (length == 0) {
							return;
						}

						length = name.Length;
						template = template.Trim ();
						string[] environments = null;
						index = name.IndexOf ('|');
						if (index > 0) {
							if (length <= 1) {
								return;
							}

							string environmentParams = name.Substring (index + 1);
							name = name.Substring (0, index).Trim ();
							length = name.Length;
							if (length == 0) {
								return;
							}

							environments = environmentParams.Trim ().Split ('|');
						}

						if (isConfigEnvironments (beanConfig, environments)) {
							template = beanConfig.getExpression (BeanConfigIO.unTransferred (template));
							if (blockBuilder != null) {
								if (template.Length > 0) {
									if (template.Length > 0) {
										blockBuilder.Append ("\r\n");
										blockBuilder.Append (template);
									}
								}

								template = blockBuilder.ToString ();
								blockBuilder = null;
								blockAppending = 0;
							}
								
							CallbackTemplate<string> callbackTemplate = chr == 0 ? beanConfigTemplates == null ? null : Get<CallbackTemplate<string>> (beanConfigTemplates, name) : null;
							if (callbackTemplate == null) {
								if (beanConfig == null || beanConfig.OutEnvironmentDenied) {
									object value = template;
									if (chr != 0) {
										object old;
										switch (chr) {
										case '.':
											old = dynaTo<string> (Get<object> (configMap, name), null);
											if (old != null) {
												value = old + template;
											}

											break;
										case '#':
											old = dynaTo<string> (Get<object> (configMap, name), null);
											if (old != null) {
												value = old + "\r\n" + template;
											}

											break;
										case '+':
											old = dynaTo<IList> (Get<object> (configMap, name), null);
											if (old != null) {
												((IList)old).Add (template);
												value = old;
											}

											break;
										default:
											break;
										}
									}

									configMap.Remove (name);
									configMap [name] = value;
								}

							} else {
								callbackTemplate (template);
							}

						} else if (blockBuilder != null) {
							blockBuilder = null;
							blockAppending = 0;
						}
					}
				}
			}
		}

		public static void readPropertiesFile (BeanConfigImpl beanConfig, IDictionary<string, object> configMap, System.IO.FileInfo propertyFile, IDictionary<string, CallbackTemplate<string>> beanConfigTemplates)
		{
			if (propertyFile.Exists) {
				try {
					readProperties (beanConfig, configMap, System.IO.File.OpenRead (propertyFile.FullName), beanConfigTemplates);

				} catch (Exception e) {
					Console.WriteLine (e.ToString ());
					Console.Write (e.StackTrace);
				}
			}
		}

		public static void readPropertiesDir (BeanConfigImpl beanConfig, IDictionary<string, object> configMap, System.IO.DirectoryInfo propertyDir, IDictionary<string, CallbackTemplate<string>> beanConfigTemplates)
		{
			if (propertyDir.Exists) {
				foreach (System.IO.FileInfo file in propertyDir.GetFiles()) {
					if (file.Name.EndsWith (".properties")) {
						readPropertiesFile (beanConfig, configMap, file, beanConfigTemplates);
					}
				}
			}
		}

		public static void writeProperties (IDictionary<object, object> configMap, System.IO.FileInfo propertyFile)
		{
			StringBuilder stringBuilder = new StringBuilder ();
			foreach (var entry in configMap) {
				stringBuilder.Append (entry.Key);
				stringBuilder.Append ('=');
				stringBuilder.Append (entry.Value);
				stringBuilder.AppendLine ();
			}

			try {
				System.IO.File.WriteAllText (propertyFile.FullName, stringBuilder.ToString ());

			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
				Console.Write (e.StackTrace);
			}
		}

		public static object getValue (BeanConfigImpl beanConfig, string name)
		{
			object value = beanConfig == null ? null : beanConfig.getConfigValue (name);
			if (value == null) {
				if (name.Length > 1) {
					char chr = name [0];
					switch (chr) {
					case '%':
						value = System.Environment.GetEnvironmentVariable (name.Substring (1));
						break;
//					case '@':
//						value = System.getProperty (name.Substring (1));
//						break;
					case '$':
						value = beanConfig == null ? value : beanConfig.getConfigValue (name.Substring (1));
						break;
					default:
						break;
					}
				}
			}

			return value;
		}

		public static object getMapObject (IDictionary<string, object> map, string name, Type t)
		{
			object obj = Get<object> (map, name);
			if (obj != null) {
				object toObject = KernelDyna.to (obj, t);
				if (toObject != obj) {
					map.Remove (name);
					map [name] = toObject;
				}

				return toObject;
			}

			return null;
		}

		public static object dynaToType (object obj, string beanName, Type t)
		{
			if (t.IsAssignableFrom (typeof(IList))) {
				if (!(obj is IList)) {
					IList list = new List<object> ();
					list.Add (obj);
					return list;
				}
			}

			return KernelDyna.to (obj, t);
		}

		public static T getMapValue<T> (IDictionary<string, object> map, string name, string beanName = null)
		{
			return (T)getMapObject (map, beanName, typeof(T));
		}

		public static T dynaTo<T> (object obj, string beanName)
		{
			return (T)dynaToType (obj, beanName, typeof(T));
		}

		protected internal virtual void loadBeanConfig (List<string> propertyFilenames, HashSet<string> loadedPropertyFilenames, IDictionary<string, CallbackTemplate<string>> beanConfigTemplates)
		{
			beanConfigTemplates ["environment"] = (value) => {
				Environment env = dynaTo<Environment> (value, null);
				if (env != null) {
					Environment = env;
				}
			};

			beanConfigTemplates ["outEnvironment"] = (value) => {
				outEnvironmentDenied = true;
			};


			beanConfigTemplates ["inEnvironment"] = (value) => {
				Environment env = dynaTo<Environment> (value, null);
				if (env != null) {
					outEnvironmentDenied = env == environment;
				}
			};

			beanConfigTemplates ["inConfigure"] = (value) => {
				outEnvironmentDenied = dynaTo<bool> (value, null);
			};

			beanConfigTemplates ["resourcePath"] = (value) => {
				ResourcePath = value;
			};

			beanConfigTemplates ["properties"] = (value) => {
				foreach (string name in value.Split(',')) {
					string filename = name.Trim ();
					if (filename.Length > 0) {
						filename = getClassPath (filename);
						if (!loadedPropertyFilenames.Contains (filename)) {
							propertyFilenames.Add (filename);
						}
					}
				}
			};
		}

		public virtual object getConfigValue (string name)
		{
			object obj = Get<object> (configMap, name);
			if (obj == null && !configMap.ContainsKey (name)) {
				if (beanConfig != null) {
					obj = beanConfig.getConfigValue (name);
				}
			}

			return obj;
		}

		public virtual string getExpression (string expression)
		{
			return getExpression (expression, false);
		}

		public virtual string getExpression (string expression, bool strict)
		{
			int fromIndex = expression.IndexOf ("${", StringComparison.Ordinal);
			int length = expression.Length;
			if (fromIndex >= 0 && fromIndex < length - 2) {
				StringBuilder stringBuilder = new StringBuilder ();
				int endIndex = 0;
				while (true) {
					if (fromIndex > endIndex) {
						stringBuilder.Append (expression.Substring (endIndex, fromIndex - endIndex));

					} else if (fromIndex < endIndex) {
						if (fromIndex < 0) {
							if (length > endIndex) {
								stringBuilder.Append (expression.Substring (endIndex, length - endIndex));
							}
						}

						break;
					}

					if ((endIndex = expression.IndexOf ('}', fromIndex)) < 0) {
						stringBuilder.Append (expression.Substring (fromIndex));
						break;
					}

					fromIndex += 2;
					if (fromIndex < endIndex) {
						object value = getValue (expression.Substring (fromIndex, endIndex - fromIndex));
						if (value == null) {
							if (strict) {
								return null;
							}

						} else {
							stringBuilder.Append (value);
						}
					}

					fromIndex = expression.IndexOf ("${", endIndex++, StringComparison.Ordinal);
				}

				expression = stringBuilder.ToString ();
				expression.Replace ("$$", "$");
			}

			return expression;
		}

		public virtual T getExpressionObject<T> (string expression, string beanName)
		{
			string name = getExpression (expression);
			if (name == expression) {
				object obj = getMapValue<T> (configMap, name, beanName);
				if (obj == null && !configMap.ContainsKey (expression)) {
					if (beanConfig != null) {
						return beanConfig.getExpressionObject<T> (expression, beanName);
					}

					return (T)(object)null;
				}

				return dynaTo<T> (obj, beanName);
			}

			return dynaTo<T> (name, beanName);
		}

		public  T getExpressionDefaultValue<T> (string expression, string beanName)
		{
			T value = getExpressionObject<T> (expression, beanName);
			if (value == null) {
				value = dynaTo<T> (expression, beanName);
			}

			return value;
		}
	}

}