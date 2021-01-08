using System.Collections.Generic;
using System.Linq;

namespace TowersOfHanoi
{
    class Peg
    {
        private Stack<byte> disks;
        private byte maxDiskCount;

        public Peg(byte maxDiskCount, bool containsDisks = false)
        {
            disks = new Stack<byte>();
            this.maxDiskCount = maxDiskCount;
            if (containsDisks)
            {
                for (byte i = 0; i < maxDiskCount; i++)
                {
                    disks.Push((byte)(maxDiskCount - i));
                }
            }
        }

        public Peg(Peg peg)
        {
            maxDiskCount = peg.maxDiskCount;
            disks = new Stack<byte>(new Stack<byte>(peg.disks));
        }

        public bool CanPush(byte diskSize)
        {
            return (disks.Count > 0) ? (disks.Count + 1 <= maxDiskCount) ? disks.Peek() > diskSize : false : (disks.Count + 1 <= maxDiskCount);
        }

        public void Push(byte diskSize)
        {
            disks.Push(diskSize);
        }

        public bool CanPop()
        {
            return disks.Count > 0;
        }

        public byte Pop()
        {
            return disks.Pop();
        }

        public byte Peek()
        {
            return disks.Peek();
        }

        public byte Count()
        {
            return (byte)disks.Count;
        }

        public List<byte> GetDisks()
        {
            return new List<byte>(disks);
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", disks)}]";
        }

        public override bool Equals(object obj)
        {
            Peg otherPeg = obj as Peg;
            return Enumerable.SequenceEqual(disks, otherPeg.disks);
        }
    }
}
