using Autofac;
using System;
using System.Threading.Tasks;

namespace CloudSpeed.Services
{
    public interface IPanPasswordHasher
    {
        Task<string> HashPassword(string password);
        
        Task<bool> VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}
