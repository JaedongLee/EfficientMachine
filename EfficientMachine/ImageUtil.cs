using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace EfficientMachine
{
    public class ImageUtil
    {
        private static readonly string AppPath = System.AppDomain.CurrentDomain.BaseDirectory;

        public static Uri GetOrDefaultImageUri(string toolName)
        {
            string imagePath = Path.Combine(AppPath, $"Resources/Tools/Icons/{toolName}.png");
            Uri imageUri = new Uri($"Resources/Tools/Icons/{toolName}.png", UriKind.Relative);
            if (!File.Exists(imagePath))
            {
                imageUri = new Uri("Resources/Tools/Icons/default.png", UriKind.Relative);
            }
            return imageUri;
        }
    }
}