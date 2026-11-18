using System.Collections;

using Harbinger.Domain.Util;

namespace Harbinger.UI.Pages.Frame;

public record FrameStripVmo(FrameVmo Frame, HarStrip Strip) : IEnumerable<HarStripLink>, IReadOnlyCollection<HarStripLink>
{
    private HarStripLink[]? _mappedLinks;
    public HarStripLink[] MappedLinks => _mappedLinks ??= Strip.Links
        .Select(Frame.MapLink)
        .ToArray();

    public int Count => MappedLinks.Length;

    public IEnumerator<HarStripLink> GetEnumerator() => new StripEnumerator(this);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private record StripEnumerator(FrameStripVmo Strip) : IEnumerator<HarStripLink>
    {
        public readonly int TotalLength = Strip.MappedLinks.Length;
        private int? _currentIndex;

        public HarStripLink Current => _currentIndex switch
        {
            null => throw new ArgumentException("You need to call MoveNext before Current!"),
            int i when i >= TotalLength => throw new ArgumentException("You have dun moved past range!"),
            int i => Strip.MappedLinks[i]
        };

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_currentIndex == null)
            {
                if (TotalLength == 0)
                    return false;
                _currentIndex = 0;
                return true;
            }
            if (_currentIndex == TotalLength - 1)
                return false;
            _currentIndex += 1;
            return true;
        }

        public void Reset()
        {
            _currentIndex = 0.CipeVal(_ => TotalLength > 0);
        }
    }

}
