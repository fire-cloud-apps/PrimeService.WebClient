# Select Box

## Select from Enum

```html
<MudSelect Variant="Variant.Outlined"
         Label="Service/Ticket Type" class="pa-0"
         @bind-Value="_inputMode.TicketType"
         OpenIcon="@Icons.Material.TwoTone.Tab" AdornmentColor="Color.Secondary">
  @foreach (TicketType item in Enum.GetValues(typeof(TicketType)))
  {
      <MudSelectItem Value="@item">@item</MudSelectItem>
  }
</MudSelect>
```
