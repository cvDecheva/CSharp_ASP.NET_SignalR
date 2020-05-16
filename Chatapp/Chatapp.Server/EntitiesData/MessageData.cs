using Chatapp.Server.Entities;
using Chatapp.Shared.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Chatapp.Server.EntitiesData
{
    public class MessageData : Data<Message, IntFilter>
    {
        #region Protected members
        protected override string TableName { get { return "Message"; } }

        protected override string ColumnNames { get { return "SenderId, ReceiverId, Date, Text, Image"; } }

        protected override string Values { get { return "@SenderId, @ReceiverId, @Date, @Text, @Image"; } }

        protected override void AddData(SqlCommand cmd, Message obj)
        {
            cmd.Parameters.Add("@SenderId", SqlDbType.Int).Value = obj.SenderId;
            cmd.Parameters.Add("@ReceiverId", SqlDbType.Int).Value = obj.ReceiverId;
            cmd.Parameters.Add("@Date", SqlDbType.Date).Value = obj.Date;
            cmd.Parameters.Add("@Text", SqlDbType.NVarChar).Value = obj.Text == null ? "" : obj.Text;
            cmd.Parameters.Add("@Image", SqlDbType.Image).Value = obj.Image == null ? new byte[0] : obj.Image;
        }

        protected override Message ReadEntity(SqlDataReader rdr)
        {
            Message message = new Message();
            message.SenderId = (int)rdr["SenderId"];
            message.ReceiverId = (int)rdr["ReceiverId"];
            message.Date = (DateTime)rdr["Date"];
            message.Text = (string)rdr["Text"];
            message.Image = !string.IsNullOrEmpty(rdr["Image"].ToString()) ? (byte[])rdr["Image"] : new byte[0];
            return message;
        }

        protected override StringBuilder UpdateString(Message message)
        {
            StringBuilder updateString = new StringBuilder();
            string[] val = Values.Split(',');
            updateString.Append("UPDATE " + TableName);
            updateString.Append(" SET SenderId='" + val[0] + "'");
            updateString.Append(", ReceiverId=" + val[1]);
            updateString.Append(", Date='" + val[2] + "'");
            updateString.Append(", Text='" + val[3] + "'");
            updateString.Append(", Image=" + val[4]);
            updateString.Append(" WHERE Id=" + message.Id);
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