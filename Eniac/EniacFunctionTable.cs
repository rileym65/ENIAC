using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eniac
{
    class EniacFunctionTable
    {
        private Boolean powered;
        private Machine machine;
        private int[] clearMode;
        private int[] operationSwitch;
        private int[] repeatSwitch;
        private String[] programIn;
        private int[] programInNumber;
        private int[] programInPosition;
        private String[] programOut;
        private int[] programOutNumber;
        private int[] programOutPosition;
        private String outputNC;
        private int outputNCNumber;
        private int outputNCPosition;
        private String outputC;
        private int outputCNumber;
        private int outputCPosition;
        private String argumentIn;
        private int argumentInNumber;
        private int argumentInPosition;
        private String outputA;
        private int outputANumber;
        private int outputAPosition;
        private String outputB;
        private int outputBNumber;
        private int outputBPosition;
        private int[] masterSign;
        private Boolean[] aDeleteSwitch;
        private Boolean[] bDeleteSwitch;
        private int[] constantA;
        private int[] constantB;
        private Boolean[] aSubSwitch;
        private Boolean[] bSubSwitch;
        private Boolean addSubMode;        /* false = add, true = sub */
        private int number;
        private String[] table;
        private int operation;
        private int trigger;
        private int argument;
        private int[] aPulses;
        private int[] bPulses;
        private int[] aCopy;
        private int[] bCopy;
        private int phase;

        public EniacFunctionTable(Machine m,int n)
        {
            machine = m;
            number = n;
            clearMode = new int[11];
            operationSwitch = new int[11];
            repeatSwitch = new int[11];
            programIn = new String[11];
            programInNumber = new int[11];
            programInPosition = new int[11];
            programOut = new String[11];
            programOutNumber = new int[11];
            programOutPosition = new int[11];
            masterSign = new int[2];
            aDeleteSwitch = new Boolean[4];
            constantA = new int[4];
            bDeleteSwitch = new Boolean[4];
            constantB = new int[4];
            aSubSwitch = new Boolean[6];
            bSubSwitch = new Boolean[6];
            aPulses = new int[11];
            bPulses = new int[11];
            aCopy = new int[11];
            bCopy = new int[11];
            table = new String[104];
            reset();
        }

        public void reset()
        {
            int i;
            powered = false;
            for (i = 0; i < 11; i++) clearMode[i] = 0;
            for (i = 0; i < 11; i++) operationSwitch[i] = 0;
            for (i = 0; i < 11; i++) repeatSwitch[i] = 0;
            for (i = 0; i < 11; i++)
            {
                programIn[i] = "";
                programInNumber[i] = -1;
                programInPosition[i] = -1;
            }
            for (i = 0; i < 11; i++)
            {
                programOut[i] = "";
                programOutNumber[i] = -1;
                programOutPosition[i] = -1;
            }
            for (i = 0; i < 2; i++) masterSign[i] = 2;
            for (i = 0; i < 4; i++) aDeleteSwitch[i] = false;
            for (i = 0; i < 4; i++) constantA[i] = 0;
            for (i = 0; i < 4; i++) bDeleteSwitch[i] = false;
            for (i = 0; i < 4; i++) constantB[i] = 0;
            for (i = 0; i < 6; i++) aSubSwitch[i] = false;
            for (i = 0; i < 6; i++) bSubSwitch[i] = false;
            for (i = 0; i < 104; i++) table[i] = "+000000000000+";
            outputNC = "";
            outputNCNumber = -1;
            outputNCPosition = -1;
            outputC = "";
            outputCNumber = -1;
            outputCPosition = -1;
            argumentIn = "";
            argumentInNumber = -1;
            argumentInPosition = -1;
            outputA = "";
            outputANumber = -1;
            outputAPosition = -1;
            outputB = "";
            outputBNumber = -1;
            outputBPosition = -1;
            operation = -1;
            phase = -3;
            trigger = -1;
        }

        public void clear()
        {
            operation = -1;
            phase = -3;
            trigger = -1;
        }

        public void loadProgram(int p, StreamReader file)
        {
            String line;
            line = "";
            p--;
            while (!file.EndOfStream && line.CompareTo("}") != 0)
            {
                line = file.ReadLine().Trim().ToLower();
                if (line.CompareTo("clear 0") == 0) clearMode[p] = 0;
                if (line.CompareTo("clear nc") == 0) clearMode[p] = 1;
                if (line.CompareTo("clear c") == 0) clearMode[p] = 2;
                if (line.CompareTo("mode add-2") == 0) operationSwitch[p] = 0;
                if (line.CompareTo("mode add-1") == 0) operationSwitch[p] = 1;
                if (line.CompareTo("mode add") == 0) operationSwitch[p] = 2;
                if (line.CompareTo("mode add+1") == 0) operationSwitch[p] = 3;
                if (line.CompareTo("mode add+2") == 0) operationSwitch[p] = 4;
                if (line.CompareTo("mode sub-2") == 0) operationSwitch[p] = 5;
                if (line.CompareTo("mode sub-1") == 0) operationSwitch[p] = 6;
                if (line.CompareTo("mode sub") == 0) operationSwitch[p] = 7;
                if (line.CompareTo("mode sub+1") == 0) operationSwitch[p] = 8;
                if (line.CompareTo("mode sub+2") == 0) operationSwitch[p] = 9;
                if (line.StartsWith("repeat"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    repeatSwitch[p] = Convert.ToInt32(line)-1;
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
            String temp;
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
                if (line.StartsWith("outputnc"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputNC(line);
//                    outputNC = line;
                }
                if (line.StartsWith("outputc"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputC(line);
//                    outputC = line;
                }
                if (line.StartsWith("argin"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setArgumentIn(line);
//                    argumentIn = line;
                }
                if (line.StartsWith("outputa"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputA(line);
//                    outputA = line;
                }
                if (line.StartsWith("outputb"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputB(line);
//                    outputB = line;
                }
                if (line.StartsWith("data "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    temp = line.Substring(0, line.IndexOf(' ')).Trim().ToUpper();
                    pos = Convert.ToInt32(temp) + 2;
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    table[pos] = line;
                }
                if (line.CompareTo("sign1 p") == 0) masterSign[0] = 0;
                if (line.CompareTo("sign1 m") == 0) masterSign[0] = 1;
                if (line.CompareTo("sign1 table") == 0) masterSign[0] = 2;
                if (line.CompareTo("sign2 p") == 0) masterSign[1] = 0;
                if (line.CompareTo("sign2 m") == 0) masterSign[1] = 1;
                if (line.CompareTo("sign2 table") == 0) masterSign[1] = 2;
                if (line.CompareTo("adel1 true") == 0) aDeleteSwitch[0] = true;
                if (line.CompareTo("adel2 true") == 0) aDeleteSwitch[1] = true;
                if (line.CompareTo("adel3 true") == 0) aDeleteSwitch[2] = true;
                if (line.CompareTo("adel4 true") == 0) aDeleteSwitch[3] = true;
                if (line.CompareTo("bdel1 true") == 0) bDeleteSwitch[0] = true;
                if (line.CompareTo("bdel2 true") == 0) bDeleteSwitch[1] = true;
                if (line.CompareTo("bdel3 true") == 0) bDeleteSwitch[2] = true;
                if (line.CompareTo("bdel4 true") == 0) bDeleteSwitch[3] = true;
                if (line.StartsWith("a1"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    if (line.CompareTo("PM1") == 0) constantA[0] = 10;
                    else if (line.CompareTo("PM2") == 0) constantA[0] = 11;
                    else constantA[0] = Convert.ToInt32(line);
                }
                if (line.StartsWith("a2"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    if (line.CompareTo("PM1") == 0) constantA[1] = 10;
                    else if (line.CompareTo("PM2") == 0) constantA[1] = 11;
                    else constantA[1] = Convert.ToInt32(line);
                }
                if (line.StartsWith("a3"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    if (line.CompareTo("PM1") == 0) constantA[2] = 10;
                    else if (line.CompareTo("PM2") == 0) constantA[2] = 11;
                    else constantA[2] = Convert.ToInt32(line);
                }
                if (line.StartsWith("a4"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    if (line.CompareTo("PM1") == 0) constantA[3] = 10;
                    else if (line.CompareTo("PM2") == 0) constantA[3] = 11;
                    else constantA[3] = Convert.ToInt32(line);
                }
                if (line.StartsWith("b1"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    if (line.CompareTo("PM1") == 0) constantB[0] = 10;
                    else if (line.CompareTo("PM2") == 0) constantB[0] = 11;
                    else constantB[0] = Convert.ToInt32(line);
                }
                if (line.StartsWith("b2"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    if (line.CompareTo("PM1") == 0) constantB[1] = 10;
                    else if (line.CompareTo("PM2") == 0) constantB[1] = 11;
                    else constantB[1] = Convert.ToInt32(line);
                }
                if (line.StartsWith("b3"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    if (line.CompareTo("PM1") == 0) constantB[2] = 10;
                    else if (line.CompareTo("PM2") == 0) constantB[2] = 11;
                    else constantB[2] = Convert.ToInt32(line);
                }
                if (line.StartsWith("b4"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    if (line.CompareTo("PM1") == 0) constantB[3] = 10;
                    else if (line.CompareTo("PM2") == 0) constantB[3] = 11;
                    else constantB[3] = Convert.ToInt32(line);
                }
                if (line.CompareTo("suba5 true") == 0) aSubSwitch[0] = true;
                if (line.CompareTo("suba6 true") == 0) aSubSwitch[1] = true;
                if (line.CompareTo("suba7 true") == 0) aSubSwitch[2] = true;
                if (line.CompareTo("suba8 true") == 0) aSubSwitch[3] = true;
                if (line.CompareTo("suba9 true") == 0) aSubSwitch[4] = true;
                if (line.CompareTo("suba10 true") == 0) aSubSwitch[5] = true;
                if (line.CompareTo("subb5 true") == 0) bSubSwitch[0] = true;
                if (line.CompareTo("subb6 true") == 0) bSubSwitch[1] = true;
                if (line.CompareTo("subb7 true") == 0) bSubSwitch[2] = true;
                if (line.CompareTo("subb8 true") == 0) bSubSwitch[3] = true;
                if (line.CompareTo("subb9 true") == 0) bSubSwitch[4] = true;
                if (line.CompareTo("subb10 true") == 0) bSubSwitch[5] = true;

            }
        }

        private void saveProgram(StreamWriter file, int prog)
        {
            Boolean flag;
            flag = false;
            if (clearMode[prog] != 0) flag = true;
            if (operationSwitch[prog] != 0) flag = true;
            if (repeatSwitch[prog] != 0) flag = true;
            if (programIn[prog].Length > 1) flag = true;
            if (programOut[prog].Length > 1) flag = true;
            if (flag)
            {
                file.WriteLine("  program " + (prog + 1).ToString() + " {");
                switch (clearMode[prog])
                {
                    case 0: file.WriteLine("    clear 0"); break;
                    case 1: file.WriteLine("    clear nc"); break;
                    case 2: file.WriteLine("    clear c"); break;
                }
                switch (operationSwitch[prog])
                {
                    case 0: file.WriteLine("    mode add-2"); break;
                    case 1: file.WriteLine("    mode add-1"); break;
                    case 2: file.WriteLine("    mode add"); break;
                    case 3: file.WriteLine("    mode add+1"); break;
                    case 4: file.WriteLine("    mode add+2"); break;
                    case 5: file.WriteLine("    mode sub-2"); break;
                    case 6: file.WriteLine("    mode sub-1"); break;
                    case 7: file.WriteLine("    mode sub"); break;
                    case 8: file.WriteLine("    mode sub+1"); break;
                    case 9: file.WriteLine("    mode sub+2"); break;
                }
                if (repeatSwitch[prog] != 0) file.WriteLine("    repeat " + (repeatSwitch[prog]+1).ToString());
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
            for (i = 0; i < 11; i++) if (clearMode[i] != 0) flag = true;
            for (i = 0; i < 11; i++) if (operationSwitch[i] != 0) flag = true;
            for (i = 0; i < 11; i++) if (repeatSwitch[i] != 0) flag = true;
            for (i = 0; i < 11; i++) if (programIn[i].Length > 1) flag = true;
            for (i = 0; i < 11; i++) if (programOut[i].Length > 1) flag = true;
            for (i = 0; i < 4; i++) if (aDeleteSwitch[i]) flag = true;
            for (i = 0; i < 4; i++) if (constantA[i] != 0) flag = true;
            for (i = 0; i < 4; i++) if (bDeleteSwitch[i]) flag = true;
            for (i = 0; i < 4; i++) if (constantB[i] != 0) flag = true;
            for (i = 0; i < 6; i++) if (aSubSwitch[i]) flag = true;
            for (i = 0; i < 104; i++) if (table[i].CompareTo("+000000000000+") != 0) flag = true;
            if (outputNC.Length > 1) flag = true;
            if (outputC.Length > 1) flag = true;
            if (argumentIn.Length > 1) flag = true;
            if (outputA.Length > 1) flag = true;
            if (outputB.Length > 1) flag = true;
            if (masterSign[0] != 2) flag = true;
            if (masterSign[1] != 2) flag = true;
            if (powered) flag = true;
            if (flag)
            {
                file.WriteLine("function " + (number+1).ToString() + " {");
                if (powered) file.WriteLine("  power true");
                for (i = 0; i < 11; i++) saveProgram(file, i);
                if (outputNC.Length > 1) file.WriteLine("  outputnc " + outputNC);
                if (outputC.Length > 1) file.WriteLine("  outputc " + outputC);
                if (argumentIn.Length > 1) file.WriteLine("  argin " + argumentIn);
                if (outputA.Length > 1) file.WriteLine("  outputa " + outputA);
                if (outputB.Length > 1) file.WriteLine("  outputb " + outputB);
                if (masterSign[0] == 0) file.WriteLine("  sign1 p");
                if (masterSign[0] == 1) file.WriteLine("  sign1 m");
                if (masterSign[1] == 0) file.WriteLine("  sign2 p");
                if (masterSign[1] == 1) file.WriteLine("  sign2 m");
                for (i = 0; i < 4; i++)
                {
                    if (aDeleteSwitch[i]) file.WriteLine("  adel"+(i+1).ToString()+" true");
                    if (constantA[i] >= 0 && constantA[i] < 10) file.WriteLine("  a"+(i+1).ToString() + " " + constantA[i].ToString());
                    if (constantA[i] == 10) file.WriteLine("  a" + (i + 1).ToString() + " pm1");
                    if (constantA[i] == 11) file.WriteLine("  a" + (i + 1).ToString() + " pm2");
                }
                for (i = 0; i < 4; i++)
                {
                    if (bDeleteSwitch[i]) file.WriteLine("  bdel" + (i + 1).ToString() + " true");
                    if (constantB[i] >= 0 && constantB[i] < 10) file.WriteLine("  b" + (i + 1).ToString() + " " + constantB[i].ToString());
                    if (constantB[i] == 10) file.WriteLine("  b" + (i + 1).ToString() + " pm1");
                    if (constantB[i] == 11) file.WriteLine("  b" + (i + 1).ToString() + " pm2");
                }
                for (i = 0; i < 6; i++)
                {
                    if (aSubSwitch[i]) file.WriteLine("  suba" + (i + 5).ToString() + " true");
                }
                for (i = 0; i < 6; i++)
                {
                    if (bSubSwitch[i]) file.WriteLine("  subb" + (i + 5).ToString() + " true");
                }
                for (i = 0; i < 104; i++)
                {
                    if (table[i].CompareTo("+000000000000+") != 0) file.WriteLine("  data " + (i - 2).ToString() + " " + table[i]);
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

        public int getClearMode(int prog)
        {
            return clearMode[prog];
        }

        public void setClearMode(int prog, int v)
        {
            clearMode[prog] = v;
        }

        public int getOperation(int prog)
        {
            return operationSwitch[prog];
        }

        public void setOperation(int prog, int v)
        {
            operationSwitch[prog] = v;
        }

        public int getRepeat(int prog)
        {
            return repeatSwitch[prog];
        }

        public void setRepeat(int prog, int v)
        {
            repeatSwitch[prog] = v;
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

        public String getOutputNC()
        {
            return outputNC;
        }

        public void setOutputNC(String s)
        {
            outputNC = s;
            outputNCNumber = machine.getBusNumber(s);
            outputNCPosition = machine.getBusPosition(s);
        }

        public String getOutputC() {
            return outputC;
        }

        public void setOutputC(String s) {
            outputC = s;
            outputCNumber = machine.getBusNumber(s);
            outputCPosition = machine.getBusPosition(s);
        }

        public String getArgumentIn()
        {
            return argumentIn;
        }

        public void setArgumentIn(String s)
        {
            argumentIn = s;
            argumentInNumber = machine.getBusNumber(s);
            argumentInPosition = machine.getBusPosition(s);
        }

        public String getOutputA()
        {
            return outputA;
        }

        public void setOutputA(String s)
        {
            outputA = s;
            outputANumber = machine.getBusNumber(s);
            outputAPosition = machine.getBusPosition(s);
        }

        public String getOutputB()
        {
            return outputB;
        }

        public void setOutputB(String s)
        {
            outputB = s;
            outputBNumber = machine.getBusNumber(s);
            outputBPosition = machine.getBusPosition(s);
        }

        public int getMasterSign(int prog)
        {
            return masterSign[prog];
        }

        public void setMasterSign(int prog, int v)
        {
            masterSign[prog] = v;
        }

        public Boolean getADeleteSwitch(int prog)
        {
            return aDeleteSwitch[prog];
        }

        public void setADeleteSwitch(int prog, Boolean b)
        {
            aDeleteSwitch[prog] = b;
        }

        public int getConstantA(int pos)
        {
            return constantA[pos];
        }

        public void setConstantA(int pos, int v)
        {
            constantA[pos] = v;
        }

        public Boolean getBDeleteSwitch(int prog)
        {
            return bDeleteSwitch[prog];
        }

        public void setBDeleteSwitch(int prog, Boolean b)
        {
            bDeleteSwitch[prog] = b;
        }

        public int getConstantB(int pos)
        {
            return constantB[pos];
        }

        public void setConstantB(int pos, int v)
        {
            constantB[pos] = v;
        }

        public Boolean getASubSwitch(int pos)
        {
            return aSubSwitch[pos];
        }

        public void setASubSwitch(int pos, Boolean b)
        {
            aSubSwitch[pos] = b;
        }

        public Boolean getBSubSwitch(int pos)
        {
            return bSubSwitch[pos];
        }

        public void setBSubSwitch(int pos, Boolean b)
        {
            bSubSwitch[pos] = b;
        }

        public String getValue(int pos)
        {
            return table[pos + 2];
        }

        public void setValue(int pos, String s)
        {
            table[pos + 2] = s;
        }

        public void cycle1(int clocks)     /* Process outputs */
        {
            int i;
            int outA, outB;
            if (!powered) return;
            if (operation < 0) return;
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                if (phase == -3)
                {
//                    if (clearMode[operation] == 1 && outputNC.Length > 2)
//                    {
//                        machine.setBus(outputNC, 1);
//                        machine.log("Function " + (number + 1).ToString() + " sent pulse " + outputNC);
//                    }
                    if (clearMode[operation] == 1 && outputNCNumber >= 0)
                    {
                        machine.setBus(outputNCNumber, outputNCPosition, 1);
                        machine.log("Function " + (number + 1).ToString() + " sent pulse " + outputNC);
                    }
//                    if (clearMode[operation] == 2 && outputC.Length > 2)
//                    {
//                        machine.setBus(outputC, 1);
//                        machine.log("Function " + (number + 1).ToString() + " sent pulse " + outputC);
//                    }
                    if (clearMode[operation] == 2 && outputCNumber >= 0)
                    {
                        machine.setBus(outputCNumber, outputCPosition, 1);
                        machine.log("Function " + (number + 1).ToString() + " sent pulse " + outputC);
                    }
                }
                if (phase == repeatSwitch[operation] + 1)
                {
                    phase = -3;
//                    if (programOut[operation].Length > 2)
//                    {
//                        machine.setBus(programOut[operation], 1);
//                        machine.log("Function " + (number + 1).ToString() + " sent pulse " + programOut[operation]);
//                    }
                    if (programOutNumber[operation] >= 0)
                    {
                        machine.setBus(programOutNumber[operation], programOutPosition[operation], 1);
                        machine.log("Function " + (number + 1).ToString() + " sent pulse " + programOut[operation]);
                    }
                    operation = -1;
                    trigger = -1;
                }
            }
            if (phase < 1) return;
            if ((clocks & EniacCyclingUnit.C9P) == EniacCyclingUnit.C9P)
            {
                outA = 0;
                outB = 0;
                for (i = 0; i < 11; i++)
                {
                    outA <<= 1;
                    outB <<= 1;
                    if (aPulses[i] > 0) outA |= 1;
                    if (bPulses[i] > 0) outB |= 1;
                    aPulses[i]--;
                    bPulses[i]--;
                }
//                if (outputA.Length > 2) machine.setBus(outputA, outA);
//                if (outputB.Length > 2) machine.setBus(outputB, outB);
                if (outputANumber >= 0) machine.setBus(outputANumber, -1, outA);
                if (outputBNumber >= 0) machine.setBus(outputBNumber, -1, outB);
            }
            if (addSubMode && (clocks & EniacCyclingUnit.C11P) == EniacCyclingUnit.C11P)
            {
                outA = 0;
                outB = 0;
                for (i = 0; i < 6; i++)
                {
                    outA <<= 1;
                    outB <<= 1;
                    if (aSubSwitch[i]) outA |= 1;
                    if (bSubSwitch[i]) outB |= 1;
                }
//                if (outputA.Length > 2) machine.setBus(outputA, outA);
//                if (outputB.Length > 2) machine.setBus(outputB, outB);
                if (outputANumber >= 0) machine.setBus(outputANumber, -1, outA);
                if (outputBNumber >= 0) machine.setBus(outputBNumber, -1, outB);
            }
        }

        public void cycle2(int clocks)     /* Process inputs */
        {
            int i;
            int bus;
            int pm1, pm2;
            String value;
            if (!powered) return;
            for (i=0; i<11; i++)
//                if (programIn[i].Length > 2)
//                {
//                    bus = machine.getBus(programIn[i]);
//                    if (bus != 0)
//                    {
//                        trigger = i;
//                        machine.log("Function " + (number + 1).ToString() + " program " + (trigger + 1).ToString() + " triggered");
//                    }
//                }
            if (programInNumber[i] >= 0)
            {
                bus = machine.getBus(programInNumber[i], programInPosition[i]);
                if (bus != 0)
                {
                    trigger = i;
                    machine.log("Function " + (number + 1).ToString() + " program " + (trigger + 1).ToString() + " triggered");
                }
            }
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                if (operation < 0 && trigger >= 0)
                {
                    operation = trigger;
                    trigger = -1;
                    phase = -3;
                    argument = 0;
                    machine.log("Function " + (number + 1).ToString() + " program " + (operation + 1).ToString() + " started");
                    return;
                }
                if (operation < 0) return;
                phase++;
            }
            if (phase == -2 && (clocks & EniacCyclingUnit.C9P) == EniacCyclingUnit.C9P)
            {
//                if (argumentIn.Length > 2) bus = machine.getBus(argumentIn); else bus = 0;
                if (argumentInNumber >= 0) bus = machine.getBus(argumentInNumber, -1); else bus = 0;
                if ((bus & 1) == 1) argument++;
                if ((bus & 2) == 2) argument += 10;
            }
            if (phase == -1)
            {
                if (operationSwitch[operation] == 1 || operationSwitch[operation] == 3 ||
                    operationSwitch[operation] == 6 || operationSwitch[operation] == 8) {
                    if ((clocks & EniacCyclingUnit.C1P) == EniacCyclingUnit.C1P) argument++;
                }
                if ((operationSwitch[operation] >= 2 && operationSwitch[operation] <= 4) ||
                    (operationSwitch[operation] >= 7 && operationSwitch[operation] <= 9))
                {
                    if ((clocks & EniacCyclingUnit.C2P) == EniacCyclingUnit.C2P) argument++;
                }
                if (operationSwitch[operation] == 4 || operationSwitch[operation] == 9)
                {
                    if ((clocks & EniacCyclingUnit.C22P) == EniacCyclingUnit.C22P) argument++;
                }
                addSubMode = (operationSwitch[operation] >= 5);
            }
            if (phase == 0 && (clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                value = table[argument];
                pm1 = (value[0] == '+') ? 0 : 9;
                pm2 = (value[13] == '+') ? 0 : 9;
                if (masterSign[0] == 0) pm1 = 0;
                if (masterSign[0] == 1) pm1 = 9;
                if (masterSign[1] == 0) pm2 = 0;
                if (masterSign[1] == 1) pm2 = 9;
                aPulses[0] = pm1;
                bPulses[0] = pm2;
                for (i = 0; i < 4; i++)
                {
                    if (!aDeleteSwitch[i])
                    {
                        if (constantA[i] < 10) aPulses[i + 1] = constantA[i];
                        else if (constantA[i] == 10) aPulses[i + 1] = pm1;
                        else if (constantA[i] == 11) aPulses[i + 1] = pm2;
                    }
                    if (!bDeleteSwitch[i])
                    {
                        if (constantB[i] < 10) bPulses[i + 1] = constantB[i];
                        else if (constantB[i] == 10) bPulses[i + 1] = pm1;
                        else if (constantB[i] == 11) bPulses[i + 1] = pm2;
                    }
                }
                for (i = 0; i < 6; i++)
                {
                    aPulses[i + 5] = value[i + 1] - '0';
                    bPulses[i + 5] = value[i + 7] - '0';
                }
                if (addSubMode)
                {
                    for (i = 0; i < 11; i++)
                    {
                        aPulses[i] = 9 - aPulses[i];
                        bPulses[i] = 9 - bPulses[i];
                    }
                }
                for (i = 0; i < 11; i++)
                {
                    aCopy[i] = aPulses[i];
                    bCopy[i] = bPulses[i];
                }
            }
            if (phase > 0 && (clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                for (i = 0; i < 11; i++)
                {
                    aPulses[i] = aCopy[i];
                    bPulses[i] = bCopy[i];
                }
              
            }
        }
    }
}
