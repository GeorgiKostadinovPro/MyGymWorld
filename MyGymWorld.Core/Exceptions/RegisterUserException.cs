namespace MyGymWorld.Core.Exceptions
{
    using Microsoft.AspNetCore.Identity;
    using System;

    public class RegisterUserException : Exception
    {
        public RegisterUserException(IEnumerable<IdentityError> errors)
        {
            this.Errors = errors;
        } 
        
        public IEnumerable<IdentityError> Errors;
    }
}