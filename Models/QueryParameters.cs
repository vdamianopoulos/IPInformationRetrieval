namespace IPInformationRetrieval.Models
{
    public class QueryParameters
    {
        public QueryParameters(string parameterName, IEnumerable<string> parameterValues)
        {
            ParameterName = parameterName;
            ParameterValues = parameterValues;
        }

        public string ParameterName { get; set; }
        public IEnumerable<string> ParameterValues { get; set; }
    }
}
