using IPInformationRetrieval.Database.DatabaseContext;
using IPInformationRetrieval.Models;
using System.Data;

namespace IPInformationRetrieval.Helpers
{
    public static class Helpers
    {
        public static List<ReportLastAddressStats> GetReportFromReader(IDataReader reader)
        {
            var results = new List<ReportLastAddressStats>();

            if (reader == null)
                return results;

            while (reader.Read())
            {
                var CountryName = reader.GetString(reader.GetOrdinal("CountryName"));
                var AddressesCount = reader.GetInt32(reader.GetOrdinal("AddressesCount"));
                var LastAddressUpdated = reader.GetDateTime(reader.GetOrdinal("LastAddressUpdated"));
                results.Add(new ReportLastAddressStats(CountryName, AddressesCount, LastAddressUpdated));
            }
            return results;
        }

        public static bool HasIpInformationChanged(CountryAndIPAddress first, CountryAndIPAddress second)
        {
            if (first == null || second == null)
                return false;

            if (first.IPAddress.CountryId != second.IPAddress.CountryId
                || first.Country.TwoLetterCode != second.Country.TwoLetterCode
                || first.Country.ThreeLetterCode != second.Country.ThreeLetterCode
                || first.Country.Name != second.Country.Name
                )
                return true;

            return false;
        }

        public static IpAddress MergeExistingDataWithNewData(this IpAddress existingData, IpAddress newData, IpInformationDbContext _db)
        {
            existingData.CountryId = newData.CountryId;
            existingData.UpdatedAt = DateTime.UtcNow;
            return existingData;
        }
    }
}
