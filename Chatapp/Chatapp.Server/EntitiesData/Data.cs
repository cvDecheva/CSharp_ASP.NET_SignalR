using Chatapp.Server.Entities;
using Chatapp.Shared.Entities;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Chatapp.Server.EntitiesData
{
    public abstract class Data<TData, TFilter>
        where TData : Entity
        where TFilter : Filter
    {
        public void Create(TData obj)
        {
            using (SqlCommand cmd = new SqlCommand(InsertString().ToString(), Connection.Conn))
            {
                AddData(cmd, obj);
                obj.Id = (int)cmd.ExecuteScalar();
            }
        }

        public List<TData> Read(TFilter filter)
        {
            List<TData> objects = new List<TData>();
            using (SqlCommand cmd = new SqlCommand(SelectString(filter).ToString(), Connection.Conn))
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        TData entity = ReadEntity(rdr);
                        entity.Id = (int)rdr["ID"];
                        objects.Add(entity);
                    }
                }
            }
            return objects;
        }

        public void Update(TData obj)
        {
            using (SqlCommand cmd = new SqlCommand(UpdateString(obj).ToString(), Connection.Conn))
            {
                AddData(cmd, obj);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(TData obj)
        {
            using (SqlCommand cmd = new SqlCommand(DeleteString(obj).ToString(), Connection.Conn))
            {
                cmd.ExecuteNonQuery();
            }
        }

        #region Protected members
        protected abstract string TableName { get; }
        protected abstract string ColumnNames { get; }
        protected abstract string Values { get; }

        protected abstract void AddData(SqlCommand cmd, TData obj);

        protected abstract TData ReadEntity(SqlDataReader rdr);

        protected abstract StringBuilder UpdateString(TData obj);

        protected virtual StringBuilder AddFilter(TFilter filter)
        {
            return new StringBuilder();
        }
        #endregion

        #region Private methods
        private StringBuilder DeleteString(TData entity)
        {
            StringBuilder deleteString = new StringBuilder();
            deleteString.Append("DELETE FROM [" + TableName + "]");
            deleteString.Append(" WHERE ID=" + entity.Id);
            return deleteString;
        }

        private StringBuilder InsertString()
        {
            StringBuilder insertString = new StringBuilder();
            insertString.Append("INSERT INTO [" + TableName + "] (");
            insertString.Append(ColumnNames + ") OUTPUT INSERTED.ID VALUES (");
            insertString.Append(Values + ")");
            return insertString;
        }

        private StringBuilder SelectString(TFilter filter)
        {
            StringBuilder selectString = new StringBuilder();
            selectString.Append("SELECT * FROM [");
            selectString.Append(TableName + "]");
            selectString.Append(AddFilter(filter));
            return selectString;
        }
        #endregion
    }
}