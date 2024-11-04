using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Service.Interfaces
{
    public interface IAuthService
    {
        bool Authenticate(string username, string password, out string token);
        void RegisterUser(string username, string password);
    }
}
