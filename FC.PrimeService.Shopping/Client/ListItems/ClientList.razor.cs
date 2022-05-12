using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model.Settings.Forms;
using PrimeService.Model.Settings.Payments;
using Model = PrimeService.Model.Shopping;

namespace FC.PrimeService.Shopping.Client.ListItems;

public partial class ClientList
{
       
    #region Variables
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<Model.Client> pagedData;
    private MudTable<Model.Client> table;
    private int totalItems;
    private string searchString = null;
    IEnumerable<Model.Client> _data = new List<Model.Client>()
    {
        new Model.Client()
        {
            Id = "6270d1cce5452d9169e86c50",
            Mobile = "8589696623",
            Name = "SRG",
            Note = "Some Data",
            Type = new ClientType(){Title = "Individual"}
        },
        new Model.Client()
        {
            Id = "6270d1cce5452d9169e86c60",
            Mobile = "7485965612",
            Name = "ZoZo",
            Note = "Company Details",
            Type = new ClientType(){Title = "Company"}
        },
        new Model.Client()
        {
            Id = "6270d1cce5452d9169e86c51",
            Mobile = "78599312363",
            Name = "Assembly",
            Note = "Company Details",
            Type = new ClientType(){Title = "Company"}
        },
        new Model.Client()
        {
            Id = "6270d1cce5452d9169e86c52",
            Mobile = "0448956363",
            Name = "HCL",
            Note = "Company Details",
            Type = new ClientType(){Title = "Company"}
        },

    };
    
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    private TableGroupDefinition<Model.Client> _groupDefinition = new()
    {
        GroupName = "Type",
        Indentation = false,
        Expandable = false,
        Selector = (e) => e.Type.Title
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
    private async Task<TableData<Model.Client>> ServerReload(TableState state)
    {
        IEnumerable<Model.Client> data = _data;
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
            default:
                data = data.OrderByDirection(state.SortDirection, o => o.Name);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.Client>() {TotalItems = totalItems, Items = pagedData};
    }
    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
    #endregion
    
    #region Add Action
    private async Task AddAction(MouseEventArgs arg)
    {
        _navigationManager.NavigateTo("/Action/?Component=Client");
    }
    #endregion
}