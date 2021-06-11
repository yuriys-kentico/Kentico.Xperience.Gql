using CMS.Globalization;

namespace Kentico.Xperience.Gql.Schema.Types.Objects
{
    public class State
    {
        private readonly StateInfo state;

        public string Code => state.StateCode;

        public string Name => state.StateDisplayName;

        public State(StateInfo state)
        {
            this.state = state;
        }
    }
}