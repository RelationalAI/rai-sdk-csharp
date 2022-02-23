using System;
using System.ComponentModel;

namespace RAILib
{
    public enum UserStatus
    {
        Active,
        InActive,
        None,
    }

    static class UserStatuses 
    {
        public static string Value(this UserStatus status)
        {
            switch (status)
            {
                case UserStatus.Active:
                    return "ACTIVE";
                case UserStatus.InActive:
                    return "INACTIVE";
                default:
                    return "NONE"; 
            }
        }
    }
}