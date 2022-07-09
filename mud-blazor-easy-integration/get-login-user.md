# Get - Login User

Get Login User from Local Storage

```csharp
//Global Variable
private AuditUser _loginUser = null;

#region Initialization Load
protected override async Task OnInitializedAsync()
{
	//some code.
	_loginUser = await Utilities.GetLoginUser(_localStore);	
}
#endregion

```

{% hint style="danger" %}
Important Notes

* This Local Store works only on the Razor Page and does not works on the Dialog Box Load.
* `@inject ILocalStorageService`` `_`localStore comes from the`_` ``__Imports.razor`
{% endhint %}
