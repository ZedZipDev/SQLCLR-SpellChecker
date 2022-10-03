using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Text.RegularExpressions;
using SpellChecker;

public partial class UserDefinedFunctions
{
    static readonly SpellCheck sp;

    static UserDefinedFunctions()
    {
        sp = new SpellCheck();
    }

    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString SpellCorrector(SqlString strInput)
    {
        string result = "";
        using (SqlConnection m_connection = new SqlConnection("context connection=true"))
        {
            if (sp != null && !sp.IsInitialized())
            {
                //result = "* ";
                if (m_connection != null && m_connection.State != ConnectionState.Open) m_connection.Open();
                sp.m_connection = m_connection;
                sp.InitObject();
            }
        }
        Regex rx = new Regex(@"\w+");
        result += rx.Replace(strInput.ToString(), new MatchEvaluator(Synonym));
        return result;
    }
    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString SpellCorrectorRefresh(SqlString strInput)
    {
        string result = "";
        try
        {
            using (SqlConnection m_connection = new SqlConnection("context connection=true"))
            {
                if (sp != null)
                {
                    if (m_connection != null && m_connection.State != ConnectionState.Open) m_connection.Open();
                    sp.m_connection = m_connection;
                    sp.InitObject();
                }
            }
        }
        catch (Exception x)
        {
            result = x.Message;
        }
        return result;
    }
    private static string Synonym(Match m)
    {
        string xSource = m.ToString();
        string x = m.ToString().ToLower();
        string str = "";
        str = sp.correct(x); 
        if (str == "")  str = x;
        return str;
    }
};
