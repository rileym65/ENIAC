using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eniac
{
    class EniacConstantTransmitter
    {
        private int[] regA;
        private int[] regB;
        private int[] regC;
        private int[] regD;
        private int[] regE;
        private int[] regF;
        private int[] regG;
        private int[] regH;
        private int[] regJ;
        private int[] regK;
        private Boolean signA;
        private Boolean signB;
        private Boolean signC;
        private Boolean signD;
        private Boolean signE;
        private Boolean signF;
        private Boolean signG;
        private Boolean signH;
        private Boolean signAR;
        private Boolean signBR;
        private Boolean signCR;
        private Boolean signDR;
        private Boolean signER;
        private Boolean signFR;
        private Boolean signGR;
        private Boolean signHR;
        private Boolean jLSign;
        private Boolean jRSign;
        private Boolean kLSign;
        private Boolean kRSign;
        private Machine machine;
        private int[] programSwitches;
        private Boolean[] selectSwitches;
        private String[] programInputs;
        private int[] programInputsNumber;
        private int[] programInputsPosition;
        private String[] programOutputs;
        private int[] programOutputsNumber;
        private int[] programOutputsPosition;
        private String outputA;
        private int outputANumber;
        private int outputAPosition;
        private String outputS;
        private Boolean powered;
        private int[] decades;
        private Boolean sign;          /* False = positive, true = negative */
        private Boolean signR;
        private int operation;
        private int operationTrigger;
        private int need11p;

        public EniacConstantTransmitter(Machine m)
        {
            machine = m;
            regA = new int[10];
            regB = new int[10];
            regC = new int[10];
            regD = new int[10];
            regE = new int[10];
            regF = new int[10];
            regG = new int[10];
            regH = new int[10];
            regJ = new int[10];
            regK = new int[10];
            programSwitches = new int[30];
            selectSwitches = new Boolean[30];
            programInputs = new String[30];
            programInputsNumber = new int[30];
            programInputsPosition = new int[30];
            programOutputs = new String[30];
            programOutputsNumber = new int[30];
            programOutputsPosition = new int[30];
            decades = new int[10];
            reset();
        }

        public void reset()
        {
            int i;
            for (i = 0; i < 30; i++)
            {
                programSwitches[i] = 0;
                selectSwitches[i] = false;
                programInputs[i] = "";
                programInputsNumber[i] = -1;
                programInputsPosition[i] = -1;
                programOutputs[i] = "";
                programOutputsNumber[i] = -1;
                programOutputsPosition[i] = -1;
            }
            for (i = 0; i < 10; i++)
            {
                regJ[i] = 0;
                regK[i] = 0;
                decades[i] = 0;
            }
            jLSign = false;
            jRSign = false;
            kLSign = false;
            kRSign = false;
            outputA = "";
            outputS = "";
            powered = false;
            operation = -1;
            operationTrigger = -1;
            outputANumber = -1;
            outputAPosition = -1;
            need11p = 0;
        }

        public void setRegA(String s,char signR)
        {
            int i;
            signA = (s[0] == '-') ? true : false;
            signAR = (signR == '-') ? true : false;
            for (i = 1; i < s.Length; i++)
            {
                regA[i-1] = Convert.ToInt32(s.Substring(i, 1));
            }
        }

        public void setRegB(String s,char signR)
        {
            int i;
            signB = (s[0] == '-') ? true : false;
            signBR = (signR == '-') ? true : false;
            for (i = 1; i < s.Length; i++)
            {
                regB[i-1] = Convert.ToInt32(s.Substring(i, 1));
            }
        }

        public void setRegC(String s,char signR)
        {
            int i;
            signC = (s[0] == '-') ? true : false;
            signCR = (signR == '-') ? true : false;
            for (i = 1; i < s.Length; i++)
            {
                regC[i-1] = Convert.ToInt32(s.Substring(i, 1));
            }
        }

        public void setRegD(String s,char signR)
        {
            int i;
            signD = (s[0] == '-') ? true : false;
            signDR = (signR == '-') ? true : false;
            for (i = 1; i < s.Length; i++)
            {
                regD[i-1] = Convert.ToInt32(s.Substring(i, 1));
            }
        }

        public void setRegE(String s,char signR)
        {
            int i;
            signE = (s[0] == '-') ? true : false;
            signER = (signR == '-') ? true : false;
            for (i = 1; i < s.Length; i++)
            {
                regE[i-1] = Convert.ToInt32(s.Substring(i, 1));
            }
        }

        public void setRegF(String s,char signR)
        {
            int i;
            signF = (s[0] == '-') ? true : false;
            signFR = (signR == '-') ? true : false;
            for (i = 1; i < s.Length; i++)
            {
                regF[i-1] = Convert.ToInt32(s.Substring(i, 1));
            }
        }

        public void setRegG(String s,char signR)
        {
            int i;
            signG = (s[0] == '-') ? true : false;
            signGR = (signR == '-') ? true : false;
            for (i = 1; i < s.Length; i++)
            {
                regG[i-1] = Convert.ToInt32(s.Substring(i, 1));
            }
        }

        public void setRegH(String s,char signR)
        {
            int i;
            signH = (s[0] == '-') ? true : false;
            signHR = (signR == '-') ? true : false;
            for (i = 1; i < s.Length; i++)
            {
                regH[i-1] = Convert.ToInt32(s.Substring(i, 1));
            }
        }

        public void setPowered(Boolean b)
        {
            powered = b;
        }

        public Boolean getPowered()
        {
            return powered;
        }

        public void setProgramOutput(int n, String s)
        {
            programOutputs[n] = s;
            programOutputsNumber[n] = machine.getBusNumber(s);
            programOutputsPosition[n] = machine.getBusPosition(s);
        }

        public String getProgramOutput(int n)
        {
            return programOutputs[n];
        }

        public void setProgramInput(int n, String s)
        {
            programInputs[n] = s;
            programInputsNumber[n] = machine.getBusNumber(s);
            programInputsPosition[n] = machine.getBusPosition(s);
        }

        public String getProgramInput(int n)
        {
            return programInputs[n];
        }

        public void setDigitOutput(int n, String s)
        {
            if (n == 0)
            {
                outputA = s;
                outputANumber = machine.getBusNumber(s);
                outputAPosition = machine.getBusPosition(s);
            }
            if (n == 1) outputS = s;
        }

        public String getDigitOutput(int n)
        {
            if (n == 0) return outputA;
            if (n == 1) return outputS;
            return null;
        }

        public Boolean getJLSign()
        {
            return jLSign;
        }

        public void setJLSign(Boolean b)
        {
            jLSign = b;
        }

        public Boolean getJRSign()
        {
            return jRSign;
        }

        public void setJRSign(Boolean b)
        {
            jRSign = b;
        }
        public Boolean getKLSign()
        {
            return kLSign;
        }

        public void setKLSign(Boolean b)
        {
            kLSign = b;
        }
        public Boolean getKRSign()
        {
            return kRSign;
        }

        public void setKRSign(Boolean b)
        {
            kRSign = b;
        }
        public int getRegJ(int n)
        {
            return regJ[n];
        }

        public int getRegK(int n)
        {
            return regK[n];
        }

        public void setRegJ(int n, int v)
        {
            regJ[n] = v;
        }

        public void setRegK(int n, int v) {
            regK[n] = v;
        }

        public int getOperationSwitch(int n)
        {
            return programSwitches[n];
        }

        public void setOperationSwitch(int n, int p)
        {
            programSwitches[n] = p;
        }

        public Boolean getSelectSwitch(int n)
        {
            return selectSwitches[n];
        }

        public void setSelectSwitch(int n, Boolean b)
        {
            selectSwitches[n] = b;
        }

        public void save(StreamWriter file)
        {
            int i;
            String tmp;
            file.WriteLine("constant {");
            if (powered) file.WriteLine("  power " + powered.ToString());
            for (i = 0; i < 30; i++)
            {
                if (programSwitches[i] != 0 ||
                    selectSwitches[i] != false ||
                    programInputs[i].Length > 2 ||
                    programOutputs[i].Length > 2)
                {
                    file.WriteLine("  program " + (i + 1).ToString() + " {");
                    switch (programSwitches[i])
                    {
                        case 0: file.WriteLine("    mode l"); break;
                        case 1: file.WriteLine("    mode lr"); break;
                        case 2: file.WriteLine("    mode r"); break;
                    }
                    if (selectSwitches[i]) file.WriteLine("    select r");
                    else file.WriteLine("    select l");
                    if (programInputs[i].Length > 2) file.WriteLine("    programin " + programInputs[i]);
                    if (programOutputs[i].Length > 2) file.WriteLine("    programout " + programOutputs[i]);
                    file.WriteLine("    }");
                }
            }
            if (jLSign) file.WriteLine("  jlm " + jLSign.ToString());
            if (jRSign) file.WriteLine("  jrm " + jRSign.ToString());
            if (kLSign) file.WriteLine("  klm " + kLSign.ToString());
            if (kRSign) file.WriteLine("  krm " + kRSign.ToString());
            if (outputA.Length > 2) file.WriteLine("  outputa " + outputA);
            if (outputS.Length > 2) file.WriteLine("  outputs " + outputS);
            tmp = machine.digitArrayToString(jLSign, regJ);
            if (tmp.CompareTo("+0000000000") != 0) file.WriteLine("  regj " + machine.digitArrayToString(jLSign,regJ));
            tmp = machine.digitArrayToString(kLSign, regK);
            if (tmp.CompareTo("+0000000000") != 0) file.WriteLine("  regk " + machine.digitArrayToString(kLSign, regK));
            file.WriteLine("  }");
        }

        public void loadProgram(int p, StreamReader file)
        {
            String line;
            line = "";
            p--;
            while (!file.EndOfStream && line.CompareTo("}") != 0)
            {
                line = file.ReadLine().Trim().ToLower();
                if (line.CompareTo("mode l") == 0) programSwitches[p] = 0;
                if (line.CompareTo("mode lr") == 0) programSwitches[p] = 1;
                if (line.CompareTo("mode r") == 0) programSwitches[p] = 2;
                if (line.CompareTo("select l") == 0) selectSwitches[p] = false;
                if (line.CompareTo("select r") == 0) selectSwitches[p] = true;
                if (line.StartsWith("programin"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setProgramInput(p, line);
//                    programInputs[p] = line;
                }
                if (line.StartsWith("programout"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setProgramOutput(p, line);
//                    programOutputs[p] = line;
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
                if (line.StartsWith("regj"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    machine.stringToDigitArray(line, regJ);
                }
                if (line.StartsWith("regk"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    machine.stringToDigitArray(line, regK);
                }
                if (line.StartsWith("jlm"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    jLSign = Convert.ToBoolean(line);
                }
                if (line.StartsWith("jrm"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    jRSign = Convert.ToBoolean(line);
                }
                if (line.StartsWith("klm"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    kLSign = Convert.ToBoolean(line);
                }
                if (line.StartsWith("krm"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    kRSign = Convert.ToBoolean(line);
                }
                if (line.StartsWith("outputa"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDigitOutput(0, line);
//                    outputA = line;
                }
                if (line.StartsWith("outputs"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    outputS = line;
                }
                if (line.StartsWith("program"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    pos = Convert.ToInt32(line.Substring(0, line.IndexOf(' ')));
                    loadProgram(pos, file);
                }
            }
        }

        public void setupDecades()
        {
            int i;
            int pos;
            pos = operation % 10;
            need11p = 1;
            switch (pos)
            {
                case 0:
                    if (selectSwitches[operation]) {
                        for (i = 0; i < 10; i++) decades[i] = regB[i];
                        sign = signB;
                        signR = signBR;
                    } else {
                        for (i = 0; i < 10; i++) decades[i] = regA[i];
                        sign = signA;
                        signR = signAR;
                    }
                    break;
                case 1:
                    if (selectSwitches[operation])
                    {
                        for (i = 0; i < 10; i++) decades[i] = regB[i];
                        sign = signB;
                        signR = signBR;
                    }
                    else
                    {
                        for (i = 0; i < 10; i++) decades[i] = regA[i];
                        sign = signA;
                        signR = signAR;
                    }
                    break;
                case 2:
                    if (selectSwitches[operation])
                    {
                        for (i = 0; i < 10; i++) decades[i] = regD[i];
                        sign = signD;
                        signR = signDR;
                    }
                    else
                    {
                        for (i = 0; i < 10; i++) decades[i] = regC[i];
                        sign = signC;
                        signR = signCR;
                    }
                    break;
                case 3:
                    if (selectSwitches[operation])
                    {
                        for (i = 0; i < 10; i++) decades[i] = regD[i];
                        sign = signD;
                        signR = signDR;
                    }
                    else
                    {
                        for (i = 0; i < 10; i++) decades[i] = regC[i];
                        sign = signC;
                        signR = signCR;
                    }
                    break;
                case 4:
                    if (selectSwitches[operation])
                    {
                        for (i = 0; i < 10; i++) decades[i] = regF[i];
                        sign = signF;
                        signR = signFR;
                    }
                    else
                    {
                        for (i = 0; i < 10; i++) decades[i] = regE[i];
                        sign = signE;
                        signR = signER;
                    }
                    break;
                case 5:
                    if (selectSwitches[operation])
                    {
                        for (i = 0; i < 10; i++) decades[i] = regF[i];
                        sign = signF;
                        signR = signFR;
                    }
                    else
                    {
                        for (i = 0; i < 10; i++) decades[i] = regE[i];
                        sign = signE;
                        signR = signER;
                    }
                    break;
                case 6:
                    if (selectSwitches[operation])
                    {
                        for (i = 0; i < 10; i++) decades[i] = regH[i];
                        sign = signH;
                        signR = signHR;
                    }
                    else
                    {
                        for (i = 0; i < 10; i++) decades[i] = regG[i];
                        sign = signG;
                        signR = signGR;
                    }
                    break;
                case 7:
                    if (selectSwitches[operation])
                    {
                        for (i = 0; i < 10; i++) decades[i] = regH[i];
                        sign = signH;
                        signR = signHR;
                    }
                    else
                    {
                        for (i = 0; i < 10; i++) decades[i] = regG[i];
                        sign = signG;
                        signR = signGR;
                    }
                    break;
                case 8:
                    need11p = 0;
                    if (selectSwitches[operation])
                    {
                        for (i = 0; i < 10; i++) decades[i] = regK[i];
                        if (programSwitches[operation] == 0) sign = kLSign;
                        if (programSwitches[operation] == 1) sign = kLSign;
                        if (programSwitches[operation] == 2) sign = kRSign;
                    }
                    else
                    {
                        for (i = 0; i < 10; i++) decades[i] = regJ[i];
                        if (programSwitches[operation] == 0) sign = jLSign;
                        if (programSwitches[operation] == 1) sign = jLSign;
                        if (programSwitches[operation] == 2) sign = jRSign;
                    }
                    break;
                case 9:
                    need11p = 0;
                    if (selectSwitches[operation])
                    {
                        for (i = 0; i < 10; i++) decades[i] = regK[i];
                        if (programSwitches[operation] == 0) sign = kLSign;
                        if (programSwitches[operation] == 1) sign = kLSign;
                        if (programSwitches[operation] == 2) sign = kRSign;
                    }
                    else
                    {
                        for (i = 0; i < 10; i++) decades[i] = regJ[i];
                        if (programSwitches[operation] == 0) sign = jLSign;
                        if (programSwitches[operation] == 1) sign = jLSign;
                        if (programSwitches[operation] == 2) sign = jRSign;
                    }
                    break;
            }
            if (programSwitches[operation] == 0)
            {
                for (i = 5; i < 10; i++)
                {
                    decades[i] = 0;
                }
                if (sign && (need11p > 0)) need11p = 1<<5;
            }
            if (programSwitches[operation] == 2)
            {
                sign = signR;
                for (i = 0; i < 5; i++)
                {
                    decades[i] = (sign) ? 9 : 0;
                }
            }
        }

        public void cycle1(int clocks)      /* This sets any outputs */
        {
            int i;
            int a, s;
            if (!powered) return;
            if (operation < 0) return;
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
//               if (programOutputs[operation].Length > 2)
//               {
//                    machine.setBus(programOutputs[operation], 1);
//                    machine.log("Constant: sent pulse " + programOutputs[operation]);
//               }
               if (programOutputsNumber[operation] >= 0)
               {
                   machine.setBus(programOutputsNumber[operation], programOutputsPosition[operation],1);
                   machine.log("Constant: sent pulse " + programOutputs[operation]);
               }
               operation = -1;
            }
            if (operation < 0) return;
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
            if ((clocks & EniacCyclingUnit.C11P) == EniacCyclingUnit.C11P)
            {
                if (sign && (need11p > 0)) a = need11p;
                need11p = 0;
            }
//            if ((clocks & EniacCyclingUnit.C11P) == EniacCyclingUnit.C11P)
//            {
//                if (sign) a |= 1; else s |= 1;
//            }
//            if (outputA.Length > 2)
//            {
//                machine.setBus(outputA, a);
//            }
            if (outputANumber >= 0)
            {
                machine.setBus(outputANumber,-1 , a);
            }
//            if (outputS.Length > 2)
//            {
//                machine.setBus(outputS, s);
//            }
        }

        public void cycle2(int clocks)      /* This processes any inputs */
        {
            int i;
            int bus;
            if (!powered) return;
            if (operation < 0)
            {
                for (i = 0; i < 30; i++)
                {
//                    if (programInputs[i].Length > 3)
//                    {
//                        bus = machine.getBus(programInputs[i]);
//                        if (bus != 0)
//                        {
//                            operationTrigger = i;
//                            machine.log("Constant: Operation " + (i+1).ToString() + " triggered by " + programInputs[i]);
//                        }
//                    }
                    if (programInputsNumber[i] >= 0)
                    {
                        bus = machine.getBus(programInputsNumber[i],programInputsPosition[i]);
                        if (bus != 0)
                        {
                            operationTrigger = i;
                            machine.log("Constant: Operation " + (i + 1).ToString() + " triggered by " + programInputs[i]);
                        }
                    }
                }
                if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP && operationTrigger >= 0)
                {
                    operation = operationTrigger;
                    operationTrigger = -1;
                    setupDecades();
                    machine.log("Constant: Operation " + (operation+1).ToString() + " started");
                }
            }
        }



    }
}
