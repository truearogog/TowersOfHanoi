using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TowersOfHanoi
{
    class HanoiState
    {
        public byte diskCount { get; }
        public byte pegCount { get; }

        private Peg[] pegs;

        public byte[] identifier { private set; get; }

        public HanoiState(byte diskCount, byte pegCount, byte fullPeg)
        {
            this.diskCount = diskCount;
            this.pegCount = pegCount;
            //init pegs
            pegs = new Peg[pegCount];
            for (byte i = 0; i < pegCount; i++)
            {
                pegs[i] = new Peg(diskCount, i == fullPeg);
            }
            InitIdentifier();
        }
            
        public HanoiState(HanoiState state, Move move)
        {
            diskCount = state.diskCount;
            pegCount = state.pegCount;
            //init pegs
            pegs = new Peg[pegCount];
            for (int i = 0; i < pegCount; i++)
            {
                pegs[i] = new Peg(state.pegs[i]);
            }
            pegs[move.to].Push(pegs[move.from].Pop());
            InitIdentifier();
        }

        private void InitIdentifier()
        {
            identifier = new byte[diskCount];
            for (byte i = 0; i < pegCount; ++i)
            {
                List<byte> disks = pegs[i].GetDisks();
                foreach (byte disk in disks)
                {
                    identifier[disk - 1] = i;
                }
            }
        }

        public bool CanMove(byte from, byte to)
        {
            return pegs[from].CanPop() && pegs[to].CanPush(pegs[from].Peek());
        }

        public List<Move> GetPossibleMoves()
        {
            List<Move> possibleMoves = new List<Move>();
            for (byte i = 0; i < pegCount; i++)
            {
                for (byte j = 0; j < pegCount; j++)
                {
                    if (i == j)
                        continue;
                    if (CanMove(i, j))
                        possibleMoves.Add(new Move(i, j));
                }
            }
            return possibleMoves;
        }

        public Peg this[byte key]
        {
            get => new Peg(pegs[key]);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Array.ForEach(pegs, peg => sb.Append($"{peg}  "));
            return $"{sb}\n";
        }

        public override bool Equals(object obj)
        {
            HanoiState otherState = obj as HanoiState;
            return Enumerable.SequenceEqual(identifier, otherState.identifier);
        }
    }
}
