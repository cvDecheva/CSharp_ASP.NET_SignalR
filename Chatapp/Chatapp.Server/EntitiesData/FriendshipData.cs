using Chatapp.Server.Entities;
using Chatapp.Shared.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Chatapp.Server.EntitiesData
{
    public class FriendshipData : Data<Friendship, IntFilter>
    {
        #region Protected members
        protected override string TableName { get { return "Friendship"; } }

        protected override string ColumnNames { get { return "User1Id, User2Id"; } }

        protected override string Values { get { return "@User1Id, @User2Id"; } }

        protected override void AddData(SqlCommand cmd, Friendship obj)
        {
            cmd.Parameters.Add("@User1Id", SqlDbType.Int).Value = obj.User1Id;
            cmd.Parameters.Add("@User2Id", SqlDbType.Int).Value = obj.User2Id;
        }

        protected override Friendship ReadEntity(SqlDataReader rdr)
        {
            Friendship friendship = new Friendship();
            friendship.User1Id = (int)rdr["User1Id"];
            friendship.User2Id = (int)rdr["User2Id"];
            return friendship;
        }

        protected override StringBuilder UpdateString(Friendship friendship)
        {
            StringBuilder updateString = new StringBuilder();
            string[] val = Values.Split(',');
            updateString.Append("UPDATE " + TableName);
            updateString.Append(" SET User1Id=" + val[0]);
            updateString.Append(", User2Id=" + val[1]);
            updateString.Append(" WHERE Id=" + friendship.Id);
            return updateString;
        }

        protected override StringBuilder AddFilter(IntFilter filter)
        {
            StringBuilder selectString = new StringBuilder();
            if (filter.Value != null)
            {
                selectString.Append(" WHERE " + filter.ColumnName + " " + filter.Operator + " " + filter.Value);
            }
            return selectString;
        }
        #endregion
    }
}