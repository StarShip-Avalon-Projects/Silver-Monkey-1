using MonkeyCore;
using System.Data;
namespace SQLiteEditor
{
    /// <summary>
    /// Summary description for StatementParser.
    /// </summary>
    public class StatementParser
	{
		public StatementParser()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        public static bool ReturnResults(string SQLStatement, string DatabaseLocation, out string message)
        {
            DataSet ds = null;
            
            return ReturnResults(SQLStatement, DatabaseLocation, ref ds, out message);
        }

        public static bool ReturnResults(string SQLStatement, string DatabaseLocation, ref DataSet ds, out string message)
        {
            //Add a call here to a parser that will 
            //ensure the SQLStatement is properly formed

            if (SQLStatement.ToLower().StartsWith("select") || SQLStatement.ToLower().StartsWith("pragma"))
            {
                SQLiteDatabase db = new SQLiteDatabase(DatabaseLocation);
                ds = db.ExecuteQuery(SQLStatement);
                message = string.Format( ds != null ? "ExecuteQuery: ok" : "ExecuteQuery Failed");
                return ds == null;
            }
            else
            {
                int result = SQLiteDatabase.ExecuteNonQuery(SQLStatement);
                ds = null;
                message = string.Format("ExecuteNonQuery: Records Modified {0}", result);
                return result > -1;
            }


        }


    }
}