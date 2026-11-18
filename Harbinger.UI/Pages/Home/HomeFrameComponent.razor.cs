using Harbinger.UI.Pages.Frame;
using Harbinger.UI.Util;

using Microsoft.AspNetCore.Components;

namespace Harbinger.UI.Pages.Home;

public partial class HomeFrameComponent
{

    [Inject]
    public NavigationManager Navigation { get; set; }

    private void OnClicks()
    {
        var urlOpt = typeof(FramePage).Path();
        foreach (var url in urlOpt)
            Navigation.NavigateTo(url, forceLoad: true);
    }

}
