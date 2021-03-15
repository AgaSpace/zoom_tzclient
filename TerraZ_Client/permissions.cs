using MySql.Data.MySqlClient;
using System;
using System.Data;
using TShockAPI.DB;

public enum Levels : int
{
    None, ClientUser
}

namespace TerraZ_Client
{
    public class DataBase
    {
        public IDbConnection database;
        public DataBase(IDbConnection db)
        {
            database = db;

            IQueryBuilder provider;
            if (database.GetSqlType() != SqlType.Sqlite)
            {
                IQueryBuilder queryBuilder = new MysqlQueryCreator();
                provider = queryBuilder;
            }
            else
            {
                IQueryBuilder queryBuilder = new SqliteQueryCreator();
                provider = queryBuilder;
            }

            SqlTable table = new SqlTable("TZClientPerms", new SqlColumn[] { new SqlColumn("GroupName", MySqlDbType.Text), new SqlColumn("Permission", MySqlDbType.Text) });
            new SqlTableCreator(database, provider).EnsureTableStructure(table);
        }

        public string GetPerms(string gn)
        {
            using (QueryResult result = database.QueryReader("SELECT * FROM TZClientPerms WHERE GroupName=@0", gn))
            {
                if (result.Read())
                {
                    return result.Get<string>("Permission");
                }    
            }
            return "";
        }
    }
}
