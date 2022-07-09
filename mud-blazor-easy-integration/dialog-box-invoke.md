# Dialog Box - Invoke

## Dialog-Invoke

```html
<MudToolBar>
	<MudTooltip Text="Add Income" Arrow="true" Placement="Placement.Bottom">
		<MudFab Icon="@Icons.Material.Outlined.Add"
				Color="Color.Success" 
				Size="Size.Small"
				OnClick="AddIncome"  />
	</MudTooltip>
	<MudTooltip Text="Add Expense" Arrow="true" Placement="Placement.Bottom">
		<MudIconButton Icon="@Icons.Material.Outlined.Remove"
					   Color="Color.Secondary"
					   OnClick="AddExpense" />
	</MudTooltip>
</MudToolBar>
```

```csharp
    #region Invoke Payment Dialog

    private async Task AddIncome()
    {
        await InvokeDialog("Add Income",UserAction.ADD, new Model.Payments()
        {
            PaymentCategory = PaymentCategory.Income,
            Who = _loginUser
        });//Null indicates its an 'Add' Mode.
    }
    private async Task AddExpense()
    {
        await InvokeDialog("Add Expense",UserAction.ADD, new Model.Payments()
        {
            PaymentCategory = PaymentCategory.Expense,
            Who = _loginUser,
        });//Null indicates its an 'Add' Mode.
    }
    
    private async Task InvokeDialog(string title, 
        UserAction action = UserAction.ADD, Model.Payments model = null)
    {
        var parameters = new DialogParameters
        {
            ["Payments"] = model,
            ["UserAction"] =  action as object,
            ["Title"] = title
        }; //'null' indicates that the Dialog should open in 'Add' Mode.
        var dialog = DialogService.Show<PaymentDialog>(title, parameters, _dialogOptions);
        var result = await dialog.Result;
        
        if (result.Cancelled)
        {
            Utilities.ConsoleMessage("Cancelled.");
            OnSearch(string.Empty);
        }
        else
        {
            Guid.TryParse(result.Data.ToString(), out Guid deletedServer);
            Utilities.ConsoleMessage("Executed.");
            OnSearch(string.Empty);//Reload the server grid.
        }
    }

    #endregion#
```
