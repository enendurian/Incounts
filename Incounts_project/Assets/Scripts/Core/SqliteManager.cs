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
                        return true; // 表示查询成功且结果已提取
                    }
                    else
                    {
                        result = default(T); // 没有结果时设置默认值
                        Debug.Log("没有查询到结果");
                        return false; // 表示查询没有返回结果
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("执行查询时发生未知异常: " + ex.Message);
            result = default(T); // 出现未知异常时设置默认值
            return false;
        }
    }

    /// <summary>
    /// 判断表是否存在
    /// </summary>
    /// <param name="tableName">表名</param>
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
    /// 返回表的记录条数
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
            // 检查返回值是否为DBNull
            if (result != null && result != DBNull.Value)
            {
                // 尝试将结果转换为int
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
            // 检查返回值是否为DBNull
            if (result != null && result != DBNull.Value)
            {
                // 尝试将结果转换为int
                recordMax = Convert.ToInt32(result);
            }
        }
        return recordMax;
    }

    #region 那些不用对返回的数据进行处理的数据库操作，可以全部封装好
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
    /// 向指定数据表中插入数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="values">插入的数值</param>
    public static string InsertValues(string tableName, string[] values)
    {
        //这里应该获取数据表中字段数目并判断字符串数组长度是否匹配。
        string queryString = "INSERT INTO " + tableName + " VALUES (" + values[0];
        for (int i = 1; i < values.Length; i++)
        {
            queryString += ", " + values[i];
        }
        queryString += " )";
        return queryString;
    }

    /// <summary>
    /// 更新指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名相应的数据</param>
    /// <param name="key">关键字</param>
    /// <param name="value">关键字相应的值</param>
    public static string UpdateValues(string tableName, string[] colNames, string[] colValues, string key, string operation, string value)
    {
        //当字段名称和字段数值不正确应时引发异常
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
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名相应的数据</param>
    public static string DeleteValuesOR(string tableName, string[] colNames, string[] operations, string[] colValues)
    {
        //当字段名称和字段数值不正确应时引发异常
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
    /// 删除指定数据表内的数据
    /// </summary>
    /// <returns>The values.</returns>
    /// <param name="tableName">数据表名称</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colValues">字段名相应的数据</param>
    public static string DeleteValuesAND(string tableName, string[] colNames, string[] operations, string[] colValues)
    {
        //当字段名称和字段数值不正确应时引发异常
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
    /// 创建数据表
    /// </summary> +
    /// <returns>The table.</returns>
    /// <param name="tableName">数据表名</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colTypes">字段名类型</param>
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
    /// 创建数据表
    /// </summary> +
    /// <returns>The table.</returns>
    /// <param name="tableName">数据表名</param>
    /// <param name="colNames">字段名</param>
    /// <param name="colTypes">字段名类型</param>
    /// <param name="primaryKey">主键字段</param>
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