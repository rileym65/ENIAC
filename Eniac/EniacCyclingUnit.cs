using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eniac
{
    class EniacCyclingUnit
    {
        public const int MODE_PULSE = 1;
        public const int MODE_ADD = 2;
        public const int MODE_CONT = 0;
        public const int CPP = 512;
        public const int C10P = 256;
        public const int C9P = 128;
        public const int C1P = 64;
        public const int C2P = 32;
        public const int C22P = 16;
        public const int C4P = 8;
        public const int C11P = 4;
        public const int CCG = 2;
        public const int CRP = 1;

        private int phase;
        private Boolean powered;
        private int mode;
        private Boolean stopped;
        private Machine machine;

        public EniacCyclingUnit(Machine m) {
            phase = 0;
            powered = true;
            stopped = false;
            mode = MODE_CONT;
            machine = m;
        }

        public void reset()
        {
            stopped = false;
            mode = MODE_CONT;
        }

        public void setPowered(Boolean b)
        {
            powered = b;
        }

        public void goPressed()
        {
            stopped = false;
        }

        public int cycle() {
            int ret;
            ret = 0;
            if (powered == false) return ret;
            if (stopped) return ret;
            if (++phase >= 20) phase = 0;
            switch (phase)
            {
                case 0: ret = C1P + C9P + C10P; break;
                case 1: ret = C2P + C9P + C10P; break;
                case 2: ret = C2P + C9P + C10P; break;
                case 3: ret = C22P + C9P + C10P; break;
                case 4: ret = C22P + C9P + C10P; break;
                case 5: ret = C4P + C9P + C10P; break;
                case 6: ret = C4P + C9P + C10P; break;
                case 7: ret = C4P + C9P + C10P; break;
                case 8: ret = C4P + C9P + C10P; break;
                case 9: ret = C10P; break;
                case 10: ret = C11P; break;
                case 11: ret = CCG; break;
                case 12: ret = CCG; break;
                case 13: ret = CRP + CCG; break;
                case 14: ret = CCG; break;
                case 15: ret = CCG; break;
                case 16: ret = CCG; break;
                case 17: ret = CCG + CPP; break;
                case 19: ret = CRP; break;
            }
            if (mode == MODE_PULSE) stopped = true;
            if (mode == MODE_ADD && phase == 19)
            {
                stopped = true;
                machine.log("--------------------");
            }
            return ret;
        }

        public int getMode()
        {
            return mode;
        }

        public void setMode(int i)
        {
            mode = i;
        }

    }
}
