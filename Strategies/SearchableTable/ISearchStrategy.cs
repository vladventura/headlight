using Headlight.Models.Components;

namespace Headlight.Strategies.SearchableTable
{
    public interface ISearchStrategy
    {
        public SearchableTableData GetTableData();
    }
}
