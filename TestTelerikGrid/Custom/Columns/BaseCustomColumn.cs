using Microsoft.AspNetCore.Components;
using Telerik.Blazor;
using TestTelerikGrid.Custom.Enums;

namespace TestTelerikGrid.Custom.Columns
{
    public abstract class BaseCustomColumn : ComponentBase
    {
        [CascadingParameter] public ICustomGrid Grid { get; set; }
        [Parameter, EditorRequired] public string FieldName { get; set; }
        [Parameter] public string Name { get; set; }
        [Parameter] public string HeaderText { get; set; }
        [Parameter] public virtual ColumnTextAlign TextAlignment { get; set; } = ColumnTextAlign.Left;
        [Parameter] public bool Visible { get; set; } = true;
        [Parameter] public bool Enabled { get; set; } = true;
        [Parameter] public string Width { get; set; }
        [Parameter] public bool Sortable { get; set; }
        [Parameter] public bool Filterable { get; set; }
        [Parameter] public bool Resizable { get; set; }
        [Parameter] public decimal MinResizableWidth { get; set; }
        [Parameter] public decimal MaxResizableWidth { get; set; }

        internal abstract GridColumnType ColumnType { get; set; }

        protected override void OnInitialized()
        {
            if (Grid != null)
                Grid.AddColumn(this);
        }
    }
}
