namespace Utils._Project.Utils.Serialization
{
    public interface ISerializeContract
    {
        object Serialize(object data);
        T Deserialize<T>(object data);
    }
}