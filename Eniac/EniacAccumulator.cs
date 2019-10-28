using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

// operation codes
// 0 - a
// 1 - b
// 2 - c
// 3 - d
// 4 - e
// 5 - 0
// 6 - A
// 7 - AS
// 8 - S

namespace Eniac
{
    class EniacAccumulator
    {
        public const int MODE_ALPHA = 0;
        public const int MODE_BETA = 1;
        public const int MODE_GAMMA = 2;
        public const int MODE_DELTA = 3;
        public const int MODE_EPSILON = 4;
        public const int MODE_0 = 5;
        public const int MODE_A = 6;
        public const int MODE_AS = 7;
        public const int MODE_S = 8;

        private int[] decades;
        private Boolean sign;          /* False = positive, true = negative */
        private Boolean powered;
        private int operation;           /* -1 = idle */
        private int operationTrigger;       /* Holds which operation needs to be triggered */
        private int operationRepeats;
        private int[] operationSwitches;
        private int[] repeaters;
        private String[] programInputs;
        private int[] programInputsNumber;
        private int[] programInputsPosition;
        private String[] programOutputs;
        private int[] programOutputsNumber;
        private int[] programOutputsPosition;
        private String[] digitInputs;
        private int[] digitInputsNumber;
        private int[] digitInputsPosition;
        private String[] digitOutputs;
        private int[] digitOutputsNumber;
        private int[] digitOutputsPosition;
        private Boolean[] clearSwitches;
        private String[] inputFilters;
        private int significantFigures;
        private Machine machine;
        private int number;
        private int currentOperation;      /* Holds current operation: a b c d e 0 A AS S */
        private Boolean currentClear;      /* Clear for current operation */
        private int leftAccumulator;
        private int rightAccumulator;
        private Boolean linkOperation;
        private Boolean receiveA;

        public EniacAccumulator(Machine m,int n)
        {
            decades = new int[10];
            operationSwitches = new int[12];
            repeaters = new int[8];
            programInputs = new String[12];
            programInputsNumber = new int[12];
            programInputsPosition = new int[12];
            programOutputs = new String[8];
            programOutputsNumber = new int[8];
            programOutputsPosition = new int[8];
            digitInputs = new String[5];
            digitInputsNumber = new int[5];
            digitInputsPosition = new int[5];
            digitOutputs = new String[2];
            digitOutputsNumber = new int[2];
            digitOutputsPosition = new int[2];
            clearSwitches = new Boolean[13];
            inputFilters = new String[5];
            machine = m;
            number = n;
            reset();
        }

        public void reset()
        {
            int i;
            for (i = 0; i < 12; i++) operationSwitches[i] = 0;
            for (i = 0; i < 8; i++) repeaters[i] = 1;
            for (i = 0; i < 12; i++)
            {
                programInputs[i] = "";
                programInputsNumber[i] = -1;
                programInputsPosition[i] = -1;
            }
            for (i = 0; i < 8; i++)
            {
                programOutputs[i] = "";
                programOutputsNumber[i] = -1;
                programOutputsPosition[i] = -1;
            }
            for (i = 0; i < 5; i++)
            {
                digitInputs[i] = "";
                digitInputsNumber[i] = -1;
                digitInputsPosition[i] = -1;
            }
            for (i = 0; i < 5; i++) inputFilters[i] = "";
            for (i = 0; i < 2; i++)
            {
                digitOutputs[i] = "";
                digitOutputsNumber[i] = -1;
                digitOutputsPosition[i] = -1;
            }
            for (i = 0; i < 13; i++) clearSwitches[i] = false;
            leftAccumulator = -1;
            rightAccumulator = -1;
            allClear();
            powered = false;
            significantFigures = 10;
            receiveA = false;
            currentClear = false;
        }

        public void loadProgram(int p, StreamReader file)
        {
            String line;
            line = "";
            p--;
            while (!file.EndOfStream && line.CompareTo("}") != 0)
            {
                line = file.ReadLine().Trim().ToLower();
                if (line.CompareTo("mode alpha") == 0) operationSwitches[p] = 0;
                if (line.CompareTo("mode beta") == 0) operationSwitches[p] = 1;
                if (line.CompareTo("mode gamma") == 0) operationSwitches[p] = 2;
                if (line.CompareTo("mode delta") == 0) operationSwitches[p] = 3;
                if (line.CompareTo("mode epsilon") == 0) operationSwitches[p] = 4;
                if (line.CompareTo("mode 0") == 0) operationSwitches[p] = 5;
                if (line.CompareTo("mode a") == 0) operationSwitches[p] = 6;
                if (line.CompareTo("mode as") == 0) operationSwitches[p] = 7;
                if (line.CompareTo("mode s") == 0) operationSwitches[p] = 8;
                if (line.CompareTo("clear true") == 0) clearSwitches[p] = true;
                if (line.CompareTo("clear false") == 0) clearSwitches[p] = false;
                if (line.StartsWith("repeat") && p > 3)
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    repeaters[p-4] = Convert.ToInt32(line);
                }
                if (line.StartsWith("programin"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setProgramInput(p,line);
//                    programInputs[p] = line;
                }
                if (line.StartsWith("programout") && p > 3)
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setProgramOutput(p,line);
//                    programOutputs[p-4] = line;
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
                if (line.StartsWith("masterclear"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    clearSwitches[12] = Convert.ToBoolean(line);
                }
                if (line.StartsWith("figures"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    significantFigures = Convert.ToInt32(line);
                }
                if (line.StartsWith("alpha "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDigitInput(0, line);
                }
                if (line.StartsWith("beta "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDigitInput(1, line);
                }
                if (line.StartsWith("gamma "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDigitInput(2, line);
                }
                if (line.StartsWith("delta "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDigitInput(3, line);
                }
                if (line.StartsWith("epsilon"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDigitInput(4, line);
                }
                if (line.StartsWith("a "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDigitOutput(0, line);
                }
                if (line.StartsWith("s "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDigitOutput(1, line);
                }
                if (line.StartsWith("alphafilter "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    inputFilters[0] = line;
                }
                if (line.StartsWith("betafilter "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    inputFilters[1] = line;
                }
                if (line.StartsWith("gammafilter "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    inputFilters[2] = line;
                }
                if (line.StartsWith("deltafilter "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    inputFilters[3] = line;
                }
                if (line.StartsWith("epsilonfilter "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    inputFilters[4] = line;
                }
                if (line.StartsWith("left"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    leftAccumulator = Convert.ToInt32(line)-1;
                }
                if (line.StartsWith("right"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    rightAccumulator = Convert.ToInt32(line) - 1;
                }

                if (line.StartsWith("program"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    pos = Convert.ToInt32(line.Substring(0, line.IndexOf(' ')));
                    loadProgram(pos, file);
                }



            }
        }

        public void save(StreamWriter file)
        {
            int i;
            Boolean flag;
            flag = false;
            if (powered) flag = true;
            if (clearSwitches[12]) flag = true;
            if (significantFigures != 10) flag = true;
            for (i = 0; i < 5; i++)
                if (digitInputs[i].Length > 2) flag = true;
            for (i = 0; i < 2; i++)
                if (digitOutputs[i].Length > 2) flag = true;
            for (i = 0; i < 12; i++)
            {
                if (operationSwitches[i] != 0) flag = true;
                if (clearSwitches[i]) flag = true;
                if (programInputs[i].Length > 2) flag = true;
            }
            for (i = 0; i < 8; i++)
            {
                if (repeaters[i] != 1) flag = true;
                if (programOutputs[i].Length > 2) flag = true;
            }
            if (leftAccumulator >= 0) flag = true;
            if (rightAccumulator >= 0) flag = true;
            if (flag)
            {
                file.WriteLine("accumulator " + (number+1).ToString() + " {");
                if (powered) file.WriteLine("  power " + powered.ToString());
                if (clearSwitches[12]) file.WriteLine("  masterclear " + clearSwitches[12].ToString());
                if (significantFigures != 10) file.WriteLine("  figures " + significantFigures.ToString());
                if (digitInputs[0].Length > 2) file.WriteLine("  alpha " + digitInputs[0]);
                if (digitInputs[1].Length > 2) file.WriteLine("  beta " + digitInputs[1]);
                if (digitInputs[2].Length > 2) file.WriteLine("  gamma " + digitInputs[2]);
                if (digitInputs[3].Length > 2) file.WriteLine("  delta " + digitInputs[3]);
                if (digitInputs[4].Length > 2) file.WriteLine("  epsilon " + digitInputs[4]);
                if (digitOutputs[0].Length > 2) file.WriteLine("  a " + digitOutputs[0]);
                if (digitOutputs[1].Length > 2) file.WriteLine("  s " + digitOutputs[1]);
                if (leftAccumulator >= 0) file.WriteLine("  left " + (leftAccumulator + 1).ToString());
                if (rightAccumulator >= 0) file.WriteLine("  right " + (rightAccumulator + 1).ToString());
                if (inputFilters[0].Length > 0) file.WriteLine("  alphafilter " + inputFilters[0]);
                if (inputFilters[1].Length > 0) file.WriteLine("  betafilter " + inputFilters[1]);
                if (inputFilters[2].Length > 0) file.WriteLine("  gammafilter " + inputFilters[2]);
                if (inputFilters[3].Length > 0) file.WriteLine("  deltafilter " + inputFilters[3]);
                if (inputFilters[4].Length > 0) file.WriteLine("  epsilonfilter " + inputFilters[4]);
                for (i = 0; i < 12; i++)
                {
                    if (operationSwitches[i] != 0 ||
                        clearSwitches[i] != false ||
                        programInputs[i].Length > 2 ||
                        (i > 3 && repeaters[i-4] != 1) ||
                        (i > 3 && programOutputs[i-4].Length > 2))
                    {
                        file.WriteLine("  program " + (i + 1).ToString() + " {");
                        switch (operationSwitches[i])
                        {
                            case 0: file.WriteLine("    mode alpha"); break;
                            case 1: file.WriteLine("    mode beta"); break;
                            case 2: file.WriteLine("    mode gamma"); break;
                            case 3: file.WriteLine("    mode delta"); break;
                            case 4: file.WriteLine("    mode epsilon"); break;
                            case 5: file.WriteLine("    mode 0"); break;
                            case 6: file.WriteLine("    mode A"); break;
                            case 7: file.WriteLine("    mode AS"); break;
                            case 8: file.WriteLine("    mode S"); break;
                        }
                        file.WriteLine("    clear " + clearSwitches[i]);
                        if (programInputs[i].Length > 2) file.WriteLine("    programin " + programInputs[i]);
                        if (i > 3)
                        {
                            if (programOutputs[i-4].Length > 2) file.WriteLine("    programout " + programOutputs[i-4]);
                            file.WriteLine("    repeat " + repeaters[i - 4]);
                        }
                        file.WriteLine("    }");
                    }
                    
                }
                file.WriteLine("  }");
            }
        }

        public void allClear()
        {
            int i;
            for (i = 0; i < 10; i++) decades[i] = 0;
            sign = false;
            operation = -1;
            operationTrigger = -1;
            linkOperation = false;
            receiveA = false;
        }

        public void selectiveClear()
        {
            if (clearSwitches[12])
            {
                clearDecades();
            }
            operation = -1;
            operationTrigger = -1;
        }

        public void setClearSwitch(int n, Boolean b)
        {
            clearSwitches[n] = b;
        }

        public void setDigitInput(int n, String s)
        {
            digitInputs[n] = s;
            digitInputsNumber[n] = machine.getBusNumber(s);
            digitInputsPosition[n] = machine.getBusPosition(s);
        }

        public void setDigitOutput(int n, String s)
        {
            digitOutputs[n] = s;
            digitOutputsNumber[n] = machine.getBusNumber(s);
            digitOutputsPosition[n] = machine.getBusPosition(s);
        }

        public void setOperationSwitch(int n, int p)
        {
            operationSwitches[n] = p;
        }

        public void setPowered(Boolean b)
        {
            powered = b;
        }

        public void setProgramInput(int n, String s)
        {
            programInputs[n] = s;
            programInputsNumber[n] = machine.getBusNumber(s);
            programInputsPosition[n] = machine.getBusPosition(s);
        }

        public void setProgramOutput(int n, String s)
        {
            programOutputs[n - 4] = s;
            programOutputsNumber[n - 4] = machine.getBusNumber(s);
            programOutputsPosition[n - 4] = machine.getBusPosition(s);
        }

        public void setRepeatSwitch(int n, int t)
        {
            repeaters[n-4] = t;
        }

        public void setSignificantFigures(int n)
        {
            significantFigures = n;
        }

        public void setValue(String s)
        {
            int i;
            sign = (s[0] == '-') ? true : false;
            for (i = 1; i <= s.Length; i++)
            {
                decades[i] = Convert.ToInt32(s.Substring(i, 1));
            }
        }

        public String getDigitInput(int n)
        {
            return digitInputs[n];
        }

        public String getDigitOutput(int n)
        {
            return digitOutputs[n];
        }

        public int getOperationSwitch(int n)
        {
            return operationSwitches[n];
        }

        public Boolean getPowered()
        {
            return powered;
        }

        public String getProgramInput(int n)
        {
            return programInputs[n];
        }

        public String getProgramOutput(int n)
        {
            return programOutputs[n - 4];
        }

        public Boolean getClearSwitch(int n)
        {
            return clearSwitches[n];
        }

        public int getRepeatSwitch(int n)
        {
            return repeaters[n-5];

        }

        public String getInputFilter(int n)
        {
            return inputFilters[n];
        }

        public void setInputFilter(int n, String s)
        {
            inputFilters[n] = s;
        }

        public int getSignificantFigures()
        {
            return significantFigures;
        }

        public int getLeftAccumulator()
        {
            return leftAccumulator;
        }

        public int getRightAccumulator()
        {
            return rightAccumulator;
        }

        public void clearAccumulatorLinks()
        {
            int left, right;
            left = leftAccumulator;
            right = rightAccumulator;
            leftAccumulator = -1;
            rightAccumulator = -1;
            if (left >= 0)
            {
                machine.getAccumulator(left).clearAccumulatorLinks();
            }
            if (right >= 0)
            {
                machine.getAccumulator(right).clearAccumulatorLinks();
            }
        }

        public void setLeftAccumulator(int i,Boolean clear)
        {
            if (clear) clearAccumulatorLinks();
            leftAccumulator = i;
            rightAccumulator = -1;
            if ((leftAccumulator >= 0) && clear) machine.getAccumulator(leftAccumulator).setRightAccumulator(number,false);
        }

        public void setRightAccumulator(int i,Boolean clear)
        {
            if (clear) clearAccumulatorLinks();
            rightAccumulator = i;
            leftAccumulator = -1;
            if ((rightAccumulator >= 0) && clear) machine.getAccumulator(rightAccumulator).setLeftAccumulator(number,false);
        }

        public String getValue()
        {
            int i;
            String ret;
            if (!powered) return "+0000000000";
            ret = (sign) ? "-" : "+";
            for (i = 0; i < 10; i++)
                ret += decades[i].ToString();
            return ret;
        }

        public void clearDecades()
        {
            int i;
            for (i = 0; i < 10; i++) decades[i] = 0;
            sign = false;
            if (significantFigures != 10)
            {
                decades[significantFigures] = 5;
            }
        }

        public int[] getDecades()
        {
            return decades;
        }

        public Boolean getSign()
        {
            return sign;
        }

        public void setSign(Boolean b)
        {
            sign = b;
        }

        public void carryIn(Boolean carry)
        {
            int i;
            if (carry) decades[9]++;
            for (i = 9; i >= 0; i--)
            {
                while (decades[i] > 9)
                {
                    decades[i] -= 10;
                    if (i > 0) decades[i - 1]++;
                    else if (leftAccumulator < 0) sign = (sign) ? false : true;
                    else machine.getAccumulator(leftAccumulator).carryIn(true);
                }
            }
        }
        public void linkedOperation(int op,Boolean clear,int repeats,int oper) {
            currentOperation = op;
            currentClear = clear;
            operationRepeats = repeats;
            operation = oper;
            linkOperation = true;
        }

        public void setReceiveA(Boolean b)
        {
            receiveA = b;
            if (rightAccumulator >= 0) machine.getAccumulator(rightAccumulator).setReceiveA(b);
            if (b)
            {
                currentClear = false;
            }
        }

        public void setSpecialMode(int mode,Boolean clear,int repeats)
        {
            String function;
            function = "";
            switch (mode)
            {
                case 0: function = "alpha"; break;
                case 1: function = "beta"; break;
                case 2: function = "gamma"; break;
                case 3: function = "delta"; break;
                case 4: function = "epsilon"; break;
                case 5: function = "0"; break;
                case 6: function = "A"; break;
                case 7: function = "AS"; break;
                case 8: function = "S"; break;
            }
            currentOperation = mode;
            currentClear = clear;
            operationRepeats = repeats;
            operation = 0;
            if (rightAccumulator >= 0) machine.getAccumulator(rightAccumulator).setSpecialMode(mode, clear, repeats);
            linkOperation = true;
            machine.log("Accumulator " + (number + 1).ToString() + ": special mode " + function + " started with " + repeats.ToString() + " cycles");
        }

        public Boolean busy()
        {
            return (operation >= 0) ? true : false;
        }

        public void cycle1(int clocks)      /* This sets any outputs */
        {
            int i;
            int a, s;
            if (!powered) return;
            if (operation < 0) return;
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                operationRepeats--;
                if (operationRepeats <= 0)
                {
                    if (operation > 3 && linkOperation==false)
                    {
//                        if (programOutputs[operation - 4].Length > 2)
//                        {
//                            machine.setBus(programOutputs[operation - 4], 1);
//                            machine.log("Accumulator " + (number + 1).ToString() + " sent pulse " + programOutputs[operation-4]);
//                        }
                        if (programOutputsNumber[operation - 4] >= 0)
                        {
                            machine.setBus(programOutputsNumber[operation - 4], programOutputsPosition[operation - 4], 1);
                            machine.log("Accumulator " + (number + 1).ToString() + " sent pulse " + programOutputs[operation - 4]);
                        }
                    }
                    if (currentOperation > 4 && currentClear)
                    {
                        clearDecades();
                    }
                    operation = -1;
                    linkOperation = false;
                }
            }
            if (operation < 0) return;
            if (currentOperation < 6) return;      /* Return if not in an output operation */
            a = 0;
            s = 0;
            if ((clocks & EniacCyclingUnit.C10P) == EniacCyclingUnit.C10P)
            {
                for (i = 0; i < 10; i++)
                {
                    decades[i]++;
                }
            }
            if ((clocks & EniacCyclingUnit.C9P) == EniacCyclingUnit.C9P)
            {
                for (i = 0; i < 10; i++)
                {
                    a <<= 1;
                    s <<= 1;
                    if (decades[i] >= 10) a |= 1; else s |= 1;
                }
                if (sign) a |= 0x400; else s |= 0x400;
            }
            if (rightAccumulator < 0 && (clocks & EniacCyclingUnit.C11P) == EniacCyclingUnit.C11P && significantFigures != 0)
            {
//                if (sign) a |= (1 << (10-significantFigures));
                s |= (1 << (10-significantFigures));
            }
/*       
            if (digitOutputs[0].Length > 2)
            {
                if (currentOperation == 6 || currentOperation == 7) machine.setBus(digitOutputs[0], a);
            }
            if (digitOutputs[1].Length > 2)
            {
                if (currentOperation == 7 || currentOperation == 8) machine.setBus(digitOutputs[1], s);
            }
*/
            if (digitOutputsNumber[0] >= 0)
            {
                if (currentOperation == 6 || currentOperation == 7) machine.setBus(digitOutputsNumber[0], -1, a);
            }
            if (digitOutputsNumber[1] >= 0)
            {
                if (currentOperation == 7 || currentOperation == 8) machine.setBus(digitOutputsNumber[1], -1, s);
            }

        
        }

        public void cycle2(int clocks)      /* This processes any inputs */
        {
            int i;
            int bus;
            if (!powered) return;
            for (i = 0; i < 12; i++)
            {
//                if (operationTrigger < 0 && programInputs[i].Length > 3)
//                {
//                    bus = machine.getBus(programInputs[i]);
//                    if (bus != 0)
//                    {
//                        operationTrigger = i;
//                        machine.log("Accumulator " + (number + 1).ToString() + ": Operation " + (i+1).ToString() + " triggered by " + programInputs[i]);
//                    }
//                }

                if (operationTrigger < 0 && programInputsNumber[i] >= 0)
                {
                    bus = machine.getBus(programInputsNumber[i],programInputsPosition[i]);
                    if (bus != 0)
                    {
                        operationTrigger = i;
                        machine.log("Accumulator " + (number + 1).ToString() + ": Operation " + (i + 1).ToString() + " triggered by " + programInputs[i]);
                    }
                }
            
            
            
            }
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP && operationTrigger >= 0 && operation < 0)
            {
                operation = operationTrigger;
                operationTrigger = -1;
                currentOperation = operationSwitches[operation];
                currentClear = clearSwitches[operation];
                operationRepeats = (operation < 4) ? 1 : repeaters[operation - 4];
                linkOperation = false;
                if (leftAccumulator >= 0)
                {
                    machine.getAccumulator(leftAccumulator).linkedOperation(currentOperation, currentClear, operationRepeats, operation);
                }
                if (rightAccumulator >= 0)
                {
                    machine.getAccumulator(rightAccumulator).linkedOperation(currentOperation, currentClear, operationRepeats, operation);
                }
                machine.log("Accumulator " + (number + 1).ToString() + ": Operation " + (operation+1).ToString() + " started with " + operationRepeats.ToString() + " cycles");
                return;                 /* Prevent further processing of CPP clock */
            }
            if (operation >= 0 || receiveA)
            {
                if (currentOperation < 5 || receiveA)
                {
                    if (receiveA)
                    {
                        bus = (digitInputsNumber[0] >= 0) ? machine.getBus(digitInputsNumber[0], digitInputsPosition[0]) : 0;
//                        bus = (digitInputs[0].Length > 2) ? machine.getBus(digitInputs[0]) : 0;
                        if (inputFilters[0].Length > 0) bus = machine.translate(inputFilters[0], bus);
                    }
                    else
                    {
                        bus = (digitInputsNumber[currentOperation] >= 0) ? machine.getBus(digitInputsNumber[currentOperation],digitInputsPosition[currentOperation]) : 0;
//                        bus = (digitInputs[currentOperation].Length > 2) ? machine.getBus(digitInputs[currentOperation]) : 0;
                        if (inputFilters[currentOperation].Length > 0) bus = machine.translate(inputFilters[currentOperation], bus);
                    }
                    
                    for (i = 9; i >= 0; i--)
                    {
                        if ((bus & 1) == 1) decades[i]++;
                        bus >>= 1;
                    }
                    if ((bus & 1) == 1 && leftAccumulator < 0) sign = (sign) ? false : true;
                    if ((clocks & EniacCyclingUnit.C11P) == EniacCyclingUnit.C11P && rightAccumulator < 0)
                    {
                        if (currentClear) decades[9]++;
                    }
                    if ((clocks & EniacCyclingUnit.CCG) == EniacCyclingUnit.CCG)
                    {
                        carryIn(false);
                    }
                }
                if (currentOperation > 5)
                {
                    if ((clocks & EniacCyclingUnit.CCG) == EniacCyclingUnit.CCG)
                    {
                        for (i = 9; i >= 0; i--)
                        {
                            while (decades[i] > 9)
                            {
                                decades[i] -= 10;
                            }
                        }
                    }
                }

            }
        }

    }
}
