using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADONETHelpers
{
    public static class SqlExceptionExtensions
    {
        public static string FullMessage(this SqlException sqlException)
        {
            var builder = new StringBuilder();
            if (sqlException != null)
            {
                builder.AppendFormat("{0}{1}", sqlException.Message, Environment.NewLine);
                builder.AppendFormat("Procedure:{0}{1}", sqlException.Procedure, Environment.NewLine);
                builder.AppendFormat("Line Number:{0}{1}", sqlException.LineNumber, Environment.NewLine);
                builder.AppendFormat("{0}{1}", sqlException.Procedure, Environment.NewLine);
                builder.AppendFormat("Class(Severity) :{0}{1}", sqlException.Class, Environment.NewLine);
                builder.AppendFormat("Errors:{0}", Environment.NewLine);
                foreach (SqlError error in sqlException.Errors)
                {
                    builder.AppendFormat("{0}{1}", error.Message, Environment.NewLine);
                    builder.AppendFormat("Procedure:{0}{1}", error.Procedure, Environment.NewLine);
                    builder.AppendFormat("Line Number:{0}{1}", error.LineNumber, Environment.NewLine);
                    builder.AppendFormat("{0}{1}", error.Procedure, Environment.NewLine);
                    builder.AppendFormat("Class(Severity) :{0}{1}", error.Class, Environment.NewLine);
                }
            }
            return builder.ToString();
        }
    }
}
