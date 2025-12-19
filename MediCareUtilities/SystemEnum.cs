using System.ComponentModel;

namespace MediCareUtilities
{
    public enum EnumRole
    {
        [Description("Admin")]
        Admin = 1,

        [Description("Manager")]
        Manager = 2,

        [Description("Doctor")]
        Doctor = 3,

        [Description("Patient")]
        Patient = 4,

        [Description("Staff")]
        Staff =5,
    }
}
