using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Eniac
{
    class EniacMultiplier
    {
        private Machine machine;
        private int[] ierOperationSwitches;
        private int[] icandOperationSwitches;
        private Boolean[] ierClearSwitches;
        private Boolean[] icandClearSwitches;
        private int[] significantFigures;
        private int[] places;
        private int[] product;
        private String[] rProgramOut;
        private int[] rProgramOutNumber;
        private int[] rProgramOutPosition;
        private String[] dProgramOut;
        private int[] dProgramOutNumber;
        private int[] dProgramOutPosition;
        private String[] programIn;
        private int[] programInNumber;
        private int[] programInPosition;
        private String[] programOut;
        private int[] programOutNumber;
        private int[] programOutPosition;
        private Boolean powered;
        private String outputA;
        private String outputS;
        private String outputAS;
        private String outputAC;
        private String outputSC;
        private String outputASC;
        private String outputRS;
        private int[] productNumber;
        private int[] productPosition;
        private int outputRSNumber;
        private int outputRSPosition;
        private String outputDS;
        private int outputDSNumber;
        private int outputDSPosition;
        private String outputF;
        private int outputFNumber;
        private int outputFPosition;
        private String lhpp1Out;
        private int lhpp1OutNumber;
        private int lhpp1OutPosition;
        private String lhpp2Out;
        private int lhpp2OutNumber;
        private int lhpp2OutPosition;
        private String rhpp1Out;
        private int rhpp1OutNumber;
        private int rhpp1OutPosition;
        private String rhpp2Out;
        private int rhpp2OutNumber;
        private int rhpp2OutPosition;
        private int phase;
        private int operation;
        private int[] lhpp;
        private int[] rhpp;
        private int inputFF;

        public EniacMultiplier(Machine m)
        {
            machine = m;
            ierOperationSwitches = new int[24];
            icandOperationSwitches = new int[24];
            ierClearSwitches = new Boolean[24];
            icandClearSwitches = new Boolean[24];
            significantFigures = new int[24];
            lhpp = new int[20];
            rhpp = new int[20];
            product = new int[24];
            places = new int[24];
            programIn = new String[24];
            programInNumber = new int[24];
            programInPosition = new int[24];
            programOut = new String[24];
            programOutNumber = new int[24];
            programOutPosition = new int[24];
            rProgramOut = new String[5];
            rProgramOutNumber = new int[5];
            rProgramOutPosition = new int[5];
            dProgramOut = new String[5];
            dProgramOutNumber = new int[5];
            dProgramOutPosition = new int[5];
            productNumber = new int[7];
            productPosition = new int[7];
            reset();
        }

        public void reset()
        {
            int i;
            for (i = 0; i < 24; i++) ierOperationSwitches[i] = 0;
            for (i = 0; i < 24; i++) icandOperationSwitches[i] = 0;
            for (i = 0; i < 24; i++) ierClearSwitches[i] = false;
            for (i = 0; i < 24; i++) icandClearSwitches[i] = false;
            for (i = 0; i < 24; i++) significantFigures[i] = 0;
            for (i = 0; i < 24; i++) places[i] = 8;
            for (i = 0; i < 24; i++) product[i] = 0;
            for (i = 0; i < 24; i++)
            {
                programIn[i] = "";
                programInNumber[i] = -1;
                programInPosition[i] = -1;
            }
            for (i = 0; i < 24; i++)
            {
                programOut[i] = "";
                programOutNumber[i] = -1;
                programOutPosition[i] = -1;
            }
            for (i = 0; i < 5; i++)
            {
                rProgramOut[i] = "";
                rProgramOutNumber[i] = -1;
                rProgramOutPosition[i] = -1;
            }
            for (i = 0; i < 5; i++)
            {
                dProgramOut[i] = "";
                dProgramOutNumber[i] = -1;
                dProgramOutPosition[i] = -1;
            }
            for (i = 0; i < 7; i++)
            {
                productNumber[i] = -1;
                productPosition[i] = -1;
            }
            outputA = "";
            outputS = "";
            outputAS = "";
            outputAC = "";
            outputSC = "";
            outputASC = "";
            outputRS = "";
            outputRSNumber = -1;
            outputRSPosition = -1;
            outputDS = "";
            outputDSNumber = -1;
            outputDSPosition = -1;
            outputF = "";
            outputFNumber = -1;
            outputFPosition = -1;
            lhpp1Out = "";
            lhpp1OutNumber = -1;
            lhpp1OutPosition = -1;
            lhpp2Out = "";
            lhpp2OutNumber = -1;
            lhpp2OutPosition = -1;
            rhpp1Out = "";
            rhpp1OutNumber = -1;
            rhpp1OutPosition = -1;
            rhpp2Out = "";
            rhpp2OutNumber = -1;
            rhpp2OutPosition = -1;
            phase = 0;
            operation = -1;
            inputFF = -1;
            powered = false;
        }

        public void clear()
        {
            operation = -1;
            phase = 0;
            inputFF = -1;
        }

        public void loadProgram(int p, StreamReader file)
        {
            int tmp;
            String line;
            line = "";
            p--;
            while (!file.EndOfStream && line.CompareTo("}") != 0)
            {
                line = file.ReadLine().Trim().ToLower();
                if (line.CompareTo("iermode alpha") == 0) ierOperationSwitches[p] = 0;
                if (line.CompareTo("iermode beta") == 0) ierOperationSwitches[p] = 1;
                if (line.CompareTo("iermode gamma") == 0) ierOperationSwitches[p] = 2;
                if (line.CompareTo("iermode delta") == 0) ierOperationSwitches[p] = 3;
                if (line.CompareTo("iermode epsilon") == 0) ierOperationSwitches[p] = 4;
                if (line.CompareTo("iermode 0") == 0) ierOperationSwitches[p] = 5;
                if (line.CompareTo("ierclear true") == 0) ierClearSwitches[p] = true;
                if (line.CompareTo("ierclear false") == 0) ierClearSwitches[p] = false;
                if (line.CompareTo("icandmode alpha") == 0) icandOperationSwitches[p] = 0;
                if (line.CompareTo("icandmode beta") == 0) icandOperationSwitches[p] = 1;
                if (line.CompareTo("icandmode gamma") == 0) icandOperationSwitches[p] = 2;
                if (line.CompareTo("icandmode delta") == 0) icandOperationSwitches[p] = 3;
                if (line.CompareTo("icandmode epsilon") == 0) icandOperationSwitches[p] = 4;
                if (line.CompareTo("icandmode 0") == 0) icandOperationSwitches[p] = 5;
                if (line.CompareTo("icandclear true") == 0) icandClearSwitches[p] = true;
                if (line.CompareTo("icandclear false") == 0) icandClearSwitches[p] = false;
                if (line.CompareTo("product a") == 0) product[p] = 0;
                if (line.CompareTo("product s") == 0) product[p] = 1;
                if (line.CompareTo("product as") == 0) product[p] = 2;
                if (line.CompareTo("product 0") == 0) product[p] = 3;
                if (line.CompareTo("product ac") == 0) product[p] = 4;
                if (line.CompareTo("product sc") == 0) product[p] = 5;
                if (line.CompareTo("product asc") == 0) product[p] = 6;
                if (line.CompareTo("figures off") == 0) significantFigures[p] = 0;
                else if (line.StartsWith("figures"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    tmp = Convert.ToInt32(line) - 1;
                    if (tmp < 10) significantFigures[p] = tmp;
                }
                if (line.StartsWith("places"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    tmp = Convert.ToInt32(line) - 2;
                    if (tmp < 9) places[p] = tmp;
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
                if (line.StartsWith("ralpha"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setRProgramOut(0, line);
//                    rProgramOut[0] = line;
                }
                if (line.StartsWith("rbeta"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setRProgramOut(1, line);
//                    rProgramOut[1] = line;
                }
                if (line.StartsWith("rgamma"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setRProgramOut(2, line);
//                    rProgramOut[2] = line;
                }
                if (line.StartsWith("rdelta"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setRProgramOut(3, line);
//                    rProgramOut[3] = line;
                }
                if (line.StartsWith("repsilon"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setRProgramOut(4, line);
//                    rProgramOut[4] = line;
                }

                if (line.StartsWith("dalpha"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDProgramOut(0, line);
//                    dProgramOut[0] = line;
                }
                if (line.StartsWith("dbeta"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDProgramOut(1, line);
//                    dProgramOut[1] = line;
                }
                if (line.StartsWith("dgamma"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDProgramOut(2, line);
//                    dProgramOut[2] = line;
                }
                if (line.StartsWith("ddelta"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDProgramOut(3, line);
//                    dProgramOut[3] = line;
                }
                if (line.StartsWith("depsilon"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setDProgramOut(4, line);
//                    dProgramOut[4] = line;
                }
                if (line.StartsWith("outputa "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputA(line);
//                    outputA = line;
                }
                if (line.StartsWith("outputs "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputS(line);
//                    outputS = line;
                }
                if (line.StartsWith("outputas "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputAS(line);
//                    outputAS = line;
                }
                if (line.StartsWith("outputac "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputAC(line);
//                    outputAC = line;
                }
                if (line.StartsWith("outputsc "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputSC(line);
//                    outputSC = line;
                }
                if (line.StartsWith("outputasc "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputASC(line);
//                    outputASC = line;
                }
                if (line.StartsWith("outputrs "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputRS(line);
//                    outputRS = line;
                }
                if (line.StartsWith("outputds "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputDS(line);
//                    outputDS = line;
                }
                if (line.StartsWith("outputf "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setOutputF(line);
//                    outputF = line;
                }
                if (line.StartsWith("lhpp1 "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setLhpp1Out(line);
//                    lhpp1Out = line;
                }
                if (line.StartsWith("lhpp2 "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setLhpp2Out(line);
//                    lhpp2Out = line;
                }
                if (line.StartsWith("rhpp1 "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setRhpp1Out(line);
//                    rhpp1Out = line;
                }
                if (line.StartsWith("rhpp2 "))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    setRhpp2Out(line);
//                    rhpp2Out = line;
                }


                if (line.StartsWith("program"))
                {
                    line = line.Substring(line.IndexOf(' ')).Trim().ToUpper();
                    pos = Convert.ToInt32(line.Substring(0, line.IndexOf(' ')));
                    loadProgram(pos, file);
                }



            }
        }

        private void saveProgram(StreamWriter file,int prog)
        {
            Boolean flag;
            flag = false;
            if (ierOperationSwitches[prog] != 0) flag = true;
            if (icandOperationSwitches[prog] != 0) flag = true;
            if (ierClearSwitches[prog]) flag = true;
            if (icandClearSwitches[prog]) flag = true;
            if (significantFigures[prog] != 0) flag = true;
            if (places[prog] != 8) flag = true;
            if (programIn[prog].Length > 2) flag = true;
            if (programOut[prog].Length > 2) flag = true;
            if (flag)
            {
                file.WriteLine("  program " + (prog + 1).ToString() + " {");
                if (ierClearSwitches[prog]) file.WriteLine("    ierclear true");
                switch (ierOperationSwitches[prog])
                {
                    case 0: file.WriteLine("    iermode alpha"); break;
                    case 1: file.WriteLine("    iermode beta"); break;
                    case 2: file.WriteLine("    iermode gamma"); break;
                    case 3: file.WriteLine("    iermode delta"); break;
                    case 4: file.WriteLine("    iermode epsilon"); break;
                    case 5: file.WriteLine("    iermode 0"); break;
                }
                if (icandClearSwitches[prog]) file.WriteLine("    icandclear true");
                switch (icandOperationSwitches[prog])
                {
                    case 0: file.WriteLine("    icandmode alpha"); break;
                    case 1: file.WriteLine("    icandmode beta"); break;
                    case 2: file.WriteLine("    icandmode gamma"); break;
                    case 3: file.WriteLine("    icandmode delta"); break;
                    case 4: file.WriteLine("    icandmode epsilon"); break;
                    case 5: file.WriteLine("    icandmode 0"); break;
                }
                switch (product[prog])
                {
                    case 0: file.WriteLine("    product A"); break;
                    case 1: file.WriteLine("    product S"); break;
                    case 2: file.WriteLine("    product AS"); break;
                    case 3: file.WriteLine("    product 0"); break;
                    case 4: file.WriteLine("    product AC"); break;
                    case 5: file.WriteLine("    product SC"); break;
                    case 6: file.WriteLine("    product ASC"); break;
                }
                file.WriteLine("    figures " + (significantFigures[prog] + 1).ToString());
                file.WriteLine("    places " + (places[prog] + 2).ToString());
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
            for (i = 0; i < 24; i++)
            {
                if (ierOperationSwitches[i] != 0) flag = true;
                if (icandOperationSwitches[i] != 0) flag = true;
                if (ierClearSwitches[i]) flag = true;
                if (icandClearSwitches[i]) flag = true;
                if (significantFigures[i] != 0) flag = true;
                if (places[i] != 8) flag = true;
                if (product[i] != 0) flag = true;
                if (outputA.Length > 2) flag = true;
                if (outputS.Length > 2) flag = true;
                if (outputAS.Length > 2) flag = true;
                if (outputAC.Length > 2) flag = true;
                if (outputSC.Length > 2) flag = true;
                if (outputASC.Length > 2) flag = true;
                if (outputRS.Length > 2) flag = true;
                if (outputDS.Length > 2) flag = true;
                if (outputF.Length > 2) flag = true;
                if (lhpp1Out.Length > 2) flag = true;
                if (lhpp2Out.Length > 2) flag = true;
                if (rhpp1Out.Length > 2) flag = true;
                if (rhpp2Out.Length > 2) flag = true;
            }
            for (i = 0; i < 5; i++)
            {
                if (rProgramOut[i].Length > 2) flag = true;
                if (dProgramOut[i].Length > 2) flag = true;
            }
            if (powered) flag = true;
            if (flag)
            {
                file.WriteLine("multiplier {");
                if (powered) file.WriteLine("  power true");
                for (i = 0; i < 24; i++) saveProgram(file, i);
                if (rProgramOut[0].Length > 2) file.WriteLine("  ralpha " + rProgramOut[0]);
                if (rProgramOut[1].Length > 2) file.WriteLine("  rbeta " + rProgramOut[1]);
                if (rProgramOut[2].Length > 2) file.WriteLine("  rgamma " + rProgramOut[2]);
                if (rProgramOut[3].Length > 2) file.WriteLine("  rdelta " + rProgramOut[3]);
                if (rProgramOut[4].Length > 2) file.WriteLine("  repsilon " + rProgramOut[4]);
                if (dProgramOut[0].Length > 2) file.WriteLine("  dalpha " + dProgramOut[0]);
                if (dProgramOut[1].Length > 2) file.WriteLine("  dbeta " + dProgramOut[1]);
                if (dProgramOut[2].Length > 2) file.WriteLine("  dgamma " + dProgramOut[2]);
                if (dProgramOut[3].Length > 2) file.WriteLine("  ddelta " + dProgramOut[3]);
                if (dProgramOut[4].Length > 2) file.WriteLine("  depsilon " + dProgramOut[4]);
                if (outputA.Length > 2) file.WriteLine("  outputa " + outputA);
                if (outputS.Length > 2) file.WriteLine("  outputs " + outputS);
                if (outputAS.Length > 2) file.WriteLine("  outputas " + outputAS);
                if (outputAC.Length > 2) file.WriteLine("  outputac " + outputAC);
                if (outputSC.Length > 2) file.WriteLine("  outputsc " + outputSC);
                if (outputASC.Length > 2) file.WriteLine("  outputasc " + outputASC);
                if (outputRS.Length > 2) file.WriteLine("  outputrs " + outputRS);
                if (outputDS.Length > 2) file.WriteLine("  outputds " + outputDS);
                if (outputF.Length > 2) file.WriteLine("  outputf " + outputF);
                if (lhpp1Out.Length > 2) file.WriteLine("  lhpp1 " + lhpp1Out);
                if (lhpp2Out.Length > 2) file.WriteLine("  lhpp2 " + lhpp2Out);
                if (rhpp1Out.Length > 2) file.WriteLine("  rhpp1 " + rhpp1Out);
                if (rhpp2Out.Length > 2) file.WriteLine("  rhpp2 " + rhpp2Out);
                file.WriteLine("  }");
            }

        }

        public int getIerOperationSwitch(int prog)
        {
            return ierOperationSwitches[prog];
        }

        public void setIerOperationSwitch(int prog, int value)
        {
            ierOperationSwitches[prog] = value;
        }

        public Boolean getIerClearSwitch(int prog)
        {
            return ierClearSwitches[prog];
        }

        public void setIerClearSwitch(int prog, Boolean b)
        {
            ierClearSwitches[prog] = b;
        }

        public int getIcandOperationSwitch(int prog)
        {
            return icandOperationSwitches[prog];
        }

        public void setIcandOperationSwitch(int prog, int value)
        {
            icandOperationSwitches[prog] = value;
        }

        public Boolean getIcandClearSwitch(int prog)
        {
            return icandClearSwitches[prog];
        }

        public void setIcandClearSwitch(int prog, Boolean b)
        {
            icandClearSwitches[prog] = b;
        }

        public Boolean getPowered()
        {
            return powered;
        }

        public void setPowered(Boolean b)
        {
            powered = b;
        }

        public int getSignificantFigures(int prog)
        {
            return significantFigures[prog];
        }

        public void setSignificantFigures(int prog, int val)
        {
            significantFigures[prog] = val;
        }

        public int getPlaces(int prog)
        {
            return places[prog];
        }

        public void setPlaces(int prog, int v)
        {
            places[prog] = v;
        }

        public int getProduct(int prog)
        {
            return product[prog];
        }

        public void setProduct(int prog, int v)
        {
            product[prog] = v;
        }

        public String getRProgramOut(int prog)
        {
            return rProgramOut[prog];
        }

        public void setRProgramOut(int prog, String s)
        {
            rProgramOut[prog] = s;
            rProgramOutNumber[prog] = machine.getBusNumber(s);
            rProgramOutPosition[prog] = machine.getBusPosition(s);
        }

        public String getDProgramOut(int prog)
        {
            return dProgramOut[prog];
        }

        public void setDProgramOut(int prog, String s)
        {
            dProgramOut[prog] = s;
            dProgramOutNumber[prog] = machine.getBusNumber(s);
            dProgramOutPosition[prog] = machine.getBusPosition(s);
        }

        public String getOutputA()
        {
            return outputA;
        }

        public void setOutputA(String s)
        {
            outputA = s;
            productNumber[0] = machine.getBusNumber(s);
            productPosition[0] = machine.getBusPosition(s);
        }

        public String getOutputS()
        {
            return outputS;
        }

        public void setOutputS(String s)
        {
            outputS = s;
            productNumber[1] = machine.getBusNumber(s);
            productPosition[1] = machine.getBusPosition(s);
        }

        public String getOutputAS()
        {
            return outputAS;
        }

        public void setOutputAS(String s)
        {
            outputAS = s;
            productNumber[2] = machine.getBusNumber(s);
            productPosition[2] = machine.getBusPosition(s);
        }

        public String getOutputAC()
        {
            return outputAC;
        }

        public void setOutputAC(String s)
        {
            outputAC = s;
            productNumber[4] = machine.getBusNumber(s);
            productPosition[4] = machine.getBusPosition(s);
        }

        public String getOutputSC()
        {
            return outputSC;
        }

        public void setOutputSC(String s)
        {
            outputSC = s;
            productNumber[5] = machine.getBusNumber(s);
            productPosition[5] = machine.getBusPosition(s);
        }

        public String getOutputASC()
        {
            return outputASC;
        }

        public void setOutputASC(String s)
        {
            outputASC = s;
            productNumber[6] = machine.getBusNumber(s);
            productPosition[6] = machine.getBusPosition(s);
        }

        public String getOutputRS()
        {
            return outputRS;
        }

        public void setOutputRS(String s)
        {
            outputRS = s;
            outputRSNumber = machine.getBusNumber(s);
            outputRSPosition = machine.getBusPosition(s);
        }

        public String getOutputDS()
        {
            return outputDS;
        }

        public void setOutputDS(String s)
        {
            outputDS = s;
            outputDSNumber = machine.getBusNumber(s);
            outputDSPosition = machine.getBusPosition(s);
        }

        public String getOutputF()
        {
            return outputF;
        }

        public void setOutputF(String s)
        {
            outputF = s;
            outputFNumber = machine.getBusNumber(s);
            outputFPosition = machine.getBusPosition(s);
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

        public String getLhpp1Out()
        {
            return lhpp1Out;
        }

        public void setLhpp1Out(String s)
        {
            lhpp1Out = s;
            lhpp1OutNumber = machine.getBusNumber(s);
            lhpp1OutPosition = machine.getBusPosition(s);
        }

        public String getLhpp2Out()
        {
            return lhpp2Out;
        }

        public void setLhpp2Out(String s)
        {
            lhpp2Out = s;
            lhpp2OutNumber = machine.getBusNumber(s);
            lhpp2OutPosition = machine.getBusPosition(s);
        }

        public String getRhpp1Out()
        {
            return rhpp1Out;
        }

        public void setRhpp1Out(String s)
        {
            rhpp1Out = s;
            rhpp1OutNumber = machine.getBusNumber(s);
            rhpp1OutPosition = machine.getBusPosition(s);
        }

        public String getRhpp2Out()
        {
            return rhpp2Out;
        }

        public void setRhpp2Out(String s)
        {
            rhpp2Out = s;
            rhpp2OutNumber = machine.getBusNumber(s);
            rhpp2OutPosition = machine.getBusPosition(s);
        }

        public void cycle1(int clocks)      /* This sets any outputs */
        {
            int i;
            int bus;
            int lh1, lh2, rh1, rh2;
            int[] ier;
            int[] icand;
            int tmp;
            if (!powered) return;
            if (operation < 0)
            {
                for (i=0; i<24; i++)
//                    if (programIn[i].Length > 2)
//                    {
//                        bus = machine.getBus(programIn[i]);
//                        if (bus != 0)
//                        {
//                            inputFF = i;
//                            machine.log("Multiplier: Operation " + (i + 1).ToString() + " triggered by " + programIn[i]);
//                            phase = 0;
//                        }
//                    }
                if (programInNumber[i] >= 0)
                {
                    bus = machine.getBus(programInNumber[i], programInPosition[i]);
                    if (bus != 0)
                    {
                        inputFF = i;
                        machine.log("Multiplier: Operation " + (i + 1).ToString() + " triggered by " + programIn[i]);
                        phase = 0;
                    }
                }
            }
            if (operation < 0 && inputFF < 0) return;
            if (operation >= 0 && phase == 2 && (clocks & EniacCyclingUnit.C1P) == EniacCyclingUnit.C1P)
            {
                machine.getAccumulator(10).setReceiveA(true);
                machine.getAccumulator(12).setReceiveA(true);
            }
            if (operation >= 0 && phase == 2 && ((clocks & EniacCyclingUnit.C4P) == EniacCyclingUnit.C4P ||
                                                 (clocks & EniacCyclingUnit.C11P) == EniacCyclingUnit.C11P) &&
                significantFigures[operation] != 0)
            {
                lh1 = 0;
                lh2 = 0;
                if (significantFigures[operation] == 9) lh2 = 1 << 9;
                else lh1 = 1 << (8 - significantFigures[operation]);
//                if (lhpp1Out.Length > 2) machine.setBus(lhpp1Out, lh1);
//                if (lhpp2Out.Length > 2) machine.setBus(lhpp2Out, lh2);
                if (lhpp1OutNumber >= 0) machine.setBus(lhpp1OutNumber, lhpp1OutPosition, lh1);
                if (lhpp2OutNumber >= 0) machine.setBus(lhpp2OutNumber, lhpp2OutPosition, lh2);
            }
            if (operation >= 0 && (phase - 4) == places[operation] && (clocks & EniacCyclingUnit.C11P) == EniacCyclingUnit.C11P)
            {
                if (machine.getAccumulator(8).getSign() & machine.getAccumulator(9).getSign())
                {
                    rh1 = 1 << 10;
//                    if (rhpp1Out.Length > 2) machine.setBus(rhpp1Out, rh1);
                    if (rhpp1OutNumber >= 0) machine.setBus(rhpp1OutNumber, rhpp1OutPosition, rh1);
                }
            }
            if (phase >= 0 && (clocks & EniacCyclingUnit.CPP) == EniacCyclingUnit.CPP)
            {
                for (i = 0; i < 20; i++)
                {
                    lhpp[i] = 0;
                    rhpp[i] = 0;
                }
                if (phase >= 2 && phase <= 11)
                {
                    ier = machine.getAccumulator(8).getDecades();
                    icand = machine.getAccumulator(9).getDecades();
                    for (i = 0; i < 10; i++)
                    {
                        tmp = ier[phase - 2] * icand[i];
                        lhpp[i + (phase - 2)] = tmp / 10;
                        rhpp[i + (phase - 1)] = tmp % 10;
                    }
                }
                if (phase > 0 && (phase - 4) == places[operation])
                {
                    phase = 12;
                    machine.getAccumulator(10).setReceiveA(false);
                    machine.getAccumulator(12).setReceiveA(false);
                    if (machine.getAccumulator(8).getSign())
                    {
//                        if (outputDS.Length > 2) machine.setBus(outputDS, 1);
                        if (outputDSNumber >= 0) machine.setBus(outputDSNumber, outputDSPosition, 1);
                    }
                    if (machine.getAccumulator(9).getSign())
                    {
                        if (outputRSNumber >= 0) machine.setBus(outputRSNumber, outputRSPosition, 1);
//                        if (outputRS.Length > 2) machine.setBus(outputRS, 1);
                    }
                }
                if (phase == 13)
                {
//                    if (outputF.Length > 2) machine.setBus(outputF, 1);
                    if (outputFNumber >= 0) machine.setBus(outputFNumber, outputFPosition, 1);
                }
                if (phase == 14)
                {
//                    if (programOut[operation].Length > 2)
//                    {
//                        machine.setBus(programOut[operation], 1);
//                        machine.log("Multiplier: sent pulse " + programOut[operation]);
//                    }
                    if (programOutNumber[operation] >= 0)
                    {
                        machine.setBus(programOutNumber[operation], programOutPosition[operation], 1);
                        machine.log("Multiplier: sent pulse " + programOut[operation]);
                    }
//                    if (product[operation] == 0 && outputA.Length > 2) machine.setBus(outputA, 1);
//                    if (product[operation] == 1 && outputS.Length > 2) machine.setBus(outputS, 1);
//                    if (product[operation] == 2 && outputAS.Length > 2) machine.setBus(outputAS, 1);
//                    if (product[operation] == 4 && outputAC.Length > 2) machine.setBus(outputAC, 1);
//                    if (product[operation] == 5 && outputSC.Length > 2) machine.setBus(outputSC, 1);
//                    if (product[operation] == 6 && outputASC.Length > 2) machine.setBus(outputASC, 1);
                    if (product[operation] != 3)
                        if (productNumber[product[operation]] >= 0) machine.setBus(productNumber[product[operation]], productPosition[product[operation]], 1);
                    //                    if (machine.getAccumulator(8).getSign() && machine.getAccumulator(9).getSign()) machine.getAccumulator(12).setSign(false);
                    if (ierClearSwitches[operation]) machine.getAccumulator(8).clearDecades();
                    if (icandClearSwitches[operation]) machine.getAccumulator(9).clearDecades();
                    phase = -1;
                    operation = -1;
                    for (i = 0; i < 24; i++)
//                        if (programIn[i].Length > 2)
//                        {
//                            bus = machine.getBus(programIn[i]);
//                            if (bus != 0)
//                            {
//                                inputFF = i;
//                                machine.log("Multiplier: Operation " + (i + 1).ToString() + " triggered by " + programIn[i]);
//                                phase = 0;
//                            }
//                        }
                    if (programInNumber[i] >= 0)
                    {
                        bus = machine.getBus(programInNumber[i], programInPosition[i]);
                        if (bus != 0)
                        {
                            inputFF = i;
                            machine.log("Multiplier: Operation " + (i + 1).ToString() + " triggered by " + programIn[i]);
                            phase = 0;
                        }
                    }
                }
                if (phase == 0)
                {
                    operation = inputFF;
                    inputFF = -1;
                    machine.log("Multiplier: Operation " + (operation + 1).ToString() + " started");
//                    if (ierOperationSwitches[operation] != 5 && rProgramOut[ierOperationSwitches[operation]].Length > 2)
//                    {
//                        machine.setBus(rProgramOut[ierOperationSwitches[operation]], 1);
//                        machine.log("Multiplier: ier pulse sent " + rProgramOut[ierOperationSwitches[operation]]);
//                    }
                    if (ierOperationSwitches[operation] != 5 && rProgramOutNumber[ierOperationSwitches[operation]] >= 0)
                    {
                        machine.setBus(rProgramOutNumber[ierOperationSwitches[operation]], rProgramOutPosition[ierOperationSwitches[operation]], 1);
                        machine.log("Multiplier: ier pulse sent " + rProgramOut[ierOperationSwitches[operation]]);
                    }
//                    if (icandOperationSwitches[operation] != 5 && dProgramOut[icandOperationSwitches[operation]].Length > 2)
//                    {
//                        machine.setBus(dProgramOut[icandOperationSwitches[operation]], 1);
//                        machine.log("Multiplier: icand pulse sent " + dProgramOut[icandOperationSwitches[operation]]);
//                    }
                    if (icandOperationSwitches[operation] != 5 && dProgramOutNumber[icandOperationSwitches[operation]] >= 0)
                    {
                        machine.setBus(dProgramOutNumber[icandOperationSwitches[operation]], dProgramOutPosition[icandOperationSwitches[operation]], 1);
                        machine.log("Multiplier: icand pulse sent " + dProgramOut[icandOperationSwitches[operation]]);
                    }
                }


                phase++;
            }
            if (operation < 0) return;
            if (phase < 3 || phase > 12) return;      /* Return if not in an output operation */
            lh1 = 0;
            lh2 = 0;
            rh1 = 0;
            rh2 = 0;
            if ((clocks & EniacCyclingUnit.C10P) == EniacCyclingUnit.C10P)
            {
                for (i = 0; i < 20; i++)
                {
                    lhpp[i]++;
                    rhpp[i]++;
                }
            }
            if ((clocks & EniacCyclingUnit.C9P) == EniacCyclingUnit.C9P)
            {
                for (i = 0; i < 10; i++)
                {
                    lh1 <<= 1;
                    lh2 <<= 1;
                    rh1 <<= 1;
                    rh2 <<= 1;
                    if (lhpp[i] >= 10) lh1 |= 1;
                    if (lhpp[i + 10] >= 10) lh2 |= 1;
                    if (rhpp[i] >= 10) rh1 |= 1;
                    if (rhpp[i + 10] >= 10) rh2 |= 1;
                }
            }
//            if (lhpp1Out.Length > 2) machine.setBus(lhpp1Out, lh1);
//            if (lhpp2Out.Length > 2) machine.setBus(lhpp2Out, lh2);
//            if (rhpp1Out.Length > 2) machine.setBus(rhpp1Out, rh1);
//            if (rhpp2Out.Length > 2) machine.setBus(rhpp2Out, rh2);
            if (lhpp1OutNumber >= 0) machine.setBus(lhpp1OutNumber, lhpp1OutPosition, lh1);
            if (lhpp2OutNumber >= 0) machine.setBus(lhpp2OutNumber, lhpp2OutPosition, lh2);
            if (rhpp1OutNumber >= 0) machine.setBus(rhpp1OutNumber, rhpp1OutPosition, rh1);
            if (rhpp2OutNumber >= 0) machine.setBus(rhpp2OutNumber, rhpp2OutPosition, rh2);
        }

        public void cycle2(int clocks)      /* This processes any inputs */
        {
        }

    }
}
