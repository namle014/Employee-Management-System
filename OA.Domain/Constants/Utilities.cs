using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace OA.Core.Constants
{
    public static class Utilities
    {
        public static string MakeExceptionMessage(Exception ex)
        {
            return ex.InnerException == null ? ex.Message : ex.InnerException.Message;
        }

        public static long GetValueOfTotalRecords(List<dynamic> recordsTotal)
        {
            if (recordsTotal != null && recordsTotal.Count == 1)
            {
                return recordsTotal[0].TotalRecords;
            }
            else return 0;
        }

        public static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static T? ConvertModel<T>(object entity)
        {
            string origin = JsonConvert.SerializeObject(entity);
            return JsonConvert.DeserializeObject<T>(origin);
        }

        public static T? ConvertModel<T>(string entity)
        {
            return JsonConvert.DeserializeObject<T>(entity);
        }

        public struct SlugGenerator
        {
            public static string GenerateAliasFromName(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    return string.Empty;
                }
                string slug = name.ToLower();
                slug = slug.Replace("đ", "d");
                slug = RemoveAccents(slug);
                slug = Regex.Replace(slug, "[^a-zA-Z0-9- ]", "");
                slug = slug.Replace(" ", "-");
                slug = Regex.Replace(slug, @"-+", "-");
                slug = slug.Trim('-');
                return slug;
            }
            private static string RemoveAccents(string input)
            {
                string normalizedString = input.Normalize(NormalizationForm.FormKD);
                return new string(normalizedString
                    .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    .ToArray());
            }

        }
    }
}
