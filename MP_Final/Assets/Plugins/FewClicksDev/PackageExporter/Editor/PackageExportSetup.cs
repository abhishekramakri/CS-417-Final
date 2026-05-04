namespace FewClicksDev.PackageExporter
{
    using FewClicksDev.Core;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(menuName = "FewClicks Dev/Package Exporter/Export Setup", fileName = "exportSetup_NewPackageExportSetup")]
    public class PackageExportSetup : ScriptableObject
    {
        private const string UNITY_PACKAGE = ".unitypackage";
        private const string NO_FILES_ERROR = "There were no files to export!";

        [Header("Main Settings")]
        [SerializeField] private string displayName = "New Package";
        [SerializeField] private string packageName = "package_100";
        [SerializeField] private int displayPriority = 0;

        [Header("Export")]
        [SerializeField] private ExportPackageOptions exportOptions = ExportPackageOptions.Interactive | ExportPackageOptions.Recurse;
        [SerializeField] private string exportFolder = string.Empty;

        [Space]
        [SerializeField] private List<FileReference> packageContent = new List<FileReference>();

        public string DisplayName => displayName;
        public int DisplayPriority => displayPriority;
        public bool CanBeExported => exportFolder.IsNullEmptyOrWhitespace() == false && packageContent.Count > 0;

        public List<FileReference> PackageContent => packageContent;

        public bool IsDefault => PackageExporterUserPreferences.IsDefaultSetup(this);

        public void Export()
        {
            List<string> _files = new List<string>();

            foreach (FileReference _file in packageContent)
            {
                string _path = _file.Path;

                if (_path.IsNullEmptyOrWhitespace())
                {
                    continue;
                }

                _files.Add(_path);
            }

            if (_files.IsNullOrEmpty())
            {
                PackageExporter.Error(NO_FILES_ERROR);
                return;
            }

            AssetDatabase.ExportPackage(_files.ToArray(), $"{exportFolder}/{packageName}{UNITY_PACKAGE}", exportOptions);
        }

        public void SetAsDefault()
        {
            PackageExporterUserPreferences.DefaultSetup = this;
            PackageExporterUserPreferences.SavePreferences();
        }

        public void ChangeDisplayOrder(int _newPriority)
        {
            displayPriority = _newPriority;
            AssetsUtilities.SetAsDirty(this);
        }
    }
}
