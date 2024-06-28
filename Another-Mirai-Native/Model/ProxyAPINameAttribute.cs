namespace Another_Mirai_Native.Model
{
    [System.AttributeUsage(AttributeTargets.Method)]
    public class ProxyAPINameAttribute : Attribute
    {
        public string Description { get; }

        public ProxyAPINameAttribute(string description)
        {
            Description = description;
        }
    }
}
