using System;
using System.IO;
using SkiaSharp;

namespace myTask
{
    public static class Constants
    {
        public const string DB_NAME = "mydb.db3";

        public static string DB_PATH =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DB_NAME);
        
        public static SKColor[] COLORS = new[]
        {
            SKColor.Parse("#0277BD"),
            SKColor.Parse("#00838F"),
            SKColor.Parse("#00695C"),
            SKColor.Parse("#689F38"),
            SKColor.Parse("#388E3C"),
            SKColor.Parse("#9E9D24"),
            SKColor.Parse("#F57C00"),
            SKColor.Parse("#FFA000"),
            SKColor.Parse("#FBC02D"),
            SKColor.Parse("#F9A825"),
            SKColor.Parse("#FFC400"),
            SKColor.Parse("#FF9100"),
            SKColor.Parse("#D84315"),
            SKColor.Parse("#455A64"),
            SKColor.Parse("#455A64"),
            SKColor.Parse("#4527A0"),
            SKColor.Parse("#1565C0"),
            SKColor.Parse("#283593"),
            SKColor.Parse("#6A1B9A"),
            SKColor.Parse("#AD1457"),
            SKColor.Parse("#C62828"),
        };
    }
}