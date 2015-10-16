namespace DataBridge
{
    public class IndexSort
    {
        public IndexSort(string indexName, bool ascending = true)
        {
            this.IndexName = indexName;
            this.Ascending = ascending;
        }

        public string IndexName { get; protected set; }
        public bool Ascending { get; protected set; }
    }
}