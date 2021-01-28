using System;

namespace TowersOfHanoi
{
    static class HanoiOperations
    {
        public static ulong Move(ulong state, byte diskCount, byte from, byte to)
        {
            // saglabājam spēles stāvokļu
            ulong tempState = state;
            byte[] disks = new byte[diskCount];

            // katras ripas stieņa numura gūšana
            for (int i = 0; i < diskCount; ++i)
            {
                disks[diskCount - i - 1] = (byte)(tempState % 10);
                tempState /= 10;
            }

            // mazākas ripas meklēšana stieņā, no kuras jānoņem ripu
            int fromIndex = Array.FindIndex(disks, d => d == from);

            if (fromIndex == -1)
            {
                return state;
            }

            // mazākas ripas meklēšana stieņā, kurā jāpaliek ripu
            int toIndex = Array.FindIndex(disks, d => d == to);

            // jauna stāvokļa veidošana
            if (toIndex > fromIndex || toIndex == -1)
            {
                disks[fromIndex] = to;
                ulong newState = 0;
                ulong dec = 1;
                for (int i = diskCount; i > 0; --i)
                {
                    newState += disks[i - 1] * dec;
                    dec *= 10;
                }
                return newState;
            }

            return state;
        }
    }
}
