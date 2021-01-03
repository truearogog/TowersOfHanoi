using System.Collections.Generic;

namespace TowersOfHanoi
{
    class Peg
    {
        private Stack<byte> disks;
        private byte maxDiskCount;

        public Peg(byte maxDiscCount, bool containsDisks = false)
        {
            disks = new Stack<byte>();
            maxDiskCount = maxDiscCount;
            if (containsDisks)
            {
                for (byte i = 0; i < maxDiscCount; i++)
                {
                    disks.Push((byte)(maxDiscCount - i));
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

        public override string ToString()
        {
            return $"[{string.Join(", ", disks)}]";
        }

        public override bool Equals(object obj)
        {
            List<byte> disks1 = new List<byte>(this.disks);
            List<byte> disks2 = new List<byte>((obj as Peg).disks);
            if (disks1.Count != disks2.Count)
            {
                return false;
            }
            for (int i = 0; i < disks1.Count; i++)
            {
                if (!disks1[i].Equals(disks2[i]))
                    return false;
            }
            return true;
        }
    }
}
