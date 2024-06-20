namespace IPInformationRetrieval.Queries
{
    public static class ReportLastAddressStatsQuery
    {
        public const string query = @"
            SELECT c.Name AS CountryName, COUNT(ia.Id) AS AddressesCount,
                   MAX(ia.UpdatedAt) AS LastAddressUpdated
            FROM Countries c
            LEFT JOIN IpAddresses ia ON c.Id = ia.CountryId
            WHERE (c.@ParameterName IN (@ParameterValues) OR c.@ParameterName IN (@ParameterValues) IS NULL)
            GROUP BY c.Id, c.Name
            ORDER BY c.Name;
        ";
        public static string GetQueryWithInterpolation(string parameterName, string parameterValues)
        {
            var query = $@"
                SELECT c.Name AS CountryName, COUNT(ia.Id) AS AddressesCount,
                       MAX(ia.UpdatedAt) AS LastAddressUpdated
                FROM Countries c
                LEFT JOIN IpAddresses ia ON c.Id = ia.CountryId
                WHERE (c.{parameterName} IN ({parameterValues}) OR c.{parameterName} IN ({parameterValues}) IS NULL)
                GROUP BY c.Id, c.Name
                ORDER BY c.Name;
            ";
            return query;
        }
    }
}
