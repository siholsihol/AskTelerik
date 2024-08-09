using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections;
using System.Collections.ObjectModel;
using Telerik.Blazor.Components;
using TestTelerikGrid.Custom.Columns;
using TestTelerikGrid.Custom.Enums;
using TestTelerikGrid.Custom.Events;
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
        [Parameter] public int PageSize { get; set; } = 10;

        private List<int?> PageSizes { get; set; } = new List<int?> { null, 10, 20, 30, 40, 50 };

        #region Events
        [Parameter] public EventCallback<R_AfterAddEventArgs> R_AfterAdd { get; set; } //for default value when add
        [Parameter] public EventCallback<R_ServiceGetRecordEventArgs> R_ServiceGetRecord { get; set; } //for get new data from db
        [Parameter] public EventCallback<R_ValidationEventArgs> R_Validation { get; set; } //for validation
        [Parameter] public EventCallback<R_ServiceSaveEventArgs> R_ServiceSave { get; set; } //for save 
        [Parameter] public EventCallback<R_ServiceDeleteEventArgs> R_ServiceDelete { get; set; } //for delete 
        [Parameter] public Action<R_CheckBoxSelectRenderEventArgs> R_CheckBoxSelectRender { get; set; }
        [Parameter] public EventCallback<R_DisplayEventArgs> R_Display { get; set; }
        #endregion

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
        private async Task OnAddHandler(GridCommandEventArgs args)
        {
            try
            {
                if (R_AfterAdd.HasDelegate)
                {
                    var loAfterAddEventArgs = new R_AfterAddEventArgs(args.Item);
                    await R_AfterAdd.InvokeAsync(loAfterAddEventArgs);

                    args.Item = Utility.ConvertObjectToObject<TModel>(loAfterAddEventArgs.Data);
                }
            }
            catch (Exception ex)
            {
                args.IsCancelled = true;
                Console.WriteLine(ex.Message);
            }
        }

        private async Task OnCreateHandler(GridCommandEventArgs args)
        {
            try
            {
                var loCurrentDataClone = ((TModel)args.Item).Clone();

                if (R_Validation.HasDelegate)
                {
                    var loValidationEventArgs = new R_ValidationEventArgs(loCurrentDataClone, GridMode.Add);
                    await R_Validation.InvokeAsync(loValidationEventArgs);

                    if (loValidationEventArgs.Cancel)
                    {
                        args.IsCancelled = true;
                        return;
                    }
                }

                var loServiceSaveEventArgs = new R_ServiceSaveEventArgs(loCurrentDataClone, GridMode.Add);
                if (R_ServiceSave.HasDelegate)
                    await R_ServiceSave.InvokeAsync(loServiceSaveEventArgs);

                if (loServiceSaveEventArgs.Result == null)
                    throw new Exception("Entity Result is nothing");

                var loToBeSaveEntity = (TModel)loServiceSaveEventArgs.Result;

                DataSource.Add(loToBeSaveEntity);

                //_columns.ForEach(x => x.Enabled = false);

                await SelectItem(loToBeSaveEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task OnEditHandler(GridCommandEventArgs args)
        {
            try
            {
                var loCurrentDataClone = ((TModel)args.Item).Clone();
                var loServiceGetRecordEventArgs = new R_ServiceGetRecordEventArgs(loCurrentDataClone, null);
                if (R_ServiceGetRecord.HasDelegate)
                    await R_ServiceGetRecord.InvokeAsync(loServiceGetRecordEventArgs);

                if (loServiceGetRecordEventArgs.Result == null)
                    throw new Exception("Data not found.");

                if (loServiceGetRecordEventArgs.Result.GetType() == typeof(IList))
                    throw new Exception("Entity Result cannot contain list.");

                args.Item = Utility.ConvertObjectToObject<TModel>(loServiceGetRecordEventArgs.Result);
            }
            catch (Exception ex)
            {
                args.IsCancelled = true;
                Console.WriteLine(ex.Message);
            }
        }

        private async Task OnUpdateHandler(GridCommandEventArgs args)
        {
            try
            {
                var loCurrentDataClone = ((TModel)args.Item).Clone();

                if (R_Validation.HasDelegate)
                {
                    var loValidationEventArgs = new R_ValidationEventArgs(loCurrentDataClone, GridMode.Edit);
                    await R_Validation.InvokeAsync(loValidationEventArgs);

                    if (loValidationEventArgs.Cancel)
                    {
                        args.IsCancelled = true;
                        return;
                    }
                }

                var loCurrentState = TelerikGridRef.GetState();
                var index = DataSource.IndexOf(loCurrentState.OriginalEditItem);

                var loServiceSaveEventArgs = new R_ServiceSaveEventArgs(loCurrentDataClone, GridMode.Edit);
                if (R_ServiceSave.HasDelegate)
                    await R_ServiceSave.InvokeAsync(loServiceSaveEventArgs);

                if (loServiceSaveEventArgs.Result == null)
                    throw new Exception("Entity Result is nothing");

                var loToBeSaveEntity = (TModel)loServiceSaveEventArgs.Result;

                DataSource[index] = loToBeSaveEntity;

                loCurrentState.InsertedItem = default;
                loCurrentState.EditItem = default;
                await TelerikGridRef.SetStateAsync(loCurrentState);

                await SelectItem(loToBeSaveEntity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task OnDeleteHandler(GridCommandEventArgs args)
        {
            try
            {
                var loCurrentDataClone = ((TModel)args.Item).Clone();
                var loServiceGetRecordEventArgs = new R_ServiceGetRecordEventArgs(loCurrentDataClone, null);
                if (R_ServiceGetRecord.HasDelegate)
                    await R_ServiceGetRecord.InvokeAsync(loServiceGetRecordEventArgs);

                if (loServiceGetRecordEventArgs.Result == null)
                    throw new Exception("Data not found.");

                if (loServiceGetRecordEventArgs.Result.GetType() == typeof(IList))
                    throw new Exception("Entity Result cannot contain list.");

                args.Item = Utility.ConvertObjectToObject<TModel>(loServiceGetRecordEventArgs.Result);
                loCurrentDataClone = ((TModel)args.Item).Clone();

                if (R_ServiceDelete.HasDelegate)
                {
                    var loServiceDeleteEventArgs = new R_ServiceDeleteEventArgs(loCurrentDataClone);
                    await R_ServiceDelete.InvokeAsync(loServiceDeleteEventArgs);
                }

                int liIndex = DataSource.IndexOf((TModel)args.Item);
                if (SelectedItems != null)
                    liIndex = DataSource.IndexOf(SelectedItems.ToList()[0]);

                DataSource.RemoveAt(liIndex);

                var liPreviousIndexData = liIndex - 1;
                if (liPreviousIndexData < 0)
                    liPreviousIndexData = 0;

                if (DataSource.Count > 0)
                {
                    var loPreviousData = DataSource[liPreviousIndexData];
                    await SelectItem(loPreviousData);
                }
                else
                {
                    CurrentSelectedData = default(TModel);
                    CurrentSelectedRowIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task OnCancelHandler(GridCommandEventArgs args)
        {
            try
            {
                var loGridState = TelerikGridRef.GetState();

                loGridState.EditItem = default;
                loGridState.OriginalEditItem = default;
                loGridState.InsertedItem = default;

                await TelerikGridRef.SetStateAsync(loGridState);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task OnRowClick(GridRowClickEventArgs args)
        {
            try
            {
                await SelectItem((TModel)args.Item);

                var loDisplayEventArgs = new R_DisplayEventArgs(CurrentSelectedData, GridMode.Normal);
                if (R_Display.HasDelegate)
                    await R_Display.InvokeAsync(loDisplayEventArgs);

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
                var loCheckboxSelectColumn = _columns.FirstOrDefault(x => x.FieldName == e.Field);
                if (loCheckboxSelectColumn != null)
                {
                    return;
                }

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

        private async Task OnHandleKeyDown(KeyboardEventArgs e)
        {
            try
            {
                if (e.Key == "Delete" && CurrentSelectedData != null)
                {
                    await RaiseOnDelete(CurrentSelectedData);
                }
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

        private async Task RaiseOnDelete(TModel data)
        {
            var args = new GridCommandEventArgs()
            {
                Item = data
            };

            await TelerikGridRef?.OnDelete.InvokeAsync(args);
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

        #region CheckBoxSelectColumn
        private bool? SelectAllCheckBoxValue
        {
            get
            {
                if (DataSource.Count == 0)
                    return false;

                if (IsAllDataSelected())
                    return true;
                else if (IsAnyDataSelected())
                    return null;

                return false;
            }

            set
            {
                if (_columns.Count > 0)
                {
                    var lcFieldName = _columns.FirstOrDefault(x => x.ColumnType == GridColumnType.CheckBoxSelect).FieldName;

                    foreach (var item in DataSource)
                    {
                        item.GetType().GetProperty(lcFieldName).SetValue(item, value);
                    }
                }
            }
        }

        private bool IsAllDataSelected()
        {
            var liSelectedCount = 0;

            if (_columns.Count > 0)
            {
                var lcFieldName = _columns.FirstOrDefault(x => x.ColumnType == GridColumnType.CheckBoxSelect).FieldName;

                foreach (var item in DataSource)
                {
                    var isSelected = Convert.ToBoolean(item.GetType().GetProperty(lcFieldName).GetValue(item, null));

                    if (isSelected)
                        liSelectedCount++;
                }
            }

            return liSelectedCount == DataSource.Count();
        }

        private bool IsAnyDataSelected()
        {
            var liSelectedCount = 0;

            if (_columns.Count > 0)
            {
                var lcFieldName = _columns.FirstOrDefault(x => x.ColumnType == GridColumnType.CheckBoxSelect).FieldName;

                foreach (var item in DataSource)
                {
                    var isSelected = Convert.ToBoolean(item.GetType().GetProperty(lcFieldName).GetValue(item, null));

                    if (isSelected)
                        liSelectedCount++;
                }
            }

            return liSelectedCount > 0 && liSelectedCount < DataSource.Count();
        }

        private void OnRowRenderHandler(GridRowRenderEventArgs args)
        {
            var lcRowClass = string.Empty;

            if (R_CheckBoxSelectRender is not null)
            {
                var loRenderEventArgs = new R_CheckBoxSelectRenderEventArgs(args.Item);
                R_CheckBoxSelectRender.Invoke(loRenderEventArgs);

                lcRowClass = loRenderEventArgs.Enabled ? lcRowClass : lcRowClass + " " + "unselectable-row";
            }

            args.Class = lcRowClass;
        }
        #endregion
    }
}
