using System.Text.Json.Serialization;

namespace IPInformationRetrieval.Models
{
    public class Country
    {
        [JsonConstructor]
        public Country(int id, string name, string twoLetterCode, string threeLetterCode, DateTime? createdAt)
        {
            Id = id;
            TwoLetterCode = twoLetterCode;
            ThreeLetterCode = threeLetterCode;
            Name = name;
            CreatedAt = createdAt;
        }

        public Country(int id, string twoLetterCode, string threeLetterCode, string name)
        {
            Id = id;
            TwoLetterCode = twoLetterCode;
            ThreeLetterCode = threeLetterCode;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string TwoLetterCode { get; set; }
        public string ThreeLetterCode { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

}
