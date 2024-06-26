namespace Another_Mirai_Native.Model
{
    [System.AttributeUsage(AttributeTargets.Method)]
    sealed class ProxyAPINameAttribute : Attribute
    {
        public string Description { get; }

        public ProxyAPINameAttribute(string description)
        {
            Description = description;
        }
    }
}
