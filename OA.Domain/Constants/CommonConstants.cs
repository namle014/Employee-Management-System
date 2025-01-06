namespace OA.Core.Constants
{
    public static class CommonConstants
    {

        public struct Routes
        {
            public const string BaseRoute = "api/[controller]/[action]";
            public const string BaseRouteAdmin = "api/admin/[controller]/[action]";
            public const string BaseRouteUser = "api/user/[controller]/[action]";
            public const string SystemGroup = "APIs System";
            public const string AdminGroup = "APIs for WebAdmin";
            public const string CommonGroup = "APIs Common";
            public const string Profile = "Profile";
            public const string AddNewRole = "{role}";
            public const string DeleteRole = "{role}";
            public const string AssignRole = "{userEmail}/{role}";
            public const string Id = "{id}";
            public const string Url = "{url}";
        }

        public struct Authorize
        {
            //ROLES
            public const string Administrator = "Administrator";
            public const string Member = "Member";
            //POLICY
            public const string PolicyAdmin = "Policy Administrator";
            public const string PolicyMember = "Policy Member";
            //CUSTOM AUTHORIZATION
            public const string CustomAuthorization = "Custom Authorization";
        }

        public struct Status
        {
            public const bool InActive = false;
            public const bool Active = true;
            public const bool Default = true;
        }

        public struct AccountStatus
        {
            public const string MsgInActive = "Account is inactive!";
            public const string MsgActive = "Account is active!";
        }

        public struct LoggingEvents
        {
            public const int GenerateItems = 1000;
            public const int ListItems = 1001;
            public const int GetItem = 1002;
            public const int CreateItem = 1003;
            public const int UpdateItem = 1004;
            public const int DeleteItem = 1005;
            public const int GetItemNotFound = 4000;
            public const int UpdateItemNotFound = 4001;
        }

        public struct SpecialChar
        {
            public static char semicolon = ';';
            public static char underscore = '_';
        }

        public struct ConfigNumber
        {
            public const int limitRecord = 200;
            public const int pageSizeDefault = 50000;
        }

        public struct ConfigType
        {
            public static string typeEmail = "ACCOUNT_SEND_MAIL";
            public static string typeMailTemplate = "EMAIL_SIGNUP_SUCCESS";
            public static string sendMailAccount = "ASM_EMAIL";
            public static string sendMailPassword = "ASM_PASSWORD";
            public static string sendMailTemplate = "ASM_TEMPLATE";
            public static string sendMailHeader = "ASM_HEADER";
        }

        public struct SpecialFields
        {
            public static string id = "id";
            public static string email = "email";
            public static string type = "type";
        }

        public struct Excel
        {

            public static string formatName = "yyyyMMdd_HHmmss";
            public static string fileNameExtention = ".xlsx";
            public static string openxmlformats = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        }

        public struct Pdf
        {

            public static string formatName = "yyyyMMdd_HHmmss";
            public static string fileNameExtention = ".pdf";
            public static string format = "application/pdf";
        }

        public struct Validate
        {
            public static string idIsInvalid = "Id is invalid!";
            public static string keyIsInvalid = "Key is invalid!";
            public static string seoNameIsInvalid = "SeoName is invalid!";
            public static string inputInvalid = "Input is invalid!";
        }

        public struct FileType
        {
            public const string Zip = "Zip";
            public const string Rar = "Rar";
            public const string Audio = "Audio";
            public const string Video = "Video";
            public const string Document = "Document";
            public const string Image = "Image";
            public const string Other = "Other";
        }

        public struct ExportTypeConstant
        {
            public const string EXCEL = "EXCEL";
            public const string PDF = "PDF";
        }

        public struct Language
        {
            public const string Vietnamese = "vi";
            public const string English = "en";
        }

        public struct StoreProceduresName
        {
            public static string SearchAspNetRole = "SearchAspNetRole";
            public static string GetAllWorkShifts = "GetAllWorkShifts";
        }
        public struct TypeAllowFunction
        {
            public const string VIEW = "VIEW";
            public const string CREATE = "CREATE";
            public const string EDIT = "EDIT";
            public const string PRINT = "PRINT";
            public const string DELETE = "DELETE";
        }

        public struct HttpStatusCode
        {
            public const int OK = 200;
            public const int Unauthorized = 401;
            public const int NotFound = 404;
        }
    }
}
