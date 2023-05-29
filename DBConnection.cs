using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Forum
{
    public class DBConnection
    {
        private string connectionString;
        private NpgsqlConnection connection;

        public DBConnection()
        {
            InitializeConnection();
        }

        private void InitializeConnection()
        {
            connectionString = "Server=oregon-postgres.render.com;Port=5432;Database=numizmath;User Id=wizz;Password=ja87blcAks1Vdj5ynDtTrozmXSSn10WP;";
            connection = new NpgsqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public NpgsqlConnection GetConnection()
        {
            return connection;
        }
    }
}
