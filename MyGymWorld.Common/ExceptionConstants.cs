namespace MyGymWorld.Common
{
    using System.Runtime.InteropServices;

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
            public const string UserDoesNotExist = "User with this email does NOT exist!";
            public const string UserWasDeletedByAdmin = "You were banned by the site Admin!";
        }

        public static class ConfimEmail
        {
            public const string InvalidUserId = "User with this id does NOT exist!";
            public const string ConfirmEmailFailed = "Email was NOT confirmed!";
        }

        public static class ResetPassword
        {
            public const string InvalidEmailAddress = "User with this email does NOT exist!";
            public const string InvalidTokenOrPassword = "Your token or password is invalid!";
        }

        public static class UserErros
        {
            public const string InvalidUserId = "User with this id does NOT exist!";
        }

        public static class ProfilePictureErrors
        {
            public const string InvalidProfilePictureExtension = "The profile picture must be in formats jpg, jpeg, png!";
        }

        public static class ManagerErrors
        {
            public const string InvalidManagerId = "Manager with id {0} does NOT exist!";
            public const string InvalidManagerType = "The provided manager type is invalid!";
        }

        public static class ApplicationRoleErrors
        {
            public const string RoleAlreadyExists = "Role with this name already exists!";
            public const string InvalidRoleId = "Role with this Id does NOT exists!";
        }

        public static class GymErrors
        {
            public const string InvalidGymId = "Gym with such ID does NOT exists!";
            public const string InvalidGymType = "The provided Gym Type is invalid!";

            public const string GymAlreadyJoined = "You have already joined this gym!";
            public const string GymNotJoinedToBeLeft = "You have to join the gym in order to leave it!";
            public const string GymAlreadyLeft = "You have already left this gym!";
        }
    }
}