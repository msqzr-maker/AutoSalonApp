using System;
using Microsoft.Data.SqlClient;

namespace AutoSalonApp
{
    public static class AuthService
    {
        public static User? Login(string login, string pwd)
        {
            var dt = Db.Query(
                "SELECT UserID, Login, [Role], FullName FROM [User] WHERE Login=@l AND Password=@p",
                ("@l", login), ("@p", pwd));

            if (dt.Rows.Count == 0) return null;

            var r = dt.Rows[0];
            return new User
            {
                UserID = (int)r["UserID"],
                Login = (string)r["Login"],
                Role = (string)r["Role"],
                FullName = (string)r["FullName"]
            };
        }

        public static bool Register(string login, string pwd, string fio, out string? err)
        {
            err = login.Length < 3 ? "Логин должен быть не менее 3 символов"
                : pwd.Length < 6 ? "Пароль должен быть не менее 6 символов"
                : string.IsNullOrWhiteSpace(fio) ? "Укажите ФИО"
                : null;

            if (err != null) return false;

            try
            {
                Db.Exec("INSERT INTO [User](Login, Password, [Role], FullName) VALUES(@l, @p, 'User', @f)",
                    ("@l", login), ("@p", pwd), ("@f", fio));
                return true;
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                err = "Пользователь с таким логином уже существует";
                return false;
            }
            catch (Exception ex)
            {
                err = "Ошибка регистрации: " + ex.Message;
                return false;
            }
        }
    }
}