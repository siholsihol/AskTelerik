namespace TestTelerikTreeView.Custom.Events
{
    public class GetRecordEventArgs : EventArgs
    {
        public TreeItem CurrentItem { get; }

        public GetRecordEventArgs(TreeItem item)
        {
            CurrentItem = item;
        }
    }
}
