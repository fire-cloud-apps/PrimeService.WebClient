using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model.Settings.Payments;
using PrimeService.Model.Settings.Tickets;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class PaymentTagList
{
        
    #region Variables
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<PaymentTags> pagedData;
    private MudTable<PaymentTags> table;
    private int totalItems;
    private string searchString = null;
    IEnumerable<PaymentTags> _data = new List<PaymentTags>()
    {
        new PaymentTags()
        {
            Category =  PaymentCategory.Income,
            Amount = 10000,
            Title = "Sales Account"
        },
        new PaymentTags()
        {
            Category =  PaymentCategory.Income,
            Amount = 50000,
            Title = "Service Account"
        },
        new PaymentTags()
        {
            Category =  PaymentCategory.Expense,
            Amount = 5500,
            Title = "Employee Salary Account"
        },
        new PaymentTags()
        {
            Category =  PaymentCategory.Expense,
            Amount = 5000,
            Title = "Purchase Account"
        },
       
        
    };
    
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
    };
    
    private TableGroupDefinition<PaymentTags> _groupDefinition = new()
    {
        GroupName = "Category",
        Indentation = false,
        Expandable = false,
        Selector = (e) => e.Category
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
    private async Task<TableData<PaymentTags>> ServerReload(TableState state)
    {
        IEnumerable<PaymentTags> data = _data;
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
            case "InitialFund":
                data = data.OrderByDirection(state.SortDirection, o => o.Amount);
                break;
            default:
                data = data.OrderByDirection(state.SortDirection, o => o.Title);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<PaymentTags>() {TotalItems = totalItems, Items = pagedData};
    }
    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
    #endregion
    
    #region Dialog Open Action
    private async Task OpenDialog(PaymentTags model)
    {
        Console.WriteLine(model.Title);
        await InvokeDialog(model);
    }
    private async Task OpenAddDialog(MouseEventArgs arg)
    {
        await InvokeDialog(null);
    }

    private async Task InvokeDialog(PaymentTags model)
    {
        var parameters = new DialogParameters
            { ["_PaymentTags"] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<PaymentTagDialog>("Payment Account", parameters, _dialogOptions);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }

    #endregion
}