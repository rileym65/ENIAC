using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

namespace Eniac
{
    class EniacCardPunch
    {
        private String programInput;
        private int programInputNumber;
        private int programInputPosition;
        private String programOutput;
        private int programOutputNumber;
        private int programOutputPosition;
        private String cardStacker;
        private Machine machine;
        private Boolean powered;
        private int operation;
        private int operationTrigger;
        private String card;
        private Boolean[] couplerSwitches;
        private Boolean[] printSwitches;

        public EniacCardPunch(Machine m)
        {
            machine = m;
            couplerSwitches = new Boolean[16];
            printSwitches = new Boolean[16];
            reset();
        }

        public void reset()
        {
            int i;
            programInput = "";
            programInputNumber = -1;
            programInputPosition = -1;
            programOutput = "";
            programOutputNumber = -1;
            programOutputPosition = -1;
            cardStacker = "";
            powered = false;
            operation = -1;
            operationTrigger = -1;
            card = "";
            for (i = 0; i < 16; i++) couplerSwitches[i] = false;
            for (i = 0; i < 16; i++) printSwitches[i] = true;
        }

        public void setPowered(Boolean b)
        {
            powered = b;
        }

        public Boolean getPowered()
        {
            return powered;
        }

        public void setProgramInput(String s)
        {
            programInput = s;
            programInputNumber = machine.getBusNumber(s);
            programInputPosition = machine.getBusPosition(s);
        }

        public void setProgramOutput(String s)
        {
            programOutput = s;
            programOutputNumber = machine.getBusNumber(s);
            programOutputPosition = machine.getBusPosition(s);
        }

        public String getProgramInput()
        {
            return programInput;
        }

        public String getProgramOutput()
        {
            return programOutput;
        }

        public String getCardStacker()
        {
            return cardStacker;
        }

        public void setCardStacker(String s) {
            cardStacker = s;
        }

        public void setCouplerSwitch(int n, Boolean b)
        {
            couplerSwitches[n] = b;
        }

        public Boolean getCouplerSwitch(int n)
        {
            return couplerSwitches[n];
        }

        public void setPrintSwitch(int n, Boolean b)
        {
            printSwitches[n] = b;
        }

        public Boolean getPrintSwitch(int n)
        {
            return printSwitches[n];
        }

        public void add(String s)
        {
            cardStacker += (s + "\r\n");
        }
        
        public void save(StreamWriter file)
        {
            int i;
            Boolean flag;
            flag = false;
            for (i = 0; i < 16; i++) if (couplerSwitches[i]) flag = true;
            for (i = 0; i < 16; i++) if (printSwitches[i] == false) flag = true;
            if (powered || programInput.Length > 2 || programOutput.Length > 2) flag = true;
            if (flag)
            {
                file.WriteLine("cardpunch {");
                if (powered) file.WriteLine("  power " + powered.ToString());
                if (programInput.Length > 2) file.WriteLine("  programin " + programInput);
                if (programOutput.Length > 2) file.WriteLine("  programout " + programOutput);
                for (i = 0; i < 16; i++) if (couplerSwitches[i]) file.WriteLine("  coupler " + (i + 1).ToString() + " c");
                for (i = 0; i < 16; i++) if (printSwitches[i] == false) file.WriteLine("  print " + (i + 1).ToString() + " off");
                file.WriteLine("  }");
            }
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
                if (line.StartsWith("power"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    powered = Convert.ToBoolean(line);
                }
                if (line.StartsWith("programin"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setProgramInput(line);
//                    programInput = line;
                }
                if (line.StartsWith("programout"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setProgramOutput(line);
//                    programOutput = line;
                }
                if (line.StartsWith("coupler"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    temp = line.Substring(0, line.IndexOf(' ')).Trim().ToUpper();
                    num = Convert.ToInt32(temp) - 1;
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    if (line.StartsWith("C")) setCouplerSwitch(num, true);
                    if (line.StartsWith("0")) setCouplerSwitch(num, false);
                }
                if (line.StartsWith("print"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    temp = line.Substring(0, line.IndexOf(' ')).Trim().ToUpper();
                    num = Convert.ToInt32(temp) - 1;
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    if (line.StartsWith("OFF")) setPrintSwitch(num, false);
                    if (line.StartsWith("ON")) setPrintSwitch(num, true);
                }
            }
        }

        public String getCard()
        {
            String ret;
            ret = card;
            card = "";
            return ret;
        }

        public String replaceAt(String input, int pos, char newch)
        {
            return input.Substring(0, pos) + newch.ToString() + input.Substring(pos + 1);
        }

        private int nextGroup(int group)
        {
            while (group < 16)
            {
                if (couplerSwitches[group] == false) return group;
                group++;
            }
            return group;
        }

        private String performPunch()
        {
            String tmp;
            String signs;
            String accValue;
            int group;
            int endGroup;
            int c;
            String ret;
            int i, j;
            tmp = "";
            signs = "";
            accValue = machine.getAccumulator(0).getValue();
            signs += accValue.Substring(0, 1) + accValue.Substring(0,1);
            tmp += accValue.Substring(1);
            accValue = machine.getAccumulator(1).getValue();
            signs += accValue.Substring(0, 1) + accValue.Substring(0, 1);
            tmp += accValue.Substring(1);
            accValue = machine.getAccumulator(14).getValue();
            signs += accValue.Substring(0, 1) + accValue.Substring(0, 1);
            tmp += accValue.Substring(1);
            accValue = machine.getAccumulator(15).getValue();
            signs += accValue.Substring(0, 1) + accValue.Substring(0, 1);
            tmp += accValue.Substring(1);
            accValue = machine.getAccumulator(16).getValue();
            signs += accValue.Substring(0, 1) + accValue.Substring(0, 1);
            tmp += accValue.Substring(1);
            accValue = machine.getAccumulator(17).getValue();
            signs += accValue.Substring(0, 1) + accValue.Substring(0, 1);
            tmp += accValue.Substring(1);
            accValue = machine.getAccumulator(18).getValue();
            signs += accValue.Substring(0, 1) + accValue.Substring(0, 1);
            tmp += accValue.Substring(1);
            accValue = machine.getAccumulator(19).getValue();
            signs += accValue.Substring(0, 1) + accValue.Substring(0, 1);
            tmp += accValue.Substring(1);
/* ***** Handle negative numbers ***** */
            group = 0;
            while (group < 16)
            {
                endGroup = nextGroup(group);
                if (endGroup > 15) endGroup = 15;
                if (signs[group] == '-')
                {
                    j = 10;
                    for (i = (5 * (endGroup + 1) - 1); i >= (group*5); i--)
                    {
                        c = tmp[i] - '0';
                        if (c != 0 || j == 9) tmp = replaceAt(tmp, i, Convert.ToChar(j - c + '0'));
                        if (c != 0) j = 9;
                    }
                }
                group = endGroup + 1;
            }
/* *********************************** */
/* ***** Filter out where print switches are set to off ***** */
            for (i=0; i<16; i++)
                if (printSwitches[i] == false)
                {
                    for (j = (i * 5); j < ((i + 1) * 5); j++) tmp = replaceAt(tmp, j, ' ');
                }
/* ********************************************************** */
/* ***** Assemble final output                          ***** */
            ret = "";
            group = 0;
            while (group < 16)
            {
                endGroup = nextGroup(group);
                if (endGroup > 15) endGroup = 15;
                if (printSwitches[group])  ret += signs.Substring(group, 1);
                for (i=(group * 5); i<(5*(endGroup + 1)); i++) 
                {
                    ret += tmp[i];
                }
                group = endGroup + 1;
            }
            return ret;
            
        }

        public void cycle1(int clocks)      /* This sets any outputs */
        {
            if (!powered) return;
            if (operation < 0) return;
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                operation--;
                if (operation == 10)
                {
                    card = performPunch();
/*
                    card = "";
                    card += machine.getAccumulator(0).getValue();
                    card += machine.getAccumulator(1).getValue();
                    card += machine.getAccumulator(14).getValue();
                    card += machine.getAccumulator(15).getValue();
                    card += machine.getAccumulator(16).getValue();
                    card += machine.getAccumulator(17).getValue();
                    card += machine.getAccumulator(18).getValue();
                    card += machine.getAccumulator(19).getValue();
 */
                }
                if (operation < 0)
                {
//                    if (programOutput.Length > 2)
//                    {
//                        machine.setBus(programOutput, 1);
//                        machine.log("Punch: Sent pulse " + programOutput);
//                    }
                    if (programOutputNumber >= 0)
                    {
                        machine.setBus(programOutputNumber, programOutputPosition, 1);
                        machine.log("Punch: Sent pulse " + programOutput);
                    }
                    operation = -1;
                }
            }
        }

        public void cycle2(int clocks)      /* This processes any inputs */
        {
            int bus;
            if (!powered) return;
            if (operation < 0)
            {
//                if (programInput.Length > 3)
//                {
//                    bus = machine.getBus(programInput);
//                    if (bus != 0)
//                    {
//                        operationTrigger = 140;
//                    }
//                }
                if (programInputNumber >= 0)
                {
                    bus = machine.getBus(programInputNumber,programInputPosition);
                    if (bus != 0)
                    {
                        operationTrigger = 140;
                    }
                }
                if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP && operationTrigger >= 0)
                {
                    operation = operationTrigger;
                    operationTrigger = -1;
                    machine.log("Punch: Operation triggered by " + programInput);
                }
            }
        }

    }
}
