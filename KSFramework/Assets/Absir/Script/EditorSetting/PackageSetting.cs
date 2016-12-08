using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Absir
{
	public enum VersionControlEnum : int
	{
		AUTO = 0,
		SVN,
		GIT,
	}

	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif
	public class PackageSetting : ScriptableObject
	{
		public const string AssetPath = "Assets";

		public const string ResourcesAssetPath = "Resources";

		public const string ResourcesAssetExtension = ".asset";

		private const string PackageSettingAssetName = "PackageSetting";

		private static PackageSetting _instance;

		public static PackageSetting Instance {
			get {
				if (_instance == null) {
					_instance = Resources.Load (PackageSettingAssetName) as PackageSetting;
					if (_instance == null) {
						_instance = ScriptableObject.CreateInstance<PackageSetting> ();
						#if UNITY_EDITOR
						string properPath = Path.Combine (Application.dataPath, ResourcesAssetPath);
						if (!Directory.Exists (properPath)) {
							AssetDatabase.CreateFolder (AssetPath, ResourcesAssetPath);
						}

						string fullPath = Path.Combine (Path.Combine (AssetPath, ResourcesAssetPath), PackageSettingAssetName + ResourcesAssetExtension);
						AssetDatabase.CreateAsset (_instance, fullPath);
						#endif
					}
				}

				return _instance;
			}
		}

		#if UNITY_EDITOR
		[MenuItem ("AB_Edtior/Package/Setting")]
		public static void Edit ()
		{
			Selection.activeObject = Instance;
		}
		#endif

		private bool dirty = false;

		public void DirtyEditor ()
		{
			#if UNITY_EDITOR
			if (dirty) {
				dirty = false;
				EditorUtility.SetDirty (this);
			}
			#endif
		}

		[SerializeField]
		public VersionControlEnum VersionControl;

		//[SerializeField]
		public string[] ChannelDirs;

		//[SerializeField]
		public string[] ChannelNames;

		[SerializeField]
		public string ChannelDir;

		private int _channelIndex = -2;

		public int ChannelIndex {
			get {
				if (_channelIndex < -1) {
					_channelIndex = -1;
					if (!string.IsNullOrEmpty (ChannelDir)) {
						int length = Mathf.Min (ChannelDirs == null ? 0 : ChannelDirs.Length, ChannelNames == null ? 0 : ChannelNames.Length);
						for (int i = 0; i < length; i++) {
							if (ChannelDirs [i] == ChannelDir) {
								_channelIndex = i;
								break;
							}
						}
					}
				}

				return _channelIndex;
			}
		}

		private string _channelName;

		public string ChannelName {
			get {
				if (_channelName == null) {
					int channelIndex = ChannelIndex;
					if (ChannelIndex >= 0) {
						_channelName = ChannelNames [channelIndex];
					}

					if (_channelName == null) {
						_channelName = "";
					}
				}

				return _channelName;
			}
		}

		protected void ClearCacheSelect ()
		{
			_channelIndex = -2;
			_channelName = null;
		}

		public void SetChannelDirsNames (string[] channelDirs, string[] channelNames)
		{
			//dirty = true;
			ChannelDirs = channelDirs;
			ChannelNames = channelNames;
			ClearCacheSelect ();
		}

		public void SetEditor (VersionControlEnum versionControl, int channelIndex)
		{
			if (VersionControl != versionControl) {
				dirty = true;
				VersionControl = versionControl;
			}

			if (_channelIndex != channelIndex) {
				dirty = true;
				ClearCacheSelect ();
				ChannelDir = ChannelDirs == null || channelIndex < 0 || channelIndex >= ChannelDirs.Length ? null : ChannelDirs [channelIndex];
			}
		}

		public void SetEditorDirty ()
		{
			dirty = true;
		}
	}
}
