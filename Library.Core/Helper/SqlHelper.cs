using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Library.Core.Helper
{
    public static class SqlHelper
    {
        public static SqlParameter[] AddArrayParameters<T>(this SqlCommand cmd, string paramNameRoot, IEnumerable<T> values, SqlDbType? dbType = null, int? size = null) {
            /* An array cannot be simply added as a parameter to a SqlCommand so we need to loop through things and add it manually. 
             * Each item in the array will end up being it's own SqlParameter so the return value for this must be used as part of the
             * IN statement in the CommandText.
             */
            var parameters = new List<SqlParameter>();
            var parameterNames = new List<string>();
            var paramNbr = 1;
            foreach (var value in values) {
                var paramName = string.Format("@{0}{1}", paramNameRoot, paramNbr++);
                parameterNames.Add(paramName);
                SqlParameter p = new SqlParameter(paramName, value);
                if (dbType.HasValue)
                    p.SqlDbType = dbType.Value;
                if (size.HasValue)
                    p.Size = size.Value;
                cmd.Parameters.Add(p);
                parameters.Add(p);
            }

            cmd.CommandText = cmd.CommandText.Replace("{" + paramNameRoot + "}", string.Join(",", parameterNames));

            return parameters.ToArray();
        }
    }
}
