using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGymWorld.Data.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid();
        }
    }
}
