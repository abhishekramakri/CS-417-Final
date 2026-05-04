namespace FewClicksDev.PackageExporter
{
    using FewClicksDev.Core;
    using UnityEditor;
    using UnityEngine;

    [System.Serializable]
    public class FileReference
    {
        private const string RELATIVE_PATH = "";

        [SerializeField] private FileMode fileMode = FileMode.Object;
        [SerializeField] private Object fileObject = null;
        [SerializeField] private string folderPath = string.Empty;

        public string Path
        {
            get
            {
                switch (fileMode)
                {
                    case FileMode.Object:

                        if (fileObject == null)
                        {
                            return string.Empty;
                        }

                        return AssetDatabase.GetAssetPath(fileObject);

                    case FileMode.Folder:
                        return AssetsUtilities.ConvertAbsolutePathToDataPath(folderPath);

                    default:
                        return string.Empty;
                }
            }
        }
    }
}
