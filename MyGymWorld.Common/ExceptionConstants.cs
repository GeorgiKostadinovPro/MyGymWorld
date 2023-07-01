﻿namespace MyGymWorld.Common
{
    /// <summary>
    /// A static class that contains all custom exception messages.
    /// </summary>
    public static class ExceptionConstants
    {
        public static class RegisterUser
        {
            public const string EmailAlreadyExists = "User with this email already exists!";
            public const string UserDoesNotExist = "User with this email does NOT exist!";
        }

        public static class LoginUser
        {
            public const string InvalidLoginAttempt = "Invalid login attempt or unconfirmed email!";
        }

        public static class ConfimEmail
        {
            public const string InvalidUserId = "User with this id does NOT exist!";
            public const string ConfirmEmailFailed = "Email was NOT confirmed!";
        }
    }
}