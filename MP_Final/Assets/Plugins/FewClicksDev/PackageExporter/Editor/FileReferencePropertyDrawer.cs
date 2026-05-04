namespace FewClicksDev.PackageExporter
{
    using FewClicksDev.Core;
    using UnityEditor;
    using UnityEngine;

    using static FewClicksDev.Core.EditorDrawer;

    [CustomPropertyDrawer(typeof(FileReference))]
    public class FileReferencePropertyDrawer : CustomPropertyDrawerBase
    {
        private const float ENUM_WIDTH = 80f;

        private const string FILE_MODE_PROPERTY = "fileMode";
        private const string FILE_OBJECT_PROPERTY = "fileObject";
        private const string FOLDER_PATH_PROPERTY = "folderPath";

        private const string SELECT_FOLDER_INFO = "Select a folder that should be included in the package";

        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            if (_property.serializedObject == null)
            {
                return;
            }

            EditorGUI.BeginProperty(_position, _label, _property);

            _property.serializedObject.Update();
            SerializedProperty _fileModeProperty = _property.FindPropertyRelative(FILE_MODE_PROPERTY);
            SerializedProperty _fileObjectProperty = _property.FindPropertyRelative(FILE_OBJECT_PROPERTY);
            SerializedProperty _folderPathProperty = _property.FindPropertyRelative(FOLDER_PATH_PROPERTY);

            FileMode _fileMode = (FileMode) _fileModeProperty.enumValueIndex;

            bool _changed = false;
            float _totalWidth = _position.width - ENUM_WIDTH;

            Rect _totalRect = new Rect(_position.x, _position.y, _position.width, singleLineHeight);
            Rect _fileModeRect = new Rect(_totalRect.x, _position.y, ENUM_WIDTH, singleLineHeight);
            Rect _fileRect = new Rect(_fileModeRect.x + ENUM_WIDTH + SMALL_SPACE, _position.y, _totalWidth - SMALL_SPACE, singleLineHeight);

            EditorGUI.PropertyField(_fileModeRect, _fileModeProperty, GUIContent.none);

            using (var _changeCheck = new ChangeCheckScope())
            {
                switch (_fileMode)
                {
                    case FileMode.Object:
                        EditorGUI.PropertyField(_fileRect, _fileObjectProperty, GUIContent.none);
                        break;

                    case FileMode.Folder:

                        Rect _pathRect = new Rect(_fileRect.x, _fileRect.y, _fileRect.width - 20f - SMALL_SPACE, _fileRect.height);
                        Rect _buttonRect = new Rect(_pathRect.x + _pathRect.width + SMALL_SPACE, _fileRect.y, 20f, _fileRect.height);

                        using (new DisabledScope())
                        {
                            EditorGUI.TextField(_pathRect, AssetsUtilities.ConvertAbsolutePathToDataPath(_folderPathProperty.stringValue));
                        }

                        if (GUI.Button(_buttonRect, DOTS))
                        {
                            string _newPath = EditorUtility.OpenFolderPanel(SELECT_FOLDER_INFO, Application.dataPath, string.Empty);

                            if (_newPath.StartsWith(Application.dataPath) == false)
                            {
                                BaseLogger.Warning(EDITOR_DRAWER, FOLDER_PATH_ERROR, RED);
                            }
                            else
                            {
                                _folderPathProperty.stringValue = _newPath;
                                _property.serializedObject.ApplyModifiedProperties();

                                GUIUtility.ExitGUI();
                            }
                        }

                        break;
                }

                _changed = _changeCheck.changed;
            }

            if (_changed)
            {
                _property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
        {
            return lineHeightWithSpacing;
        }
    }
}