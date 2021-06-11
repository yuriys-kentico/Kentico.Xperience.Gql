using CMS.Globalization;

namespace Kentico.Xperience.Gql.Schema.Types.Objects
{
    public class Country
    {
        private readonly CountryInfo country;

        public string TwoLetterCode => country.CountryTwoLetterCode;

        public string Name => country.CountryDisplayName;

        public Country(CountryInfo country)
        {
            this.country = country;
        }
    }
}