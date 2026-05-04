namespace FewClicksDev.PackageExporter
{
    using FewClicksDev.Core;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    using static FewClicksDev.Core.EditorDrawer;

    public class PackageExporterWindow : CustomEditorWindow
    {
        public enum WindowMode
        {
            Export = 0,
            Setups = 1
        }


        public static GUIStyle SingleLineLabelStyle
        {
            get
            {
                if (singleLineLabelStyle == null)
                {
                    singleLineLabelStyle = Styles.CustomizedButton(DEFAULT_LINE_HEIGHT, TextAnchor.MiddleCenter, new RectOffset(0, 0, 0, 0));
                }

                return singleLineLabelStyle;
            }
        }

        public static GUIStyle SingleLineButtonStyle
        {
            get
            {
                if (singleLineButtonStyle == null)
                {
                    singleLineButtonStyle = Styles.CustomizedButton(DEFAULT_LINE_HEIGHT, TextAnchor.MiddleLeft, new RectOffset(5, 0, 0, 0));
                }

                return singleLineButtonStyle;
            }
        }

        public static GUIStyle DefaultPresetLabelStyle
        {
            get
            {
                if (defaultPresetLabelStyle == null)
                {
                    defaultPresetLabelStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.MiddleRight, padding = new RectOffset(0, 5, 0, 0) };
                }

                return defaultPresetLabelStyle;
            }
        }

        private static GUIStyle singleLineLabelStyle = null;
        private static GUIStyle singleLineButtonStyle = null;
        private static GUIStyle defaultPresetLabelStyle = null;

        private const float TOOLBAR_WIDTH = 0.75f;
        private const float INDEX_WIDTH = 30f;

        private const string ASSET = "asset";
        private const string CURRENT_SETUP = "Current setup";
        private const string EXPORT_FOLDER = "Export folder";
        private const string EXPORT_FOLDER_PICKER = "Pick an export folder for the package.";
        private const string REFRESH_SETUPS = "Refresh setups";
        private const string EXPORT = "Export";
        private const string EXPORT_ALL = "Export all";
        private const string EXPORT_SETUPS = "Export Setups";
        private const string NEW_SETUP_NAME = "New setup name";
        private const string CREATE_NEW_SETUP = "Create new setup";
        private const string CHOOSE_THE_FOLDER = "Choose the folder for a setup.";
        private const string DEFAULT = "(Default) ";
        private const string FILTER_BY_NAME = "Filter by name";

        private const string LIST_OF_EXPORT_SETUPS = "Here is a list of all Export Setups found in the project. It's sorted based on the 'Display priority' in the scriptable object. Click on the setup name to preview it.";
        private const string FOLDER_PATH_ERROR = "The folder path must be within the current project assets folder. Path changed to the Application.dataPath";
        private const string NO_SETUPS_ERROR = "Package Exporter didn't found any setups inside of your project. You can create them using controls below or using the create menu 'FewClicks Dev/Package Exporter/Export Setup'.";
        private const string EXPORT_INFO = "Use the button below to export current setup. Please be aware if you choose to inlcude the dependencies, package size may be bigger than you expect.";

        protected override string windowName => PackageExporter.NAME;
        protected override string version => PackageExporter.VERSION;
        protected override Vector2 minWindowSize => new Vector2(520f, 680f);
        protected override Color mainColor => PackageExporter.MAIN_COLOR;

        protected override bool askForReview => true;
        protected override string reviewURL => "https://assetstore.unity.com/packages/tools/utilities/package-exporter-310282#reviews";
        protected override bool hasDocumentation => true;
        protected override string documentationURL => "https://docs.google.com/document/d/1UYx63l94qBZ-DfboWMcWTruQJJgukmV893Y3EyazXoA/edit?usp=sharing";
        
        private WindowMode windowMode = WindowMode.Export;

        private PackageExportSetup currentSetup = null;
        private SerializedObject currentSetupSerializedObject = null;
        private SerializedProperty displayNameProperty = null;
        private SerializedProperty packageNameProperty = null;
        private SerializedProperty displayPriorityProperty = null;

        private SerializedProperty exportOptionsProperty = null;
        private SerializedProperty exportFolderProperty = null;

        private FileReferenceReorderableList fileReferencesList = null;

        private string newSetupName = "exportSetup_NewPackage";
        private string nameFilter = string.Empty;

        private PackageExportSetup[] exportSetups = null;
        private List<PackageExportSetup> sortedAndFilteredSetups = new List<PackageExportSetup>();

        private Vector2 exportScrollPosition = Vector2.zero;
        private Vector2 setupsScrollPosition = Vector2.zero;

        public void RefreshSetups()
        {
            exportSetups = AssetsUtilities.GetAssetsOfType<PackageExportSetup>();

            assignCurrentSetupIfNull();
            sortAndFilerSetups();
        }

        protected override void drawWindowGUI()
        {
            NormalSpace();
            windowMode = this.DrawEnumToolbar(windowMode, TOOLBAR_WIDTH, mainColor);

            SmallSpace();
            DrawLine();
            SmallSpace();

            switch (windowMode)
            {
                case WindowMode.Export:
                    drawExportTab();
                    break;

                case WindowMode.Setups:
                    drawSetupsTab();
                    break;
            }
        }

        private void drawExportTab()
        {
            if (exportSetups.IsNullOrEmpty())
            {
                drawNoSetups();
                return;
            }

            if (currentSetupSerializedObject == null)
            {
                changeCurrentSetup(currentSetup);
            }

            using (new LabelWidthScope(180f))
            {
                var _new = EditorGUILayout.ObjectField(CURRENT_SETUP, currentSetup, typeof(PackageExportSetup), false) as PackageExportSetup;

                if (_new != currentSetup)
                {
                    changeCurrentSetup(_new);
                }

                if (currentSetup == null)
                {
                    return;
                }

                SmallSpace();
                currentSetupSerializedObject.Update();
                EditorGUILayout.PropertyField(displayNameProperty);
                EditorGUILayout.PropertyField(packageNameProperty);
                EditorGUILayout.PropertyField(displayPriorityProperty);

                EditorGUILayout.PropertyField(exportOptionsProperty);
                DrawFolderPicker(exportFolderProperty, EXPORT_FOLDER, EXPORT_FOLDER_PICKER, false);

                if (currentSetup.CanBeExported)
                {
                    NormalSpace();
                    EditorGUILayout.HelpBox(EXPORT_INFO, MessageType.Info);
                    SmallSpace();

                    using (new HorizontalScope())
                    {
                        FlexibleSpace();

                        if (DrawClearBoxButton(EXPORT, mainColor, FixedWidthAndHeight(halfSizeButtonWidth, DEFAULT_LINE_HEIGHT)))
                        {
                            currentSetup.Export();
                        }

                        FlexibleSpace();
                    }

                    SmallSpace();
                }

                NormalSpace();

                using (var _scrollViewScope = new ScrollViewScope(exportScrollPosition))
                {
                    exportScrollPosition = _scrollViewScope.scrollPosition;
                    fileReferencesList.Draw();
                }

                currentSetupSerializedObject.ApplyModifiedProperties();
            }
        }

        private void drawSetupsTab()
        {
            if (exportSetups.IsNullOrEmpty())
            {
                drawNoSetups();
                return;
            }

            drawCreatePreset();
            SmallSpace();
            DrawLine();
            SmallSpace();
            EditorGUILayout.HelpBox(LIST_OF_EXPORT_SETUPS, MessageType.Info);
            SmallSpace();

            using (new HorizontalScope())
            {
                if (DrawBoxButton(REFRESH_SETUPS, FixedWidthAndHeight(halfSizeButtonWidth, DEFAULT_LINE_HEIGHT)))
                {
                    RefreshSetups();
                }

                FlexibleSpace();

                if (DrawBoxButton(EXPORT_ALL, FixedWidthAndHeight(halfSizeButtonWidth, DEFAULT_LINE_HEIGHT)))
                {
                    foreach (var _setup in sortedAndFilteredSetups)
                    {
                        if (_setup == null)
                        {
                            continue;
                        }

                        _setup.Export();
                    }
                }
            }

            NormalSpace();

            using (var _changeScope = new ChangeCheckScope())
            {
                nameFilter = EditorGUILayout.TextField(FILTER_BY_NAME, nameFilter);

                if (_changeScope.changed)
                {
                    sortAndFilerSetups();
                }
            }

            SmallSpace();

            using (var _scrollScope = new ScrollViewScope(setupsScrollPosition))
            {
                int _index = 0;

                foreach (var _setup in sortedAndFilteredSetups)
                {
                    if (_setup == null)
                    {
                        continue;
                    }

                    drawSingleSetup(_index, _setup);
                    _index++;
                }
            }
        }

        private void drawSingleSetup(int _index, PackageExportSetup _setup)
        {
            using (new HorizontalScope())
            {
                GUILayout.Label($"{_index + 1}", SingleLineLabelStyle, FixedWidth(INDEX_WIDTH));

                if (GUILayout.Button(_setup.DisplayName, SingleLineButtonStyle, FixedHeight(DEFAULT_LINE_HEIGHT)))
                {
                    changeCurrentSetup(_setup);
                    windowMode = WindowMode.Export;
                }

                if (_setup.IsDefault)
                {
                    Rect _rect = GUILayoutUtility.GetLastRect();
                    GUI.Label(_rect, DEFAULT, DefaultPresetLabelStyle);
                }

                if (GUILayout.Button(string.Empty, Styles.FixedSelect(DEFAULT_LINE_HEIGHT), FixedWidthAndHeight(DEFAULT_LINE_HEIGHT)))
                {
                    AssetsUtilities.Ping(_setup);
                }

                if (_setup.CanBeExported)
                {
                    if (GUILayout.Button(EXPORT, Styles.BoxButton, FixedWidthAndHeight(80f, DEFAULT_LINE_HEIGHT)))
                    {
                        _setup.Export();
                    }
                }

                if (GUILayout.Button(string.Empty, Styles.FixedSettings(DEFAULT_LINE_HEIGHT), FixedWidthAndHeight(DEFAULT_LINE_HEIGHT)))
                {
                    PackageExporterGenericMenu.ShowForSetup(this, Event.current, _setup, sortedAndFilteredSetups);
                }
            }
        }

        private void drawNoSetups()
        {
            SmallSpace();
            EditorGUILayout.HelpBox(NO_SETUPS_ERROR, MessageType.Info);
            NormalSpace();

            drawCreatePreset();
        }

        private void drawCreatePreset()
        {
            newSetupName = EditorGUILayout.TextField(NEW_SETUP_NAME, newSetupName);
            SmallSpace();

            using (new HorizontalScope())
            {
                FlexibleSpace();

                if (DrawClearBoxButton(CREATE_NEW_SETUP, PackageExporter.MAIN_COLOR, FixedWidthAndHeight(windowWidthScaled(0.6f), DEFAULT_LINE_HEIGHT)))
                {
                    var _path = EditorUtility.SaveFilePanel(CHOOSE_THE_FOLDER, Application.dataPath, newSetupName, ASSET);

                    if (_path.IsNullEmptyOrWhitespace())
                    {
                        return;
                    }

                    if (_path.StartsWith(Application.dataPath) == false)
                    {
                        PackageExporter.Error(FOLDER_PATH_ERROR);
                        return;
                    }

                    var _newSetup = ScriptableObject.CreateInstance<PackageExportSetup>();
                    _newSetup.name = newSetupName;

                    AssetDatabase.CreateAsset(_newSetup, AssetsUtilities.ConvertAbsolutePathToDataPath(_path));
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    AssetsUtilities.Ping(_newSetup);
                    RefreshSetups();
                }

                FlexibleSpace();
            }
        }

        private void assignCurrentSetupIfNull()
        {
            if (currentSetup == null && exportSetups.IsNullOrEmpty() == false)
            {
                if (PackageExporterUserPreferences.DefaultSetup != null)
                {
                    changeCurrentSetup(PackageExporterUserPreferences.DefaultSetup);
                }
                else
                {
                    changeCurrentSetup(exportSetups[0]);
                }
            }
        }

        private void sortAndFilerSetups()
        {
            sortedAndFilteredSetups.Clear();

            if (exportSetups.IsNullOrEmpty())
            {
                return;
            }

            foreach (var _setup in exportSetups)
            {
                if (string.IsNullOrEmpty(nameFilter) || _setup.DisplayName.ToLower().Contains(nameFilter.ToLower()))
                {
                    sortedAndFilteredSetups.Add(_setup);
                }
            }

            sortedAndFilteredSetups = sortedAndFilteredSetups.OrderBy(_setup => _setup.DisplayPriority).ToList();
        }

        private void changeCurrentSetup(PackageExportSetup _setup)
        {
            if (_setup == null)
            {
                return;
            }

            currentSetup = _setup;
            currentSetupSerializedObject = new SerializedObject(currentSetup);

            displayNameProperty = currentSetupSerializedObject.FindProperty("displayName");
            packageNameProperty = currentSetupSerializedObject.FindProperty("packageName");
            displayPriorityProperty = currentSetupSerializedObject.FindProperty("displayPriority");

            exportOptionsProperty = currentSetupSerializedObject.FindProperty("exportOptions");
            exportFolderProperty = currentSetupSerializedObject.FindProperty("exportFolder");

            if (fileReferencesList != null)
            {
                DestroyImmediate(fileReferencesList);
            }

            fileReferencesList = ScriptableObject.CreateInstance<FileReferenceReorderableList>();
            fileReferencesList.Init(currentSetup, currentSetup.PackageContent, "Files and Folders to include", true, true);
        }

        [MenuItem("Window/FewClicks Dev/Package Exporter", priority = 107)]
        public static void ShowWindow()
        {
            var _window = GetWindow<PackageExporterWindow>();
            _window.RefreshSetups();
            _window.Show();
        }

        public static void ShowWindow(PackageExportSetup _setup)
        {
            var _window = GetWindow<PackageExporterWindow>();
            _window.RefreshSetups();
            _window.changeCurrentSetup(_setup);
            _window.windowMode = WindowMode.Export;
            _window.Show();
        }
    }
}
