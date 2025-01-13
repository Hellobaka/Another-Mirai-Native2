#if NET5_0_OR_GREATER
using Net.Codecrete.QrCodeGenerator;
#endif

namespace Another_Mirai_Native
{
    public class QRCodeHelper
    {
        // This part of the code is from "https://github.com/eric2788/Lagrange.Core/blob/fd20a5aec81cacd56d60f3130cf057461300fd3f/Lagrange.OneBot/Utility/QrCodeHelper.cs#L30C52-L30C52"
        // Thanks to "https://github.com/eric2788"
        // https://github.com/LagrangeDev/Lagrange.Core/issues/732
        public static void Output(string text, bool compatibilityMode)
        {
#if NET5_0_OR_GREATER
            var segments = QrSegment.MakeSegments(text);
            var qrCode = QrCode.EncodeSegments(segments, QrCode.Ecc.Low);

            if (compatibilityMode)
            {
                for (var y = 0; y < qrCode.Size; y++)
                {
                    for (var x = 0; x < qrCode.Size; x++)
                    {
                        var color = qrCode.GetModule(x, y);
                        if (color)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("  ");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write("  ");
                            Console.ResetColor();
                        }
                    }
                    Console.Write("\n");
                }
            }
            else
            {
                var (bottomHalfBlock, topHalfBlock, emptyBlock, fullBlock) = ("▄", "▀", " ", "█");

                for (var y = 0; y < qrCode.Size + 2; y += 2)
                {
                    for (var x = 0; x < qrCode.Size + 2; ++x)
                    {
                        var foregroundBlack = qrCode.GetModule(x - 1, y - 1);
                        var backgroundBlack = qrCode.GetModule(x - 1, y) || y > qrCode.Size;

                        if (foregroundBlack && !backgroundBlack)
                        {
                            Console.Write(bottomHalfBlock);
                        }
                        else if (!foregroundBlack && backgroundBlack)
                        {
                            Console.Write(topHalfBlock);
                        }
                        else if (foregroundBlack && backgroundBlack)
                        {
                            Console.Write(emptyBlock);

                        }
                        else if (!foregroundBlack && !backgroundBlack)
                        {
                            Console.Write(fullBlock);
                        }
                    }
                    Console.Write("\n");
                }
            }
#endif
        }
    }
}
