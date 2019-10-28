using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eniac
{
    class EniacDivider
    {
        private Machine machine;
        private Boolean powered;
        private Boolean[] numeratorClear;
        private int[] numeratorReceive;
        private Boolean[] denominatorClear;
        private int[] denominatorReceive;
        private Boolean[] roundoffSwitch;
        private int[] places;
        private Boolean[] interlockSwitch;
        private int[] answer;
        private String[] interlockIn;
        private int[] interlockInNumber;
        private int[] interlockInPosition;
        private String[] programIn;
        private int[] programInNumber;
        private int[] programInPosition;
        private String[] programOut;
        private int[] programOutNumber;
        private int[] programOutPosition;
        private String digitOut;
        private int digitOutNumber;
        private int digitOutPosition;
        private int trigger;
        private int operation;
        private int phase;
        private int[] pulses;
        private int pos;
        private Boolean sign;
        private Boolean addsub;
        private int lastDigit;
        private Boolean interlock;
        private Boolean divRoot;
        private int quotientIC;
        private int numeratorIC;
        private int denominatorIC;
        private int shifterIC;
        private Boolean subtracting;

        public EniacDivider(Machine m)
        {
            machine = m;
            numeratorClear = new Boolean[8];
            numeratorReceive = new int[8];
            denominatorClear = new Boolean[8];
            denominatorReceive = new int[8];
            roundoffSwitch = new Boolean[8];
            interlockSwitch = new Boolean[8];
            answer = new int[8];
            places = new int[8];
            interlockIn = new String[8];
            interlockInNumber = new int[8];
            interlockInPosition = new int[8];
            programIn = new String[8];
            programInNumber = new int[8];
            programInPosition = new int[8];
            programOut = new String[8];
            programOutNumber = new int[8];
            programOutPosition = new int[8];
            pulses = new int[11];
            reset();
        }

        public void reset()
        {
            int i;
            powered = false;
            phase = -1;
            for (i = 0; i < 8; i++)
            {
                numeratorClear[i] = false;
                numeratorReceive[i] = 0;
                denominatorClear[i] = false;
                denominatorReceive[i] = 0;
                roundoffSwitch[i] = false;
                places[i] = 0;
                interlockSwitch[i] = false;
                answer[i] = 0;
                interlockIn[i] = "";
                interlockInNumber[i] = -1;
                interlockInPosition[i] = -1;
                programIn[i] = "";
                programInNumber[i] = -1;
                programInPosition[i] = -1;
                programOut[i] = "";
                programOutNumber[i] = -1;
                programOutPosition[i] = -1;
                digitOut = "";
            }
            quotientIC = 1;
            numeratorIC = 2;
            denominatorIC = 4;
            shifterIC = 6;
            digitOutNumber = -1;
            digitOutPosition = -1;
            clear();
        }

        public void clear()
        {
            trigger = -1;
            operation = -1;
            phase = -1;
            interlock = false;
            subtracting = false;
        }

        public void loadProgram(int p, StreamReader file)
        {
            String line;
            line = "";
            p--;
            while (!file.EndOfStream && line.CompareTo("}") != 0)
            {
                line = file.ReadLine().Trim().ToLower();
                if (line.CompareTo("nummode alpha") == 0) numeratorReceive[p] = 0;
                if (line.CompareTo("nummode beta") == 0) numeratorReceive[p] = 1;
                if (line.CompareTo("nummode 0") == 0) numeratorReceive[p] = 2;
                if (line.CompareTo("numclear true") == 0) numeratorClear[p] = true;
                if (line.CompareTo("numclear false") == 0) numeratorClear[p] = false;
                if (line.CompareTo("denmode alpha") == 0) denominatorReceive[p] = 0;
                if (line.CompareTo("denmode beta") == 0) denominatorReceive[p] = 1;
                if (line.CompareTo("denmode 0") == 0) denominatorReceive[p] = 2;
                if (line.CompareTo("denclear true") == 0) denominatorClear[p] = true;
                if (line.CompareTo("denclear false") == 0) denominatorClear[p] = false;
                if (line.CompareTo("round true") == 0) roundoffSwitch[p] = true;
                if (line.CompareTo("round false") == 0) roundoffSwitch[p] = false;
                if (line.CompareTo("mode div4") == 0) places[p] = 0;
                if (line.CompareTo("mode div7") == 0) places[p] = 1;
                if (line.CompareTo("mode div8") == 0) places[p] = 2;
                if (line.CompareTo("mode div9") == 0) places[p] = 3;
                if (line.CompareTo("mode div10") == 0) places[p] = 4;
                if (line.CompareTo("mode root4") == 0) places[p] = 5;
                if (line.CompareTo("mode root7") == 0) places[p] = 6;
                if (line.CompareTo("mode root8") == 0) places[p] = 7;
                if (line.CompareTo("mode root9") == 0) places[p] = 8;
                if (line.CompareTo("mode root10") == 0) places[p] = 9;
                if (line.CompareTo("interlock true") == 0) interlockSwitch[p] = true;
                if (line.CompareTo("interlock false") == 0) interlockSwitch[p] = false;
                if (line.CompareTo("answer 1") == 0) answer[p] = 0;
                if (line.CompareTo("answer 2") == 0) answer[p] = 1;
                if (line.CompareTo("answer 3") == 0) answer[p] = 2;
                if (line.CompareTo("answer 4") == 0) answer[p] = 3;
                if (line.CompareTo("answer off") == 0) answer[p] = 4;
                if (line.StartsWith("interlockin"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setInterlockIn(p, line);
//                    interlockIn[p] = line;
                }
                if (line.StartsWith("programin"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setProgramIn(p, line);
//                    programIn[p] = line;
                }
                if (line.StartsWith("programout"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setProgramOut(p, line);
//                    programOut[p] = line;
                }
            }
        }

        public void load(StreamReader file)
        {
            int pos;
            String line = "";
            reset();
            while (!file.EndOfStream && line.CompareTo("}") != 0)
            {
                line = file.ReadLine().Trim().ToLower();
                if (line.StartsWith("power"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    powered = Convert.ToBoolean(line);
                }
                if (line.StartsWith("program"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    pos = Convert.ToInt32(line.Substring(0, line.IndexOf(' ')));
                    loadProgram(pos, file);
                }
                if (line.StartsWith("digitout"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDigitOut(line);
//                    digitOut = line;
                }
                if (line.StartsWith("quotientic"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    quotientIC = Convert.ToInt32(line)-1;
                }
                if (line.StartsWith("numeratoric"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    numeratorIC = Convert.ToInt32(line) - 1;
                }
                if (line.StartsWith("denominatoric"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    denominatorIC = Convert.ToInt32(line) - 1;
                }
                if (line.StartsWith("shifteric"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    shifterIC = Convert.ToInt32(line) - 1;
                }
            }
        }

        private void saveProgram(StreamWriter file, int prog)
        {
            Boolean flag;
            flag = false;
            if (numeratorClear[prog]) flag = true;
            if (numeratorReceive[prog] != 0) flag = true;
            if (denominatorClear[prog]) flag = true;
            if (denominatorReceive[prog] != 0) flag = true;
            if (roundoffSwitch[prog]) flag = true;
            if (places[prog] != 0) flag = true;
            if (interlockSwitch[prog]) flag = true;
            if (answer[prog] != 0) flag = true;
            if (interlockIn[prog].Length > 2) flag = true;
            if (programIn[prog].Length > 2) flag = true;
            if (programOut[prog].Length > 2) flag = true;
            if (flag)
            {
                file.WriteLine("  program " + (prog + 1).ToString() + " {");
                if (numeratorClear[prog]) file.WriteLine("    numclear true");
                switch (numeratorReceive[prog])
                {
                    case 0: file.WriteLine("    nummode alpha"); break;
                    case 1: file.WriteLine("    nummode beta"); break;
                    case 2: file.WriteLine("    nummode 0"); break;
                }
                if (denominatorClear[prog]) file.WriteLine("    denclear true");
                switch (denominatorReceive[prog])
                {
                    case 0: file.WriteLine("    denmode alpha"); break;
                    case 1: file.WriteLine("    denmode beta"); break;
                    case 2: file.WriteLine("    denmode 0"); break;
                }
                if (roundoffSwitch[prog]) file.WriteLine("    round true");
                switch (places[prog])
                {
                    case 0: file.WriteLine("    mode div4"); break;
                    case 1: file.WriteLine("    mode div7"); break;
                    case 2: file.WriteLine("    mode div8"); break;
                    case 3: file.WriteLine("    mode div9"); break;
                    case 4: file.WriteLine("    mode div10"); break;
                    case 5: file.WriteLine("    mode root4"); break;
                    case 6: file.WriteLine("    mode root7"); break;
                    case 7: file.WriteLine("    mode root8"); break;
                    case 8: file.WriteLine("    mode root9"); break;
                    case 9: file.WriteLine("    mode root10"); break;
                }
                if (interlockSwitch[prog]) file.WriteLine("    interlock true");
                switch (answer[prog])
                {
                    case 0: file.WriteLine("    answer 1"); break;
                    case 1: file.WriteLine("    answer 2"); break;
                    case 2: file.WriteLine("    answer 3"); break;
                    case 3: file.WriteLine("    answer 4"); break;
                    case 4: file.WriteLine("    answer off"); break;
                }
                if (interlockIn[prog].Length > 2) file.WriteLine("    interlockin " + interlockIn[prog]);
                if (programIn[prog].Length > 2) file.WriteLine("    programin " + programIn[prog]);
                if (programOut[prog].Length > 2) file.WriteLine("    programout " + programOut[prog]);
                file.WriteLine("    }");
            }
        }

        public void save(StreamWriter file)
        {
            int i;
            Boolean flag;
            flag = false;
            if (powered) flag = true;
            for (i = 0; i < 8; i++) if (numeratorClear[i]) flag = true;
            for (i = 0; i < 8; i++) if (numeratorReceive[i] != 0) flag = true;
            for (i = 0; i < 8; i++) if (denominatorClear[i]) flag = true;
            for (i = 0; i < 8; i++) if (denominatorReceive[i] != 0) flag = true;
            for (i = 0; i < 8; i++) if (roundoffSwitch[i]) flag = true;
            for (i = 0; i < 8; i++) if (places[i] != 0) flag = true;
            for (i = 0; i < 8; i++) if (interlockSwitch[i]) flag = true;
            for (i = 0; i < 8; i++) if (answer[i] != 0) flag = true;
            for (i = 0; i < 8; i++) if (interlockIn[i].Length > 2) flag = true;
            for (i = 0; i < 8; i++) if (programIn[i].Length > 2) flag = true;
            for (i = 0; i < 8; i++) if (programOut[i].Length > 2) flag = true;
            if (quotientIC != 1) flag = true;
            if (numeratorIC != 2) flag = true;
            if (denominatorIC != 4) flag = true;
            if (shifterIC != 6) flag = true;
            if (digitOut.Length > 2) flag = true;
            if (flag)
            {
                file.WriteLine("divider {");
                if (powered) file.WriteLine("  power true");
                for (i = 0; i < 8; i++) saveProgram(file, i);
                if (digitOut.Length > 2) file.WriteLine("  digitout " + digitOut);
                if (quotientIC != 1) file.WriteLine("  quotientic " + (quotientIC + 1).ToString());
                if (numeratorIC != 2) file.WriteLine("  numeratoric " + (numeratorIC + 1).ToString());
                if (denominatorIC != 4) file.WriteLine("  denominatoric " + (denominatorIC + 1).ToString());
                if (shifterIC != 6) file.WriteLine("  shifteric " + (shifterIC + 1).ToString());
                file.WriteLine("  }");
            }
        }

        public Boolean getPowered()
        {
            return powered;
        }

        public void setPowered(Boolean b)
        {
            powered = b;
        }

        public Boolean getNumeratorClear(int prog)
        {
            return numeratorClear[prog];
        }

        public void setNumeratorClear(int prog, Boolean b)
        {
            numeratorClear[prog] = b;
        }

        public int getNumeratorReceive(int prog)
        {
            return numeratorReceive[prog];
        }

        public void setNumeratorReceive(int prog, int v)
        {
            numeratorReceive[prog] = v;
        }

        public Boolean getDenominatorClear(int prog)
        {
            return denominatorClear[prog];
        }

        public void setDenominatorClear(int prog, Boolean b)
        {
            denominatorClear[prog] = b;
        }

        public int getDenominatorReceive(int prog)
        {
            return denominatorReceive[prog];
        }

        public void setDenominatorReceive(int prog, int v)
        {
            denominatorReceive[prog] = v;
        }

        public Boolean getRoundoff(int prog)
        {
            return roundoffSwitch[prog];
        }

        public void setRoundoff(int prog, Boolean b)
        {
            roundoffSwitch[prog] = b;
        }

        public int getPlaces(int prog)
        {
            return places[prog];
        }

        public void setPlaces(int prog, int v)
        {
            places[prog] = v;
        }

        public Boolean getInterlockSwitch(int prog)
        {
            return interlockSwitch[prog];
        }

        public void setInterlockSwitch(int prog, Boolean b)
        {
            interlockSwitch[prog] = b;
        }

        public int getAnswerSwitch(int prog)
        {
            return answer[prog];
        }

        public void setAnswerSwitch(int prog, int v)
        {
            answer[prog] = v;
        }

        public String getInterlockIn(int prog)
        {
            return interlockIn[prog];
        }

        public void setInterlockIn(int prog, String s)
        {
            interlockIn[prog] = s;
            interlockInNumber[prog] = machine.getBusNumber(s);
            interlockInPosition[prog] = machine.getBusPosition(s);
        }

        public String getProgramIn(int prog)
        {
            return programIn[prog];
        }

        public void setProgramIn(int prog, String s)
        {
            programIn[prog] = s;
            programInNumber[prog] = machine.getBusNumber(s);
            programInPosition[prog] = machine.getBusPosition(s);
        }

        public String getProgramOut(int prog)
        {
            return programOut[prog];
        }

        public void setProgramOut(int prog, String s)
        {
            programOut[prog] = s;
            programOutNumber[prog] = machine.getBusNumber(s);
            programOutPosition[prog] = machine.getBusPosition(s);
        }

        public String getDigitOut()
        {
            return digitOut;
        }

        public void setDigitOut(String s)
        {
            digitOut = s;
            digitOutNumber = machine.getBusNumber(s);
            digitOutPosition = machine.getBusPosition(s);
        }

        public int getQuotientIC()
        {
            return quotientIC;
        }

        public void setQuotientIC(int v)
        {
            quotientIC = v;
        }

        public int getNumeratorIC()
        {
            return numeratorIC;
        }

        public void setNumeratorIC(int v)
        {
            numeratorIC = v;
        }

        public int getDenominatorIC()
        {
            return denominatorIC;
        }

        public void setDenominatorIC(int v)
        {
            denominatorIC = v;
        }

        public int getShifterIC()
        {
            return shifterIC;
        }

        public void setShifterIC(int v)
        {
            shifterIC = v;
        }

        public void cycle1(int clocks)        /* Process outputs */
        {
            int i;
            int d;
            int bus;
            if (!powered) return;
            for (i = 0; i < 8; i++)
//                if (programIn[i].Length > 2)
//                {
//                    bus = machine.getBus(programIn[i]);
//                    if (bus != 0)
//                    {
//                        trigger = i;
//                        machine.log("Divider: Operation " + (i + 1).ToString() + " triggered by " + programIn[i]);
//                        phase = 0;
//                    }
//                }
            if (programInNumber[i] >= 0)
            {
                bus = machine.getBus(programInNumber[i],programInPosition[i]);
                if (bus != 0)
                {
                    trigger = i;
                    machine.log("Divider: Operation " + (i + 1).ToString() + " triggered by " + programIn[i]);
                    phase = 0;
                }
            }
            if ((phase == 6 || phase == 11 || ((phase == 4 || phase == 7 || phase == 8 || phase == 9) && divRoot)) && (clocks & EniacCyclingUnit.C9P) == EniacCyclingUnit.C9P)
            {
                d = 0;
                for (i = 0; i < 11; i++)
                {
                    d <<= 1;
                    if (pulses[i] > 0) d |= 1;
                    pulses[i]--;
                }
//                if (digitOut.Length > 2) machine.setBus(digitOut, d);
                if (digitOutNumber >= 0) machine.setBus(digitOutNumber, -1, d);
            }
            if (subtracting && (phase == 6 || phase == 11 || ((phase == 4 || phase == 7 || phase == 8 || phase == 9) && divRoot)) && (clocks & EniacCyclingUnit.C11P) == EniacCyclingUnit.C11P)
            {
                d = 1 << (10 - pos);
//                if (digitOut.Length > 2) machine.setBus(digitOut, d);
                if (digitOutNumber >= 0) machine.setBus(digitOutNumber, -1, d);
            }
        }

        public void cycle2(int clocks)        /* Process inputs */
        {
            int i;
            int bus;
            if (!powered) return;
            if (phase > 0)
            {
                for (i=0; i<8; i++)
//                    if (interlockIn[i].Length > 2)
//                    {
//                        bus = machine.getBus(interlockIn[i]);
//                        if (bus != 0)
//                        {
//                            interlock = true;
//                            machine.log("Divider: interlock received");
//                        }
//                    }
                if (interlockInNumber[i] >= 0)
                {
                    bus = machine.getBus(interlockInNumber[i],interlockInPosition[i]);
                    if (bus != 0)
                    {
                        interlock = true;
                        machine.log("Divider: interlock received");
                    }
                }
            }
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                if (phase == 0 && operation < 0 && trigger >= 0)
                {
                    operation = trigger;
                    trigger = -1;
                    divRoot = (places[operation] < 5) ? false : true;
                    machine.log("Divider: Operation " + (operation + 1).ToString() + " started");
                    if (numeratorReceive[operation] == 0) machine.getAccumulator(numeratorIC).setSpecialMode(EniacAccumulator.MODE_ALPHA, false, 1);
                    if (numeratorReceive[operation] == 1) machine.getAccumulator(numeratorIC).setSpecialMode(EniacAccumulator.MODE_BETA, false, 1);
                    if (!divRoot && denominatorReceive[operation] == 0) machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_ALPHA, false, 1);
                    if (!divRoot && denominatorReceive[operation] == 1) machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_BETA, false, 1);
                    interlock = !interlockSwitch[operation];
                    phase = 1;
                }
                else if (phase == 1)
                {
                    if (!divRoot) machine.getAccumulator(quotientIC).setSpecialMode(EniacAccumulator.MODE_0, true, 1);
                    machine.getAccumulator(shifterIC).setSpecialMode(EniacAccumulator.MODE_0, true, 1);
                    phase = 2;
                }
                else if (phase == 2)
                {
                    pos = 2;
                    lastDigit = (divRoot) ? places[operation] -1 : places[operation] + 4;
                    if (lastDigit > 4) lastDigit += 2;
                    phase = (divRoot) ? 3 : 4;
                }
                else if (phase == 3)
                {
                    for (i = 0; i < 11; i++) pulses[i] = 0;
                    addsub = false;
                    pulses[pos] = 1;
                    machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_GAMMA, false, 1);
                    subtracting = false;
                    phase = 4;
                }
                else if (phase == 4)
                {
                    sign = machine.getAccumulator(numeratorIC).getSign();
                    addsub = (machine.getAccumulator(numeratorIC).getSign() == machine.getAccumulator(denominatorIC).getSign()) ? true : false;
                    machine.getAccumulator(numeratorIC).setSpecialMode(EniacAccumulator.MODE_GAMMA, false, 1);
                    if (addsub) machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_S, false, 1);
                    else machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_A, false, 1);
                    phase = 5;
                }
                else if (phase == 5)
                {
                    for (i = 0; i < 11; i++) pulses[i] = 0;
                    pulses[pos] = (divRoot) ? 2 : 1;
                    subtracting = false;
                    if (!addsub)
                    {
                        for (i = 0; i <= pos; i++) pulses[i] = 9 - pulses[i];
                        subtracting = true;
                    }
                    if (!divRoot) machine.getAccumulator(quotientIC).setSpecialMode(EniacAccumulator.MODE_ALPHA, false, 1);
                    if (divRoot) machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_GAMMA, false, 1);
                    phase = 6;
                }
                else if (phase == 6)
                {
                    if (machine.getAccumulator(numeratorIC).getSign() == sign) phase = 4;
                    else
                    {
                        machine.getAccumulator(numeratorIC).setSpecialMode(EniacAccumulator.MODE_A, true, 1);
                        machine.getAccumulator(shifterIC).setSpecialMode(EniacAccumulator.MODE_ALPHA, false, 1);
                        if (divRoot)
                        {
                            for (i = 0; i < 11; i++) pulses[i] = 0;
                            pulses[pos] = 1;
                            subtracting = false;
                            if (addsub)
                            {
                                for (i = 0; i <= pos; i++) pulses[i] = 9 - pulses[i];
                                subtracting = true;
                            }
                            machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_GAMMA, false, 1);
                        }
                        phase = 7;
                    }
                }
                else if (phase == 7)
                {
                    machine.getAccumulator(numeratorIC).setSpecialMode(EniacAccumulator.MODE_GAMMA, false, 1);
                    machine.getAccumulator(shifterIC).setSpecialMode(EniacAccumulator.MODE_A, true, 1);
                    pos++;
                    if (divRoot && pos < lastDigit)
                    {
                        for (i = 0; i < 11; i++) pulses[i] = 0;
                        pulses[pos] = 1;
                        if (addsub) for (i = 0; i <= pos; i++) pulses[i] = 9 - pulses[i];
                        subtracting = addsub;
                        machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_GAMMA, false, 1);
                    }
                    if (pos <= lastDigit) phase = 4;
                    else if (divRoot) phase = 8;
                    else if (roundoffSwitch[operation]) phase = 9;
                    else phase = 12;
                }
                else if (phase == 8)
                {
                    for (i = 0; i < 11; i++) pulses[i] = 0;
                    pulses[lastDigit] = 2;
                    if (!addsub) for (i = 0; i <= lastDigit; i++) pulses[i] = 9 - pulses[i];
                    subtracting = !addsub;
                    machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_GAMMA, false, 1);
                    pos = lastDigit;
                    if (roundoffSwitch[operation]) phase = 9;
                    else phase = 12;
                }
                else if (phase == 9)
                {
                    sign = machine.getAccumulator(numeratorIC).getSign();
                    addsub = (machine.getAccumulator(numeratorIC).getSign() == machine.getAccumulator(denominatorIC).getSign()) ? true : false;
                    machine.getAccumulator(numeratorIC).setSpecialMode(EniacAccumulator.MODE_GAMMA, false, 5);
                    if (addsub) machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_S, false, 5);
                    else machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_A, false, 5);
                    phase = 10;
                }
                else if (phase == 10)    /* Check sign and add/sub if needed */
                {
                    if (!machine.getAccumulator(numeratorIC).busy())
                    {
                        if (sign != machine.getAccumulator(numeratorIC).getSign()) phase = 12;
                        else
                        {
                            for (i = 0; i < 11; i++) pulses[i] = 0;
                            pulses[lastDigit] = 1;
                            subtracting = false;
                            if (!addsub)
                            {
                                for (i = 0; i <= lastDigit; i++) pulses[i] = 9 - pulses[i];
                                subtracting = true;
                            }
                            machine.getAccumulator(quotientIC).setSpecialMode(EniacAccumulator.MODE_ALPHA, false, 1);
                            phase = 11;
                        }
                    }
                }
                else if (phase == 11)
                {
                    phase = 12;
                }
                else if (phase == 12)
                {
                    if (interlock)
                    {
//                        if (programOut[operation].Length > 2)
//                        {
//                            machine.setBus(programOut[operation], 1);
//                            machine.log("Divider: sent pulse " + programOut[operation]);
//                        }
                        if (programOutNumber[operation] >= 0)
                        {
                            machine.setBus(programOutNumber[operation], programOutPosition[operation], 1);
                            machine.log("Divider: sent pulse " + programOut[operation]);
                        }
                        if (numeratorClear[operation]) machine.getAccumulator(numeratorIC).setSpecialMode(EniacAccumulator.MODE_0, true, 1);
                        if (denominatorClear[operation]) machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_0, true, 1);
                        if (answer[operation] == 0) machine.getAccumulator(quotientIC).setSpecialMode(EniacAccumulator.MODE_A, false, 1);
                        if (answer[operation] == 1) machine.getAccumulator(quotientIC).setSpecialMode(EniacAccumulator.MODE_S, false, 1);
                        if (!divRoot)
                        {
                            if (answer[operation] == 2) machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_A, false, 1);
                            if (answer[operation] == 3) machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_S, false, 1);
                        }
                        else
                        {
                            if (answer[operation] == 2) machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_A, denominatorClear[operation], 1);
                            if (answer[operation] == 3) machine.getAccumulator(denominatorIC).setSpecialMode(EniacAccumulator.MODE_S, denominatorClear[operation], 1);
                        }
                        operation = -1;
                        phase = -1;
                    }
                }
            }
        }

    }
}
