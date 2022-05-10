using System.Text.Json;
using FC.PrimeService.Common.Settings.Dialog;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using PrimeService.Model;
using PrimeService.Model.Settings;

namespace FC.PrimeService.Common.Settings.ListItems;

public partial class EmployeeList
{
    #region Initialization
    [Inject] ISnackbar Snackbar { get; set; }
    MudForm form;
    private bool _loading = false;
    private Employee _inputMode;
    bool success;
    string[] errors = { };
    string _outputJson;
    private bool _processing = false;
    private bool _isReadOnly = true;
    private IEnumerable<Employee> pagedData;
    private MudTable<Employee> table;

    private int totalItems;
    private string searchString = null;
    private DialogOptions _dialogOptions = new DialogOptions()
    {
        MaxWidth = MaxWidth.Large, 
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true, 
        
    };
    #endregion
    
    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await  Task.Delay(2000);
        //An Ajax call to get company details
        //for now it is filled as Static value
        _inputMode = new Employee()
        {
            User = new User()
            {
                Name = "SRG",
                Email = "srg@gmail.com"
            },
            WorkLocation = new WorkLocation()
            {
                Title = "Main Location"
            },
            Mobile = "9895609696",
            Salary = 15000
        };
        _loading = false;
        StateHasChanged();
    }
    
    /// <summary>
    /// Here we simulate getting the paged, filtered and ordered data from the server
    /// </summary>
    private async Task<TableData<Employee>> ServerReload(TableState state)
    {
        IEnumerable<Employee> data = new List<Employee>()
        {
           
        };
            //await  _httpClient.GetFromJsonAsync<List<User>>("/public/v2/users");
        await Task.Delay(300);
        data = data.Where(element =>
        {
            if (string.IsNullOrWhiteSpace(searchString))
                return true;
            if (element.User.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (element.Mobile.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                return true;
           
            return false;
        }).ToArray();
        totalItems = data.Count();
        switch (state.SortLabel)
        {
            case "Name":
                data = data.OrderByDirection(state.SortDirection, o => o.User.Name);
                break;
            case "Mobile":
                data = data.OrderByDirection(state.SortDirection, o => o.Mobile);
                break;
            case "Work Location":
                data = data.OrderByDirection(state.SortDirection, o => o.WorkLocation.Title);
                break;
        }
        
        pagedData = data.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();
        Console.WriteLine($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Employee>() {TotalItems = totalItems, Items = pagedData};
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }

    #region Dialog Open Action
    private async Task OpenDialog(Employee workLocation)
    {
        Console.WriteLine(JsonSerializer.Serialize(workLocation));
        var parameters = new DialogParameters { ["_employee"] = workLocation };
        var dialog = DialogService.Show<EmployeeDialog>("Employee", parameters, options:_dialogOptions);
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
        var parameters = new DialogParameters { ["_employee"] = null };//'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<EmployeeDialog>("Employee", parameters, options:_dialogOptions);
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