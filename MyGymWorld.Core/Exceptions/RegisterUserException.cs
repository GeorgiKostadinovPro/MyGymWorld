namespace MyGymWorld.Core.Exceptions
{
    using Microsoft.AspNetCore.Identity;
    using System;

    public class RegisterUserException : Exception
    {
        public IEnumerable<IdentityError> Errors;

        public RegisterUserException(IEnumerable<IdentityError> errors)
        {
            this.Errors = errors;
        }
    }
}