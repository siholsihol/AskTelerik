using Telerik.FontIcons;

namespace TestTelerikTreeView.Custom
{
    public class TreeItem
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public bool HasChildren { get; set; }
        public FontIcon? Icon { get; set; }
    }
}
