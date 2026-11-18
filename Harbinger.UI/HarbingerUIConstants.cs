using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Harbinger.UI;

public static class HarbingerUIConstants
{
    public static readonly IComponentRenderMode HarbingerRender = new InteractiveServerRenderMode(prerender: false);

}
