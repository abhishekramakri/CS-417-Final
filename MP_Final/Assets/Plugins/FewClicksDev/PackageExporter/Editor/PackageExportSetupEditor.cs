namespace FewClicksDev.PackageExporter
{
    using FewClicksDev.Core;
    using UnityEditor;
    using UnityEngine;

    using static FewClicksDev.Core.EditorDrawer;

    [CustomEditor(typeof(PackageExportSetup))]
    public class PackageExportSetupEditor : CustomInspectorBase
    {
        private const string OPEN_THE_WINDOW = "Open in the window";
        private const string EXPORT = "Export";
        private const string PICK_FOLDER_INFO = "Pick a folder to export the package to.";

        private PackageExportSetup setup = null;

        private SerializedProperty displayNameProperty = null;
        private SerializedProperty packageNameProperty = null;
        private SerializedProperty displayPriorityProperty = null;

        private SerializedProperty exportOptionsProperty = null;
        private SerializedProperty exportFolderProperty = null;

        private FileReferenceReorderableList fileReferencesList = null;

        protected override void OnEnable()
        {
            base.OnEnable();

            setup = target as PackageExportSetup;

            displayNameProperty = serializedObject.FindProperty("displayName");
            packageNameProperty = serializedObject.FindProperty("packageName");
            displayPriorityProperty = serializedObject.FindProperty("displayPriority");

            exportOptionsProperty = serializedObject.FindProperty("exportOptions");
            exportFolderProperty = serializedObject.FindProperty("exportFolder");

            fileReferencesList = ScriptableObject.CreateInstance<FileReferenceReorderableList>();
            fileReferencesList.Init(setup, setup.PackageContent, "Files and Folders to include", true, true);
        }

        protected override void drawInspectorGUI()
        {
            drawScript();

            using (new LabelWidthScope(180f))
            {
                EditorGUILayout.PropertyField(displayNameProperty);
                EditorGUILayout.PropertyField(packageNameProperty);
                EditorGUILayout.PropertyField(displayPriorityProperty);

                EditorGUILayout.PropertyField(exportOptionsProperty);
                DrawFolderPicker(exportFolderProperty, exportFolderProperty.displayName, PICK_FOLDER_INFO, false);

                NormalSpace();
                fileReferencesList.Draw();
                NormalSpace();
            }

            float _buttonWidth = inspectorWidth * 0.7f;

            using (new HorizontalScope())
            {
                FlexibleSpace();

                if (DrawBoxButton(OPEN_THE_WINDOW, FixedWidthAndHeight(_buttonWidth, DEFAULT_LINE_HEIGHT)))
                {
                    PackageExporterWindow.ShowWindow(setup);
                }

                FlexibleSpace();
            }

            if (setup.CanBeExported)
            {
                SmallSpace();

                using (new HorizontalScope())
                {
                    FlexibleSpace();

                    if (DrawClearBoxButton(EXPORT, PackageExporter.MAIN_COLOR, FixedWidthAndHeight(_buttonWidth, DEFAULT_LINE_HEIGHT)))
                    {
                        setup.Export();
                    }

                    FlexibleSpace();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
