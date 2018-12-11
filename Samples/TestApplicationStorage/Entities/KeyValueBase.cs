namespace TestApplicationStorage
{
    public class KeyValueBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public KeyValueBase()
        {
        }

        public KeyValueBase(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
