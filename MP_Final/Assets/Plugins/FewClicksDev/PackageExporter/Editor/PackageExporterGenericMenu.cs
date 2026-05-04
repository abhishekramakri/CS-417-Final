namespace FewClicksDev.PackageExporter
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    public static class PackageExporterGenericMenu
    {
        private static readonly GUIContent SELECT_CONTENT = new GUIContent("Select", "Select the export setup in the project view.");
        private static readonly GUIContent DELETE_CONTENT = new GUIContent("Delete", "Delete the export setup.");
        private static readonly GUIContent MOVE_UP_CONTENT = new GUIContent("Move up", "Move this setup up in the list.");
        private static readonly GUIContent MOVE_DOWN_CONTENT = new GUIContent("Move down", "Move this setup down in the list.");
        private static readonly GUIContent SET_AS_DEFAULT_SETUP = new GUIContent("Set as default", "Set this setup as the one opened by default.");

        public static void ShowForSetup(PackageExporterWindow _window, Event _currentEvent, PackageExportSetup _preset, List<PackageExportSetup> _visibleSetups)
        {
            GenericMenu _menu = new GenericMenu();

            int _indexOfPreset = _visibleSetups.IndexOf(_preset);

            _menu.AddDisabledItem(new GUIContent(_preset.DisplayName));
            _menu.AddSeparator(string.Empty);
            _menu.AddItem(SELECT_CONTENT, false, _selectAndPing);
            _menu.AddItem(DELETE_CONTENT, false, _delete);

            if (_visibleSetups.Count > 1)
            {
                _menu.AddSeparator(string.Empty);

                if (_indexOfPreset != 0)
                {
                    _menu.AddItem(MOVE_UP_CONTENT, false, _moveUp);
                }

                if (_indexOfPreset != _visibleSetups.Count - 1)
                {
                    _menu.AddItem(MOVE_DOWN_CONTENT, false, _moveDown);
                }
            }

            if (_preset.IsDefault == false)
            {
                _menu.AddSeparator(string.Empty);
                _menu.AddItem(SET_AS_DEFAULT_SETUP, false, _setAsDefault);
            }

            _menu.ShowAsContext();

            _currentEvent.Use();

            void _selectAndPing()
            {
                Selection.activeObject = _preset;
                EditorGUIUtility.PingObject(_preset);
            }

            void _delete()
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_preset));
                _window.RefreshSetups();
            }

            void _moveUp()
            {
                int _currentPriority = _preset.DisplayPriority;
                int _previousSetupPriority = _visibleSetups[_indexOfPreset - 1].DisplayPriority;
                _preset.ChangeDisplayOrder(_previousSetupPriority);
                _visibleSetups[_indexOfPreset - 1].ChangeDisplayOrder(_currentPriority);

                _window.RefreshSetups();
            }

            void _moveDown()
            {
                int _currentPriority = _preset.DisplayPriority;
                int _nextSetupPriority = _visibleSetups[_indexOfPreset + 1].DisplayPriority;
                _preset.ChangeDisplayOrder(_nextSetupPriority);
                _visibleSetups[_indexOfPreset + 1].ChangeDisplayOrder(_currentPriority);

                _window.RefreshSetups();
            }

            void _setAsDefault()
            {
                _preset.SetAsDefault();
            }
        }
    }
}
