using RealWord.Db.Entities;
using RealWord.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWord.Web.Helpers
{
    public interface IAuthentication
    {
        string Generate(User user);
        User LoginUser(UserLoginDto userLogin);
    }
}
