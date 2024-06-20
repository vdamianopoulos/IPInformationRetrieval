namespace IPInformationRetrieval.Models
{
    public class ReportLastAddressStats
    {
        public ReportLastAddressStats(string countryName, int addressesCount, DateTime lastAddressUpdated)
        {
            CountryName = countryName;
            AddressesCount = addressesCount;
            LastAddressUpdated = lastAddressUpdated;
        }

        public string CountryName { get; set; }
        public int AddressesCount { get; set; }
        public DateTime LastAddressUpdated { get; set; }
    }
}
