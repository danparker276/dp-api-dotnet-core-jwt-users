using dp.data.AdoNet.DataAccessObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace dp.data.Interfaces
{
    public interface IDaoFactory
    {
        UserDao UserDao { get; }
    }

}
