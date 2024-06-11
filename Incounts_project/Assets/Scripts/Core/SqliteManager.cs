using System;
using System.IO;
using Mono.Data.Sqlite;
using UnityEngine;

public class SqliteManager
{
    private SqliteConnection _sqlConnection;

    public SqliteManager(string dbPath)
    {
        if (!File.Exists(dbPath))
        {
            Debug.Log($"Do Not Exists path {dbPath}");
            CreateDbSqlite(dbPath);
        }
        Debug.Log($"Exists path {dbPath}");
        ConnectDbSqlite(dbPath);
    }

    private bool CreateDbSqlite(string dbPath)
    {
        try
        {
            if (!Directory.Exists(new FileInfo(dbPath).Directory.FullName))
            {
                Directory.CreateDirectory(new FileInfo(dbPath).Directory.FullName);
            }
            SqliteConnection.CreateFile(dbPath);
            Debug.Log($"Create Database finished");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating database: {e.Message}");
            return false;
        }
    }

    private bool ConnectDbSqlite(string dbPath)
    {
        try
        {
            _sqlConnection = new SqliteConnection(new SqliteConnectionStringBuilder() { DataSource = dbPath }.ToString());
            _sqlConnection.Open();
            Debug.Log($"Connect Database finished");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error connecting database: {e.Message}");
            return false;
        }
    }

    public void Dispose()
    {
        if (_sqlConnection != null)
        {
            _sqlConnection.Close();
            _sqlConnection.Dispose();
        }
        _sqlConnection = null;
    }

    public void ExecuteQuery(string queryString, Action<SqliteDataReader> action)
    {
        using var command = _sqlConnection.CreateCommand();
        command.CommandText = queryString;
        using var reader = command.ExecuteReader();
        action?.Invoke(reader);
    }

    public bool ExecuteQuery<T>(string queryString, out T result, Func<SqliteDataReader, T> dataRetriever)
    {
        try
        {
            using (var command = _sqlConnection.CreateCommand())
            {
                command.CommandText = queryString;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = dataRetriever(reader);
                        return true; // ��ʾ��ѯ�ɹ��ҽ������ȡ
                    }
                    else
                    {
                        result = default(T); // û�н��ʱ����Ĭ��ֵ
                        Debug.Log("û�в�ѯ�����");
                        return false; // ��ʾ��ѯû�з��ؽ��
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("ִ�в�ѯʱ����δ֪�쳣: " + ex.Message);
            result = default(T); // ����δ֪�쳣ʱ����Ĭ��ֵ
            return false;
        }
    }

    /// <summary>
    /// �жϱ��Ƿ����
    /// </summary>
    /// <param name="tableName">����</param>
    /// <returns></returns>
    public bool TableExists(string tableName)
    {
        bool tableExists = false;
        using var command = _sqlConnection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=@tableName";
        command.Parameters.AddWithValue("@tableName", tableName);
        using (var reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                tableExists = true;
            }
        }
        return tableExists;
    }

    /// <summary>
    /// ���ر�ļ�¼����
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public int GetRecordCount(string tableName)
    {
        int recordCount = 0;
        using (var command = _sqlConnection.CreateCommand())
        {
            command.CommandText = $"SELECT COUNT(*) FROM {tableName}";
            object result = command.ExecuteScalar();
            // ��鷵��ֵ�Ƿ�ΪDBNull
            if (result != null && result != DBNull.Value)
            {
                // ���Խ����ת��Ϊint
                recordCount = Convert.ToInt32(result);
            }
        }
        return recordCount;
    }

    public int GetMaxInt(string tableName,string colName)
    {
        int recordMax = 0;
        using (var command = _sqlConnection.CreateCommand())
        {
            command.CommandText = $"SELECT MAX({colName}) FROM {tableName}";
            object result = command.ExecuteScalar();
            // ��鷵��ֵ�Ƿ�ΪDBNull
            if (result != null && result != DBNull.Value)
            {
                // ���Խ����ת��Ϊint
                recordMax = Convert.ToInt32(result);
            }
        }
        return recordMax;
    }

    #region ��Щ���öԷ��ص����ݽ��д�������ݿ����������ȫ����װ��
    public void InsertValues(string tableName, string[] values)
    {
        string cmd = SqlCommandGenerator.InsertValues(tableName, values);
        Debug.Log(cmd);
        ExecuteQuery(cmd, null);
    }

    public void UpdateValues(string tableName, string[] colNames, string[] colValues, string key, string operation, string value)
    {
        string cmd = SqlCommandGenerator.UpdateValues(tableName, colNames, colValues, key, operation, value);
        ExecuteQuery(cmd, null);
    }

    public void DeleteValuesOR(string tableName, string[] colNames, string[] operations, string[] colValues)
    {
        string cmd = SqlCommandGenerator.DeleteValuesOR(tableName, colNames, operations, colValues);
        ExecuteQuery(cmd, null);
    }

    public void DeleteValuesAND(string tableName, string[] colNames, string[] operations, string[] colValues)
    {
        string cmd = SqlCommandGenerator.DeleteValuesAND(tableName, colNames, operations, colValues);
        ExecuteQuery(cmd, null);
    }

    public void CreateTable(string tableName, string[] colNames, string[] colTypes)
    {
        string cmd = SqlCommandGenerator.CreateTable(tableName, colNames, colTypes);
        ExecuteQuery(cmd, null);
    }

    public void CreateTable(string tableName, string[] colNames, string[] colTypes, string primaryKey)
    {
        string cmd = SqlCommandGenerator.CreateTable(tableName, colNames, colTypes, primaryKey);
        ExecuteQuery(cmd, null);
    }
    #endregion
}

public class SqlCommandGenerator
{
    /// <summary>
    /// ��ָ�����ݱ��в�������
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">���ݱ�����</param>
    /// <param name="values">�������ֵ</param>
    public static string InsertValues(string tableName, string[] values)
    {
        //����Ӧ�û�ȡ���ݱ����ֶ���Ŀ���ж��ַ������鳤���Ƿ�ƥ�䡣
        string queryString = "INSERT INTO " + tableName + " VALUES (" + values[0];
        for (int i = 1; i < values.Length; i++)
        {
            queryString += ", " + values[i];
        }
        queryString += " )";
        return queryString;
    }

    /// <summary>
    /// ����ָ�����ݱ��ڵ�����
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">���ݱ�����</param>
    /// <param name="colNames">�ֶ���</param>
    /// <param name="colValues">�ֶ�����Ӧ������</param>
    /// <param name="key">�ؼ���</param>
    /// <param name="value">�ؼ�����Ӧ��ֵ</param>
    public static string UpdateValues(string tableName, string[] colNames, string[] colValues, string key, string operation, string value)
    {
        //���ֶ����ƺ��ֶ���ֵ����ȷӦʱ�����쳣
        if (colNames.Length != colValues.Length)
        {
            throw new SqliteException("colNames.Length!=colValues.Length");
        }

        string queryString = "UPDATE " + tableName + " SET " + colNames[0] + "=" + colValues[0];
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += ", " + colNames[i] + "=" + colValues[i];
        }
        queryString += " WHERE " + key + operation + value;
        return queryString;
    }

    /// <summary>
    /// ɾ��ָ�����ݱ��ڵ�����
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">���ݱ�����</param>
    /// <param name="colNames">�ֶ���</param>
    /// <param name="colValues">�ֶ�����Ӧ������</param>
    public static string DeleteValuesOR(string tableName, string[] colNames, string[] operations, string[] colValues)
    {
        //���ֶ����ƺ��ֶ���ֵ����ȷӦʱ�����쳣
        if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
        {
            throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + colValues[0];
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += "OR " + colNames[i] + operations[i] + colValues[i];
        }
        return queryString;
    }

    /// <summary>
    /// ɾ��ָ�����ݱ��ڵ�����
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">���ݱ�����</param>
    /// <param name="colNames">�ֶ���</param>
    /// <param name="colValues">�ֶ�����Ӧ������</param>
    public static string DeleteValuesAND(string tableName, string[] colNames, string[] operations, string[] colValues)
    {
        //���ֶ����ƺ��ֶ���ֵ����ȷӦʱ�����쳣
        if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
        {
            throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
        }

        string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + colValues[0];
        for (int i = 1; i < colValues.Length; i++)
        {
            queryString += " AND " + colNames[i] + operations[i] + colValues[i];
        }
        return queryString;
    }

    /// <summary>
    /// �������ݱ�
    /// </summary> +
    /// <returns>The table.</returns>
    /// <param name="tableName">���ݱ���</param>
    /// <param name="colNames">�ֶ���</param>
    /// <param name="colTypes">�ֶ�������</param>
    public static string CreateTable(string tableName, string[] colNames, string[] colTypes)
    {
        string queryString = "CREATE TABLE " + tableName + "( " + colNames[0] + " " + colTypes[0];
        for (int i = 1; i < colNames.Length; i++)
        {
            queryString += ", " + colNames[i] + " " + colTypes[i];
        }
        queryString += "  ) ";
        return queryString;
    }

    /// <summary>
    /// �������ݱ�
    /// </summary> +
    /// <returns>The table.</returns>
    /// <param name="tableName">���ݱ���</param>
    /// <param name="colNames">�ֶ���</param>
    /// <param name="colTypes">�ֶ�������</param>
    /// <param name="primaryKey">�����ֶ�</param>
    public static string CreateTable(string tableName, string[] colNames, string[] colTypes, string primaryKey)
    {
        string queryString = "CREATE TABLE " + tableName + "( " + colNames[0] + " " + colTypes[0];
        for (int i = 1; i < colNames.Length; i++)
        {
            queryString += ", " + colNames[i] + " " + colTypes[i];
        }
        queryString += $", PRIMARY KEY ({primaryKey}))";
        return queryString;
    }

    /// <summary>
    /// Reads the table.
    /// </summary>
    /// <returns>The table.</returns>
    /// <param name="tableName">Table name.</param>
    /// <param name="items">Items.</param>
    /// <param name="colNames">Col names.</param>
    /// <param name="operations">Operations.</param>
    /// <param name="colValues">Col values.</param>
    public static string ReadTable(string tableName, string[] items, string[] colNames, string[] operations, string[] colValues)
    {
        string queryString = "SELECT " + items[0];
        for (int i = 1; i < items.Length; i++)
        {
            queryString += ", " + items[i];
        }
        queryString += " FROM " + tableName + " WHERE " + colNames[0] + " " + operations[0] + " " + colValues[0];
        for (int i = 0; i < colNames.Length; i++)
        {
            queryString += " AND " + colNames[i] + " " + operations[i] + " " + colValues[0] + " ";
        }
        return queryString;
    }
}