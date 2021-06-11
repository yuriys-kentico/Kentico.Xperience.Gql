namespace Kentico.Xperience.Gql.Schema.Types.Objects
{
    public abstract class ErrorObject
    {
        public string? Error { get; protected set; }

        protected ErrorObject(string? errorMessage)
        {
            Error = errorMessage;
        }
    }
}