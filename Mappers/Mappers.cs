using IPInformationRetrieval.Models;

namespace IPInformationRetrieval.Mappers
{
    public static class Mappers
    {
        public static CountryAndIPAddress MapToModel(string results, string ip)
        {
            var ipData = results.Trim().Split(';');
            if (ipData.Length != 4) //1;GR;GRC;Greece
            {
                throw new Exception("Invalid Response from IP2C");
            }

            return new CountryAndIPAddress()
            {
                Country = new Country(
                    int.Parse(ipData[0]),
                    ipData[1],
                    ipData[2],
                    ipData[3]
                ),
                IPAddress = new IpAddress
                (
                    int.Parse(ipData[0]),
                    ip
                )
            };
        }
    }
}
