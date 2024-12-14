using System.Text.RegularExpressions;

namespace OA.Service.Helpers
{
    public static class ConvertBase64String
    {
        public static FileUploadResult ConvertBase64ToImage(string base64String, string uploadPath, string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(base64String))
                {
                    throw new ArgumentException("Base64 string is null or empty.");
                }

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
                var fullPath = Path.Combine(rootPath, yyyy, mm);

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                var fileName = $"{slug}_{Guid.NewGuid()}.{type}";
                var filePath = Path.Combine(fullPath, fileName);

                File.WriteAllBytes(filePath, imgData);

                return new FileUploadResult { FilePath = filePath, FileType = type };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while converting Base64 to image: {ex.Message}");
                throw new ApplicationException("Failed to convert Base64 to image.", ex);
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

    public class FileUploadResult
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
    }
}
