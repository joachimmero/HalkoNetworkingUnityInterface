using System;

namespace HalkoNetworking.RemoteMethod
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HalkoMethod : Attribute
    {
    }
}
