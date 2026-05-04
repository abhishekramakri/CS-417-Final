namespace FewClicksDev.PackageExporter
{
    using FewClicksDev.Core.ReorderableList;
    using UnityEditor;

    public class FileReferenceReorderableList : ReorderableList<FileReference>
    {
        protected override SerializedObject getSerializedObject()
        {
            return new SerializedObject(this);
        }
    }
}
