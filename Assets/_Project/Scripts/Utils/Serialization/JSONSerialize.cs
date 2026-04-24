using Newtonsoft.Json;

namespace Utils._Project.Utils.Serialization
{
    public class JSONSerialize : ISerializeContract
    {
        T ISerializeContract.Deserialize<T>(object data)
        {
            return JsonConvert.DeserializeObject<T>(data.ToString());
        }

        object ISerializeContract.Serialize(object data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}