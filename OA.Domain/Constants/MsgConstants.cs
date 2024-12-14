namespace OA.Core.Constants
{
    public static class MsgConstants
    {
        public static class SuccessMessages
        {
            public const string CreateUserSuccess = "User created successfully! ";
            public const string CreateSuccess = "{0} created successfully! ";
            public const string AssignRoleSuccess = "Assign role for {0} success!";
            public const string AssignDefaultRoleSuccess = "Assign default role success! ";
            public const string SendEmailSuccess = "Send email success! ";
            public const string SendTextMsgSuccess = "Send text message success! ";
            public const string UpdateUserSuccess = "User updated successfully! ";
            public const string ChangeStatusSuccess = "Change status {0} success! ";
            public const string CheckValidUserNameSuccess = "{0} is valid! ";
            public const string CheckValidRoleNameSuccess = "{0} is valid! ";
            public const string FieldIsValid = "This {0} is valid! ";
            public const string UpdateSuccess = "{0} updated successfully! ";
            public const string RemoveSuccess = "{0} removed successfully! ";
            public const string LoginSuccess = "Login successful!";
            public const string ChangePasswordSuccess = "Password changed successfully! ";
        }
        public static class WarningMessages
        {
            public const string NotFoundData = "Data is not found!";
            public const string NotFound = "{0} not found!";
        }
        public static class ErrorMessages
        {
            public const string ErrorGetAll = "Get all {0} errors!";
            public const string ErrorGetById = "Get {0} by id errors!";
            public const string ErrorCreate = "Create {0} errors!";
            public const string ErrorUpdate = "Update {0} error!";
            public const string ErrorRemove = "Remove {0} error!";
            public const string ErrorExport = "Failed to export data!";
            public const string ErrorChangeStatus = "Change status {0} errors!";
            public const string ErrorResetPassword = "Reset password errors!";
            public const string ErrorRequestResetPassword = "Request eset password errors!";
            public const string ErrorAssignRole = "Assign role for {0} errors!";
            public const string UserMustHaveRole = "User must have the {0} role to perform this action!";
            public const string TaskFailed = "Task has failed!";
            public const string AssignDefaultRoleFailed = "Assign default role failed! ";
            public const string SendEmailFailed = "Send email failed! ";
            public const string SendTextMsgFailed = "Send text message failed! ";
            public const string DataIsUsed = "Data is used in other tables, can not be modified (for updating or deleting)!";
            public const string ErrorBadRequest = "Cannot proceed data due to wrong request.";
        }
        public static class Error404Messages
        {
            public const string BadRequest = "{0} inputs incorrect!";
            public const string FieldIsInvalid = "{0} is invalid!";
            public const string FieldCanNotEmpty = "{0} can not empty!";
            public const string ErrorCheckValidRoleName = "{0} is invalid!";
            public const string InvalidUsernameOrPassword = "Username or password is invalid!";
            public const string InvalidPassword = "Password is invalid!";
            public const string InvalidUsername = "Username is invalid!";
            public const string InvalidEmail = "Email is invalid!";
            public const string ObjectIsDeleted = "Object {0} is deleted!";
        }
        public static class Existed
        {
            public const string ObjectIsExisted = "Object {0} is existed";
            public const string ObjectNotExisted = "Object {0} not existed";
            public const string FieldIsExisted = "Field {0} is existed";
            public const string FieldNotExisted = "Field {0} not existed";
        }
    }
}
