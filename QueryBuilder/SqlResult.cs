using System.Collections.Generic;
using System.Linq;

namespace SqlKata.QueryBuilder
{
    public class SqlResult
    {
        public string RawSql { get; set; }
        public List<object> RawBindings { get; set; }

        public SqlResult(string sql, List<object> bindings)
        {
            RawSql = sql;
            RawBindings = bindings;
        }

        public string Sql
        {
            get
            {
                return Helper.ReplaceAll(RawSql, "?", x => "@p" + x);
            }
        }

        public Dictionary<string, object> Bindings
        {
            get
            {
                var namedParams = new Dictionary<string, object>();

                for (var i = 0; i < RawBindings.Count; i++)
                {
                    namedParams["p" + i] = RawBindings[i];
                }

                return namedParams;
            }
        }

        public override string ToString()
        {
            return Helper.ReplaceAll(RawSql, "?", i =>
            {
                var value = RawBindings[i];

                if (value == null)
                {
                    return "NULL";
                }

                var textValue = value.ToString();

                if (IsNumber(textValue))
                {
                    return textValue;
                }

                return "'" + textValue + "'";

            });
        }

        private static bool IsNumber(string val)
        {
            return !string.IsNullOrEmpty(val) && val.All(char.IsDigit);
        }

        public static SqlResult operator +(SqlResult a, SqlResult b)
        {
            var sql = a.RawSql + ";" + b.RawSql;

            var bindings = a.RawBindings.Concat(b.RawBindings).ToList();

            var result = new SqlResult(sql, bindings);

            return result;
        }

    }
}