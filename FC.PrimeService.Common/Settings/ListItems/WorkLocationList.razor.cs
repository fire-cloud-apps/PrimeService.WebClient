using System.Net.Http.Json;
using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MongoDB.Bson;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;


namespace FC.PrimeService.Common.Settings.ListItems;

public partial class WorkLocationList
{

    #region Variables
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    private WorkLocation _inputMode;
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<WorkLocation> pagedData;
    private MudTable<WorkLocation> table;

    private int totalItems;
    private string searchString = null;
    #endregion

    #region Initialization Load
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await  Task.Delay(2000);
        //An Ajax call to get company details
        //for now it is filled as Static value
        _inputMode = new WorkLocation()
        {
            Id = "6270d1cce5452d9169e86c50",
            Title = "Main Location",
            Address = "No:234, Mail Box, Chennai, TN, India.",
            Phone = "91 96562 22336"
        };
        _loading = false;
        StateHasChanged();
    }
    #endregion

    #region Grid View
    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<WorkLocation>> ServerReload(TableState state)
    {
        IEnumerable<WorkLocation> data = new List<WorkLocation>()
        {
            new WorkLocation()
            {
                Address = "123, Address, Chennai",
                Phone = "78452 96963",
                Title = "Main Location"
            },
            new WorkLocation()
            {
                Address = "26, Address, Chennai",
                Phone = "528452 96963",
                Title = "Secondary Location"
            },
        };
            //await  _httpClient.GetFromJsonAsync<List<User>>("/public/v2/users");
        await Task.Delay(300);
        data = data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Phone.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if ($"{element.Id} {element.Phone} {element.Title}".Contains(searchString))
                return true;
            return false;
        }).ToArray();
        totalItems = data.Count();
        switch (state.SortLabel)
        {
            case "Title":
                data = data.OrderByDirection(state.SortDirection, o => o.Title);
                break;
            case "Phone":
                data = data.OrderByDirection(state.SortDirection, o => o.Phone);
                break;
            case "Address":
                data = data.OrderByDirection(state.SortDirection, o => o.Address);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<WorkLocation>() {TotalItems = totalItems, Items = pagedData};
    }
    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }
    #endregion
    
    #region Dialog Open Action
    private async Task OpenDialog(WorkLocation workLocation)
    {
        Console.WriteLine(JsonSerializer.Serialize(workLocation));
        var parameters = new DialogParameters { ["_WorkLocation"] = workLocation };
        var dialog = DialogService.Show<WorkLocationDialog>("Work Location", parameters);
        var result = await dialog.Result;
        
        if (!result.Cancelled)
        {
            //In a real world scenario we would reload the data from the source here since we "removed" it in the dialog already.
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
            //Servers.RemoveAll(item => item.Id == deletedServer);
        }
    }
    private async Task OpenAddDialog(MouseEventArgs arg)
    {
        var parameters = new DialogParameters { ["_WorkLocation"] = null };//'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<WorkLocationDialog>("Work Location", parameters);
        var result = await dialog.Result;
        
        if (!result.Cancelled)
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
        }
    }
    

    #endregion

    #region Submit Button with Animation
    async Task ProcessSomething()
    {
        _processing = true;
        await Task.Delay(2000);
        _processing = false;
    }
    private async Task Submit()
    {
        await form.Validate();

        if (form.IsValid)
        {
            // //Todo some animation.
            await ProcessSomething();
            
            //Do server actions.
            _outputJson = JsonSerializer.Serialize(_inputMode);

            //Success Message
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomRight;
            Snackbar.Configuration.SnackbarVariant = Variant.Filled;
            //Snackbar.Configuration.VisibleStateDuration  = 2000;
            //Can also be done as global configuration. Ref:
            //https://mudblazor.com/components/snackbar#7f855ced-a24b-4d17-87fc-caf9396096a5
            Snackbar.Add("Submited!", Severity.Success);
        }
        else
        {
            _outputJson = "Validation Error occured.";
            Console.WriteLine(_outputJson);
        }
    }
    #endregion
    
}