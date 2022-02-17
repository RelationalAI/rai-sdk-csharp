using System;

namespace RAILib
{
    public enum Role
    {
        User,
        Admin
    }

    static class Roles 
    {
        public static String Value(this Role role)
        {
            switch (role)
            {
                case Role.User:
                    return "user";
                case Role.Admin:
                    return "admin";
                default:
                    throw new SystemException(String.Format("role '{0}' not supported", role));     
            }
        }
    }
}