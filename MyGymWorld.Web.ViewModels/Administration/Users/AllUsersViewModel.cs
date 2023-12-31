﻿namespace MyGymWorld.Web.ViewModels.Administration.Users
{
    using System.Collections.Generic;

    public class AllUsersViewModel
    {
        public AllUsersViewModel()
        {
            this.Users = new List<UserViewModel>();
        }

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public List<UserViewModel> Users { get; set; }
    }
}
