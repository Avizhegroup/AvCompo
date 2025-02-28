namespace AvCompo.Ui;
public partial class AvMaterialIcon
{
    public Guid Id = Guid.NewGuid();

    [Parameter] public string Class { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public string IconName { get; set; }
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    public async Task OnClickIcon(MouseEventArgs e)
    {
        await OnClick.InvokeAsync(e);
    }
}
