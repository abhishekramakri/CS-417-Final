namespace FewClicksDev.PackageExporter
{
    using FewClicksDev.Core;
    using UnityEngine;

    public enum FileMode
    {
        Object = 0,
        Folder = 1
    }

    public static class PackageExporter
    {
        public const string NAME = "Package Exporter";
        public const string CAPS_NAME = "PACKAGE EXPORTER";
        public const string VERSION = "1.1.0";

        public static readonly Color MAIN_COLOR = new Color(0.622642f, 0.167408f, 0.338121f, 1f);
        public static readonly Color LOGS_COLOR = new Color(0.705882f, 0.298039f, 0.449081f, 1f);

        public static void Log(string _message)
        {
            BaseLogger.Log(CAPS_NAME, _message, LOGS_COLOR);
        }

        public static void Warning(string _message)
        {
            BaseLogger.Warning(CAPS_NAME, _message, LOGS_COLOR);
        }

        public static void Error(string _message)
        {
            BaseLogger.Error(CAPS_NAME, _message, LOGS_COLOR);
        }
    }
}
