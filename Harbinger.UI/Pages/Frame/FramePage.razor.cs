using Harbinger.Domain.Util;

namespace Harbinger.UI.Pages.Frame;

public partial class FramePage
{

    private FrameSessionVmo? _session;
    private FrameSessionVmo Session => _session ??= new FrameSessionVmo(UIUpdateTask: () => InvokeAsync(StateHasChanged));




    private HarLoc _selectedStartCorner = HarLoc.BottomLeftCorner;
    private long SelectedCornerId
    {
        get => (int)_selectedStartCorner;
        set
        {
            var converted = (HarLoc)(int)value;
            if (converted == _selectedStartCorner)
                return;
            _selectedStartCorner = converted;
        }
    }


    private static readonly StartCornerOption[] CornerOptions = [
        new StartCornerOption(HarLoc.BottomLeftCorner, "Bottom left"),
        new StartCornerOption(HarLoc.TopLeftCorner, "Top left")
     ];
    private record StartCornerOption(HarLoc Location, string Name)
    {
        public readonly long Id = (int)Location;
    }

    private string UpdateButtonClassAttr => (Session.UpdateButtonIsPushed, Session.IsDisabled) switch
    {
        (true, _) => "pushed",
        (_, true) => "disabled",
        _ => "pushable"
    };
    private string SaveButtonClassAttr => ("disabled".Cipe(_ => Session.IsDisabled) ?? "available")
        .Pipe(abl => "frame-save-button"
            .Pipe(cls => $"{cls} {cls}-{abl}")
        );
        



    private string? UpdateButtonDisabledAttr => Session.DisabledAttr;

    private void OnUpdateButtonClicked() => _ = Session.UpdateFrame();

    private void OnSaveButtonClicked() => _ = Session.UpdateFrame();

}
