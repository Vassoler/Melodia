using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class DataBase
{
    private string database = "melodia_database.db";
    private string connectionString;

    public DataBase()
    {
        Debug.Log("Criação do banco.");
        if (Application.platform != RuntimePlatform.Android)
        {
            this.connectionString = Application.dataPath + "/StreamingAssets/"+database;

            if (!File.Exists(connectionString))
            {
                File.Create(connectionString);
            }
            connectionString = "URI=file:" + connectionString;
            string pathSQL = Application.streamingAssetsPath + "/melodia.sql";
            string sqlFile = System.IO.File.ReadAllText(@pathSQL);
            IniciarBase(sqlFile);
        }
        else
        {
            Debug.Log("Banco para Android");
            this.connectionString = Application.persistentDataPath + "/" + database;
            if (!File.Exists(connectionString))
            {
                Debug.Log("Criação banco para Android");
                WWW load = new WWW("jar:file://" + Application.dataPath + "!/assets/" + database);
                while (!load.isDone) { }

                File.WriteAllBytes(this.connectionString, load.bytes);
                
            }
            this.connectionString = "URI=file:" + connectionString;

            /*string path = "jar:file://" + Application.dataPath + "!/assets/melodia.sql";
            WWW wwwfile = new WWW(path);
            while (!wwwfile.isDone) { }
            var pathSQL = Application.persistentDataPath + "/melodia.sql"; ;
            File.WriteAllBytes(pathSQL, wwwfile.bytes);
            string sqlFile = File.ReadAllText(@pathSQL);
            IniciarBase(sqlFile);*/

            string oriPath = Application.streamingAssetsPath + "/melodia.sql";

            // Android only use WWW to read file
            WWW reader = new WWW(oriPath);
            while (!reader.isDone) { }

            string sqlFile = reader.text;
            IniciarBase(sqlFile);
        }
        
        
    }

    public Dictionary<int, List<string>> Select(string query)
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        return Select(query, param);
    }


    public Dictionary<int, List<string>> Select(string query, Dictionary<string,string> param)
    {
        Dictionary<int, List<string>> retorno = new Dictionary<int, List<string>>(); ;
        using (var connection = new SqliteConnection(this.connectionString))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                foreach(var p in param)
                {
                    command.Parameters.Add(new SqliteParameter(p.Key, p.Value));
                }
               
                using (var reader = command.ExecuteReader())
                {
                    int k = 0;
                    while (reader.Read())
                    {
                        List<string> result = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            result.Add(reader.GetValue(i).ToString());
                        }
                        retorno.Add(k,result);
                        k++;
                    }
                }
            }
        }
        return retorno;
    }

    public int Insert(string query, Dictionary<string, string> param)
    {
        using (var connection = new SqliteConnection(this.connectionString))
        {
            connection.Open();
            using (var transaction = connection.BeginTransaction())
            {
                using (var command = connection.CreateCommand())
                {
                  
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    foreach (var p in param)
                    {
                        command.Parameters.Add(new SqliteParameter(p.Key, p.Value));
                    }

                    command.Transaction = transaction;

                    var rows = command.ExecuteNonQuery();

                    command.CommandText = "select last_insert_rowid();";

                    Int64 fkId64 = (Int64)command.ExecuteScalar();
                    int fkId = (int)fkId64;
                    transaction.Commit();

                    return fkId;
                }
            }
        }
    }

    public string DateTimeSQLite(DateTime datetime)
    {
        string dateTimeFormat = "{0}-{1}-{2} {3}:{4}:{5}.{6}";
        return string.Format(dateTimeFormat, datetime.Year, datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second, datetime.Millisecond);
    }

    private void IniciarBase(string sqlFile)
    {
        Debug.Log(sqlFile);

        Dictionary<int, List<string>> retorno = Select("SELECT name FROM sqlite_master WHERE type='table' AND name='login';");

        Debug.Log(retorno.Count);

        if(retorno.Count == 0)
        {
            
            using (var connection = new SqliteConnection(this.connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    using (var command = connection.CreateCommand())
                    {

                        command.CommandText = sqlFile;
                        command.CommandType = CommandType.Text;

                        command.Transaction = transaction;

                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
            Debug.Log("Banco Iniciado");
        }

        
    }

}
