using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model.Settings;
using PrimeService.Model.Settings.Tickets;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class TaxList
{
     
    #region Variables
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    bool success;
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<Tax> pagedData;
    private MudTable<Tax> table;
    private int totalItems;
    private string searchString = null;
    //Ref: https://www.paisabazaar.com/tax/gst-rates/
    IEnumerable<Tax> _data = new List<Tax>()
    {
        new Tax()
        {
            Title = "0.00% - No Tax",
            TaxRate = 0.0f,
            Description = "No tax applied for the goods."
        },
        new Tax()
        {
            Title = "0.25% - Tax",
            TaxRate = 0.25f,
            Description = "Cut and semi-polished stones are included under this tax slab."
        },
        new Tax()
        {
            Title = "5% - Tax",
            TaxRate = 5.0f,
            Description = "Household necessities such as edible oil, sugar, spices, tea, and coffee (except instant) are included. Coal, Mishti/Mithai (Indian Sweets) and Life-saving drugs are also covered under this GST slab."
        },
        new Tax()
        {
            Title = "12% - Tax",
            TaxRate = 12.0f,
            Description = "This includes computers and processed food."
        },
        new Tax()
        {
            Title = "18% - Tax",
            TaxRate = 18.0f,
            Description = "Hair oil, toothpaste and soaps, capital goods and industrial intermediaries are covered in this slab."
        },
        new Tax()
        {
            Title = "28% - Tax",
            TaxRate = 28.0f,
            Description = "Luxury items such as small cars, consumer durables like AC and Refrigerators, premium cars, cigarettes and aerated drinks, High-end motorcycles are included here."
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
    private async Task<TableData<Tax>> ServerReload(TableState state)
    {
        IEnumerable<Tax> data = _data;
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
            case "Percentage":
                data = data.OrderByDirection(state.SortDirection, o => o.TaxRate);
                break;
            default:
                data = data.OrderByDirection(state.SortDirection, o => o.TaxRate);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Tax>() {TotalItems = totalItems, Items = pagedData};
    }
    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
    #endregion
    
    #region Dialog Open Action
    private async Task OpenDialog(Tax model)
    {
        Console.WriteLine(model.Title);
        await InvokeDialog("_Tax","Tax Category", model);
    }
    private async Task OpenAddDialog(MouseEventArgs arg)
    {
        await InvokeDialog("_Tax","Tax Category", null);
    }

    private async Task InvokeDialog(string parameter, string title, Tax model)
    {
        var parameters = new DialogParameters
            { [parameter] = model }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<TaxDialog>(title, parameters, _dialogOptions);
        var result = await dialog.Result;

        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }

    #endregion
}