using System.Collections.Immutable;
using static OA.Core.Constants.MsgConstants;

namespace OA.Domain.VModels
{
    public class ErrorMessage
    {
        public string? ErrorCode { get; set; }
        public string? Message { get; set; }
        public string? Detail { get; set; }
    }

    public class ErrorDictionary
    {
        private const string ERROR_PREFIX = "ERR";

        public static ImmutableDictionary<int, ErrorMessage> Values = ImmutableDictionary.CreateRange<int, ErrorMessage>(
        [
            new KeyValuePair<int, ErrorMessage>(401, new ErrorMessage
            {
                ErrorCode = $"{ERROR_PREFIX}_UNAUTHORIZED",
                Message = "Unauthorized",
            }),
            new KeyValuePair<int, ErrorMessage>(403, new ErrorMessage
            {
                ErrorCode = $"{ERROR_PREFIX}_FORBIDDEN",
                Message = "Forbidden",
            }),
            new KeyValuePair<int, ErrorMessage>(400, new ErrorMessage
            {
                ErrorCode = $"{ERROR_PREFIX}_BAD_REQUEST",
                Message = ErrorMessages.ErrorBadRequest,
            }),
            new KeyValuePair<int, ErrorMessage>(404, new ErrorMessage
            {
                ErrorCode = $"{ERROR_PREFIX}_NOT_FOUND",
                Message = WarningMessages.NotFoundData,
            }),
            new KeyValuePair<int, ErrorMessage>(409, new ErrorMessage
            {
                ErrorCode = $"{ERROR_PREFIX}_CANNOT_DELETE_DATA",
                Message = ErrorMessages.DataIsUsed,
            }),
        ]);
    }

    public class ErrorVModel
    {
        public IEnumerable<ErrorMessage>? Data { get; set; }
    }
}
