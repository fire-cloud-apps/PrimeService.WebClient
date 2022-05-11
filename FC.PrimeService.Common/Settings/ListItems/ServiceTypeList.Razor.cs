using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Tickets;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class ServiceTypeList
{
    
    #region Variables
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<ServiceType> pagedData;
    private MudTable<ServiceType> table;
    private int totalItems;
    private string searchString = null;
    IEnumerable<ServiceType> _data = new List<ServiceType>()
    {
        new ServiceType()
        {
            Price = 100,
            Cost = 10,
            Title = "Laptop Repair",
            Warranty = 30,
            Category = new ServiceCategory()
            {
                CategoryName = "Repair"
            }
        },
        new ServiceType()
        {
            Price = 300,
            Cost = 20,
            Title = "Desktop Repair",
            Warranty = 60,
            Category = new ServiceCategory()
            {
                CategoryName = "Repair"
            }
        },
        new ServiceType()
        {
            Price = 150,
            Cost = 15,
            Title = "Mobile Service",
            Warranty = 45,
            Category = new ServiceCategory()
            {
                CategoryName = "Service"
            }
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
    private async Task<TableData<ServiceType>> ServerReload(TableState state)
    {
        IEnumerable<ServiceType> data = _data;
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
            case "Price":
                data = data.OrderByDirection(state.SortDirection, o => o.Price);
                break;
            case "Category":
                data = data.OrderByDirection(state.SortDirection, o => o.Category?.CategoryName);
                break;
            default:
                data = data.OrderByDirection(state.SortDirection, o => o.Price);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<ServiceType>() {TotalItems = totalItems, Items = pagedData};
    }
    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
    #endregion
    
    #region Dialog Open Action
    private async Task OpenDialog(ServiceType model)
    {
        Console.WriteLine(model.Title);
        await InvokeDialog("_ServiceType","Service Type", model);
    }
    private async Task OpenAddDialog(MouseEventArgs arg)
    {
        await InvokeDialog("_ServiceType","Service Type", null);
    }

    private async Task InvokeDialog(string parameter, string title, ServiceType model)
    {
        var parameters = new DialogParameters
            { [parameter] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<ServiceTypeDialog>(title, parameters, _dialogOptions);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }

    #endregion
}