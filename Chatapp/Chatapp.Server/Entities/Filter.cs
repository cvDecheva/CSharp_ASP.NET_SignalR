namespace Chatapp.Server.Entities
{
    public abstract class Filter
    {
        public string ColumnName { get; set; }

        public string Operator { get; set; }
    }
}