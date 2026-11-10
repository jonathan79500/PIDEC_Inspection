using System;

namespace Inspection.Application.Exceptions;

// The thing the request asks for does not exist (inspector, document, owner, file ...).
// The API answers 404.
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

// The request is valid in shape but breaks a business rule
// (unknown contact, skill added twice, file too large, only one document of this type allowed ...).
// The API answers 400.
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}