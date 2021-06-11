using System.Threading.Tasks;

namespace Kentico.Xperience.Gql.Core.Services
{
    public interface ISchemaRefreshService
    {
        Task Refresh();
    }
}