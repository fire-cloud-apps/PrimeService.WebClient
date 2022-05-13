using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model.Settings.Tickets;
using PrimeService.Model.Shopping;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class ProductCategoryList
{
    
    #region Variables
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<ProductCategory> pagedData;
    private MudTable<ProductCategory> table;
    private int totalItems;
    private string searchString = null;
    IEnumerable<ProductCategory> _data = new List<ProductCategory>()
    {
        new ProductCategory()
        {
            CategoryName = "PC"
        },
        new ProductCategory()
        {
            CategoryName = "Laptop"
        },
        new ProductCategory()
        {
            CategoryName = "Wires"
        },
        new ProductCategory()
        {
            CategoryName = "Electronics"
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
    private async Task<TableData<ProductCategory>> ServerReload(TableState state)
    {
        IEnumerable<ProductCategory> data = _data;
            //await  _httpClient.GetFromJsonAsync<List<User>>("/public/v2/users");
        await Task.Delay(300);
        data = data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.CategoryName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }).ToArray();
        totalItems = data.Count();
        switch (state.SortLabel)
        {
            case "Name":
                data = data.OrderByDirection(state.SortDirection, o => o.CategoryName);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<ProductCategory>() {TotalItems = totalItems, Items = pagedData};
    }
    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
    #endregion
    
    #region Dialog Open Action
    private async Task OpenDialog(ProductCategory model)
    {
        Console.WriteLine(model.CategoryName);
        await InvokeDialog("_ProductCategory","Product Category", model);
    }
    private async Task OpenAddDialog(MouseEventArgs arg)
    {
        await InvokeDialog("_ProductCategory","Product Category", null);
    }

    private async Task InvokeDialog(string parameter, string title, ProductCategory model)
    {
        var parameters = new DialogParameters
            { [parameter] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<ProductCategoryDialog>(title, parameters, _dialogOptions);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }

    #endregion
}