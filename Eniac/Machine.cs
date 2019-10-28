using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eniac
{
    class Machine
    {
        public const int DIGIT_TRAY_SECTIONS = 8;
        public const int DIGIT_TRAY_COUNT = 5;
        public const int PROGRAM_TRAY_SECTIONS = 9;
        public const int PROGRAM_TRAY_COUNT = 11;

        private EniacAccumulator[] accumulators;
        private EniacCyclingUnit cyclingUnit;
        private EniacInitiatingUnit initUnit;
        private EniacConstantTransmitter constUnit;
        private EniacCardPunch punchUnit;
        private EniacCardReader readerUnit;
        private EniacMasterProgrammer masterUnit;
        private EniacMultiplier multiplierUnit;
        private EniacDivider dividerUnit;
        private EniacFunctionTable[] functionTables;
//        private int[] digitTrays;
//        private int[] programTrays;
        private int[] trays;
        private ArrayList filters;

        private int currentTray;
        private char currentBus;
        private int currentPos;
        private String card;
        private long cycles;
        private int programTrayOffset;

        private String debug;

        private MainForm mainForm;

        public Machine(MainForm mf)
        {
            int i;
            mainForm = mf;
            debug = "Machine Started\r\n";
            accumulators = new EniacAccumulator[20];
//            digitTrays = new int[DIGIT_TRAY_SECTIONS * DIGIT_TRAY_COUNT];
//            programTrays = new int[PROGRAM_TRAY_SECTIONS * PROGRAM_TRAY_COUNT];
            trays = new int[(DIGIT_TRAY_SECTIONS * DIGIT_TRAY_COUNT) + (PROGRAM_TRAY_SECTIONS * PROGRAM_TRAY_COUNT)];
            programTrayOffset = DIGIT_TRAY_SECTIONS * DIGIT_TRAY_COUNT;
            for (i = 0; i < 20; i++) accumulators[i] = new EniacAccumulator(this,i);
            functionTables = new EniacFunctionTable[3];
            for (i = 0; i < 3; i++) functionTables[i] = new EniacFunctionTable(this,i);
            cyclingUnit = new EniacCyclingUnit(this);
            initUnit = new EniacInitiatingUnit(this);
            constUnit = new EniacConstantTransmitter(this);
            punchUnit = new EniacCardPunch(this);
            readerUnit = new EniacCardReader(this);
            masterUnit = new EniacMasterProgrammer(this);
            multiplierUnit = new EniacMultiplier(this);
            dividerUnit = new EniacDivider(this);
            filters = new ArrayList();
            cycles = 0;
            clearAllBusses();
            addDefaultFilters();
        }

        public ArrayList getFilters()
        {
            return filters;
        }

        public void addFilter(String n, int m0, int m1, int m2, int m3, int m4, int m5, int m6, int m7, int m8, int m9, int m10)
        {
            int[] map;
            EniacFilter filter;
            map = new int[11];
            map[0] = m0; map[1] = m1; map[2] = m2; map[3] = m3; map[4] = m4; map[5] = m5;
            map[6] = m6; map[7] = m7; map[8] = m8; map[9] = m9; map[10] = m10;
            filter = findFilter(n);
            if (filter == null)
            {
                filter = new EniacFilter(n);
                filter.setMap(map);
                filters.Add(filter);
            }
            else
            {
                filter.setMap(map);
            }
        }
        private void addDefaultFilters()
        {
            addFilter("SHL1", 0, -1, 1, 2, 3, 4, 5, 6, 7, 8, 9);
            addFilter("SHL2", 0, -1, -1, 1, 2, 3, 4, 5, 6, 7, 8);
            addFilter("SHL3", 0, -1, -1, -1, 1, 2, 3, 4, 5, 6, 7);
            addFilter("SHL4", 0, -1, -1, -1, -1, 1, 2, 3, 4, 5, 6);
            addFilter("SHL5", 0, -1, -1, -1, -1, -1, 1, 2, 3, 4, 5);
            addFilter("SHL6", 0, -1, -1, -1, -1, -1, -1, 1, 2, 3, 4);
            addFilter("SHL7", 0, -1, -1, -1, -1, -1, -1, -1, 1, 2, 3);
            addFilter("SHL8", 0, -1, -1, -1, -1, -1, -1, -1, -1, 1, 2);
            addFilter("SHL9", 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1);
            addFilter("SHR1", 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, -1);
            addFilter("SHR2", 0, 3, 4, 5, 6, 7, 8, 9, 10, -1, -1);
            addFilter("SHR3", 0, 4, 5, 6, 7, 8, 9, 10, -1, -1, -1);
            addFilter("SHR4", 0, 5, 6, 7, 8, 9, 10, -1, -1, -1, -1);
            addFilter("SHR5", 0, 6, 7, 8, 9, 10, -1, -1, -1, -1, -1);
            addFilter("SHR6", 0, 7, 8, 9, 10, -1, -1, -1, -1, -1, -1);
            addFilter("SHR7", 0, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1);
            addFilter("SHR8", 0, 9, 10, -1, -1, -1, -1, -1, -1, -1, -1);
            addFilter("SHR9", 0, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1);
            addFilter("SWP5", 0, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5);
            addFilter("HI1", 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1);
            addFilter("HI2", 0, 1, 2, -1, -1, -1, -1, -1, -1, -1, -1);
            addFilter("HI3", 0, 1, 2, 3, -1, -1, -1, -1, -1, -1, -1);
            addFilter("HI4", 0, 1, 2, 3, 4, -1, -1, -1, -1, -1, -1);
            addFilter("HI5", 0, 1, 2, 3, 4, 5, -1, -1, -1, -1, -1);
            addFilter("HI6", 0, 1, 2, 3, 4, 5, 6, -1, -1, -1, -1);
            addFilter("HI7", 0, 1, 2, 3, 4, 5, 6, 7, -1, -1, -1);
            addFilter("HI8", 0, 1, 2, 3, 4, 5, 6, 7, 8, -1, -1);
            addFilter("HI9", 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, -1);
            addFilter("LO1", 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10);
            addFilter("LO2", 0, -1, -1, -1, -1, -1, -1, -1, -1, 9, 10);
            addFilter("LO3", 0, -1, -1, -1, -1, -1, -1, -1, 8, 9, 10);
            addFilter("LO4", 0, -1, -1, -1, -1, -1, -1, 7, 8, 9, 10);
            addFilter("LO5", 0, -1, -1, -1, -1, -1, 6, 7, 8, 9, 10);
            addFilter("LO6", 0, -1, -1, -1, -1, 5, 6, 7, 8, 9, 10);
            addFilter("LO7", 0, -1, -1, -1, 4, 5, 6, 7, 8, 9, 10);
            addFilter("LO8", 0, -1, -1, 3, 4, 5, 6, 7, 8, 9, 10);
            addFilter("LO9", 0, -1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
            addFilter("ROR1", 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1);
            addFilter("ROR2", 0, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2);
            addFilter("ROR3", 0, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3);
            addFilter("ROR4", 0, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4);
            addFilter("ROR5", 0, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5);
            addFilter("ROR6", 0, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6);
            addFilter("ROR7", 0, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7);
            addFilter("ROR8", 0, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8);
            addFilter("ROR9", 0, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9);
            addFilter("ROL1", 0, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9);
            addFilter("ROL2", 0, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8);
            addFilter("ROL3", 0, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7);
            addFilter("ROL4", 0, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6);
            addFilter("ROL5", 0, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5);
            addFilter("ROL6", 0, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4);
            addFilter("ROL7", 0, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3);
            addFilter("ROL8", 0, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2);
            addFilter("ROL9", 0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1);
        }

        public EniacFilter findFilter(String n)
        {
            int i;
            EniacFilter f;
            for (i = 0; i < filters.Count; i++)
            {
                f = (EniacFilter)(filters[i]);
                if (n.CompareTo(f.getName()) == 0) return f;
            }
            return null;
        }

        public int translate(String filterName, int value)
        {
            EniacFilter f;
            f = findFilter(filterName);
            if (f == null) return value;
            return f.translate(value);
        }

        public String digitArrayToString(Boolean sign, int[] digits)
        {
            String ret;
            int i;
            ret = (sign) ? "-" : "+";
            for (i = 0; i < 10; i++) ret += digits[i].ToString();
            return ret;
        }

        public Boolean stringToDigitArray(String s,int[] digits)
        {
            Boolean ret;
            int i;
            ret = false;
            if (s[0] == '-' || s[0] == '+')
            {
                ret = (s[0] == '-') ? true : false;
                s = s.Substring(1);
            }
            i = s.Length - 1;
            if (i> 9) i = 9;
            while (i >= 0)
            {
                digits[i] = s[i] - 48;
                i--;
            }
            return ret;
        }

        public String readCard()
        {
            log("Read Card called");
            return mainForm.readCard();
        }

        public void stackCard(String s)
        {
            mainForm.stackCard(s);
        }

        public EniacMultiplier getMultiplier()
        {
            return multiplierUnit;
        }

        public EniacDivider getDivider()
        {
            return dividerUnit;
        }

        public EniacCardReader getReader()
        {
            return readerUnit;
        }

        public EniacCardPunch getPunch()
        {
            return punchUnit;
        }

        public EniacAccumulator getAccumulator(int n)
        {
            return accumulators[n];
        }

        public EniacFunctionTable getFunctionTable(int n)
        {
            return functionTables[n];
        }

        public EniacInitiatingUnit getInitUnit()
        {
            return initUnit;
        }

        public EniacConstantTransmitter getConstUnit()
        {
            return constUnit;
        }

        public EniacMasterProgrammer getMasterUnit()
        {
            return masterUnit;
        }

        public EniacCyclingUnit getCyclingUnit()
        {
            return cyclingUnit;
        }

        public String getDebug()
        {
            String ret;
            ret = debug;
            debug = "";
            return ret;
        }

        public void clearDebug()
        {
            debug = "";
        }

        public String getCard()
        {
            String ret;
            ret = card;
            card = "";
            return ret;
        }

        public void log(String s)
        {
            debug += ("[" + cycles.ToString() + "] " + s + "\r\n");
        }

        public Boolean parseBus(String bus)
        {
            int i;
            int bias;
            if (bus.StartsWith("P") || bus.StartsWith("D")) {
                currentBus = bus[0];
                bias = (currentBus == 'P') ? 9 : 5;
                bus = bus.Substring(1);
                currentTray = bias * Convert.ToInt32(bus.Substring(0,1));
                bus = bus.Substring(2);
                i = bus[0]-65;
                if (i < 0) i = 0;
                if (i >= bias) i = bias - 1;
                currentTray += i;
                bus = bus.Substring(1);
                if (bus.Length < 2)
                {
                    currentPos = -1;
                    return true;
                }
                bus = bus.Substring(1);
                currentPos = Convert.ToInt32(bus);
                return true;
            }
            return false;
        }

        public int getBusNumber(String bus)
        {
            if (parseBus(bus))
            {
                return (currentBus == 'D') ? currentTray : currentTray + programTrayOffset;
            }
            return -1;
        }

        public int getBusPosition(String bus)
        {
            if (parseBus(bus)) return currentPos;
            return -1;
        }
/*
        public void setBus(String bus, int v)
        {
            if (parseBus(bus))
            {
                if (currentBus == 'P')
                {
                    if (currentPos >= 0) v <<= (11-currentPos);
                    trays[currentTray + programTrayOffset] |= v;
//                    programTrays[currentTray] |= v;
                }
                if (currentBus == 'D')
                {
                    trays[currentTray] |= v;
//                    digitTrays[currentTray] |= v;
                }
            }
        }
*/
        public void setBus(int bus, int pos, int v)
        {
            if (pos >= 0)
            {
                v <<= (11 - pos);
            }
            trays[bus] |= v;
        }
/*
        public int getBus(String bus)
        {
            int mask;
            if (parseBus(bus))
            {
                if (currentBus == 'P')
                {
                    if (currentPos < 0) return trays[currentTray + programTrayOffset];
//                    if (currentPos < 0) return programTrays[currentTray];
                    mask = 1 << (11-currentPos);
                    if ((trays[currentTray + programTrayOffset] & mask) == mask) return 1;
//                    if ((programTrays[currentTray] & mask) == mask) return 1;
                    return 0;
                }
                if (currentBus == 'D')
                {
                    return trays[currentTray];
//                    return digitTrays[currentTray];
                }
            }
            return 0;
        }
*/
        public int getBus(int bus, int pos)
        {
            int mask;
            if (pos < 0) return trays[bus];
            mask = 1 << (11 - pos);
            if ((trays[bus] & mask) == mask) return 1;
            return 0;
        }

        public long getCycles()
        {
            return cycles;
        }

        public void goPressed()
        {
            initUnit.go();
            cycles = 0;
        }

        public void clearPressed()
        {
            initUnit.clear();
            cycles = 0;
        }

        public void clearAllBusses()
        {
            int i;
            for (i = 0; i < ((DIGIT_TRAY_SECTIONS * DIGIT_TRAY_COUNT) + (PROGRAM_TRAY_SECTIONS * PROGRAM_TRAY_COUNT)); i++) trays[i] = 0;
//            for (i = 0; i < DIGIT_TRAY_SECTIONS * DIGIT_TRAY_COUNT; i++) digitTrays[i] = 0;
//            for (i = 0; i < PROGRAM_TRAY_SECTIONS * PROGRAM_TRAY_COUNT; i++) programTrays[i] = 0;
        }

        public void cycle()
        {
            int i;
            int clocks;
            clearAllBusses();
            clocks = cyclingUnit.cycle();
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                cycles++;
            }
            i = initUnit.cycle1(clocks);
            if ((i & EniacInitiatingUnit.INIT_CLEAR_PRESSED) == EniacInitiatingUnit.INIT_CLEAR_PRESSED)
            {
                for (i = 0; i < 20; i++) accumulators[i].allClear();
                for (i = 0; i < 3; i++) functionTables[i].clear();
                masterUnit.allClear();
                multiplierUnit.clear();
                dividerUnit.clear();
                readerUnit.clear();
            }
            if ((i & EniacInitiatingUnit.INIT_SEL_CLEAR) == EniacInitiatingUnit.INIT_SEL_CLEAR)
            {
                for (i = 0; i < 20; i++) accumulators[i].selectiveClear();
            }
            constUnit.cycle1(clocks);
            punchUnit.cycle1(clocks);
            readerUnit.cycle1(clocks);
            masterUnit.cycle1(clocks);
            for (i = 0; i < 3; i++) functionTables[i].cycle1(clocks);
            for (i = 0; i < 20; i++) accumulators[i].cycle1(clocks);
            dividerUnit.cycle1(clocks);
            multiplierUnit.cycle1(clocks);
            initUnit.cycle2(clocks);
            constUnit.cycle2(clocks);
            punchUnit.cycle2(clocks);
            readerUnit.cycle2(clocks);
            masterUnit.cycle2(clocks);
            card = punchUnit.getCard();
            multiplierUnit.cycle2(clocks);
            dividerUnit.cycle2(clocks);
            for (i = 0; i < 3; i++) functionTables[i].cycle2(clocks);
            for (i = 0; i < 20; i++) accumulators[i].cycle2(clocks);
        }
    }
}
