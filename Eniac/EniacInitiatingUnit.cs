using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eniac
{
    class EniacInitiatingUnit
    {
        public const int INIT_GO_PRESSED = 1;
        public const int INIT_CLEAR_PRESSED = 2;
        public const int INIT_SEL_CLEAR = 4;

        private Boolean clearPressed;
        private Boolean goPressed;
        private String goOutput;
        private int goOutputNumber;
        private int goOutputPosition;
        private Machine machine;
        private String[] clearIn;
        private int[] clearInNumber;
        private int[] clearInPosition;
        private String[] clearOut;
        private int[] clearOutNumber;
        private int[] clearOutPosition;
        private int clearStimulated;

        public EniacInitiatingUnit(Machine m)
        {
            clearPressed = false;
            goPressed = false;
            goOutput = "";
            machine = m;
            clearIn = new String[6];
            clearInNumber = new int[6];
            clearInPosition = new int[6];
            clearOut = new String[6];
            clearOutNumber = new int[6];
            clearOutPosition = new int[6];
            reset();
        }

        public void reset()
        {
            int i;
            for (i = 0; i < 6; i++)
            {
                clearIn[i] = "";
                clearInNumber[i] = -1;
                clearInPosition[i] = -1;
            }
            for (i = 0; i < 6; i++)
            {
                clearOut[i] = "";
                clearOutNumber[i] = -1;
                clearOutPosition[i] = -1;
            }
            goOutput = "";
            goOutputNumber = -1;
            goOutputPosition = -1;
            clearStimulated = -1;
        }

        public void save(StreamWriter file)
        {
            int i;
            file.WriteLine("initiating {");
            file.WriteLine("  go " + goOutput);
            for (i = 0; i < 6; i++)
                if (clearIn[i].Length > 2) file.WriteLine("  clearin " + i.ToString() + " " + clearIn[i]);
            for (i = 0; i < 6; i++)
                if (clearOut[i].Length > 2) file.WriteLine("  clearout " + i.ToString() + " " + clearOut[i]);
            file.WriteLine("  }");
        }

        public void load(StreamReader file)
        {
            String line = "";
            String temp;
            int num;
            reset();
            while (!file.EndOfStream && line.CompareTo("}") != 0)
            {
                line = file.ReadLine().Trim().ToLower();
                if (line.StartsWith("go "))
                {
                    line = line.Substring(3).Trim().ToUpper();
                    setGoOutput(line);
//                    goOutput = line;
                }
                if (line.StartsWith("clearin"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    temp = line.Substring(0, line.IndexOf(' ')).Trim().ToUpper();
                    num = Convert.ToInt32(temp);
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setClearIn(num, line);
//                    clearIn[num] = line;
                }
                if (line.StartsWith("clearout"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    temp = line.Substring(0, line.IndexOf(' ')).Trim().ToUpper();
                    num = Convert.ToInt32(temp);
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setClearOut(num, line);
//                    clearOut[num] = line;
                }
            }
        }

        public int cycle1(int clocks)
        {
            int ret;
            ret = 0;
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                if (goPressed)
                {
                    ret |= INIT_GO_PRESSED;
//                    if (goOutput.Length > 2)
//                    {
//                        machine.setBus(goOutput, 1);
//                        machine.log("CPP sent to " + goOutput);
//                    }
                    if (goOutputNumber >= 0)
                    {
                        machine.setBus(goOutputNumber, goOutputPosition, 1);
                        machine.log("CPP sent to " + goOutput);
                    }

                    goPressed = false;
                }
                if (clearStimulated >= 0)
                {
                    if (clearOutNumber[clearStimulated] >= 0)
                    {
                        machine.setBus(clearOutNumber[clearStimulated], clearOutPosition[clearStimulated], 1);
                        machine.log("Initiating: pulse sent on " + clearOut[clearStimulated]);
                    }
                    clearStimulated = -1;
                }
            }
            if (clearStimulated >= 0 && (clocks & EniacCyclingUnit.CCG) == EniacCyclingUnit.CCG)
            {
                ret |= INIT_SEL_CLEAR;
            }
            if (clearPressed)
            {
                clearPressed = false;
                ret |= INIT_CLEAR_PRESSED;
            }
            return ret;
        }

        public void cycle2(int clocks)
        {
            int i;
            for (i=0; i<6; i++)
//                if (clearIn[i].Length > 2)
//                {
//                    if (clearStimulated < 0 && machine.getBus(clearIn[i]) != 0)
//                    {
//                        clearStimulated = i;
//                        machine.log("Initiating: Clear " + i.ToString() + " triggered by " + clearIn[i]);
//                    }
//                }
            if (clearInNumber[i] >= 0)
            {
                if (clearStimulated < 0 && machine.getBus(clearInNumber[i], clearInPosition[i]) != 0)
                {
                    clearStimulated = i;
                    machine.log("Initiating: Clear " + i.ToString() + " triggered by " + clearIn[i]);
                }
            }
        }

        public void go()
        {
            goPressed = true;
            machine.log("Go was pressed");
        }

        public void clear()
        {
            clearPressed = true;
        }

        public void setGoOutput(String s)
        {
            goOutput = s;
            goOutputNumber = machine.getBusNumber(s);
            goOutputPosition = machine.getBusPosition(s);
        }

        public String getGoOutput()
        {
            return goOutput;
        }

        public String getClearIn(int i)
        {
            return clearIn[i];
        }

        public void setClearIn(int i, String s)
        {
            clearIn[i] = s;
            clearInNumber[i] = machine.getBusNumber(s);
            clearInPosition[i] = machine.getBusPosition(s);
        }

        public String getClearOut(int i)
        {
            return clearOut[i];
        }

        public void setClearOut(int i, String s)
        {
            clearOut[i] = s;
            clearOutNumber[i] = machine.getBusNumber(s);
            clearOutPosition[i] = machine.getBusPosition(s);
        }

    }
}
