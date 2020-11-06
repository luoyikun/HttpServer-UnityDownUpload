using System.Collections;
using System.Collections.Generic;

using MySql.Data.MySqlClient;
using System.Data;
using System;
public class MySqlMgr
{
    //连接类对象
    public MySqlConnection m_connection = null;
    //IP地址
    private string host;
    //端口号
    private string port;
    //用户名
    private string userName;
    //密码
    private string password;
    //数据库名称
    private string databaseName;
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="_host">ip地址</param>
    /// <param name="_userName">用户名</param>
    /// <param name="_password">密码</param>
    /// <param name="_databaseName">数据库名称</param>
    public MySqlMgr(string _host, string _port, string _userName, string _password, string _databaseName)
    {
        host = _host;
        port = _port;
        userName = _userName;
        password = _password;
        databaseName = _databaseName;
        OpenSql();
    }
    /// <summary>
    /// 打开数据库
    /// </summary>
    public void OpenSql()
    {
        try
        {
            if (m_connection == null)
            {
                string mySqlString = string.Format("Database={0};Data Source={1};User Id={2};Password={3};port={4}"
                  , databaseName, host, userName, password, port);
                m_connection = new MySqlConnection(mySqlString);
                m_connection.Open();
                Console.WriteLine("连接数据库成功");
            }
        }
        catch (Exception e)
        {
            throw new Exception("服务器连接失败，请重新检查MySql服务是否打开。" + e.Message.ToString());
        }
    }

    /// <summary>
    /// 关闭数据库
    /// </summary>
    public void CloseSql()
    {
        if (m_connection != null)
        {
            m_connection.Close();
            m_connection.Dispose();
            m_connection = null;
        }
    }
   
    /// <summary>
    /// 查找全部
    /// </summary>
    /// <param name="tabName">表名</param>
    /// <param name="func"></param>
    public void SelectAll(string tabName,Action<MySqlDataReader> func)
    {
        try
        {
            OpenSql();
            string sql = "select * from " + tabName;
            MySqlCommand com = new MySqlCommand(sql, m_connection);
            MySqlDataReader reader = com.ExecuteReader();
            while (true)
            {
                if (reader.Read())
                {
                    func(reader);
                }
                else
                {
                    break;
                }
            }
            reader.Close();
            CloseSql();
        }
        catch (Exception e)
        {
            
        }
    }

    /// <summary>
    /// 修改数据
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="con"></param>
    public int UpdateOneLine(string tableName, string[] findKeys,string[] operations, string[] findValues,string[] allKeys,string[] allValues)
    {
        //string sql = ("update Student set Sage='18' where Sname='学生A'");
        string sql = "update " + tableName + " set ";
        for (int i = 0; i < allKeys.Length; i++)
        {
            sql += allKeys[i] + "='" + allValues[i] + "' ";
            if (i != allKeys.Length - 1)
            {
                sql += ',';
            }
        }
        sql += " where ";
        for (int i = 0; i < findKeys.Length; i++)
        {
            sql += findKeys[i] + operations[i] + "'" + findValues[i] + "' ";
        }

        int res = SqlExecute(sql);
        return res;
    }

    int SqlExecute(string sql)
    {
       
        OpenSql();
        MySqlCommand com = new MySqlCommand(sql, m_connection);
        int res = com.ExecuteNonQuery();
        //com.Dispose();
        CloseSql();
        return res;
    }

    public int DeleteLine(string tableName,string findKeys,string[] findValues)
    {
        //delete from luoyangpt919 where id in ('1.1', '1')
        string sql = "delete from " + tableName + " where "  + findKeys + " in (";
        for (int i = 0; i < findValues.Length; i++)
        {
            sql +="'" +  findValues[i] + "'";
            if (i != findValues.Length - 1)
            {
                sql += ",";
            }
        }
        sql += ")";
        int res = SqlExecute(sql);
        return res;
    }

    public int AddOneLine(string tableName, string[] keys, string[] values)
    {
        //"insert into Student(Sid,Sname,Sage,Sdept) values('1001','学生A','20','土木工程')"
        string sql = "insert into " + tableName + "(";
        for (int i = 0; i < keys.Length; i++)
        {
            sql += keys[i];
            if (i != keys.Length - 1)
            {
                sql += ",";
            }
        }

        sql += ") values (";
        for (int i = 0; i < values.Length; i++)
        {
            sql += "'" + values[i] + "'";
            if (i != values.Length - 1)
            {
                sql += ",";
            }
        }
        sql += ")";
        int res = SqlExecute(sql);
        return res;
    }
}
