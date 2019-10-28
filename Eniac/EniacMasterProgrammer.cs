using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eniac
{
    class EniacMasterProgrammer
    {
        private Machine machine;
        private int[] decades;
        private int[] steppers;
        private Boolean powered;
        private int[] decadeSwitches;
        private Boolean[] associationSwitches;
        private String[] decadeDirectInputs;
        private int[] decadeDirectInputsNumber;
        private int[] decadeDirectInputsPosition;
        private String[] stepperDirectInputs;
        private int[] stepperDirectInputsNumber;
        private int[] stepperDirectInputsPosition;
        private String[] stepperInputs;
        private int[] stepperInputsNumber;
        private int[] stepperInputsPosition;
        private String[] stepperDirectClear;
        private int[] stepperDirectClearNumber;
        private int[] stepperDirectClearPosition;
        private String[] stepperOutputs;
        private int[] stepperOutputsNumber;
        private int[] stepperOutputsPosition;
        private int[] stepperClearSwitches;
        private Boolean[] inputFF;
        private Boolean[] outputFF;
        private Boolean[] stepperFF;
        private Boolean listening;

        public EniacMasterProgrammer(Machine m)
        {
            machine = m;
            decades = new int[20];
            steppers = new int[10];
            decadeSwitches = new int[120];
            associationSwitches = new Boolean[8];
            decadeDirectInputs = new String[20];
            decadeDirectInputsNumber = new int[20];
            decadeDirectInputsPosition = new int[20];
            stepperClearSwitches = new int[10];
            stepperDirectInputs = new String[10];
            stepperDirectInputsNumber = new int[10];
            stepperDirectInputsPosition = new int[10];
            stepperInputs = new String[10];
            stepperInputsNumber = new int[10];
            stepperInputsPosition = new int[10];
            stepperDirectClear = new String[10];
            stepperDirectClearNumber = new int[10];
            stepperDirectClearPosition = new int[10];
            stepperOutputs = new String[60];
            stepperOutputsNumber = new int[60];
            stepperOutputsPosition = new int[60];
            inputFF = new Boolean[10];
            outputFF = new Boolean[10];
            stepperFF = new Boolean[10];
            powered = false;
            reset();
        }

        public void reset()
        {
            int i;
            for (i = 0; i < 120; i++) decadeSwitches[i] = 0;
            for (i = 0; i < 8; i++) associationSwitches[i] = false;
            for (i = 0; i < 20; i++)
            {
                decadeDirectInputs[i] = "";
                decadeDirectInputsNumber[i] = -1;
                decadeDirectInputsPosition[i] = -1;
            }
            for (i = 0; i < 10; i++) stepperClearSwitches[i] = 5;
            for (i = 0; i < 10; i++)
            {
                stepperDirectInputs[i] = "";
                stepperDirectInputsNumber[i] = -1;
                stepperDirectInputsPosition[i] = -1;
            }
            for (i = 0; i < 10; i++)
            {
                stepperInputs[i] = "";
                stepperInputsNumber[i] = -1;
                stepperInputsPosition[i] = -1;
            }
            for (i = 0; i < 10; i++)
            {
                stepperDirectClear[i] = "";
                stepperDirectClearNumber[i] = -1;
                stepperDirectClearPosition[i] = -1;
            }
            for (i = 0; i < 60; i++)
            {
                stepperOutputs[i] = "";
                stepperOutputsNumber[i] = -1;
                stepperOutputsPosition[i] = -1;
            }
            powered = false;
            listening = false;
            allClear();
        }

        public void allClear()
        {
            int i;
            for (i = 0; i < 20; i++) decades[i] = 0;
            for (i = 0; i < 10; i++) steppers[i] = 0;
            for (i = 0; i < 10; i++) inputFF[i] = false;
            for (i = 0; i < 10; i++) outputFF[i] = false;
            for (i = 0; i < 10; i++) stepperFF[i] = false;
            listening = false;
        }

        private void loadStepper(StreamReader file, int st)
        {
            int num;
            String line;
            String temp;
            line = "";
            while (!file.EndOfStream && line.CompareTo("}") != 0)
            {
                line = file.ReadLine().Trim().ToLower();
                if (line.StartsWith("clear"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    stepperClearSwitches[st] = Convert.ToInt32(line) - 1;
                }
                if (line.StartsWith("directin"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setStepperDirectInput(st, line);
//                    stepperDirectInputs[st] = line;
                }
                if (line.StartsWith("in"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setStepperInput(st, line);
//                    stepperInputs[st] = line;
                }
                if (line.StartsWith("directclear"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setStepperDirectClear(st, line);
//                    stepperDirectClear[st] = line;
                }
                if (line.StartsWith("out"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    temp = line.Substring(0, line.IndexOf(' ')).Trim().ToUpper();
                    num = Convert.ToInt32(temp) - 1;
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setStepperOutput(st, num, line);
//                    stepperOutputs[st*6+num] = line;
                }
            }
        }

        public void load(StreamReader file)
        {
            String line;
            String temp;
            int decade;
            int stage;
            int value;
            line = "";
            reset();
            while (!file.EndOfStream && line.CompareTo("}") != 0)
            {
                line = file.ReadLine().Trim().ToLower();
                if (line.StartsWith("power"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    powered = Convert.ToBoolean(line);
                }
                if (line.StartsWith("decade "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    temp = line.Substring(0,line.IndexOf(' ')).Trim().ToUpper();
                    decade = Convert.ToInt32(temp)-1;
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    temp = line.Substring(0, line.IndexOf(' ')).Trim().ToUpper();
                    stage = Convert.ToInt32(temp)-1;
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    value = Convert.ToInt32(line);
                    decadeSwitches[stage * 20 + decade] = value;
                }
                if (line.StartsWith("association 1 k")) associationSwitches[0] = true;
                if (line.StartsWith("association 1 j")) associationSwitches[0] = false;
                if (line.StartsWith("association 2 j")) associationSwitches[1] = true;
                if (line.StartsWith("association 2 h")) associationSwitches[1] = false;
                if (line.StartsWith("association 3 h")) associationSwitches[2] = true;
                if (line.StartsWith("association 3 g")) associationSwitches[2] = false;
                if (line.StartsWith("association 4 g")) associationSwitches[3] = true;
                if (line.StartsWith("association 4 f")) associationSwitches[3] = false;
                if (line.StartsWith("association 5 e")) associationSwitches[4] = true;
                if (line.StartsWith("association 5 d")) associationSwitches[4] = false;
                if (line.StartsWith("association 6 d")) associationSwitches[5] = true;
                if (line.StartsWith("association 6 c")) associationSwitches[5] = false;
                if (line.StartsWith("association 7 c")) associationSwitches[6] = true;
                if (line.StartsWith("association 7 b")) associationSwitches[6] = false;
                if (line.StartsWith("association 8 b")) associationSwitches[7] = true;
                if (line.StartsWith("association 8 a")) associationSwitches[7] = false;
                if (line.StartsWith("decadedirectin"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    temp = line.Substring(0, line.IndexOf(' ')).Trim().ToUpper();
                    decade = Convert.ToInt32(temp) - 1;
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDecadeDirectInput(decade, line);
//                    decadeDirectInputs[decade] = line;
                }
                if (line.StartsWith("stepper "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    temp = line.Substring(0, line.IndexOf(' ')).Trim().ToUpper();
                    decade = temp[0] - 'A';
                    loadStepper(file, decade);
                }
            }
        }

        public void save(StreamWriter file)
        {
            int i,j;
            int decade;
            int stage;
            Boolean flag;
            flag = false;
            if (powered) flag = true;
            for (stage = 0; stage < 6; stage++)
                for (decade = 0; decade < 20; decade++)
                    if (decadeSwitches[stage * 20 + decade] != 0) flag = true;
            for (i = 0; i < 8; i++) if (associationSwitches[i]) flag = true;
            for (i = 0; i < 20; i++) if (decadeDirectInputs[i].Length > 1) flag = true;
            for (i = 0; i < 10; i++) if (stepperClearSwitches[i] != 5) flag = true;
            for (i = 0; i < 10; i++) if (stepperDirectInputs[i].Length > 1) flag = true;
            for (i = 0; i < 10; i++) if (stepperInputs[i].Length > 1) flag = true;
            for (i = 0; i < 10; i++) if (stepperDirectClear[i].Length > 1) flag = true;
            if (flag)
            {
                file.WriteLine("master {");
                if (powered) file.WriteLine("  power " + powered.ToString());
                for (stage = 0; stage < 6; stage++)
                    for (decade = 0; decade < 20; decade++)
                        if (decadeSwitches[stage * 20 + decade] != 0) file.WriteLine("  decade " + (decade+1).ToString() + " " +
                                                                                                   (stage+1).ToString() + " " +
                                                                                                   decadeSwitches[stage * 20 + decade].ToString());
                if (associationSwitches[0]) file.WriteLine("  association 1 K");
                if (associationSwitches[1]) file.WriteLine("  association 2 J");
                if (associationSwitches[2]) file.WriteLine("  association 3 H");
                if (associationSwitches[3]) file.WriteLine("  association 4 G");
                if (associationSwitches[4]) file.WriteLine("  association 5 E");
                if (associationSwitches[5]) file.WriteLine("  association 6 D");
                if (associationSwitches[6]) file.WriteLine("  association 7 C");
                if (associationSwitches[7]) file.WriteLine("  association 8 B");
                for (i = 0; i < 20; i++)
                    if (decadeDirectInputs[i].Length > 1) file.WriteLine("  decadedirectin " + (i+1).ToString() + " " + decadeDirectInputs[i]);
                for (i = 0; i < 10; i++)
                {
                    flag = false;
                    if (stepperClearSwitches[i] != 5) flag = true;
                    if (stepperDirectInputs[i].Length > 1) flag = true;
                    if (stepperInputs[i].Length > 1) flag = true;
                    if (stepperDirectClear[i].Length > 1) flag = true;
                    for (j = 0; j < 6; j++) if (stepperOutputs[i * 6 + j].Length > 1) flag = true;
                    if (flag)
                    {
                        file.WriteLine("  stepper " + Convert.ToChar(i + 'A').ToString() + " {");
                        if (stepperClearSwitches[i] != 5) file.WriteLine("    clear " + (stepperClearSwitches[i] + 1).ToString());
                        if (stepperDirectInputs[i].Length > 1) file.WriteLine("    directin " + stepperDirectInputs[i]);
                        if (stepperInputs[i].Length > 1) file.WriteLine("    in " + stepperInputs[i]);
                        if (stepperDirectClear[i].Length > 1) file.WriteLine("    directclear " + stepperDirectClear[i]);
                        for (j = 0; j < 6; j++) if (stepperOutputs[i * 6 + j].Length > 1)
                                file.WriteLine("    out " + (j + 1).ToString() + " " + stepperOutputs[i * 6 + j]);
                        file.WriteLine("    }");
                    }
                }
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

        public int getDecadeSwitch(int decade, int stage)
        {
            stage--;
            return decadeSwitches[stage * 20 + decade];
        }

        public void setDecadeSwitch(int decade, int stage, int value)
        {
            stage--;
            decadeSwitches[stage * 20 + decade] = value;
        }

        public Boolean getAssociationSwitch(int i)
        {
            return associationSwitches[i];
        }

        public void setAssociationSwitch(int i, Boolean b)
        {
            associationSwitches[i] = b;
        }

        public String getDecadeDirectInput(int i)
        {
            return decadeDirectInputs[i];
        }

        public void setDecadeDirectInput(int i, String s)
        {
            decadeDirectInputs[i] = s;
            decadeDirectInputsNumber[i] = machine.getBusNumber(s);
            decadeDirectInputsPosition[i] = machine.getBusPosition(s);
        }

        public int getStepperClearSwitch(int i)
        {
            return stepperClearSwitches[i];
        }

        public void setStepperClearSwitch(int i, int v)
        {
            stepperClearSwitches[i] = v;
        }

        public String getStepperDirectInput(int i)
        {
            return stepperDirectInputs[i];
        }

        public void setStepperDirectInput(int i, String s)
        {
            stepperDirectInputs[i] = s;
            stepperDirectInputsNumber[i] = machine.getBusNumber(s);
            stepperDirectInputsPosition[i] = machine.getBusPosition(s);
        }

        public String getStepperInput(int i)
        {
            return stepperInputs[i];
        }

        public void setStepperInput(int i, String s)
        {
            stepperInputs[i] = s;
            stepperInputsNumber[i] = machine.getBusNumber(s);
            stepperInputsPosition[i] = machine.getBusPosition(s);
        }

        public String getStepperDirectClear(int i)
        {
            return stepperDirectClear[i];
        }

        public void setStepperDirectClear(int i, String s)
        {
            stepperDirectClear[i] = s;
            stepperDirectClearNumber[i] = machine.getBusNumber(s);
            stepperDirectClearPosition[i] = machine.getBusPosition(s);
        }

        public String getStepperOutput(int stepper, int stage)
        {
            return stepperOutputs[stepper * 6 + stage];
        }

        public void setStepperOutput(int stepper, int stage, String s)
        {
            stepperOutputs[stepper * 6 + stage] = s;
            stepperOutputsNumber[stepper * 6 + stage] = machine.getBusNumber(s);
            stepperOutputsPosition[stepper * 6 + stage] = machine.getBusPosition(s);
        }

        private int getStepperForDecade(int d)
        {
            switch (d)
            {
                case 0: return 9;
                case 1: return (associationSwitches[0]) ? 9 : 8;
                case 2: return 8;
                case 3: return (associationSwitches[1]) ? 8 : 7;
                case 4: return 7;
                case 5: return 7;
                case 6: return 7;
                case 7: return (associationSwitches[2]) ? 7 : 6;
                case 8: return 6;
                case 9: return (associationSwitches[3]) ? 6 : 5;
                case 10: return 4;
                case 11: return (associationSwitches[4]) ? 4 : 3;
                case 12: return 3;
                case 13: return (associationSwitches[5]) ? 3 : 2;
                case 14: return 2;
                case 15: return 2;
                case 16: return 2;
                case 17: return (associationSwitches[6]) ? 2 : 1;
                case 18: return 1;
                case 19: return (associationSwitches[7]) ? 1 : 0;

            }
            return 0;
        }

        private void clearDecadesForStepper(int st)
        {
            switch (st)
            {
                case 0:
                    if (associationSwitches[7] == false) decades[19] = 0;
                    break;
                case 1:
                    if (associationSwitches[7] == true) decades[19] = 0;
                    decades[18] = 0;
                    if (associationSwitches[6] == false) decades[17] = 0;
                    break;
                case 2:
                    if (associationSwitches[6] == true) decades[17] = 0;
                    decades[16] = 0;
                    decades[15] = 0;
                    decades[14] = 0;
                    if (associationSwitches[5] == false) decades[13] = 0;
                    break;
                case 3:
                    if (associationSwitches[5] == true) decades[13] = 0;
                    decades[12] = 0;
                    if (associationSwitches[4] == false) decades[11] = 0;
                    break;
                case 4:
                    if (associationSwitches[4] == true) decades[11] = 0;
                    decades[10] = 0;
                    break;
                case 5:
                    if (associationSwitches[3] == false) decades[9] = 0;
                    break;
                case 6:
                    if (associationSwitches[3] == true) decades[9] = 0;
                    decades[8] = 0;
                    if (associationSwitches[2] == false) decades[7] = 0;
                    break;
                case 7:
                    if (associationSwitches[2] == true) decades[7] = 0;
                    decades[6] = 0;
                    decades[5] = 0;
                    decades[4] = 0;
                    if (associationSwitches[1] == false) decades[3] = 0;
                    break;
                case 8:
                    if (associationSwitches[1] == true) decades[3] = 0;
                    decades[2] = 0;
                    if (associationSwitches[0] == false) decades[1] = 0;
                    break;
                case 9:
                    if (associationSwitches[0] == true) decades[1] = 0;
                    decades[0] = 0;
                    break;
            }
        }

        private int getDecadeGroupStart(int st)
        {
            int start;
            start = -1;
            switch (st)
            {
                case 0:   /* A */
                    start = (associationSwitches[7]) ? -1 : 19;
                    break;
                case 1:  /* B */
                    start = (associationSwitches[6]) ? 18 : 17;
                    break;
                case 2:  /* C */
                    start = (associationSwitches[5]) ? 14 : 13;
                    break;
                case 3:  /* D */
                    start = (associationSwitches[4]) ? 12 : 11;
                    break;
                case 4:  /* E */
                    start = 10;
                    break;
                case 5:   /* F */
                    start = (associationSwitches[3]) ? -1 : 9;
                    break;
                case 6:  /* G */
                    start = (associationSwitches[2]) ? 8 : 7;
                    break;
                case 7:  /* H */
                    start = (associationSwitches[1]) ? 4 : 3;
                    break;
                case 8:  /* J */
                    start = (associationSwitches[0]) ? 2 : 1;
                    break;
                case 9:  /* K */
                    start = 10;
                    break;
            }
            return start;
        }

        private int getDecadeGroupEnd(int st)
        {
            int end;
            end = -1;
            switch (st)
            {
                case 0:   /* A */
                    end = (associationSwitches[7]) ? -1 : 19;
                    break;
                case 1:  /* B */
                    end = (associationSwitches[7]) ? 19 : 18;
                    break;
                case 2:  /* C */
                    end = (associationSwitches[6]) ? 17 : 16;
                    break;
                case 3:  /* D */
                    end = (associationSwitches[5]) ? 13 : 12;
                    break;
                case 4:  /* E */
                    end = (associationSwitches[4]) ? 11 : 10;
                    break;
                case 5:   /* F */
                    end = (associationSwitches[3]) ? -1 : 9;
                    break;
                case 6:  /* G */
                    end = (associationSwitches[3]) ? 9 : 8;
                    break;
                case 7:  /* H */
                    end = (associationSwitches[2]) ? 7 : 6;
                    break;
                case 8:  /* J */
                    end = (associationSwitches[1]) ? 3 : 2;
                    break;
                case 9:  /* K */
                    end = (associationSwitches[0]) ? 1 : 0;
                    break;
            }

            return end;
        }

        private Boolean checkDecades(int st)
        {
            int i;
            Boolean ret;
            int start;
            int end;
            start = getDecadeGroupStart(st);
            end = getDecadeGroupEnd(st);
            ret = false;
            if (start >= 0 && end >= 0)
            {
                ret = true;
                for (i = start; i <= end; i++)
                    if (decades[i] != decadeSwitches[steppers[st] * 20 + i]) ret = false;
            }
            return ret;
        }

        private Boolean incDecadesForStepper(int st,int start)
        {
            Boolean ret;
            int i;
            int carry;
            int end;
            carry = 1;
            if (start < 0) start = getDecadeGroupStart(st);
            end = getDecadeGroupEnd(st);
            if (start >= 0 && end >= 0)
            {
                for (i = start; i <= end; i++)
                {
                    decades[i] += carry;
                    carry = (decades[i] > 9) ? 1 : 0;
                    if (decades[i] > 9) decades[i] -= 10;
                }

            }
            ret = checkDecades(st);
            return ret;
        }

        public String getDecades()
        {
            int i;
            String ret;
            ret = "";
            for (i = 19; i >= 0; i--)
                ret += Convert.ToChar(decades[i] + '0').ToString();
            return ret;
        }

        public String getSteppers()
        {
            int i;
            String ret;
            ret = "";
            for (i = 0; i < 10; i++)
                ret += Convert.ToChar(steppers[i] + '1').ToString();
            return ret;
        }

        public void cycle1(int clocks)      /* This sets any outputs */
        {
            int i;
            if (powered == false) return;
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                for (i = 0; i < 10; i++)
                {
                    if (stepperFF[i])
                    {
                        steppers[i]++;
                        if (steppers[i] > stepperClearSwitches[i]) steppers[i] = 0;
                        clearDecadesForStepper(i);
                        stepperFF[i] = false;
                    }
                    if (outputFF[i])
                    {
//                        if (stepperOutputs[i * 6 + (steppers[i])].Length > 1)
//                        {
//                            machine.setBus(stepperOutputs[i * 6 + (steppers[i])], 1);
//                            machine.log("Master: Stepper " + Convert.ToChar(i + 65).ToString() + " sent pulse " + stepperOutputs[i * 6 + (steppers[i])]);
//                        }
                        if (stepperOutputsNumber[i * 6 + (steppers[i])] >= 0)
                        {
                            machine.setBus(stepperOutputsNumber[i * 6 + (steppers[i])], stepperOutputsPosition[i*6 + (steppers[i])], 1);
                            machine.log("Master: Stepper " + Convert.ToChar(i + 65).ToString() + " sent pulse " + stepperOutputs[i * 6 + (steppers[i])]);
                        }
                        outputFF[i] = false;
                        if (incDecadesForStepper(i, -1)) stepperFF[i] = true;
                    }
                }
            }
        }

        public void cycle2(int clocks)      /* This checks any inputs */
        {
            int i;
            int bus;
            if (powered == false) return;
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP) listening = true;
            if ((clocks & EniacCyclingUnit.C4P) == EniacCyclingUnit.C4P) listening = false;
            for (i = 0; i < 20; i++)
//                if (decadeDirectInputs[i].Length > 2)
//                {
//                    bus = machine.getBus(decadeDirectInputs[i]);
//                    if (bus != 0)
//                    {
//                        if (incDecadesForStepper(getStepperForDecade(i), i)) stepperFF[getStepperForDecade(i)] = true;
//                    }
//                }
            if (decadeDirectInputsNumber[i] >= 0)
            {
                bus = machine.getBus(decadeDirectInputsNumber[i],decadeDirectInputsPosition[i]);
                if (bus != 0)
                {
                    if (incDecadesForStepper(getStepperForDecade(i), i)) stepperFF[getStepperForDecade(i)] = true;
                }
            }
            for (i = 0; i < 10; i++)
            {
//                if (stepperDirectInputs[i].Length > 2)
//                {
//                    bus = machine.getBus(stepperDirectInputs[i]);
//                    if (bus != 0)
//                    {
//                        steppers[i]++;
//                        if (steppers[i] > stepperClearSwitches[i]) steppers[i] = 0;
//                        machine.log("Master: Stepper " + Convert.ToChar(i + 65).ToString() + " stepped by " + stepperDirectInputs[i]);
//                    }
//                }
                if (stepperDirectInputsNumber[i] >= 0)
                {
                    bus = machine.getBus(stepperDirectInputsNumber[i], stepperDirectInputsPosition[i]);
                    if (bus != 0)
                    {
                        steppers[i]++;
                        if (steppers[i] > stepperClearSwitches[i]) steppers[i] = 0;
                        machine.log("Master: Stepper " + Convert.ToChar(i + 65).ToString() + " stepped by " + stepperDirectInputs[i]);
                    }
                }
                //                if (stepperDirectClear[i].Length > 2)
//                {
//                    bus = machine.getBus(stepperDirectClear[i]);
//                    if (bus != 0)
//                    {
//                        steppers[i] = 0;
//                        clearDecadesForStepper(i);
//                        machine.log("Master: Stepper " + Convert.ToChar(i + 65).ToString() + " cleared by " + stepperDirectClear[i]);
//                    }
//                }
                if (stepperDirectClearNumber[i] >= 0)
                {
                    bus = machine.getBus(stepperDirectClearNumber[i], stepperDirectClearPosition[i]);
                    if (bus != 0)
                    {
                        steppers[i] = 0;
                        clearDecadesForStepper(i);
                        machine.log("Master: Stepper " + Convert.ToChar(i + 65).ToString() + " cleared by " + stepperDirectClear[i]);
                    }
                }
            }
            if (listening)
                for (i = 0; i < 10; i++)
                    //                    if (stepperInputs[i].Length > 2)
                    //                    {
                    //                        bus = machine.getBus(stepperInputs[i]);
                    //                        if (bus != 0)
                    //                        {
                    //                            inputFF[i] = true;
                    //                            machine.log("Master: Stepper " + Convert.ToChar(i + 65).ToString() + " input triggered by " + stepperInputs[i]);
                    //                        }
                    //                    }
                    if (stepperInputsNumber[i] >= 0)
                    {
                        bus = machine.getBus(stepperInputsNumber[i], stepperInputsPosition[i]);
                        if (bus != 0)
                        {
                            inputFF[i] = true;
                            machine.log("Master: Stepper " + Convert.ToChar(i + 65).ToString() + " input triggered by " + stepperInputs[i]);
                        }
                    }
            if ((clocks & EniacCyclingUnit.C4P) == EniacCyclingUnit.C4P)
            {
                for (i = 0; i < 10; i++)
                    if (inputFF[i])
                    {
                        outputFF[i] = true;
                        inputFF[i] = false;
                    }
            }

        }

    }
}
