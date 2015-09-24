namespace Bridge
{
    public class Field
    {
        public Field(string name)
        {
            this.Name = name;
        }

        public string Name { get; protected set; }
    }
}