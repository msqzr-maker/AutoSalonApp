using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace AutoSalonApp
{
    public static class Db
    {
        const string Cs = @"Data Source=localhost;Initial Catalog=AutoSalon;Integrated Security=True;TrustServerCertificate=True";

        public static SqlConnection Open()
        {
            var c = new SqlConnection(Cs);
            c.Open();
            return c;
        }

        public static DataTable Query(string sql, params (string, object)[] p)
        {
            using var conn = Open();
            using var cmd = new SqlCommand(sql, conn);
            foreach (var (n, v) in p) cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);
            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static int Exec(string sql, params (string, object)[] p)
        {
            using var conn = Open();
            using var cmd = new SqlCommand(sql, conn);
            foreach (var (n, v) in p) cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);
            return cmd.ExecuteNonQuery();
        }

        public static void Tx(params (string sql, (string, object)[] p)[] steps)
        {
            using var conn = Open();
            using var tx = conn.BeginTransaction();
            try
            {
                foreach (var (sql, ps) in steps)
                {
                    using var cmd = new SqlCommand(sql, conn, tx);
                    foreach (var (n, v) in ps) cmd.Parameters.AddWithValue(n, v ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }
    }
}