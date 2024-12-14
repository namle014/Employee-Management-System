namespace OA.Core.Constants
{
    public class Enums
    {
        public enum Common
        {
            /// <summary>
            /// Exception
            /// </summary>
            Exception = -1,
            /// <summary>
            /// Duplicate field value
            /// </summary>
            Duplicate = 2,
            /// <summary>
            /// Has Child
            /// </summary>
            HasChildError = 3,
        }
        public enum Create
        {
            /// <summary>
            /// Exception
            /// </summary>
            Exception = -1,
            /// <summary>
            /// Insert data and others is success
            /// </summary>
            SuccessAll = 1,
            /// <summary>
            /// Insert data is success but it sent email to confirm failed.
            /// </summary>
            InsertSuccessButSentEmailFailed = 2,
            /// <summary>
            /// Filed is conflicted (email, username, alias-name, alias-code,...)
            /// </summary>
            InsertFailedReasonFieldConflicted = 3,
            /// <summary>
            /// Foreign key is required
            /// </summary>
            InsertFailedReasonFKRequired = 4,
        }
    }
}
