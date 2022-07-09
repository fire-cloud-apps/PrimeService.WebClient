# Auto-Complete

## Auto-Complete

<details>

<summary>UI  Code</summary>

```html
 <MudAutocomplete T="Client" Label="Find Customer"
           Disabled="@_isReadOnly" HelperText="Find Customer by Mobile"
           @bind-Value="_inputMode.Client"
           OpenIcon="@Icons.Material.TwoTone.Search"
           ToStringFunc="@(e => e == null ? null : $"{e.Mobile}")"
           SearchFunc="@Client_SearchAsync" Immediate="true" Required="true"
      RequiredError="Please Select Customer" ResetValueOnEmptyText="false"
      CloseIcon="@Icons.Material.Filled.Search" AdornmentColor="Color.Tertiary"/>
```

</details>

<details>

<summary>Code-Behind</summary>

{% code title="Utilities.cs" %}
```csharp
    public static async Task<ResponseData<Client>> GetClients(AppSettings appSettings, IHttpService httpService, string searchValue = "", string searchField="Mobile")
    {
        string url= $"{appSettings.App.ServiceUrl}{appSettings.API.ClientApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = (string.IsNullOrEmpty(searchValue)) ? string.Empty : searchValue,
            Page = 0,
            PageSize = 10,
            SortLabel = "Name",
            SearchField = searchField,
            SortDirection = "A"
        };
        var responseModel = await httpService.POST<ResponseData<Client>>(url, pageMetaData);
        return responseModel;
    }
```
{% endcode %}

{% code title="Codebehind.razor.cs" %}
```csharp
    #region Client Search - Autocomplete
    private IEnumerable<Shop.Client> _clients = new List<Shop.Client>();
    async Task<IEnumerable<Shop.Client>> Client_SearchAsync(string value)
    {
        var responseData = await Utilities.GetClients(_appSettings, _httpService, value);
        _clients = responseData.Items;
        Console.WriteLine($"Find Payment Tags : '{value}'" );
        return _clients;
    }
    #endregion

    
    
```
{% endcode %}

</details>
