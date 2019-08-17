using dp.business.Enums;
using dp.data.Interfaces;

namespace dp.data
{
    public class DaoFactories
    {
        public static IDaoFactory GetFactory(DataProvider dataProvider, string dpDBConnectionString)
        {
            switch (dataProvider)
            {
                case DataProvider.AdoNet:
                    return new AdoNet.DaoFactory(dpDBConnectionString);

                default:
                    return new AdoNet.DaoFactory(dpDBConnectionString);
            }
        }
    }
}
