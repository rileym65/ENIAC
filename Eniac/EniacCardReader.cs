using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eniac
{
    class EniacCardReader
    {
        private Boolean powered;
        private Machine machine;
        private String programInput;
        private int programInputNumber;
        private int programInputPosition;
        private String programOutput;
        private int programOutputNumber;
        private int programOutputPosition;
        private String interlockIn;
        private int interlockInNumber;
        private int interlockInPosition;
        private int operation;
        private int operationTrigger;
        private String card;
        private Boolean interlock;
        private Boolean buttonStart;

        public EniacCardReader(Machine m)
        {
            machine = m;
            reset();
        }

        public void reset()
        {
            programInput = "";
            programInputNumber = -1;
            programInputPosition = -1;
            programOutput = "";
            programOutputNumber = -1;
            programOutputPosition = -1;
            interlockIn = "";
            interlockInNumber = -1;
            interlockInPosition = -1;
            powered = false;
            operation = -1;
            operationTrigger = -1;
            interlock = false;
        }

        public void clear()
        {
            interlock = false;
            buttonStart = false;
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

        public String getInterlockIn()
        {
            return interlockIn;
        }

        public void setInterlockIn(String s)
        {
            interlockIn = s;
            interlockInNumber = machine.getBusNumber(s);
            interlockInPosition = machine.getBusPosition(s);
        }

        public void save(StreamWriter file)
        {
            if (powered || programInput.Length > 2 || programOutput.Length > 2 || interlockIn.Length > 2)
            {
                file.WriteLine("cardreader {");
                if (powered) file.WriteLine("  power " + powered.ToString());
                if (programInput.Length > 2) file.WriteLine("  programin " + programInput);
                if (programOutput.Length > 2) file.WriteLine("  programout " + programOutput);
                if (interlockIn.Length > 2) file.WriteLine("  interlock " + interlockIn);
                file.WriteLine("  }");
            }
        }

        public void load(StreamReader file)
        {
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
                if (line.StartsWith("interlock"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setInterlockIn(line);
//                    interlockIn = line;
                }
            }
        }

        public void start()
        {
            buttonStart = true;
            operationTrigger = 250;
        }

        public String replaceAt(String input, int pos, char newch)
        {
            return input.Substring(0, pos) + newch.ToString() + input.Substring(pos + 1);
        }

        public void cycle1(int clocks)      /* This sets any outputs */
        {
            int i;
            int j;
            int s;
            int c;
            EniacConstantTransmitter constUnit;
            String tmp;
            char signL;
            char signR;
            if (!powered) return;
            if (operation < 0) return;
            if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                if (operation > 0) operation--;
                if (operation == 10)
                {
                    card = machine.readCard();
                    if (card == null)
                    {
                        machine.log("Card reader empty");
                        operation = -1;
                        operationTrigger = -1;
                        return;
                    }
                    machine.log("Card: " + card);
                    machine.stackCard(card);
                    constUnit = machine.getConstUnit();
                    for (i = 0; i < 8; i++)
                    {
                        tmp = "";
                        signL = '+';
                        signR = '+';
                        while (tmp.Length != 10)
                        {
                            if (card.Length < 1) card += "0";
                            if (card[0] == '+' || card[0] == '-')
                            {
                                if (tmp.Length < 5) { signL = card[0]; signR = signL; }  else signR = card[0];
                            }
                            else if (card[0] >= '0' && card[0] <= '9') tmp += card[0];
                            else tmp += '0';
                            card = card.Substring(1);
                        }
                        if (signL == '-')
                        {
//                            s = 10;
                            s = 9;
                            for (j = 0; j <5; j++)
                            {
                                c = tmp[j] - '0';
                                if (c != 0 || s == 9) tmp = replaceAt(tmp, j, Convert.ToChar(s - c + '0'));
                                if (c != 0) s = 9;
                            }
                        }
                        if (signR == '-')
                        {
                            //                            s = 10;
                            s = 9;
                            for (j = 5; j < 10; j++)
                            {
                                c = tmp[j] - '0';
                                if (c != 0 || s == 9) tmp = replaceAt(tmp, j, Convert.ToChar(s - c + '0'));
                                if (c != 0) s = 9;
                            }
                        }
                        tmp = signL + tmp;
                        machine.log("Read number " + tmp + " from card");
                        switch (i)
                        {
                            case 0: constUnit.setRegA(tmp,signR); break;
                            case 1: constUnit.setRegB(tmp,signR); break;
                            case 2: constUnit.setRegC(tmp,signR); break;
                            case 3: constUnit.setRegD(tmp,signR); break;
                            case 4: constUnit.setRegE(tmp,signR); break;
                            case 5: constUnit.setRegF(tmp,signR); break;
                            case 6: constUnit.setRegG(tmp,signR); break;
                            case 7: constUnit.setRegH(tmp,signR); break;
                        }
                    }
                }
                if (operation == 0)
                {
//                    if (interlock && programOutput.Length > 2)
//                    {
//                        machine.setBus(programOutput, 1);
//                        operation = -1;
//                    }
                    if (interlock && programOutputNumber >= 0)
                    {
                        machine.setBus(programOutputNumber, programOutputPosition, 1);
                        operation = -1;
                    }
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
//                        operationTrigger = 250;
//                    }
//                }
                if (programInputNumber >= 0)
                {
                    bus = machine.getBus(programInputNumber, programInputPosition);
                    if (bus != 0)
                    {
                        operationTrigger = 250;
                    }
                }
                if ((clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP && operationTrigger >= 0)
                {
                    operation = operationTrigger;
                    operationTrigger = -1;
                    interlock = buttonStart;
                    buttonStart = false;
                    machine.log("Reader operation started");
                }
            }
//            if (operation >= 0 && interlockIn.Length > 2)
//            {
//                bus = machine.getBus(interlockIn);
//                if (bus != 0) interlock = true;
//            }
            if (operation >= 0 && interlockInNumber >= 0)
            {
                bus = machine.getBus(interlockInNumber, interlockInPosition);
                if (bus != 0) interlock = true;
            }
        }

    }
}
