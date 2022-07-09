# Table

## Table Integration

<details>

<summary>Code Behind - Data Load</summary>

```csharp
    #region Grid View
    /// <summary>
    /// Used to Refresh Table data.
    /// </summary>
    private MudTable<Model.TicketService>? _mudTable;
    
    /// <summary>
    /// To do Ajax Search in the 'MudTable'
    /// </summary>
    private string _searchString = string.Empty;
    private string _searchField = "Name";
    /// <summary>
    /// Server Side pagination with, filtered and ordered data from the API Service.
    /// </summary>
    private async Task<TableData<Model.TicketService>> ServerReload(TableState state)
    {
        #region Ajax Call to Get data by Batch
        var responseModel = await GetDataByBatch(state);
        #endregion
        
        Utilities.ConsoleMessage($"Table State : {JsonSerializer.Serialize(state)}");
        return new TableData<Model.TicketService>() {TotalItems = responseModel.TotalItems, Items = responseModel.Items};
    }
    
    /// <summary>
    /// Do Ajax call to get 'TicketService' Data
    /// </summary>
    /// <param name="state">Current Table State</param>
    /// <returns>Sales Data.</returns>
    private async Task<ResponseData<Model.TicketService>> GetDataByBatch(TableState state)
    {
        string url = $"{_appSettings.App.ServiceUrl}{_appSettings.API.TicketApi.GetBatch}";
        PageMetaData pageMetaData = new PageMetaData()
        {
            SearchText = _searchString,
            Page = state.Page,
            PageSize = state.PageSize,
            SortLabel = (string.IsNullOrEmpty(state.SortLabel)) ? "Name" : state.SortLabel,
            SearchField = _searchField,
            SortDirection = (state.SortDirection == SortDirection.Ascending) ? "A" : "D"
        };
        var responseModel = await _httpService.POST<ResponseData<Model.TicketService>>(url, pageMetaData);
        return responseModel;
    }

    private void OnSearch(string text, string field = "TicketNo")
    {
        _searchString = text;
        _searchField = field;
        _mudTable?.ReloadServerData();//If we put Async, Loading progress bar is not closing.
        StateHasChanged();
    }
    #endregion
    
```

</details>

<details>

<summary>UI-Code</summary>

```html
<MudTable ServerData="@(new Func<TableState, Task<TableData<TicketService>>>(ServerReload))"
				  Dense="true" Hover="true" @ref="_mudTable"
				  GroupBy="@_groupDefinition"
				  GroupHeaderStyle="background-color:var(--mud-palette-background-grey)"
				  GroupFooterClass="mb-4">
			<ColGroup>
				@if (_groupDefinition.Expandable)
				{
					<col style="width: 60px;"/>
				}
				<col/>
				<col/>
				<col/>
				<col/>
				<col/>
			</ColGroup>
			<ToolBarContent>
				<MudTextField T="string" ValueChanged="@(s => OnSearch(s))"
							  FullWidth="true"
							  Placeholder="Search" Adornment="Adornment.Start"
							  AdornmentIcon="@Icons.Material.Filled.Search"
							  IconSize="Size.Medium" Class="mt-0"></MudTextField>
			</ToolBarContent>
	<HeaderContent>
		<MudTh><MudTableSortLabel SortLabel="Date" T="Sales">Date</MudTableSortLabel></MudTh>
		<MudTh><MudTableSortLabel T="Sales">Bill#</MudTableSortLabel></MudTh>
		<MudTh><MudTableSortLabel T="Sales">Items</MudTableSortLabel></MudTh>
		<MudTh><MudTableSortLabel SortLabel="Quantity" T="Sales">Quantity</MudTableSortLabel></MudTh>
		<MudTh><MudTableSortLabel T="Sales">Price</MudTableSortLabel></MudTh>
	</HeaderContent>
	<GroupHeaderTemplate>
		<MudTh Class="mud-table-cell-custom-group" colspan="5">@($"{context.GroupName}: {context.Key}")</MudTh>
	</GroupHeaderTemplate>
	<RowTemplate>
		<MudTd DataLabel="Date">
			@context.CreatedDate
		</MudTd>
		<MudTd DataLabel="Bill#">

			<MudTooltip Color="Color.Tertiary" Arrow="true">
				<ChildContent>
					<MudLink Href=@($"/Ticket/?viewId=Ticket&Id={@context.Id}")
							 Target="_blank">
						@context.TicketNo
					</MudLink>
				</ChildContent>

				<TooltipContent>
					<MudText Typo="Typo.body2"> Edit - '@context.Reasons' </MudText>
				</TooltipContent>
			</MudTooltip>
		</MudTd>
		<MudTd DataLabel="Reason">
			<MudTooltip Arrow="true" Text="@context.Reasons" >
				 @(context.Reasons.Substring(0, 10) + "...")
			</MudTooltip>
		   
		</MudTd>
		<MudTd DataLabel="Balance">
			@context.BalanceAmount
		</MudTd>
		<MudTd DataLabel="Total">
			@context.TotalAmount
		</MudTd>
	</RowTemplate>
			<NoRecordsContent>
				<MudText>No matching records found</MudText>
			</NoRecordsContent>
			<LoadingContent>
				<MudText>Loading...</MudText>
			</LoadingContent>
			<PagerContent>
				<MudTablePager/>
			</PagerContent>
</MudTable>
```

</details>
