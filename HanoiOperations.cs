using System;

namespace TowersOfHanoi
{
    static class HanoiOperations
    {
        public static ulong Move(ulong state, byte diskCount, byte from, byte to)
        {
            ulong tempState = state;
            byte[] disks = new byte[diskCount];
            for (int i = 0; i < diskCount; ++i)
            {
                disks[diskCount - i - 1] = (byte)(tempState % 10);
                tempState /= 10;
            }

            int fromIndex = Array.FindIndex(disks, d => d == from);

            if (fromIndex == -1)
            {
                return state;
            }

            int toIndex = Array.FindIndex(disks, d => d == to);

            if (toIndex > fromIndex || toIndex == -1)
            {
                disks[fromIndex] = to;
                ulong res = 0;
                ulong m = 1;
                for (int i = diskCount; i > 0; --i)
                {
                    res += disks[i - 1] * m;
                    m *= 10;
                }
                return res;
            }

            return state;
        }
    }
}
