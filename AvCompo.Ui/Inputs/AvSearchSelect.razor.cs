namespace AvCompo.Ui;
public partial class AvSearchSelect<TItem, TValue>
{
    public Guid Id = Guid.NewGuid();
    public int HighlightedIndex = -1;
    public bool IsDropdownVisible = false;

    [Parameter] public IEnumerable<TItem> Items { get; set; } = Enumerable.Empty<TItem>();
    [Parameter] public string TextProperty { get; set; } = string.Empty;
    [Parameter] public string ValueProperty { get; set; } = string.Empty;
    [Parameter] public string Placeholder { get; set; } = string.Empty;
    [Parameter] public string Width { get; set; } = "200px";
    [Parameter] public TValue? Value { get; set; }
    [Parameter] public EventCallback<TValue?> ValueChanged { get; set; }

    [Inject] public IJSRuntime JSRuntime { get; set; }

    public string? SearchText { get; set; }
    public ElementReference SearchInputRef { get; set; }
    public List<TItem> FilteredItems => SearchText.HasNoValue() ? Items.ToList()
                                                                : Items.Where(item => GetItemText(item).Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("informElementOutsideClick", DotNetObjectReference.Create(this), $"search-select-{Id}");
        }
    }

    public string GetItemText(TItem item)
    {
        if (TextProperty.HasNoValue())
        {
            return item?.ToString() ?? string.Empty;
        }

        var property = typeof(TItem).GetProperty(TextProperty);

        return property?.GetValue(item)?.ToString() ?? string.Empty;
    }

    public TValue? GetItemValue(TItem item)
    {
        if (ValueProperty.HasNoValue())
        {
            return default!;
        }

        var property = typeof(TItem).GetProperty(ValueProperty);

        return (TValue?)property?.GetValue(item) ?? default!;
    }

    public async Task OnClearItemSelect()
    {
        Value = default!;

        SearchText = string.Empty;

        HighlightedIndex = -1;

        await ValueChanged.InvokeAsync(Value);

        IsDropdownVisible = false;
    }

    public async Task SelectItem(TItem item, int index)
    {
        Value = GetItemValue(item);

        HighlightedIndex = index;

        await ValueChanged.InvokeAsync(Value);

        IsDropdownVisible = false;
    }

    public async Task OnSelectBoxClick()
    {
        await ToggleDropdown();
    }

    public async Task ToggleDropdown(bool? shouldOpen = null)
    {
        if (shouldOpen is not null)
        {
            IsDropdownVisible = shouldOpen.Value;
        }
        else
        {
            IsDropdownVisible = !IsDropdownVisible;
        }

        if (IsDropdownVisible)
        {
            if (Value is null)
            {
                HighlightedIndex = -1;
            }

            await Task.Delay(500);

            try
            {
                await SearchInputRef.FocusAsync();
            }
            catch (Exception e)
            {
            }
        }
    }

    public async Task OnSearchTextChanged(ChangeEventArgs e)
    {
        SearchText = e.Value?.ToString();

        HighlightedIndex = -1;
    }

    public async Task OnSearchTextFocus()
    {
        IsDropdownVisible = true;
    }

    [JSInvokable]
    public void InformElementOutsideClick()
    {
        IsDropdownVisible = false;

        StateHasChanged();
    }

    public async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (!IsDropdownVisible)
        {
            return;
        }

        switch (e.Key)
        {
            case "ArrowDown":
                HighlightedIndex = (HighlightedIndex+1) % FilteredItems.Count;
                await ScrollToHighlightedDropDownItem();
                break;
            case "ArrowUp":
                HighlightedIndex = (HighlightedIndex - 1 + FilteredItems.Count) % FilteredItems.Count;
                await ScrollToHighlightedDropDownItem();
                break;
            case "Enter":
                if (HighlightedIndex >= 0 && HighlightedIndex < FilteredItems.Count)
                {
                    await SelectItem(FilteredItems[HighlightedIndex], HighlightedIndex);
                }
                break;
        }
    }

    public async Task ScrollToHighlightedDropDownItem()
    {
        await JSRuntime.InvokeVoidAsync("scrollToHighlightedDropDownItem", $"search-select-{Id}", HighlightedIndex);
    }
}