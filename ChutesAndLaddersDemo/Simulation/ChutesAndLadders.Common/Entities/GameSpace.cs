using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Entities
{
    public class GameSpace
    {
        public int Index { get; set; }
        public int? PathTo { get; set; }

        public GameSpace(int index)
        {
            this.Index = index;
            this.PathTo = GameSpace.GetPathTo(index);
        }

        public override string ToString()
        {
            string result = this.Index.ToString();
            if (this.PathTo.HasValue)
                result += $"({this.PathTo.Value.ToString()})";
            return result;
        }

        private static int? GetPathTo(int index)
        {
            int? pathTo = null;
            switch (index)
            {
                case 1:
                    pathTo = 38;
                    break;

                case 4:
                    pathTo = 14;
                    break;

                case 9:
                    pathTo = 31;
                    break;

                case 16:
                    pathTo = 6;
                    break;

                case 21:
                    pathTo = 42;
                    break;

                case 28:
                    pathTo = 84;
                    break;

                case 36:
                    pathTo = 44;
                    break;

                case 47:
                    pathTo = 26;
                    break;

                case 49:
                    pathTo = 11;
                    break;

                case 51:
                    pathTo = 67;
                    break;

                case 56:
                    pathTo = 53;
                    break;

                case 62:
                    pathTo = 19;
                    break;

                case 64:
                    pathTo = 60;
                    break;

                case 71:
                    pathTo = 91;
                    break;

                case 80:
                    pathTo = 100;
                    break;

                case 87:
                    pathTo = 24;
                    break;

                case 93:
                    pathTo = 73;
                    break;

                case 95:
                    pathTo = 75;
                    break;

                case 98:
                    pathTo = 78;
                    break;
            }

            return pathTo;
        }
    }
}