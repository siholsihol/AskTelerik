namespace TestTelerikGrid.Custom.Events
{
    public class R_ServiceGetRecordEventArgs : EventArgs
    {
        public object Data { get; }
        public object Result { get; set; } = null;

        public R_ServiceGetRecordEventArgs(object data, object result)
        {
            Data = data;
        }
    }
}
