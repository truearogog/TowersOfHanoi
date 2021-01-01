using System;

namespace TowersOfHanoi
{
    static class FrameStewart
    {
        public static uint Hanoi(uint disks, uint pegs)
        {
            if (disks == 0)
            {
                return 0;
            }
            if (disks == 1 && pegs > 1)
            {
                return 1;
            }
            if (pegs == 3)
            {
                return (uint)(Math.Pow(2, disks) - 1);
            }
            if (pegs >= 3 && disks > 0)
            {
                uint minMoves = uint.MaxValue;
                for (uint kdisks = 1; kdisks < disks; kdisks++)
                {
                    uint kmoves = 2 * Hanoi(kdisks, pegs) + Hanoi(disks - kdisks, pegs - 1);
                    minMoves = Math.Min(minMoves, kmoves);
                }
                return minMoves;
            }
            return 0;
        }
    }
}
