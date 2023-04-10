namespace ProjCommon.Exceptions;

[System.Serializable]
public class InvalidConfigException : System.Exception
{
    public InvalidConfigException() { }
    public InvalidConfigException(string message) : base(message) { }
    public InvalidConfigException(string message, System.Exception inner) : base(message, inner) { }
    protected InvalidConfigException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}