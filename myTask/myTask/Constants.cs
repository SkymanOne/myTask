using System;
using System.IO;

namespace myTask
{
    public static class Constants
    {
        public const string DB_NAME = "mydb.db3";

        public static string DB_PATH =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DB_NAME);
    }
}