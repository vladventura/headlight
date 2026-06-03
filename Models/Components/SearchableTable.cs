using Headlight.AppCode.Globals;

namespace Headlight.Models.Components
{
    public class SearchableTableData
    {
        public const string HtmlId = "searchable-table";
        public const string HtmlClass = "searchable-table";
        public List<SearchableTableColumn> Columns { get; set; } = [];
        public List<SearchableTableRow> Rows { get; set; } = [];
        public string? HtmlAttributes { get; set; }
        public bool Paginate { get; set; } = false;

        public SearchableTableColumn AddColumn(string name, string? htmlAttr = null)
        {
            Columns.Add(new() 
            { 
                Index = Columns.Count, 
                Key = name, 
                HtmlAttributes = htmlAttr 
            });
            return Columns.Last();
        }

        public SearchableTableRow AddRow(string? htmlAttr = null)
        {
            Rows.Add(new()
            {
                HtmlAttributes = htmlAttr
            });
            return Rows.Last();
        }
    }

    public class SearchableTableColumn
    {
        public int Index { get; set; }
        public string Key { get; set; } = "";
        public string? HtmlAttributes { get; set; }
    }

    public class SearchableTableRow
    {
        public List<SearchableTableRowCell> Cells { get; set; } = [];
        public string? HtmlAttributes { get; set; }

        public SearchableTableRowCell AddCell(int index, string value)
        {
            Cells.Add(new()
            {
                Index = index,
                Value = value
            });

            return Cells.Last();
        }
    }

    public class SearchableTableRowCell
    {
        public int Index { get; set; }
        public string Value { get; set; } = "";
        public string? HtmlAttributes { get; set; }
        public SvgIcon? Icon { get; set; }
        public bool Clickable { get; set; } = false;
        public string? ClickableHtmlAttributes { get; set; }
        public bool ShouldRender { get; set; } = true;
    }
}
