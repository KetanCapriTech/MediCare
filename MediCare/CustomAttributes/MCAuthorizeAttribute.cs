using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace MediCare.CustomAttributes
{
    public class MCAuthorizeAttribute : TypeFilterAttribute
    {
        public MCAuthorizeAttribute(int roleId = 0) : base(typeof(MCAuthorizeFilter)) 
        {
            Arguments = new object[] { roleId };
        }
    }
}
