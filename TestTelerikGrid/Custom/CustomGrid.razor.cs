using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;
using Telerik.Blazor.Components;
using TestTelerikGrid.Custom.Columns;
using TestTelerikGrid.Helper;

namespace TestTelerikGrid.Custom
{
    public partial class CustomGrid<TModel> : ComponentBase, ICustomGrid
    {
        [Parameter] public ObservableCollection<TModel> DataSource { get; set; } = new ObservableCollection<TModel>();
        [Parameter] public string Id { get; set; } = Utility.GenerateComponentId("grid");
        [Parameter] public string AddNewRowText { get; set; } = "Add New";
        [Parameter] public RenderFragment GridColumnsContent { get; set; }
        [Parameter] public string Width { get; set; } = "100%";
        [Parameter] public string Height { get; set; } = string.Empty;
        [Parameter] public decimal RowHeight { get; set; } = 0;

        public IEnumerable<TModel> SelectedItems { get; set; } = Enumerable.Empty<TModel>();
        private List<BaseCustomColumn> _columns = new List<BaseCustomColumn>();
        private TelerikGrid<TModel> TelerikGridRef;

        public void AddColumn(BaseCustomColumn column)
        {
            _columns.Add(column);
        }

        #region Public Properties
        public int CurrentSelectedRowIndex { get; private set; }
        public TModel CurrentSelectedData { get; private set; }
        #endregion

        #region Private Methods
        private void OnCreateHandler(GridCommandEventArgs args)
        {
            Console.WriteLine("Create event is fired.");
        }

        private void OnEditHandler(GridCommandEventArgs args)
        {
            Console.WriteLine("Edit event is fired.");
        }

        private void OnUpdateHandler(GridCommandEventArgs args)
        {
            Console.WriteLine("Update event is fired.");
        }

        private void OnDeleteHandler(GridCommandEventArgs args)
        {
            Console.WriteLine("Delete event is fired.");
        }

        private void OnCancelHandler(GridCommandEventArgs args)
        {
            Console.WriteLine("Cancel event is fired.");
        }

        private async Task OnRowClick(GridRowClickEventArgs args)
        {
            try
            {
                await SelectItem((TModel)args.Item);

                args.ShouldRender = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task OnRowDoubleClick(GridRowClickEventArgs e)
        {
            try
            {
                await RaiseOnEdit((TModel)e.Item);

                var loCurrentDataClone = ((TModel)e.Item).Clone();
                var loCurrentState = TelerikGridRef.GetState();
                loCurrentState.InsertedItem = default;

                loCurrentState.EditItem = loCurrentDataClone;
                loCurrentState.OriginalEditItem = (TModel)e.Item;

                await TelerikGridRef.SetStateAsync(loCurrentState);

                e.ShouldRender = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task RaiseOnEdit(TModel data)
        {
            var args = new GridCommandEventArgs()
            {
                Item = data
            };

            await TelerikGridRef.OnEdit.InvokeAsync(args);
        }

        private void SetSelectedItems(TModel poData)
        {
            SelectedItems = Enumerable.Empty<TModel>();
            SelectedItems = SelectedItems.Add(poData);
        }
        #endregion

        #region Public Methods
        public async Task SelectItem(TModel poItem)
        {
            await SelectItem(DataSource.IndexOf(poItem));
        }

        public Task SelectItem(int piRowIndex, ObservableCollection<TModel> poList = null)
        {
            try
            {
                if (piRowIndex == -1) return Task.CompletedTask;

                var loDataList = poList != null ? poList : DataSource;

                var loTargetSelectedItem = loDataList[piRowIndex];

                CurrentSelectedData = loTargetSelectedItem;
                CurrentSelectedRowIndex = loDataList.IndexOf(CurrentSelectedData);
                SetSelectedItems(CurrentSelectedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}
