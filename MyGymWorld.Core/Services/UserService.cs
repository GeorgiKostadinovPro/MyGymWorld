﻿namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Administration.Users;
    using MyGymWorld.Web.ViewModels.Users;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;

        private readonly IMapper mapper;
        private readonly IRepository repository;

        private readonly IAddressService addressService;
        private readonly ITownService townService;
        private readonly ICountryService countryService;

        public UserService(
            UserManager<ApplicationUser> _userManager, 
            IMapper _mapper,
            IRepository _repository,
            IAddressService _addressService,
            ITownService _townService,
            ICountryService _countryService)
        {
            this.userManager = _userManager;

            this.mapper = _mapper;
            this.repository = _repository;

            this.addressService = _addressService;
            this.townService = _townService;
            this.countryService = _countryService;
        }

        public async Task<(ApplicationUser, IdentityResult)> CreateUserAsync(CreateUserInputModel createUserInputModel)
        {
            ApplicationUser userToCreate = this.mapper.Map<ApplicationUser>(createUserInputModel);

            userToCreate.CreatedOn = DateTime.UtcNow;

            IdentityResult result = await this.userManager.CreateAsync(userToCreate, createUserInputModel.Password);

            return (userToCreate , result);
        }

        public async Task<(ApplicationUser, IdentityResult)> EditUserAsync(string userId, EditUserInputModel editUserInputModel)
        {
            ApplicationUser userToEdit = await this.GetUserByIdAsync(userId);

            if (userToEdit == null)
            {
                throw new InvalidOperationException(ExceptionConstants.UserErros.InvalidUserId);
            }

            userToEdit.UserName = editUserInputModel.UserName;
            userToEdit.Email = editUserInputModel.Email; 
            userToEdit.FirstName = editUserInputModel.FirstName;
            userToEdit.LastName = editUserInputModel.LastName;
            userToEdit.PhoneNumber = editUserInputModel.PhoneNumber;
            userToEdit.ModifiedOn = DateTime.UtcNow;

            if (editUserInputModel.Address == null)
            {
                if (userToEdit.AddressId != null)
                {
                    userToEdit.AddressId = null;
                }
            }
            else
            {
                Address address = await this.addressService.GetAddressByNameAsync(editUserInputModel.Address!);

                if (address != null)
                {
                    userToEdit.AddressId = address.Id;
                    userToEdit.Address = address;
                }
                else
                {
                    Address newAddress = new Address
                    {
                        Name = editUserInputModel.Address!,
                        TownId = Guid.Parse(editUserInputModel.TownId!),
                        CreatedOn = DateTime.UtcNow
                    };

                    await this.repository.AddAsync(newAddress);
                    await this.repository.SaveChangesAsync();

                    userToEdit.Address = newAddress;
                }
            }

            IdentityResult result = await this.userManager.UpdateAsync(userToEdit);
            
            return (userToEdit, result);
        }

        public async Task DeleteUserAsync(string userId)
        {
            ApplicationUser userToDelete = await this.repository.All<ApplicationUser>(u => u.IsDeleted == false)
                .Include(u => u.Manager)
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

            userToDelete.IsDeleted = true;
            userToDelete.DeletedOn = DateTime.UtcNow;

            if (userToDelete.ManagerId != null)
            {
                userToDelete.Manager.IsDeleted = true;
                userToDelete.Manager.DeletedOn = DateTime.UtcNow;
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task SetUserProfilePictureAsync(string userId, ImageUploadResult imageUploadResult)
        {
            ApplicationUser user = await this.GetUserByIdAsync(userId);

            string profilePictureUri = imageUploadResult!.SecureUri!.AbsoluteUri;

            user.ProfilePictureUri = profilePictureUri;
            user.ProfilePicturePublicId = imageUploadResult.PublicId;

            await this.userManager.UpdateAsync(user);
        }

        public async Task DeleteUserProfilePictureAsync(string userId)
        {
            ApplicationUser user = await this.GetUserByIdAsync(userId);

            user.ProfilePictureUri = null;
            user.ProfilePicturePublicId = null;

            await this.userManager.UpdateAsync(user);
        }

        public async Task<(string, string)> GetUserProfilePictureUriAndPublicIdAsync(string userId)
        {
            ApplicationUser user = await this.GetUserByIdAsync(userId);

            return (user.ProfilePictureUri, user.ProfilePicturePublicId);
        }

        public async Task<string> GenerateUserEmailConfirmationTokenAsync(ApplicationUser user)
        {
            string emailConfirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);

            byte[] emailConfirmationTokenBytes = Encoding.UTF8.GetBytes(emailConfirmationToken);

            string validEmailToken = WebEncoders.Base64UrlEncode(emailConfirmationTokenBytes);

            return validEmailToken;
        }

        public async Task<string> GenerateUserPasswordResetTokenAsync(ApplicationUser user)
        {
            string resetPasswordToken = await this.userManager.GeneratePasswordResetTokenAsync(user);

            byte[] resetPasswordTokenBytes = Encoding.UTF8.GetBytes(resetPasswordToken);

            string validResetPasswordToken = WebEncoders.Base64UrlEncode(resetPasswordTokenBytes);

            return validResetPasswordToken;
        }

        public async Task<bool> CheckIfUserHasConfirmedEmailAsync(string userId)
        {
            ApplicationUser user = await this.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException(ExceptionConstants.ConfimEmail.InvalidUserId);
            }

            return user.EmailConfirmed;
        }
        
        public async Task<bool> CheckUserPasswordAsync(ApplicationUser user, string password)
        {
            bool result = await this.userManager.CheckPasswordAsync(user, password);

            return result;
        }
        
        public async Task<bool> CheckIfUserExistsByEmailAsync(string email)
        {
            ApplicationUser user = await this.GetUserByEmailAsync(email);

            return user != null ? true : false;
        }
        
        public async Task<bool> CheckIfUserExistsByUsernameAsync(string username)
        {
            ApplicationUser user = await this.GetUserByUsernameAsync(username);

            return user != null ? true : false;
        }
        
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            ApplicationUser applicationUser = await this.userManager
               .FindByIdAsync(userId);

            return applicationUser;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            ApplicationUser applicationUser = await this.userManager
               .FindByEmailAsync(email);

            return applicationUser; 
        }

        public async Task<ApplicationUser> GetUserByUsernameAsync(string username)
        {
            ApplicationUser applicationUser = await this.userManager
               .FindByNameAsync(username);

            return applicationUser;
        }

        public async Task<ApplicationUser> GetAdministratorAsync()
        {
            foreach (var user in this.userManager.Users)
            {
                bool isAdmin = await this.userManager.IsInRoleAsync(user, "Administrator");

                if (isAdmin)
                {
                    return user;
                }
            }

            return null!;
        }

        public async Task<bool> CheckIfUserExistsByPhoneNumberAsync(string phoneNumber)
        {
            bool result = await this.repository.AllReadonly<ApplicationUser>()
                .AnyAsync(u => u.PhoneNumber == phoneNumber);
              
            return result;
        }

        public async Task<UserProfileViewModel> GetUserToDisplayByIdAsync(string userId)
        {
            ApplicationUser user = await this.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException(ExceptionConstants.UserErros.InvalidUserId);
            }

            string firstName = user.FirstName ?? "None";
            string lastName = user.LastName ?? "None";
            string profilePictureUrl = user.ProfilePictureUri ?? "None";
            string phoneNumber = user.PhoneNumber ?? "None";
            string address = "None";

            if (user.AddressId != null)
            {
                Address userAddress = await this.addressService.GetAddressByIdAsync(user.AddressId.Value);
                Town userTown = await this.townService.GetTownByIdAsync(userAddress.TownId);
                Country userCountry = await this.countryService.GetCountryByIdAsync(userTown.CountryId);

                address = string.Concat(userAddress.Name, $", {userTown.Name}, {userCountry.Name}");
            }

            UserProfileViewModel userProfileViewModel = new UserProfileViewModel()
            {
                Id = userId,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = firstName,
                LastName = lastName,
                ProfilePictureUri = profilePictureUrl,
                PhoneNumber = phoneNumber,
                Address = address,
                EventsCount = user.UsersEvents.Count,
                ArticlesAcount = user.UsersArticles.Count,
                LikesCount = user.Likes.Count,
                DislikesCount = user.Dislikes.Count,
                CommentsCount = user.Comments.Count
            };

            return userProfileViewModel;
        }

        public async Task<EditUserInputModel> GetUserForEditByIdAsync(string userId)
        {
            ApplicationUser user = await this.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException(ExceptionConstants.UserErros.InvalidUserId);
            }

            string firstName = user.FirstName ?? string.Empty;
            string lastName = user.LastName ?? string.Empty;
            string phoneNumber = user.PhoneNumber ?? string.Empty;
            string address = user.Address != null ? user.Address.Name ?? string.Empty : string.Empty;

            EditUserInputModel editUserInputModel = new EditUserInputModel()
            {
                Id = userId,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Address = address
            };

            return editUserInputModel;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await this.userManager.Users.ToArrayAsync();
        }

        public async Task<List<UserViewModel>> GetAllForAdministrationAsync()
        {
            List<UserViewModel> allUsersViewModel = new List<UserViewModel>();

            var users = await this.repository.AllReadonly<ApplicationUser>(u => u.IsDeleted == false)
                .Include(u => u.Manager)
                .ToArrayAsync();

            foreach (ApplicationUser user in users)
            {
                IList<string> userRoles = await this.userManager.GetRolesAsync(user);
                    
                string roleName = userRoles.FirstOrDefault()!;

                if (roleName != null && roleName == ApplicationRoleConstants.AdministratorRoleName)
                {
                    continue;
                }

                UserViewModel userViewModel = this.mapper.Map<UserViewModel>(user);

                if (roleName == null)
                {
                    userViewModel.Role = GlobalConstants.RegularUser;
                }
                else
                {
                    userViewModel.Role = roleName;
                }

                allUsersViewModel.Add(userViewModel);
            }

            return allUsersViewModel;
        }
    }
}