using CMS.Globalization;

namespace Kentico.Xperience.Gql.Schema.Types.Objects
{
    public class CountryState : ErrorObject
    {
        public Country? Country { get; }

        public State? State { get; }

        public CountryState(
            CountryInfo? country,
            StateInfo? state
            ) : base(null)
        {
            if (country != null)
            {
                Country = new Country(country);
            }

            if (state != null)
            {
                State = new State(state);
            }
        }

        public CountryState(string errorMessage) : base(errorMessage)
        {
            Country = null!;
            State = null!;
        }
    }
}