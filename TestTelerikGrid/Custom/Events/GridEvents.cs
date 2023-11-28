using TestTelerikGrid.Custom.Enums;

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

    public class R_ValidationEventArgs : EventArgs
    {
        public GridMode GridMode { get; }
        public object Data { get; }
        public bool Cancel { get; set; } = false;
        public Dictionary<string, string> ErrorMessages { get; set; } = new Dictionary<string, string>();

        public R_ValidationEventArgs(object data, GridMode gridMode)
        {
            GridMode = gridMode;
            Data = data;
        }
    }

    public class R_ServiceSaveEventArgs : EventArgs
    {
        public GridMode GridMode { get; }
        public object Data { get; set; }
        public object Result { get; set; }

        public R_ServiceSaveEventArgs(object data, GridMode gridMode)
        {
            GridMode = gridMode;
            Data = data;
        }
    }

    public class R_AfterAddEventArgs : EventArgs
    {
        public object Data { get; set; }

        public R_AfterAddEventArgs(object poData)
        {
            Data = poData;
        }
    }

    public class R_ServiceDeleteEventArgs : EventArgs
    {
        public object Data { get; }

        public R_ServiceDeleteEventArgs(object data)
        {
            Data = data;
        }
    }
}
