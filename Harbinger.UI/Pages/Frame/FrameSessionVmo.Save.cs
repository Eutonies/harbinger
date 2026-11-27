using Harbinger.Domain.Util;
using Harbinger.Domain.Model;
using Harbinger.Domain.Generation;

namespace Harbinger.UI.Pages.Frame;

public partial record FrameSessionVmo
{
    private Option<string>? _existingDataFileHashCode;
    public Option<string> ExistingDataFileHashCode => _existingDataFileHashCode
        .Map
}
