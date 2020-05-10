using Microsoft.AspNetCore.Mvc;
using System;

namespace Service.Models {
    public class ValidationException : Exception {
        public ExceptionType Type;
        public ValidationException(string message, ExceptionType type) : base(message) { Type = type; }
        public static ValidationException UserIdNotSpecified() => new ValidationException("User id not specified", ExceptionType.BadRequest);
        public static ValidationException UserNotFound() => new ValidationException("User not found", ExceptionType.NotFound);
        public static ObjectResult GetResponse(ExceptionType type, object input) {
            return type switch {
                ExceptionType.BadRequest => new BadRequestObjectResult(input),
                ExceptionType.NotFound => new NotFoundObjectResult(input),
                _ => throw new ArgumentException("Unknown exception type"),
            };
        }
    }
    public enum ExceptionType {
        BadRequest,
        NotFound
    }
}
