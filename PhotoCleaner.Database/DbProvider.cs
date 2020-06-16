using System;
using System.Configuration;

namespace PhotoCleaner.Database
{
    public class DBProvider
    {
        public static string GetDatabase()
        {
            DBType dbType;
            bool supported = Enum.TryParse(ConfigurationManager.AppSettings["databaseType"], out dbType);
            if (!supported)
                throw new ConfigurationErrorsException($"{ConfigurationManager.AppSettings["databaseType"]} is not supported databse");
                
            switch(dbType)
            {
                case DBType.SQL:
                    return "SQL";
                case DBType.Mongo:
                    return "Mongo";
                default:
                    return "SQL";
            }
        }
    }
}
