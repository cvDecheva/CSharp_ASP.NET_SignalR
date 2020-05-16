using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Chatapp.Server.Entities;
using Chatapp.Shared.Entities;

namespace Chatapp.Server.EntitiesData
{
    public class UserData : Data<User, StringFilter>
    {
        #region Protected members
        protected override string TableName { get { return "User"; } }

        protected override string ColumnNames { get { return "Username, Password, Name, LastName, Email, BirthDate, Image"; } }

        protected override string Values { get { return "@Username, @Password, @Name, @LastName, @Email, @BirthDate, @Image"; } }

        protected override void AddData(SqlCommand cmd, User obj)
        {
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = obj.Username;
            cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = obj.Password;
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = obj.Name;
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = obj.LastName;
            cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = obj.Email;
            cmd.Parameters.Add("@BirthDate", SqlDbType.DateTime).Value = obj.BirthDate;
            cmd.Parameters.Add("@Image", SqlDbType.Image).Value = obj.Image;
        }

        protected override User ReadEntity(SqlDataReader rdr)
        {
            User user = new User();
            user.Username = (string)rdr["Username"];
            user.Password = (string)rdr["Password"];
            user.Name = (string)rdr["Name"];
            user.LastName = (string)rdr["LastName"];
            user.Email = (string)rdr["Email"];
            user.BirthDate = (DateTime)rdr["BirthDate"];
            user.Image = !string.IsNullOrEmpty(rdr["Image"].ToString()) ? (byte[])rdr["Image"] : new byte[0];
            return user;
        }

        protected override StringBuilder UpdateString(User user)
        {
            StringBuilder updateString = new StringBuilder();
            string[] val = Values.Split(',');
            updateString.Append("UPDATE " + TableName);
            updateString.Append(" SET Username='" + val[0] + "'");
            updateString.Append(", Password='" + val[1] + "'");
            updateString.Append(", Name='" + val[2] + "'");
            updateString.Append(", LastName='" + val[3] + "'");
            updateString.Append(", Email=" + val[4]);
            updateString.Append(", BirthDate=" + val[5]);
            updateString.Append(", Image=" + val[5]);
            updateString.Append(" WHERE ID=" + user.Id);
            return updateString;
        }

        protected override StringBuilder AddFilter(StringFilter filter)
        {
            StringBuilder selectString = new StringBuilder();
            if (filter.Value != null)
            {
                selectString.Append(" WHERE " + filter.ColumnName + " " + filter.Operator + " '%" + filter.Value + "%'");
            }
            return selectString;
        }
        #endregion
    }
}