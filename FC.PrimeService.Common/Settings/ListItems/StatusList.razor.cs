using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Tickets;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class StatusList
{
    
    #region Variables
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<Status> pagedData;
    private MudTable<Status> table;
    private int totalItems;
    private string searchString = null;
    IEnumerable<Status> _data = new List<Status>()
    {
        new Status()
        {
            Name = "New"
        },
        new Status()
        {
            Name = "In Progress"
        },
        new Status()
        {
            Name = "Pending"
        },
        new Status()
        {
            Name = "Waiting Parts"
        },
        new Status()
        {
            Name = "Done"
        },
        new Status()
        {
            Name = "Closed"
        },
        new Status()
        {
            Name = "Cancelled"
        },
    };
    
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    #endregion

    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await  Task.Delay(2000);
        //An Ajax call to get company details
        
        _loading = false;
        StateHasChanged();
    }
    #endregion

    #region Grid View
    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<Status>> ServerReload(TableState state)
    {
        IEnumerable<Status> data = _data;
            //await  _httpClient.GetFromJsonAsync<List<User>>("/public/v2/users");
        await Task.Delay(300);
        data = data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }).ToArray();
        totalItems = data.Count();
        switch (state.SortLabel)
        {
            case "Name":
                data = data.OrderByDirection(state.SortDirection, o => o.Name);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Status>() {TotalItems = totalItems, Items = pagedData};
    }
    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
    #endregion
    
    #region Dialog Open Action
    private async Task OpenDialog(Status status)
    {
        Console.WriteLine(status.Name);
        await InvokeDialog("_status","Status", status);
    }
    private async Task OpenAddDialog(MouseEventArgs arg)
    {
        await InvokeDialog("_status","Status", null);
    }

    private async Task InvokeDialog(string parameter, string title, Status status)
    {
        var parameters = new DialogParameters
            { [parameter] = status }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<StatusDialog>(title, parameters, _dialogOptions);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }

    #endregion

    
}