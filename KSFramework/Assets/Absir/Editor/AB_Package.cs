using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Absir
{
	[CustomEditor (typeof(PackageSetting))]
	public class AB_Package : Editor
	{
		protected static string ChannelsPath;

		void OnEnable ()
		{
			ChannelsPath = Path.Combine (Application.dataPath, "../Channels");
		}

		public override void OnInspectorGUI ()
		{
			PackageSetting setting = PackageSetting.Instance;
			EditorGUILayout.Separator ();
			{
				VersionControlEnum versionControl = (VersionControlEnum)EditorGUILayout.EnumPopup ("VersionControl", setting.VersionControl);
				string[] channelNames = setting.ChannelNames;
				if (channelNames == null) {
					ScanChannels ();
				}

				EditorGUILayout.LabelField ("ChannelDir", setting.ChannelDir);
				int channelIndex = EditorGUILayout.Popup ("ChannelName", setting.ChannelIndex, channelNames);
				setting.SetEditor (versionControl, channelIndex);
			}

			EditorGUILayout.Separator ();
			EditorGUILayout.Space ();
			if (GUILayout.Button ("Scan Channels")) {
				ScanChannels ();
			}

			EditorGUILayout.Space ();
			if (GUILayout.Button ("Enter Channel")) {
				EnterChannel (setting.ChannelDir);
			}

			if (GUILayout.Button ("Stash(UnVersion Files) Channel")) {
				ExitStashChannel ();
			}

			EditorGUILayout.Separator ();

			setting.DirtyEditor ();
		}

		protected static IDictionary<string, object> GetChannelConfig (string channelDir)
		{
			IDictionary<string, object> configMap = new Dictionary<string, object> ();
			BeanConfigImpl.readPropertiesFile (null, configMap, new FileInfo (Path.Combine (ChannelsPath, channelDir + "/config.properties")), null);
			return configMap;
		}

		protected static string GetChannelName (string channelDir, IDictionary<string, object> configMap)
		{
			string channelName = BeanConfigImpl.getMapValue<string> (configMap, "channelName");
			return string.IsNullOrEmpty (channelName) ? channelDir : (channelName + "(" + channelDir + ")");
		}

		protected static void ScanChannels ()
		{
			Debug.Log ("ScanChannels Path = " + ChannelsPath);
			List<string> channelDirs = new List<string> ();
			List<string> channelNames = new List<string> ();
			foreach (string dirPath in Directory.GetDirectories (ChannelsPath)) {
				FileInfo configFile = new FileInfo (Path.Combine (dirPath, "config.properties"));
				if (configFile.Exists) {
					string channelDir = new DirectoryInfo (dirPath).Name;
					IDictionary<string, object> configMap = GetChannelConfig (channelDir);
					string channelName = GetChannelName (channelDir, configMap);
					channelDirs.Add (channelDir);
					channelNames.Add (channelName);
				}
			}
				
			PackageSetting.Instance.SetChannelDirsNames (channelDirs.ToArray (), channelNames.ToArray ());
		}

		public static bool IsIngoreFile (FileInfo fileInfo)
		{
			return fileInfo.Name [0] == '.';
		}

		public static bool IsIngoreFile (DirectoryInfo dirInfo)
		{
			return dirInfo.Name [0] == '.';
		}

		public static void ForeachFromDirectoryInfo (string parentDir, DirectoryInfo parentDirInfo, Action<string, FileInfo> relativeAction)
		{
			foreach (FileInfo fileInfo in parentDirInfo.GetFiles()) {
				if (!IsIngoreFile (fileInfo)) {
					relativeAction (parentDir + fileInfo.Name, fileInfo);
				}
			}

			foreach (DirectoryInfo dirInfo in parentDirInfo.GetDirectories()) {
				if (!IsIngoreFile (dirInfo)) {
					ForeachFromDirectoryInfo (parentDir + dirInfo.Name + "/", dirInfo, relativeAction);
				}
			}
		}

		public static void CreateDir (DirectoryInfo destDir)
		{
			if (!destDir.Exists) {
				CreateDir (destDir.Parent);
				destDir.Create ();
			}
		}

		public static void CopyFileAutoCreateDir (string sourceFileName, string destFileName, bool overwrite)
		{
			if (!overwrite && File.Exists (destFileName)) {
				return;
			}

			FileInfo destFile = new FileInfo (destFileName);
			CreateDir (destFile.Directory);
			File.Copy (sourceFileName, destFileName, overwrite);
		}

		protected static readonly char[] EqualChars = ":=".ToCharArray ();

		protected static void EnterChannel (string channelDir)
		{
			if (string.IsNullOrEmpty (channelDir)) {
				new UnityException ("EnterChannel channelDir could not be empty");
			}
				
			DirectoryInfo channelDirInfo = new DirectoryInfo (Path.Combine (ChannelsPath, channelDir));
			if (!channelDirInfo.Exists) {
				new UnityException ("EnterChannel channelDir[" + channelDir + "] not exists");
			}
				
			IDictionary<string, object> configMap = GetChannelConfig (channelDir);
			string channelName = GetChannelName (channelDir, configMap);
			Debug.Log ("EnterChannel = " + channelName);
			BeanConfigImpl channelsConfigImpl = new BeanConfigImpl (null, ChannelsPath);

			// foreach properties dir modify property
			DirectoryInfo propertiesDir = new DirectoryInfo (Path.Combine (channelDirInfo.FullName, "Properties"));
			if (propertiesDir.Exists) {
				ForeachFromDirectoryInfo ("", propertiesDir, (relative, propertiesFile) => {
					string projectPropertiesFile = Path.Combine (ChannelsPath, "../" + relative);
					if (File.Exists (projectPropertiesFile)) {
						IDictionary<string, object> propertiesMap = new Dictionary<string, object> ();
						BeanConfigImpl.readPropertiesFile (channelsConfigImpl, propertiesMap, propertiesFile, null);
						if (propertiesMap.Count > 0) {
							bool dirty = false;
							StringBuilder stringBuilder = new StringBuilder ();
							BeanConfigIO.doWithReadLine (File.OpenRead (projectPropertiesFile), new CallbackBreakAction<string> ((line) => {
								int pos = line.IndexOfAny (EqualChars);
								if (pos > 0) {
									string name = line.Substring (0, pos).Trim ();
									string value = BeanConfigImpl.getMapValue<string> (propertiesMap, name);
									if (value != null) {
										dirty = true;
										Debug.Log ("EnterChannel set properties[" + relative + "] " + name + " = " + value);
										line = line.Substring (0, pos + 1) + " " + value;
									}
								}

								stringBuilder.Append (line + "\n");
							}));

							if (dirty) {
								File.WriteAllText (projectPropertiesFile, stringBuilder.ToString ());
							}
						}
					
					} else {
						Debug.LogWarning ("EnterChannel could not found project setting file for " + relative);
					}
				});
			}
				
			FileInfo overSharedFile = new FileInfo (Path.Combine (channelDirInfo.FullName, "over_shared.properties"));
			if (overSharedFile.Exists) {
				IDictionary<string, object> overSharedMap = new Dictionary<string, object> ();
				BeanConfigImpl.readPropertiesFile (channelsConfigImpl, overSharedMap, overSharedFile, null);
				IList overs = BeanConfigImpl.getMapValue<IList> (overSharedMap, "over");
				if (overs != null) {
					Debug.Log ("EnterChannel over count = " + overs.Count);
					foreach (object obj in overs) {
						string over = obj.ToString ();
						string overPath = Path.Combine (ChannelsPath, "../Over_Shared/" + over);
						if (Directory.Exists (overPath)) {
							Debug.Log ("EnterChannel over shared dir => " + over);
							ForeachFromDirectoryInfo ("", new DirectoryInfo (overPath), (relative, overFile) => {
								string destOverPath = Path.Combine (ChannelsPath, "../" + over + "/" + relative);
								CopyFileAutoCreateDir (overFile.FullName, destOverPath, true);
							});
					
						} else if (File.Exists (overPath)) {
							string destOverPath = Path.Combine (ChannelsPath, "../" + over);
							Debug.Log ("EnterChannel over shared file => " + over);
							CopyFileAutoCreateDir (overPath, destOverPath, true);

						} else {
							Debug.LogWarning ("EnterChannel over shared not found => " + over);
						}
					}
				}
			}

			DirectoryInfo overDir = new DirectoryInfo (Path.Combine (channelDirInfo.FullName, "Over"));
			if (overDir.Exists) {
				Debug.Log ("EnterChannel Over dir => " + overDir);
				ForeachFromDirectoryInfo ("", overDir, (relative, overFile) => {
					string destOverPath = Path.Combine (ChannelsPath, "../" + relative);
					CopyFileAutoCreateDir (overFile.FullName, destOverPath, true);
				});
			}

			AssetDatabase.Refresh ();
		}

		protected static void ExitStashChannel ()
		{
			string channelDir = PackageSetting.Instance.ChannelDir;
			Debug.Log ("ExitStashChannel = " + channelDir);
			//git clean -dn
			//http://stackoverflow.com/questions/216049/how-do-i-get-a-list-of-all-unversioned-files-from-svn
		}

		protected VersionControlEnum GetAutoVersionControl ()
		{
			DirectoryInfo dir = new DirectoryInfo (Path.Combine (ChannelsPath, "../"));
			DirectoryInfo svnDir = new DirectoryInfo (Path.Combine (dir.FullName, ".svn"));
			if (svnDir.Exists) {
				return VersionControlEnum.SVN;
			}

			while (dir != null && dir.Exists) {
				DirectoryInfo gitDir = new DirectoryInfo (Path.Combine (dir.FullName, ".git"));
				if (gitDir.Exists) {
					return VersionControlEnum.GIT;
				}

				dir = dir.Parent;
			}
				
			return VersionControlEnum.AUTO;
		}



	}
}
