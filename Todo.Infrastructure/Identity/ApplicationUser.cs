﻿using Microsoft.AspNetCore.Identity;

namespace Todo.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser(string userName) :
        base(userName)
    {

    }
}
