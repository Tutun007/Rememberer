using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Rememberer
{
    class MyDbConnector
    {
        private string connectionString;
        private MySqlConnection connection;
        
        public MyDbConnector(string Server, string Port, string Database, string User, string Password)
        {
            connectionString = "Server="+Server+";Port="+Port+"; database="+Database+"; UID="+User+"; password="+Password;
            connection = new MySqlConnection(connectionString);
        }       

        public void Open()
        {
            if(connection.State!=System.Data.ConnectionState.Open)
                connection.Open();            
        }

        public void Close()
        {
            connection.Close();
        }
        
        public List<Record> SelectRecords()
        {
            connection.Open();
            MySqlCommand query = new MySqlCommand("SELECT * FROM Record", connection);
            List<Record> records = new List<Record>();
            var result = query.ExecuteReader();
            while (result.Read())
            {
                records.Add(new Record((int)result["id"], (string)result["text"], Status.DB));
            }
            connection.Close();
            return records;
        }

        public void DeleteRecords(List<Record> records)
        {
            if (records.Count == 0)
                return;
            connection.Open();
            string queryString = "DELETE FROM Record WHERE";
            foreach(var a in records)
            {
                queryString += " id=" + a.Id+" OR";
            }
            MySqlCommand query = new MySqlCommand(queryString.Remove(queryString.Length-3), connection);
            var result = query.ExecuteNonQuery();
            connection.Close();
        }

        public void InsertRecords(List<Record> records)
        {
            if (records.Count == 0)
                return;
            connection.Open();
            string queryString = "INSERT INTO Record (Text) VALUES";
            for (int i=0;i<records.Count;i++)
            {
                queryString += " (@" + i + "),";
            }
            MySqlCommand query = new MySqlCommand(queryString.Remove(queryString.Length - 1), connection);
            for (int i = 0; i < records.Count; i++)
            {
                query.Parameters.AddWithValue("@"+i, records[i].Text);
            }
            var result = query.ExecuteNonQuery();
            connection.Close();
        }

        public void UpdateRecords(List<Record> records)
        {
            if (records.Count == 0)
                return;
            connection.Open();
            string queryString;
            MySqlCommand query;
            foreach (var a in records)
            {
                queryString =  "UPDATE Record SET Text=@0 WHERE id="+a.Id;
                query = new MySqlCommand(queryString, connection);
                query.Parameters.AddWithValue("@0", a.Text);
                query.ExecuteNonQuery();
            }
            connection.Close();
        }
    }
}
