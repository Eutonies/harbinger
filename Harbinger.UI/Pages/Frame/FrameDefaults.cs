using Harbinger.Domain.Generation;
using Harbinger.Domain.Model;
using Harbinger.Domain.Util;

namespace Harbinger.UI.Pages.Frame;

public static class FrameDefaults
{
    public static class Generation
    {
        public const int FrameWidthCm = 120;
        public const int FrameWidthMm = FrameWidthCm * 10;
        public const int FrameHeightCm = 90;
        public const int FrameHeightMm = FrameWidthCm * 10;

        public const int NumberOfLEDs = 1200;
        public const int NumberOfColumns = 23;
        public const HarLoc StartCorner = HarLoc.BottomLeftCorner;
        public const decimal DistanceBetweenLEDsCm = 1.6667m;
        public const decimal DistanceBetweenLEDsMm = DistanceBetweenLEDsCm * 10m;
        public const decimal DistanceBetweenColumnsCm = 5.2m;
        public const decimal DistanceBetweenColumnsMm = DistanceBetweenColumnsCm * 10m;

        private static HarFram? _frame;
        public static HarFram Frame = _frame ??= HarFram.From(FrameWidthMm, FrameHeightMm);

        public static readonly GenIn Input = new GenIn(
            Frame: Frame,
            NumberOfLeds: NumberOfLEDs,
            DistanceBetweenLeds: DistanceBetweenLEDsMm,
            DistanceBetweenColumns: DistanceBetweenColumnsMm,
            StartCorner: StartCorner,
            NoOfColumnsInput: NumberOfColumns,
            InputTurns: [],
            InputCols: []
            );

        private static HarStrip? _output;
        public static HarStrip Output => _output ??= StripGenerator.Generate(Input);

        private static FrameGenerationOutputAveragesVmo? _defaultAverages;
        public static FrameGenerationOutputAveragesVmo DefaultAverages => _defaultAverages ??=
            new FrameGenerationOutputAveragesVmo(GenerationInput: Input, Strip: Output);

    }






}
