namespace TestTelerikTreeView.Custom.Events
{
    public class RefreshTreeViewEventArgs : EventArgs
    {
        public List<TreeItem> TreeItems { get; set; } = new();
    }
}
