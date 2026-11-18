using Harbinger.Domain.Util;
using Harbinger.Domain.Model;
using Harbinger.Domain.Generation;

namespace Harbinger.UI.Pages.Frame;

public partial record FrameSessionVmo
{

    // All values in mm
    private int InputFrameWidthInMillimeter => (_frameWidth * 10m).ToInt();
    private int InputFrameHeightInMillimeter => (_frameHeight * 10m).ToInt();

    private HarFram InputFrame => new HarFram(MinX: 0, MinY: 0, MaxX: InputFrameWidthInMillimeter, MaxY: InputFrameHeightInMillimeter);

    private int InputNumberOfLEDs => _numberOfLEDs;
    private Option<int> InputNumberOfColumns => Option.Some(_numberOfColumns);
    private decimal InputDistanceBetweenLEDsInMillimeter => _distanceBetweenLEDsCm * 10m;
    private decimal InputDistanceBetweenColumsInMillimeter => _distanceBetweenColumnsCm * 10m;

    private HarLoc InputStartCorner => _selectedStartCorner.Vertical() switch
    {
        HarLoc.Top => HarLoc.TopLeftCorner,
        _ => HarLoc.BottomLeftCorner
    };


    private GenIn GenerationInput => new GenIn(
        Frame: InputFrame,
        NumberOfLeds: InputNumberOfLEDs,
        DistanceBetweenLeds: InputDistanceBetweenLEDsInMillimeter,
        DistanceBetweenColumns: InputDistanceBetweenColumsInMillimeter,
        StartCorner: InputStartCorner,
        NoOfColumnsInput: InputNumberOfColumns.OptVal(),
        InputTurns: [],
        InputCols: []
    );


    public Task UpdateFrame() => DoWithUpdateButtonPushedAsync(async () =>
    {
        try
        {
            await Task.CompletedTask;
            var genInput = GenerationInput;
            var gened = genInput.Generate();
            var info = new GenerationInfo(InputStrip: gened, Input: genInput, Session: this);
            UpdateColumns(genInput);
            UpdateTurns(genInput);
            _generationInfo = Option.Some(info);
            UpdateUI();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
        }

    });

    private void UpdateColumns(GenIn genInput)
    {
        foreach(var inf in _generationInfo)
        {
            if(inf.Input.NumberOfLeds != genInput.NumberOfLeds || inf.Input.NumberOfColumns != genInput.NumberOfColumns)
            {
                _columns = [];
            }
        }
        var existingCols = _columns
            .GroupBy(_ => _.ColumnIndex)
            .ToDictionary(_ => _.Key, _ => _.First());
        _columns = Enumerable.Range(0, genInput.NumberOfColumns)
            .Select(indx => existingCols.TryGetValue(indx, out var exi) ?
                exi :
                new FrameColumnVmo(
                    ColumnIndex: indx,
                    NumberOfLEDsInput: Option.None<int>(),
                    XCoordinateInput: Option.None<int>(),
                    OnUpdate: UpdateUI,
                    Session: this
                )).ToArray();
      

    }

    private void UpdateTurns(GenIn genInput)
    {
        foreach (var inf in _generationInfo)
        {
            if (inf.Input.NumberOfLeds != genInput.NumberOfLeds || inf.Input.NumberOfColumns != genInput.NumberOfColumns)
            {
                _turns = [];
            }
        }
        var existingTurns = Turns
            .GroupBy(_ => _.TurnIndex)
            .ToDictionary(_ => _.Key, _ => _.First());
        _turns = Enumerable.Range(0, genInput.NumberOfColumns)
            .Select(indx => existingTurns.TryGetValue(indx, out var exi) ?
                exi :
                new FrameTurnVmo(
                    TurnIndex: indx,
                    LEDIndexInput: Option.None<int>(),
                    CoordinateInput: Option.None<XY>(),
                    OnUpdate: UpdateUI
                )).ToArray();


    }



}
