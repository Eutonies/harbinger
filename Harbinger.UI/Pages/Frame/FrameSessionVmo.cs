using Harbinger.Domain.Model;
using Harbinger.Domain.Util;
using Defs = Harbinger.UI.Pages.Frame.FrameDefaults.Generation;

namespace Harbinger.UI.Pages.Frame;

public partial record FrameSessionVmo(
    Func<Task> UIUpdateTask
    )
{
    private void UpdateUI() => _ = UIUpdateTask();
    private Task UpdateUIAsync() => UIUpdateTask();

    private Option<GenerationInfo> _generationInfo = Option.None<GenerationInfo>();

    public Option<int> ColumnLEDCountSuggestionFor(int colIndx) => _generationInfo
        .Where(inf => inf.Input.NumberOfColumns == _numberOfColumns && _numberOfLEDs == inf.Input.NumberOfLeds)
        .FlatMap(inf => inf.Averages.NumberOfLEDsExclTurn(colIndx))
        .MapVal(_ => _ + 1);

    private FrameVmo _uiFrame = Defs.Frame.Pipe(_ => _.ToVmo());
    public FrameVmo UiFrame => _uiFrame;

    public Option<FrameStripVmo> StripVmo => _generationInfo.Map(_ => _.Strip);

    private bool _hasInitedColumns = false;
    private IReadOnlyCollection<FrameColumnVmo> _columns = [];
    public IReadOnlyCollection<FrameColumnVmo> Columns => _hasInitedColumns ? _columns : (_columns = Enumerable.Range(0, Defs.NumberOfColumns)
        .Select(_ => new FrameColumnVmo(ColumnIndex: _, NumberOfLEDsInput: Option.None<int>(), XCoordinateInput: Option.None<int>(), OnUpdate: UpdateUI, Session: this))
        .ToReadOnly()
        .Ripa(_ => _hasInitedColumns = true));

    private bool _hasInitedTurns = false;
    private IReadOnlyCollection<FrameTurnVmo> _turns = [];
    public IReadOnlyCollection<FrameTurnVmo> Turns => _hasInitedTurns ? _turns : (_turns = Enumerable.Range(0, Defs.NumberOfColumns)
        .Select(_ => new FrameTurnVmo(TurnIndex: _, LEDIndexInput: Option.None<int>(), CoordinateInput: Option.None<XY>(), OnUpdate: UpdateUI))
        .ToReadOnly()
        .Ripa(_ => _hasInitedTurns = true));


    private int _frameWidth = Defs.FrameWidthCm;
    public int FrameWidth
    {
        get => _frameWidth;
        set
        {
            if (value == _frameWidth)
                return;
            _frameWidth = value;
            CheckAndUpdate();
        }
    }

    private int _frameHeight = Defs.FrameHeightCm;
    public int FrameHeight
    {
        get => _frameHeight;
        set
        {
            if (value == _frameHeight)
                return;
            _frameHeight = value;
            CheckAndUpdate();
        }
    }

    private int _numberOfLEDs = Defs.NumberOfLEDs;
    public int NumberOfLEDs
    {
        get => _numberOfLEDs;
        set
        {
            if (_numberOfLEDs == value)
                return;
            _numberOfLEDs = value;
            CheckAndUpdate();
        }
    }

    private int _numberOfColumns = Defs.NumberOfColumns;
    public int NumberOfColumns
    {
        get => _numberOfColumns;
        set
        {
            if (_numberOfColumns == value)
                return;
            _numberOfColumns = value;
            CheckAndUpdate();
        }
    }



    private HarLoc _selectedStartCorner = Defs.StartCorner;
    public long SelectedCornerId
    {
        get => (int)_selectedStartCorner;
        set
        {
            var converted = (HarLoc)(int)value;
            if (converted == _selectedStartCorner)
                return;
            _selectedStartCorner = converted;
            CheckAndUpdate();
        }
    }

    private decimal _distanceBetweenLEDsCm = Defs.DistanceBetweenLEDsCm;
    public decimal DistanceBetweenLEDsCm
    {
        get => _distanceBetweenLEDsCm;
        set
        {
            if (_distanceBetweenLEDsCm == value)
                return;
            _distanceBetweenLEDsCm = value;
            CheckAndUpdate();
        }
    }

    private decimal _distanceBetweenColumnsCm = Defs.DistanceBetweenColumnsCm;
    public decimal DistanceBetweenColumnsCm
    {
        get => _distanceBetweenColumnsCm;
        set
        {
            if (_distanceBetweenColumnsCm == value)
                return;
            _distanceBetweenColumnsCm = value;
            CheckAndUpdate();
        }
    }

    public bool InputHasChangedSinceLastUpdate()
    {
        if (_generationInfo.IsNone)
            return true;
        var inputNow = GenerationInput;
        foreach(var inf in _generationInfo)
        {
            var inputThen = inf.Input;
            var isSame = inputThen.IsSameAs(inputNow);
            return !isSame;
        }
        return true;
    }


    private void CheckAndUpdate()
    {
        var input = GenerationInput;
        UpdateColumns(input);
        UpdateTurns(input);
        UpdateFrame(input.Frame);
    }

    private void UpdateFrame(HarFram fram)
    {
        if(!fram.IsSameAs(_uiFrame.FromFrame))
        {
            _uiFrame = fram.ToVmo();
            UpdateUI();
        }
            
    }

    private record GenerationInfo(HarStrip InputStrip, GenIn Input, FrameSessionVmo Session)
    {
        public readonly FrameGenerationOutputAveragesVmo Averages = new FrameGenerationOutputAveragesVmo(Input, InputStrip);

        public FrameVmo Frame => Session.UiFrame;

        private FrameStripVmo? _strip;
        public FrameStripVmo Strip => _strip ??= Frame * InputStrip;
    }


}
