using System;

namespace CoreNotify.Functions.Classes
{
    public class SqlAdapter
    {
        public SqlAdapter(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}
