using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class ClientTypeList
{
        #region Variables
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<ClientType> pagedData;
    private MudTable<ClientType> table;
    private int totalItems;
    private string searchString = null;
    IEnumerable<ClientType> _data = new List<ClientType>()
    {
        new ClientType()
        {
            Title = "Person"
        },
        new ClientType()
        {
            Title = "Organization"
        },
        new ClientType()
        {
            Title = "NGO"
        },
        new ClientType()
        {
            Title = "Government"
        },
    };
    
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.ExtraSmall,
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
    private async Task<TableData<ClientType>> ServerReload(TableState state)
    {
        IEnumerable<ClientType> data = _data;
            //await  _httpClient.GetFromJsonAsync<List<User>>("/public/v2/users");
        await Task.Delay(300);
        data = data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }).ToArray();
        totalItems = data.Count();
        switch (state.SortLabel)
        {
            case "Title":
                data = data.OrderByDirection(state.SortDirection, o => o.Title);
                break;
            default:
                data = data.OrderByDirection(state.SortDirection, o => o.Title);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<ClientType>() {TotalItems = totalItems, Items = pagedData};
    }
    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
    #endregion
    
    #region Dialog Open Action
    private async Task OpenDialog(ClientType model)
    {
        Console.WriteLine(model.Title);
        await InvokeDialog(model);
    }
    private async Task OpenAddDialog(MouseEventArgs arg)
    {
        await InvokeDialog(null);
    }

    private async Task InvokeDialog(ClientType model)
    {
        var parameters = new DialogParameters
            { ["_ClientType"] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<ClientTypeDialog>("Client Type", parameters, _dialogOptions);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }

    #endregion
}