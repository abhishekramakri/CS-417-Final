namespace FewClicksDev.PackageExporter
{
    using FewClicksDev.Core;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    using static FewClicksDev.Core.EditorDrawer;

    public static class PackageExporterUserPreferences
    {
        private const string PREFS_PATH = "FewClicks Dev/Package Exporter";
        private const string LABEL = "Package Exporter";
        private const SettingsScope SETTINGS_SCOPE = SettingsScope.User;

        private static readonly string PREFS_PREFIX = $"{PlayerSettings.productName}.FewClicksDev.{LABEL}.";
        private static readonly string[] KEYWORDS = new string[] { "FewClicks Dev", LABEL, "Package", "Exporter" };

        public static readonly GUIContent DEFAULT_SETUP_CONTENT = new GUIContent("Default setup", "Flag specifying if Folders should be included in the assets list");

        private const float LABEL_WIDTH = 250f;

        private const string RESET_TO_DEFAULTS = "Reset to defaults";
        private const string SETUPS = "Setups";

        public static PackageExportSetup DefaultSetup = null;
        private static bool arePrefsLoaded = false;

        static PackageExporterUserPreferences()
        {
            LoadPreferences();
        }

        [SettingsProvider]
        public static SettingsProvider PreferencesSettingsProvider()
        {
            SettingsProvider provider = new SettingsProvider(PREFS_PATH, SETTINGS_SCOPE)
            {
                label = LABEL,
                guiHandler = (searchContext) =>
                {
                    OnGUI();
                },

                keywords = new HashSet<string>(KEYWORDS)
            };

            return provider;
        }

        public static void OnGUI()
        {
            using (new ScopeGroup(new IndentScope(), new LabelWidthScope(LABEL_WIDTH)))
            {
                if (arePrefsLoaded == false)
                {
                    LoadPreferences();
                }

                DrawHeader(SETUPS);
                DefaultSetup = EditorGUILayout.ObjectField(DEFAULT_SETUP_CONTENT, DefaultSetup, typeof(PackageExportSetup), true) as PackageExportSetup;
                NormalSpace();

                using (new HorizontalScope())
                {
                    FlexibleSpace();

                    if (DrawBoxButton(RESET_TO_DEFAULTS, FixedWidthAndHeight(EditorGUIUtility.currentViewWidth / 2f, DEFAULT_LINE_HEIGHT)))
                    {
                        ResetToDefaults();
                    }

                    FlexibleSpace();
                }

                if (GUI.changed == true)
                {
                    SavePreferences();
                }
            }
        }

        public static void SavePreferences()
        {
            string _presetGUID = DefaultSetup != null ? AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(DefaultSetup)) : string.Empty;
            EditorPrefs.SetString(PREFS_PREFIX + nameof(DefaultSetup), _presetGUID);
        }

        public static void LoadPreferences()
        {
            string _presetGUID = EditorPrefs.GetString(PREFS_PREFIX + nameof(DefaultSetup), string.Empty);

            if (_presetGUID.IsNullEmptyOrWhitespace() == false)
            {
                DefaultSetup = AssetDatabase.LoadAssetAtPath<PackageExportSetup>(AssetDatabase.GUIDToAssetPath(_presetGUID));
            }

            arePrefsLoaded = true;
        }

        public static void ResetToDefaults()
        {
            DefaultSetup = null;

            SavePreferences();
        }

        public static bool IsDefaultSetup(PackageExportSetup _setup)
        {
            if (_setup == null || DefaultSetup == null)
            {
                return false;
            }

            if (DefaultSetup == _setup)
            {
                return true;
            }

            return false;
        }
    }
}
