using System.Text.Json.Serialization;

namespace IPInformationRetrieval.Models
{
    public class IpAddress
    {
        [JsonConstructor]
        public IpAddress(int? id, int countryId, string iP, DateTime? createdAt = null, DateTime? updatedAt = null)
        {
            Id = id;
            CountryId = countryId;
            IP = iP;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        public IpAddress(int countryId, string ip)
        {
            CountryId = countryId;
            IP = ip;
        }

        public int? Id { get; set; }
        public int CountryId { get; set; }
        public string IP { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
