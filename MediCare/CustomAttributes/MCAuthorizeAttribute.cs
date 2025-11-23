using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace MediCare.CustomAttributes
{
    public class MCAuthorizeAttribute : TypeFilterAttribute
    {
        public MCAuthorizeAttribute() : base(typeof(MCAuthorizeAttribute)) 
        {
            
        }
    }
}
