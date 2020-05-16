using Chatapp.Server.Entities;
using Chatapp.Shared.Entities;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Chatapp.Server.EntitiesData
{
    public class FriendshipRequestData : Data<FriendshipRequest, IntFilter>
    {
        #region Protected members
        protected override string TableName { get { return "FriendshipRequest"; } }

        protected override string ColumnNames { get { return "SenderId, ReceiverId"; } }

        protected override string Values { get { return "@SenderId, @ReceiverId"; } }

        protected override void AddData(SqlCommand cmd, FriendshipRequest obj)
        {
            cmd.Parameters.Add("@SenderId", SqlDbType.Int).Value = obj.SenderId;
            cmd.Parameters.Add("@ReceiverId", SqlDbType.Int).Value = obj.ReceiverId;
        }

        protected override FriendshipRequest ReadEntity(SqlDataReader rdr)
        {
            FriendshipRequest friendshipRequest = new FriendshipRequest();
            friendshipRequest.SenderId = (int)rdr["SenderId"];
            friendshipRequest.ReceiverId = (int)rdr["ReceiverId"];
            return friendshipRequest;
        }

        protected override StringBuilder UpdateString(FriendshipRequest friendshipRequest)
        {
            StringBuilder updateString = new StringBuilder();
            string[] val = Values.Split(',');
            updateString.Append("UPDATE " + TableName);
            updateString.Append(" SET SenderId=" + val[0]);
            updateString.Append(", ReceiverId=" + val[1]);
            updateString.Append(" WHERE Id=" + friendshipRequest.Id);
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