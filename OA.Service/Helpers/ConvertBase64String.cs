using System;
using System.IO;
using System.Text.RegularExpressions;

namespace OA.Service.Helpers
{
    public static class ConvertBase64String
    {
        public static Tuple<string, string> ConvertBase64ToImage(string base64String, string uploadPath, string name)
        {
            try
            {
                var slug = Slugify(name);
                var type = GetImageType(base64String);
                var dataPartIndex = base64String.IndexOf(',');
                if (dataPartIndex > 0)
                {
                    base64String = base64String.Substring(dataPartIndex + 1);
                }
                var imgData = Convert.FromBase64String(base64String);
                string rootPath = uploadPath;
                var yyyy = DateTime.Now.ToString("yyyy");
                var mm = DateTime.Now.ToString("MM");
                var fullPath = rootPath + "/" + yyyy + "/" + mm;
                bool exists = Directory.Exists(fullPath);
                if (!exists) Directory.CreateDirectory(fullPath);                
                var fileName = $"{slug}_{Guid.NewGuid()}.{type.ToUpper()}";
                var filePath = Path.Combine($"{fullPath}/{fileName}");
                File.WriteAllBytes(filePath, imgData);
                return Tuple.Create(filePath, type);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw; 
            }
        }

        private static string Slugify(string text)
        {
            text = text.ToLower();
            text = Regex.Replace(text, "[^a-z0-9\\-]", "");
            text = text.Replace(" ", "-");
            text = Regex.Replace(text, "-{2,}", "-"); 
            return text;
        }

        private static string GetImageType(string base64String)
        {
            var type = string.Empty;
            try
            {
                var header = base64String.Split(',')[0];
                if (!string.IsNullOrEmpty(header))
                {
                    if (header.StartsWith("data:image/png;base64"))
                    {
                        type = "png";
                    }
                    else if (header.StartsWith("data:image/jpeg;base64"))
                    {
                        type = "jpeg";
                    }
                    else if (header.StartsWith("data:image/jpg;base64"))
                    {
                        type = "jpg";
                    }
                    else if (header.StartsWith("data:image/bmp;base64"))
                    {
                        type = "bmp";
                    }
                    else if (header.StartsWith("data:image/tiff;base64"))
                    {
                        type = "tiff";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return type.ToLower();
        }
    }
}
