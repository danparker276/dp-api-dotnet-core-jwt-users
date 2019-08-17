using System;
using System.Collections.Generic;
using System.Text;

namespace dp.business.Enums
{

    public enum DataProvider
    {
        Npgsql,
        AdoNet
    }

    public enum UserType
    {
        Anon =0,
        User = 1,
        Admin = 2,
    }
    public enum UserStatus
    {
        New = 0,
        Acitve = 1,
        Inactive = 2
    }


}
