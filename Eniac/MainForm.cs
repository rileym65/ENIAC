using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Eniac
{
    public partial class MainForm : Form
    {
        Machine computer;
        private String lastValue;
        private String lastSteppers;
        private String lastDecades;
        private Boolean populating;
        private long breakPoint;

        public MainForm()
        {
            Font = new Font(Font.Name, 8.25f * 96f / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);
            InitializeComponent();
            computer = new Machine(this);
            lastSteppers = "";
            lastDecades = "";
            initializeElements();
            leftAccumulator.SelectedIndex = 0;
            rightAccumulator.SelectedIndex = 0;
            multiplierPanelSelect.SelectedIndex = 0;
            functionTableSelect.SelectedIndex = 0;
            printerPanelSelect.SelectedIndex = 0;
            populating = false;
            breakPoint = -1;
            portableScrollbar.Value = 0;
       }

        private void initializeElements() {
            accSelection.SelectedIndex = 0;
            masterPanelSelect.SelectedIndex = 0;
        }

        private EniacAccumulator getAccumulatorForTab()
        {
            int i;
            i = accSelection.SelectedIndex;
            if (i >= 0 && i < 20) return computer.getAccumulator(i);
            return null;
        }

        public String readCard()
        {
            String ret;
            String[] temp;
            int i;
            ret = null;
            if (cardInputHopper.Lines.Length > 0 && cardInputHopper.Lines[0].Length > 5)
            {
                ret = cardInputHopper.Lines[0];
                temp = new String[cardInputHopper.Lines.Length - 1];
                for (i = 1; i < cardInputHopper.Lines.Length; i++) temp[i - 1] = cardInputHopper.Lines[i];
                cardInputHopper.Lines = temp;
            }
            return ret;
        }

        public void stackCard(String s)
        {
            cardOutputStacker.AppendText(s + "\r\n");
        }

        private void setComboBox(ComboBox cb, String s)
        {
            int i;
            int pos;
            pos = -1;
            for (i = 0; i < cb.Items.Count; i++)
            {
                if (String.Compare(s, cb.Items[i].ToString()) == 0) pos = i;
            }
            if (pos >= 0) cb.SelectedIndex = pos;
        }

        private void showCurrentAccumulator() {
            String value;
            EniacAccumulator acc;
            acc = getAccumulatorForTab();
            value = acc.getValue();
            if (String.Compare(value, lastValue) != 0)
            {
                accValueSign.Text = value.Substring(0, 1);
                accDigit0.Text = value.Substring(1, 1);
                accDigit1.Text = value.Substring(2, 1);
                accDigit2.Text = value.Substring(3, 1);
                accDigit3.Text = value.Substring(4, 1);
                accDigit4.Text = value.Substring(5, 1);
                accDigit5.Text = value.Substring(6, 1);
                accDigit6.Text = value.Substring(7, 1);
                accDigit7.Text = value.Substring(8, 1);
                accDigit8.Text = value.Substring(9, 1);
                accDigit9.Text = value.Substring(10, 1);
                lastValue = value;
            }
        }

        private void showCurrentSteppers()
        {
            String value;
            int bias;
            value = computer.getMasterUnit().getSteppers();
            if (value.CompareTo(lastSteppers) != 0)
            {
                bias = (masterPanelSelect.SelectedIndex == 0) ? 0 : 5;
                stepperOutA.Text = value[0 + bias].ToString();
                stepperOutB.Text = value[1 + bias].ToString();
                stepperOutC.Text = value[2 + bias].ToString();
                stepperOutD.Text = value[3 + bias].ToString();
                stepperOutE.Text = value[4 + bias].ToString();
                lastSteppers = value;
            }
        }

        private void showCurrentDecades()
        {
            String value;
            int bias;
            value = computer.getMasterUnit().getDecades();
            if (value.CompareTo(lastDecades) != 0)
            {
                bias = (masterPanelSelect.SelectedIndex == 0) ? 0 : 10;
                decadeOut0.Text = value[0 + bias].ToString();
                decadeOut1.Text = value[1 + bias].ToString();
                decadeOut2.Text = value[2 + bias].ToString();
                decadeOut3.Text = value[3 + bias].ToString();
                decadeOut4.Text = value[4 + bias].ToString();
                decadeOut5.Text = value[5 + bias].ToString();
                decadeOut6.Text = value[6 + bias].ToString();
                decadeOut7.Text = value[7 + bias].ToString();
                decadeOut8.Text = value[8 + bias].ToString();
                decadeOut9.Text = value[9 + bias].ToString();
                lastDecades = value;
            }
        }

        private void populateInitTab()
        {
            EniacInitiatingUnit unit = computer.getInitUnit();
            setComboBox(initGoOutput, unit.getGoOutput());
            setComboBox(clearIn0, unit.getClearIn(0));
            setComboBox(clearIn1, unit.getClearIn(1));
            setComboBox(clearIn2, unit.getClearIn(2));
            setComboBox(clearIn3, unit.getClearIn(3));
            setComboBox(clearIn4, unit.getClearIn(4));
            setComboBox(clearIn5, unit.getClearIn(5));
            setComboBox(clearOut0, unit.getClearOut(0));
            setComboBox(clearOut1, unit.getClearOut(1));
            setComboBox(clearOut2, unit.getClearOut(2));
            setComboBox(clearOut3, unit.getClearOut(3));
            setComboBox(clearOut4, unit.getClearOut(4));
            setComboBox(clearOut5, unit.getClearOut(5));
            setComboBox(readerProgramIn, computer.getReader().getProgramInput());
            setComboBox(readerProgramOut, computer.getReader().getProgramOutput());
            setComboBox(readerInterlock, computer.getReader().getInterlockIn());
            setComboBox(punchProgramIn, computer.getPunch().getProgramInput());
            setComboBox(punchProgramOut, computer.getPunch().getProgramOutput());
        }

        private void populateConst1Tab()
        {
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            if (constUnit.getPowered()) constPowerOn.Checked = true; else constPowerOff.Checked = true;
            const1Function.SelectedIndex = constUnit.getOperationSwitch(0);
            if (constUnit.getSelectSwitch(0)) const1B.Checked = true; else const1A.Checked = true;
            const2Function.SelectedIndex = constUnit.getOperationSwitch(1);
            if (constUnit.getSelectSwitch(1)) const2B.Checked = true; else const2A.Checked = true;
            const3Function.SelectedIndex = constUnit.getOperationSwitch(2);
            if (constUnit.getSelectSwitch(2)) const3B.Checked = true; else const3A.Checked = true;
            const4Function.SelectedIndex = constUnit.getOperationSwitch(3);
            if (constUnit.getSelectSwitch(3)) const4B.Checked = true; else const4A.Checked = true;
            const5Function.SelectedIndex = constUnit.getOperationSwitch(4);
            if (constUnit.getSelectSwitch(4)) const5B.Checked = true; else const5A.Checked = true;
            const6Function.SelectedIndex = constUnit.getOperationSwitch(5);
            if (constUnit.getSelectSwitch(5)) const6B.Checked = true; else const6A.Checked = true;
            const7Function.SelectedIndex = constUnit.getOperationSwitch(6);
            if (constUnit.getSelectSwitch(6)) const7B.Checked = true; else const7A.Checked = true;
            const8Function.SelectedIndex = constUnit.getOperationSwitch(7);
            if (constUnit.getSelectSwitch(7)) const8B.Checked = true; else const8A.Checked = true;
            const9Function.SelectedIndex = constUnit.getOperationSwitch(8);
            if (constUnit.getSelectSwitch(8)) const9B.Checked = true; else const9A.Checked = true;
            const10Function.SelectedIndex = constUnit.getOperationSwitch(9);
            if (constUnit.getSelectSwitch(9)) const10B.Checked = true; else const10A.Checked = true;

            const11Function.SelectedIndex = constUnit.getOperationSwitch(10);
            if (constUnit.getSelectSwitch(10)) const11B.Checked = true; else const11A.Checked = true;
            const12Function.SelectedIndex = constUnit.getOperationSwitch(11);
            if (constUnit.getSelectSwitch(11)) const12B.Checked = true; else const12A.Checked = true;
            const13Function.SelectedIndex = constUnit.getOperationSwitch(12);
            if (constUnit.getSelectSwitch(12)) const13B.Checked = true; else const13A.Checked = true;
            const14Function.SelectedIndex = constUnit.getOperationSwitch(13);
            if (constUnit.getSelectSwitch(13)) const14B.Checked = true; else const14A.Checked = true;
            const15Function.SelectedIndex = constUnit.getOperationSwitch(14);
            if (constUnit.getSelectSwitch(14)) const15B.Checked = true; else const15A.Checked = true;
            const16Function.SelectedIndex = constUnit.getOperationSwitch(15);
            if (constUnit.getSelectSwitch(15)) const16B.Checked = true; else const16A.Checked = true;
            const17Function.SelectedIndex = constUnit.getOperationSwitch(16);
            if (constUnit.getSelectSwitch(16)) const17B.Checked = true; else const17A.Checked = true;
            const18Function.SelectedIndex = constUnit.getOperationSwitch(17);
            if (constUnit.getSelectSwitch(17)) const18B.Checked = true; else const18A.Checked = true;
            const19Function.SelectedIndex = constUnit.getOperationSwitch(18);
            if (constUnit.getSelectSwitch(18)) const19B.Checked = true; else const19A.Checked = true;
            const20Function.SelectedIndex = constUnit.getOperationSwitch(19);
            if (constUnit.getSelectSwitch(19)) const20B.Checked = true; else const20A.Checked = true;

            const21Function.SelectedIndex = constUnit.getOperationSwitch(20);
            if (constUnit.getSelectSwitch(20)) const21B.Checked = true; else const21A.Checked = true;
            const22Function.SelectedIndex = constUnit.getOperationSwitch(21);
            if (constUnit.getSelectSwitch(21)) const22B.Checked = true; else const22A.Checked = true;
            const23Function.SelectedIndex = constUnit.getOperationSwitch(22);
            if (constUnit.getSelectSwitch(22)) const23B.Checked = true; else const23A.Checked = true;
            const24Function.SelectedIndex = constUnit.getOperationSwitch(23);
            if (constUnit.getSelectSwitch(23)) const24B.Checked = true; else const24A.Checked = true;
            const25Function.SelectedIndex = constUnit.getOperationSwitch(24);
            if (constUnit.getSelectSwitch(24)) const25B.Checked = true; else const25A.Checked = true;
            const26Function.SelectedIndex = constUnit.getOperationSwitch(25);
            if (constUnit.getSelectSwitch(25)) const26B.Checked = true; else const26A.Checked = true;
            const27Function.SelectedIndex = constUnit.getOperationSwitch(26);
            if (constUnit.getSelectSwitch(26)) const27B.Checked = true; else const27A.Checked = true;
            const28Function.SelectedIndex = constUnit.getOperationSwitch(27);
            if (constUnit.getSelectSwitch(27)) const28B.Checked = true; else const28A.Checked = true;
            const29Function.SelectedIndex = constUnit.getOperationSwitch(28);
            if (constUnit.getSelectSwitch(28)) const29B.Checked = true; else const29A.Checked = true;
            const30Function.SelectedIndex = constUnit.getOperationSwitch(29);
            if (constUnit.getSelectSwitch(29)) const30B.Checked = true; else const30A.Checked = true;
            
            
            setComboBox(const1OutputA, constUnit.getDigitOutput(0));
            setComboBox(const1OutputS, constUnit.getDigitOutput(1));
            setComboBox(const1ProgIn, constUnit.getProgramInput(0));
            setComboBox(const2ProgIn, constUnit.getProgramInput(1));
            setComboBox(const3ProgIn, constUnit.getProgramInput(2));
            setComboBox(const4ProgIn, constUnit.getProgramInput(3));
            setComboBox(const5ProgIn, constUnit.getProgramInput(4));
            setComboBox(const6ProgIn, constUnit.getProgramInput(5));
            setComboBox(const7ProgIn, constUnit.getProgramInput(6));
            setComboBox(const8ProgIn, constUnit.getProgramInput(7));
            setComboBox(const9ProgIn, constUnit.getProgramInput(8));
            setComboBox(const10ProgIn, constUnit.getProgramInput(9));
            setComboBox(const11ProgIn, constUnit.getProgramInput(10));
            setComboBox(const12ProgIn, constUnit.getProgramInput(11));
            setComboBox(const13ProgIn, constUnit.getProgramInput(12));
            setComboBox(const14ProgIn, constUnit.getProgramInput(13));
            setComboBox(const15ProgIn, constUnit.getProgramInput(14));
            setComboBox(const16ProgIn, constUnit.getProgramInput(15));
            setComboBox(const17ProgIn, constUnit.getProgramInput(16));
            setComboBox(const18ProgIn, constUnit.getProgramInput(17));
            setComboBox(const19ProgIn, constUnit.getProgramInput(18));
            setComboBox(const20ProgIn, constUnit.getProgramInput(19));
            setComboBox(const21ProgIn, constUnit.getProgramInput(20));
            setComboBox(const22ProgIn, constUnit.getProgramInput(21));
            setComboBox(const23ProgIn, constUnit.getProgramInput(22));
            setComboBox(const24ProgIn, constUnit.getProgramInput(23));
            setComboBox(const25ProgIn, constUnit.getProgramInput(24));
            setComboBox(const26ProgIn, constUnit.getProgramInput(25));
            setComboBox(const27ProgIn, constUnit.getProgramInput(26));
            setComboBox(const28ProgIn, constUnit.getProgramInput(27));
            setComboBox(const29ProgIn, constUnit.getProgramInput(28));
            setComboBox(const30ProgIn, constUnit.getProgramInput(29));
            setComboBox(const1ProgOut, constUnit.getProgramOutput(0));
            setComboBox(const2ProgOut, constUnit.getProgramOutput(1));
            setComboBox(const3ProgOut, constUnit.getProgramOutput(2));
            setComboBox(const4ProgOut, constUnit.getProgramOutput(3));
            setComboBox(const5ProgOut, constUnit.getProgramOutput(4));
            setComboBox(const6ProgOut, constUnit.getProgramOutput(5));
            setComboBox(const7ProgOut, constUnit.getProgramOutput(6));
            setComboBox(const8ProgOut, constUnit.getProgramOutput(7));
            setComboBox(const9ProgOut, constUnit.getProgramOutput(8));
            setComboBox(const10ProgOut, constUnit.getProgramOutput(9));
            setComboBox(const11ProgOut, constUnit.getProgramOutput(10));
            setComboBox(const12ProgOut, constUnit.getProgramOutput(11));
            setComboBox(const13ProgOut, constUnit.getProgramOutput(12));
            setComboBox(const14ProgOut, constUnit.getProgramOutput(13));
            setComboBox(const15ProgOut, constUnit.getProgramOutput(14));
            setComboBox(const16ProgOut, constUnit.getProgramOutput(15));
            setComboBox(const17ProgOut, constUnit.getProgramOutput(16));
            setComboBox(const18ProgOut, constUnit.getProgramOutput(17));
            setComboBox(const19ProgOut, constUnit.getProgramOutput(18));
            setComboBox(const20ProgOut, constUnit.getProgramOutput(19));
            setComboBox(const21ProgOut, constUnit.getProgramOutput(20));
            setComboBox(const22ProgOut, constUnit.getProgramOutput(21));
            setComboBox(const23ProgOut, constUnit.getProgramOutput(22));
            setComboBox(const24ProgOut, constUnit.getProgramOutput(23));
            setComboBox(const25ProgOut, constUnit.getProgramOutput(24));
            setComboBox(const26ProgOut, constUnit.getProgramOutput(25));
            setComboBox(const27ProgOut, constUnit.getProgramOutput(26));
            setComboBox(const28ProgOut, constUnit.getProgramOutput(27));
            setComboBox(const29ProgOut, constUnit.getProgramOutput(28));
            setComboBox(const30ProgOut, constUnit.getProgramOutput(29));

        }

        private void populateConst2Tab()
        {
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            regJ0.SelectedIndex = constUnit.getRegJ(0);
            regJ1.SelectedIndex = constUnit.getRegJ(1);
            regJ2.SelectedIndex = constUnit.getRegJ(2);
            regJ3.SelectedIndex = constUnit.getRegJ(3);
            regJ4.SelectedIndex = constUnit.getRegJ(4);
            regJ5.SelectedIndex = constUnit.getRegJ(5);
            regJ6.SelectedIndex = constUnit.getRegJ(6);
            regJ7.SelectedIndex = constUnit.getRegJ(7);
            regJ8.SelectedIndex = constUnit.getRegJ(8);
            regJ9.SelectedIndex = constUnit.getRegJ(9);
            regK0.SelectedIndex = constUnit.getRegK(0);
            regK1.SelectedIndex = constUnit.getRegK(1);
            regK2.SelectedIndex = constUnit.getRegK(2);
            regK3.SelectedIndex = constUnit.getRegK(3);
            regK4.SelectedIndex = constUnit.getRegK(4);
            regK5.SelectedIndex = constUnit.getRegK(5);
            regK6.SelectedIndex = constUnit.getRegK(6);
            regK7.SelectedIndex = constUnit.getRegK(7);
            regK8.SelectedIndex = constUnit.getRegK(8);
            regK9.SelectedIndex = constUnit.getRegK(9);
            if (constUnit.getJLSign()) jlm.Checked = true; else jlp.Checked = true;
            if (constUnit.getJRSign()) jrm.Checked = true; else jrp.Checked = true;
            if (constUnit.getKLSign()) klm.Checked = true; else klp.Checked = true;
            if (constUnit.getKRSign()) krm.Checked = true; else krp.Checked = true;
        }

        private void populatePunchTab()
        {
            EniacCardPunch punch = computer.getPunch();
            if (punch.getPowered()) punchPowerOn.Checked = true; else punchPowerOff.Checked = true;
        }

        private void populateReaderTab()
        {
            EniacCardReader reader = computer.getReader();
            if (reader.getPowered()) readerPowerOn.Checked = true; else readerPowerOff.Checked = true;
        }

        private void populateAccumulatorTab(EniacAccumulator acc)
        {
            int i;
            ArrayList filters;
            filters = computer.getFilters();
            populating = true;
            if (acc.getPowered()) acc1PowerOn.Checked = true; else acc1PowerOff.Checked = true;
            if (acc.getClearSwitch(0)) acc1Func1CC.Checked = true; else acc1Func1C0.Checked = true;
            acc1Func1Function.SelectedIndex = acc.getOperationSwitch(0);
            if (acc.getClearSwitch(1)) acc1Func2CC.Checked = true; else acc1Func2C0.Checked = true;
            acc1Func2Function.SelectedIndex = acc.getOperationSwitch(1);
            if (acc.getClearSwitch(2)) acc1Func3CC.Checked = true; else acc1Func3C0.Checked = true;
            acc1Func3Function.SelectedIndex = acc.getOperationSwitch(2);
            if (acc.getClearSwitch(3)) acc1Func4CC.Checked = true; else acc1Func4C0.Checked = true;
            acc1Func4Function.SelectedIndex = acc.getOperationSwitch(3);
            if (acc.getClearSwitch(4)) acc1Func5CC.Checked = true; else acc1Func5C0.Checked = true;
            acc1Func5Function.SelectedIndex = acc.getOperationSwitch(4);
            if (acc.getClearSwitch(5)) acc1Func6CC.Checked = true; else acc1Func6C0.Checked = true;
            acc1Func6Function.SelectedIndex = acc.getOperationSwitch(5);
            if (acc.getClearSwitch(6)) acc1Func7CC.Checked = true; else acc1Func7C0.Checked = true;
            acc1Func7Function.SelectedIndex = acc.getOperationSwitch(6);
            if (acc.getClearSwitch(7)) acc1Func8CC.Checked = true; else acc1Func8C0.Checked = true;
            acc1Func8Function.SelectedIndex = acc.getOperationSwitch(7);
            if (acc.getClearSwitch(8)) acc1Func9CC.Checked = true; else acc1Func9C0.Checked = true;
            acc1Func9Function.SelectedIndex = acc.getOperationSwitch(8);
            if (acc.getClearSwitch(9)) acc1Func10CC.Checked = true; else acc1Func10C0.Checked = true;
            acc1Func10Function.SelectedIndex = acc.getOperationSwitch(9);
            if (acc.getClearSwitch(10)) acc1Func11CC.Checked = true; else acc1Func11C0.Checked = true;
            acc1Func11Function.SelectedIndex = acc.getOperationSwitch(10);
            if (acc.getClearSwitch(11)) acc1Func12CC.Checked = true; else acc1Func12C0.Checked = true;
            acc1Func12Function.SelectedIndex = acc.getOperationSwitch(11);
            acc1Rep5.SelectedIndex = acc.getRepeatSwitch(5) - 1;
            acc1Rep6.SelectedIndex = acc.getRepeatSwitch(6) - 1;
            acc1Rep7.SelectedIndex = acc.getRepeatSwitch(7) - 1;
            acc1Rep8.SelectedIndex = acc.getRepeatSwitch(8) - 1;
            acc1Rep9.SelectedIndex = acc.getRepeatSwitch(9) - 1;
            acc1Rep10.SelectedIndex = acc.getRepeatSwitch(10) - 1;
            acc1Rep11.SelectedIndex = acc.getRepeatSwitch(11) - 1;
            acc1Rep12.SelectedIndex = acc.getRepeatSwitch(12) - 1;
            setComboBox(acc1InA, acc.getDigitInput(0));
            setComboBox(acc1InB, acc.getDigitInput(1));
            setComboBox(acc1InC, acc.getDigitInput(2));
            setComboBox(acc1InD, acc.getDigitInput(3));
            setComboBox(acc1InE, acc.getDigitInput(4));
            setComboBox(acc1OutA, acc.getDigitOutput(0));
            setComboBox(acc1OutS, acc.getDigitOutput(1));
            setComboBox(acc1Prog1In, acc.getProgramInput(0));
            setComboBox(acc1Prog2In, acc.getProgramInput(1));
            setComboBox(acc1Prog3In, acc.getProgramInput(2));
            setComboBox(acc1Prog4In, acc.getProgramInput(3));
            setComboBox(acc1Prog5In, acc.getProgramInput(4));
            setComboBox(acc1Prog6In, acc.getProgramInput(5));
            setComboBox(acc1Prog7In, acc.getProgramInput(6));
            setComboBox(acc1Prog8In, acc.getProgramInput(7));
            setComboBox(acc1Prog9In, acc.getProgramInput(8));
            setComboBox(acc1Prog10In, acc.getProgramInput(9));
            setComboBox(acc1Prog11In, acc.getProgramInput(10));
            setComboBox(acc1Prog12In, acc.getProgramInput(11));
            setComboBox(acc1Prog5Out, acc.getProgramOutput(4));
            setComboBox(acc1Prog6Out, acc.getProgramOutput(5));
            setComboBox(acc1Prog7Out, acc.getProgramOutput(6));
            setComboBox(acc1Prog8Out, acc.getProgramOutput(7));
            setComboBox(acc1Prog9Out, acc.getProgramOutput(8));
            setComboBox(acc1Prog10Out, acc.getProgramOutput(9));
            setComboBox(acc1Prog11Out, acc.getProgramOutput(10));
            setComboBox(acc1Prog12Out, acc.getProgramOutput(11));
            if (acc.getClearSwitch(12)) accSigDigitsCC.Checked = true; else accSigDigitsC0.Checked = true;
            accSigDigits.SelectedIndex = acc.getSignificantFigures();
            leftAccumulator.SelectedIndex = acc.getLeftAccumulator() + 1;
            rightAccumulator.SelectedIndex = acc.getRightAccumulator() + 1;
            accFilter1.Items.Clear();
            accFilter2.Items.Clear();
            accFilter3.Items.Clear();
            accFilter4.Items.Clear();
            accFilter5.Items.Clear();
            accFilter1.Items.Add("");
            accFilter2.Items.Add("");
            accFilter3.Items.Add("");
            accFilter4.Items.Add("");
            accFilter5.Items.Add("");
            for (i = 0; i < filters.Count; i++)
            {
                accFilter1.Items.Add(((EniacFilter)filters[i]).getName());
                accFilter2.Items.Add(((EniacFilter)filters[i]).getName());
                accFilter3.Items.Add(((EniacFilter)filters[i]).getName());
                accFilter4.Items.Add(((EniacFilter)filters[i]).getName());
                accFilter5.Items.Add(((EniacFilter)filters[i]).getName());
            }
            setComboBox(accFilter1, acc.getInputFilter(0));
            setComboBox(accFilter2, acc.getInputFilter(1));
            setComboBox(accFilter3, acc.getInputFilter(2));
            setComboBox(accFilter4, acc.getInputFilter(3));
            setComboBox(accFilter5, acc.getInputFilter(4));
            showCurrentAccumulator();
            populating = false;
        }

        private void populateFiltersTab()
        {
            int i;
            ArrayList filters;
            filters = computer.getFilters();
            filterSelect.Items.Clear();
            for (i = 0; i < filters.Count; i++)
            {
                filterSelect.Items.Add(((EniacFilter)filters[i]).getName());
            }
            filterSelect.SelectedIndex = 0;
            showSelectedFilter();
        }

        private void populateMasterProgrammerTab()
        {
            int bias;
            EniacMasterProgrammer master;
            master = computer.getMasterUnit();
            bias = (masterPanelSelect.SelectedIndex == 0) ? 10 : 0;
            if (master.getPowered()) masterPowerOn.Checked = true; else masterPowerOff.Checked = true;
            decadeSwitch110.SelectedIndex = master.getDecadeSwitch(bias + 0, 1);
            decadeSwitch109.SelectedIndex = master.getDecadeSwitch(bias + 1, 1);
            decadeSwitch108.SelectedIndex = master.getDecadeSwitch(bias + 2, 1);
            decadeSwitch107.SelectedIndex = master.getDecadeSwitch(bias + 3, 1);
            decadeSwitch106.SelectedIndex = master.getDecadeSwitch(bias + 4, 1);
            decadeSwitch105.SelectedIndex = master.getDecadeSwitch(bias + 5, 1);
            decadeSwitch104.SelectedIndex = master.getDecadeSwitch(bias + 6, 1);
            decadeSwitch103.SelectedIndex = master.getDecadeSwitch(bias + 7, 1);
            decadeSwitch102.SelectedIndex = master.getDecadeSwitch(bias + 8, 1);
            decadeSwitch101.SelectedIndex = master.getDecadeSwitch(bias + 9, 1);
            decadeSwitch210.SelectedIndex = master.getDecadeSwitch(bias + 0, 2);
            decadeSwitch209.SelectedIndex = master.getDecadeSwitch(bias + 1, 2);
            decadeSwitch208.SelectedIndex = master.getDecadeSwitch(bias + 2, 2);
            decadeSwitch207.SelectedIndex = master.getDecadeSwitch(bias + 3, 2);
            decadeSwitch206.SelectedIndex = master.getDecadeSwitch(bias + 4, 2);
            decadeSwitch205.SelectedIndex = master.getDecadeSwitch(bias + 5, 2);
            decadeSwitch204.SelectedIndex = master.getDecadeSwitch(bias + 6, 2);
            decadeSwitch203.SelectedIndex = master.getDecadeSwitch(bias + 7, 2);
            decadeSwitch202.SelectedIndex = master.getDecadeSwitch(bias + 8, 2);
            decadeSwitch201.SelectedIndex = master.getDecadeSwitch(bias + 9, 2);
            decadeSwitch310.SelectedIndex = master.getDecadeSwitch(bias + 0, 3);
            decadeSwitch309.SelectedIndex = master.getDecadeSwitch(bias + 1, 3);
            decadeSwitch308.SelectedIndex = master.getDecadeSwitch(bias + 2, 3);
            decadeSwitch307.SelectedIndex = master.getDecadeSwitch(bias + 3, 3);
            decadeSwitch306.SelectedIndex = master.getDecadeSwitch(bias + 4, 3);
            decadeSwitch305.SelectedIndex = master.getDecadeSwitch(bias + 5, 3);
            decadeSwitch304.SelectedIndex = master.getDecadeSwitch(bias + 6, 3);
            decadeSwitch303.SelectedIndex = master.getDecadeSwitch(bias + 7, 3);
            decadeSwitch302.SelectedIndex = master.getDecadeSwitch(bias + 8, 3);
            decadeSwitch301.SelectedIndex = master.getDecadeSwitch(bias + 9, 3);
            decadeSwitch410.SelectedIndex = master.getDecadeSwitch(bias + 0, 4);
            decadeSwitch409.SelectedIndex = master.getDecadeSwitch(bias + 1, 4);
            decadeSwitch408.SelectedIndex = master.getDecadeSwitch(bias + 2, 4);
            decadeSwitch407.SelectedIndex = master.getDecadeSwitch(bias + 3, 4);
            decadeSwitch406.SelectedIndex = master.getDecadeSwitch(bias + 4, 4);
            decadeSwitch405.SelectedIndex = master.getDecadeSwitch(bias + 5, 4);
            decadeSwitch404.SelectedIndex = master.getDecadeSwitch(bias + 6, 4);
            decadeSwitch403.SelectedIndex = master.getDecadeSwitch(bias + 7, 4);
            decadeSwitch402.SelectedIndex = master.getDecadeSwitch(bias + 8, 4);
            decadeSwitch401.SelectedIndex = master.getDecadeSwitch(bias + 9, 4);
            decadeSwitch510.SelectedIndex = master.getDecadeSwitch(bias + 0, 5);
            decadeSwitch509.SelectedIndex = master.getDecadeSwitch(bias + 1, 5);
            decadeSwitch508.SelectedIndex = master.getDecadeSwitch(bias + 2, 5);
            decadeSwitch507.SelectedIndex = master.getDecadeSwitch(bias + 3, 5);
            decadeSwitch506.SelectedIndex = master.getDecadeSwitch(bias + 4, 5);
            decadeSwitch505.SelectedIndex = master.getDecadeSwitch(bias + 5, 5);
            decadeSwitch504.SelectedIndex = master.getDecadeSwitch(bias + 6, 5);
            decadeSwitch503.SelectedIndex = master.getDecadeSwitch(bias + 7, 5);
            decadeSwitch502.SelectedIndex = master.getDecadeSwitch(bias + 8, 5);
            decadeSwitch501.SelectedIndex = master.getDecadeSwitch(bias + 9, 5);
            decadeSwitch610.SelectedIndex = master.getDecadeSwitch(bias + 0, 6);
            decadeSwitch609.SelectedIndex = master.getDecadeSwitch(bias + 1, 6);
            decadeSwitch608.SelectedIndex = master.getDecadeSwitch(bias + 2, 6);
            decadeSwitch607.SelectedIndex = master.getDecadeSwitch(bias + 3, 6);
            decadeSwitch606.SelectedIndex = master.getDecadeSwitch(bias + 4, 6);
            decadeSwitch605.SelectedIndex = master.getDecadeSwitch(bias + 5, 6);
            decadeSwitch604.SelectedIndex = master.getDecadeSwitch(bias + 6, 6);
            decadeSwitch603.SelectedIndex = master.getDecadeSwitch(bias + 7, 6);
            decadeSwitch602.SelectedIndex = master.getDecadeSwitch(bias + 8, 6);
            decadeSwitch601.SelectedIndex = master.getDecadeSwitch(bias + 9, 6);
            bias = (masterPanelSelect.SelectedIndex == 0) ? 4 : 0;
            assoc4A.Checked = (master.getAssociationSwitch(bias+3)) ? false : true;
            assoc4B.Checked = (master.getAssociationSwitch(bias+3)) ? true : false;
            assoc3A.Checked = (master.getAssociationSwitch(bias + 2)) ? false : true;
            assoc3B.Checked = (master.getAssociationSwitch(bias + 2)) ? true : false;
            assoc2A.Checked = (master.getAssociationSwitch(bias + 1)) ? false : true;
            assoc2B.Checked = (master.getAssociationSwitch(bias + 1)) ? true : false;
            assoc1A.Checked = (master.getAssociationSwitch(bias + 0)) ? false : true;
            assoc1B.Checked = (master.getAssociationSwitch(bias + 0)) ? true : false;
            if (masterPanelSelect.SelectedIndex == 0)
            {
                masterLabel1.Text = "B";
                masterLabel2.Text = "C";
                masterLabel3.Text = "C";
                masterLabel4.Text = "C";
                masterLabel5.Text = "D";
                masterLabel6.Text = "E";
                assoc4A.Text = "A";
                assoc4B.Text = "B";
                assoc3A.Text = "B";
                assoc3B.Text = "C";
                assoc2A.Text = "C";
                assoc2B.Text = "D";
                assoc1A.Text = "D";
                assoc1B.Text = "E";
                stepperA.Text = "A";
                stepperB.Text = "B";
                stepperC.Text = "C";
                stepperD.Text = "D";
                stepperE.Text = "E";
            }
            else
            {
                masterLabel1.Text = "G";
                masterLabel2.Text = "H";
                masterLabel3.Text = "H";
                masterLabel4.Text = "H";
                masterLabel5.Text = "J";
                masterLabel6.Text = "K";
                assoc4A.Text = "F";
                assoc4B.Text = "G";
                assoc3A.Text = "G";
                assoc3B.Text = "H";
                assoc2A.Text = "H";
                assoc2B.Text = "J";
                assoc1A.Text = "J";
                assoc1B.Text = "K";
                stepperA.Text = "F";
                stepperB.Text = "G";
                stepperC.Text = "H";
                stepperD.Text = "J";
                stepperE.Text = "K";
            }
            bias = (masterPanelSelect.SelectedIndex == 0) ? 10 : 0;
            setComboBox(decadeDI9, master.getDecadeDirectInput(bias + 9));
            setComboBox(decadeDI8, master.getDecadeDirectInput(bias + 8));
            setComboBox(decadeDI7, master.getDecadeDirectInput(bias + 7));
            setComboBox(decadeDI6, master.getDecadeDirectInput(bias + 6));
            setComboBox(decadeDI5, master.getDecadeDirectInput(bias + 5));
            setComboBox(decadeDI4, master.getDecadeDirectInput(bias + 4));
            setComboBox(decadeDI3, master.getDecadeDirectInput(bias + 3));
            setComboBox(decadeDI2, master.getDecadeDirectInput(bias + 2));
            setComboBox(decadeDI1, master.getDecadeDirectInput(bias + 1));
            setComboBox(decadeDI0, master.getDecadeDirectInput(bias + 0));
            bias = (masterPanelSelect.SelectedIndex == 0) ? 0 : 5;
            stepperClearA.SelectedIndex = master.getStepperClearSwitch(0 + bias);
            stepperClearB.SelectedIndex = master.getStepperClearSwitch(1 + bias);
            stepperClearC.SelectedIndex = master.getStepperClearSwitch(2 + bias);
            stepperClearD.SelectedIndex = master.getStepperClearSwitch(3 + bias);
            stepperClearE.SelectedIndex = master.getStepperClearSwitch(4 + bias);
            bias = (masterPanelSelect.SelectedIndex == 1) ? 5 : 0;
            setComboBox(stepperDIA, master.getStepperDirectInput(bias + 0));
            setComboBox(stepperInA, master.getStepperInput(bias + 0));
            setComboBox(stepperDirectClearA, master.getStepperDirectClear(bias + 0));
            setComboBox(stepperAOut1, master.getStepperOutput(bias + 0, 0));
            setComboBox(stepperAOut2, master.getStepperOutput(bias + 0, 1));
            setComboBox(stepperAOut3, master.getStepperOutput(bias + 0, 2));
            setComboBox(stepperAOut4, master.getStepperOutput(bias + 0, 3));
            setComboBox(stepperAOut5, master.getStepperOutput(bias + 0, 4));
            setComboBox(stepperAOut6, master.getStepperOutput(bias + 0, 5));
            setComboBox(stepperDIB, master.getStepperDirectInput(bias + 1));
            setComboBox(stepperInB, master.getStepperInput(bias + 1));
            setComboBox(stepperDirectClearB, master.getStepperDirectClear(bias + 1));
            setComboBox(stepperBOut1, master.getStepperOutput(bias + 1, 0));
            setComboBox(stepperBOut2, master.getStepperOutput(bias + 1, 1));
            setComboBox(stepperBOut3, master.getStepperOutput(bias + 1, 2));
            setComboBox(stepperBOut4, master.getStepperOutput(bias + 1, 3));
            setComboBox(stepperBOut5, master.getStepperOutput(bias + 1, 4));
            setComboBox(stepperBOut6, master.getStepperOutput(bias + 1, 5));
            setComboBox(stepperDIC, master.getStepperDirectInput(bias + 2));
            setComboBox(stepperInC, master.getStepperInput(bias + 2));
            setComboBox(stepperDirectClearC, master.getStepperDirectClear(bias + 2));
            setComboBox(stepperCOut1, master.getStepperOutput(bias + 2, 0));
            setComboBox(stepperCOut2, master.getStepperOutput(bias + 2, 1));
            setComboBox(stepperCOut3, master.getStepperOutput(bias + 2, 2));
            setComboBox(stepperCOut4, master.getStepperOutput(bias + 2, 3));
            setComboBox(stepperCOut5, master.getStepperOutput(bias + 2, 4));
            setComboBox(stepperCOut6, master.getStepperOutput(bias + 2, 5));
            setComboBox(stepperDID, master.getStepperDirectInput(bias + 3));
            setComboBox(stepperInD, master.getStepperInput(bias + 3));
            setComboBox(stepperDirectClearD, master.getStepperDirectClear(bias + 3));
            setComboBox(stepperDOut1, master.getStepperOutput(bias + 3, 0));
            setComboBox(stepperDOut2, master.getStepperOutput(bias + 3, 1));
            setComboBox(stepperDOut3, master.getStepperOutput(bias + 3, 2));
            setComboBox(stepperDOut4, master.getStepperOutput(bias + 3, 3));
            setComboBox(stepperDOut5, master.getStepperOutput(bias + 3, 4));
            setComboBox(stepperDOut6, master.getStepperOutput(bias + 3, 5));
            setComboBox(stepperDIE, master.getStepperDirectInput(bias + 4));
            setComboBox(stepperInE, master.getStepperInput(bias + 4));
            setComboBox(stepperDirectClearE, master.getStepperDirectClear(bias + 4));
            setComboBox(stepperEOut1, master.getStepperOutput(bias + 4, 0));
            setComboBox(stepperEOut2, master.getStepperOutput(bias + 4, 1));
            setComboBox(stepperEOut3, master.getStepperOutput(bias + 4, 2));
            setComboBox(stepperEOut4, master.getStepperOutput(bias + 4, 3));
            setComboBox(stepperEOut5, master.getStepperOutput(bias + 4, 4));
            setComboBox(stepperEOut6, master.getStepperOutput(bias + 4, 5));
            lastSteppers = "";
            showCurrentSteppers();
            lastDecades = "";
            showCurrentDecades();
        }

        private void populateMultiplierTab()
        {
            int bias;
            EniacMultiplier mult;
            mult = computer.getMultiplier();
            bias = multiplierPanelSelect.SelectedIndex * 8;
            if (mult.getPowered()) multiplierPowerOn.Checked = true; else multiplierPowerOff.Checked = true;
            multIerOperation1.SelectedIndex = mult.getIerOperationSwitch(0 + bias);
            if (mult.getIerClearSwitch(0+bias)) multIerClearC1.Checked = true; else multIerClear01.Checked = true;
            multIerOperation2.SelectedIndex = mult.getIerOperationSwitch(1 + bias);
            if (mult.getIerClearSwitch(1 + bias)) multIerClearC2.Checked = true; else multIerClear02.Checked = true;
            multIerOperation3.SelectedIndex = mult.getIerOperationSwitch(2 + bias);
            if (mult.getIerClearSwitch(2 + bias)) multIerClearC3.Checked = true; else multIerClear03.Checked = true;
            multIerOperation4.SelectedIndex = mult.getIerOperationSwitch(3 + bias);
            if (mult.getIerClearSwitch(3 + bias)) multIerClearC4.Checked = true; else multIerClear04.Checked = true;
            multIerOperation5.SelectedIndex = mult.getIerOperationSwitch(4 + bias);
            if (mult.getIerClearSwitch(4 + bias)) multIerClearC5.Checked = true; else multIerClear05.Checked = true;
            multIerOperation6.SelectedIndex = mult.getIerOperationSwitch(5 + bias);
            if (mult.getIerClearSwitch(5 + bias)) multIerClearC6.Checked = true; else multIerClear06.Checked = true;
            multIerOperation7.SelectedIndex = mult.getIerOperationSwitch(6 + bias);
            if (mult.getIerClearSwitch(6 + bias)) multIerClearC7.Checked = true; else multIerClear07.Checked = true;
            multIerOperation8.SelectedIndex = mult.getIerOperationSwitch(7 + bias);
            if (mult.getIerClearSwitch(7 + bias)) multIerClearC8.Checked = true; else multIerClear08.Checked = true;
            multIcandOperation1.SelectedIndex = mult.getIcandOperationSwitch(0 + bias);
            if (mult.getIcandClearSwitch(0 + bias)) multIcandClearC1.Checked = true; else multIcandClear01.Checked = true;
            multIcandOperation2.SelectedIndex = mult.getIcandOperationSwitch(1 + bias);
            if (mult.getIcandClearSwitch(1 + bias)) multIcandClearC2.Checked = true; else multIcandClear02.Checked = true;
            multIcandOperation3.SelectedIndex = mult.getIcandOperationSwitch(2 + bias);
            if (mult.getIcandClearSwitch(2 + bias)) multIcandClearC3.Checked = true; else multIcandClear03.Checked = true;
            multIcandOperation4.SelectedIndex = mult.getIcandOperationSwitch(3 + bias);
            if (mult.getIcandClearSwitch(3 + bias)) multIcandClearC4.Checked = true; else multIcandClear04.Checked = true;
            multIcandOperation5.SelectedIndex = mult.getIcandOperationSwitch(4 + bias);
            if (mult.getIcandClearSwitch(4 + bias)) multIcandClearC5.Checked = true; else multIcandClear05.Checked = true;
            multIcandOperation6.SelectedIndex = mult.getIcandOperationSwitch(5 + bias);
            if (mult.getIcandClearSwitch(5 + bias)) multIcandClearC6.Checked = true; else multIcandClear06.Checked = true;
            multIcandOperation7.SelectedIndex = mult.getIcandOperationSwitch(6 + bias);
            if (mult.getIcandClearSwitch(6 + bias)) multIcandClearC7.Checked = true; else multIcandClear07.Checked = true;
            multIcandOperation8.SelectedIndex = mult.getIcandOperationSwitch(7 + bias);
            if (mult.getIcandClearSwitch(7 + bias)) multIcandClearC8.Checked = true; else multIcandClear08.Checked = true;
            multFigures1.SelectedIndex = mult.getSignificantFigures(0 + bias);
            multFigures2.SelectedIndex = mult.getSignificantFigures(1 + bias);
            multFigures3.SelectedIndex = mult.getSignificantFigures(2 + bias);
            multFigures4.SelectedIndex = mult.getSignificantFigures(3 + bias);
            multFigures5.SelectedIndex = mult.getSignificantFigures(4 + bias);
            multFigures6.SelectedIndex = mult.getSignificantFigures(5 + bias);
            multFigures7.SelectedIndex = mult.getSignificantFigures(6 + bias);
            multFigures8.SelectedIndex = mult.getSignificantFigures(7 + bias);
            multPlaces1.SelectedIndex = mult.getPlaces(0 + bias);
            multPlaces2.SelectedIndex = mult.getPlaces(1 + bias);
            multPlaces3.SelectedIndex = mult.getPlaces(2 + bias);
            multPlaces4.SelectedIndex = mult.getPlaces(3 + bias);
            multPlaces5.SelectedIndex = mult.getPlaces(4 + bias);
            multPlaces6.SelectedIndex = mult.getPlaces(5 + bias);
            multPlaces7.SelectedIndex = mult.getPlaces(6 + bias);
            multPlaces8.SelectedIndex = mult.getPlaces(7 + bias);
            multProduct1.SelectedIndex = mult.getProduct(0 + bias);
            multProduct2.SelectedIndex = mult.getProduct(1 + bias);
            multProduct3.SelectedIndex = mult.getProduct(2 + bias);
            multProduct4.SelectedIndex = mult.getProduct(3 + bias);
            multProduct5.SelectedIndex = mult.getProduct(4 + bias);
            multProduct6.SelectedIndex = mult.getProduct(5 + bias);
            multProduct7.SelectedIndex = mult.getProduct(6 + bias);
            multProduct8.SelectedIndex = mult.getProduct(7 + bias);
            setComboBox(multRAlpha, mult.getRProgramOut(0));
            setComboBox(multRBeta, mult.getRProgramOut(1));
            setComboBox(multRGamma, mult.getRProgramOut(2));
            setComboBox(multRDelta, mult.getRProgramOut(3));
            setComboBox(multREpsilon, mult.getRProgramOut(4));
            setComboBox(multDAlpha, mult.getDProgramOut(0));
            setComboBox(multDBeta, mult.getDProgramOut(1));
            setComboBox(multDGamma, mult.getDProgramOut(2));
            setComboBox(multDDelta, mult.getDProgramOut(3));
            setComboBox(multDEpsilon, mult.getDProgramOut(4));
            setComboBox(multAOut, mult.getOutputA());
            setComboBox(multSOut, mult.getOutputS());
            setComboBox(multASOut, mult.getOutputAS());
            setComboBox(multACOut, mult.getOutputAC());
            setComboBox(multSCOut, mult.getOutputSC());
            setComboBox(multASCOut, mult.getOutputASC());
            setComboBox(multRSOut, mult.getOutputRS());
            setComboBox(multDSOut, mult.getOutputDS());
            setComboBox(multFOut, mult.getOutputF());
            setComboBox(multProgIn1, mult.getProgramIn(0+bias));
            setComboBox(multProgIn2, mult.getProgramIn(1 + bias));
            setComboBox(multProgIn3, mult.getProgramIn(2 + bias));
            setComboBox(multProgIn4, mult.getProgramIn(3 + bias));
            setComboBox(multProgIn5, mult.getProgramIn(4 + bias));
            setComboBox(multProgIn6, mult.getProgramIn(5 + bias));
            setComboBox(multProgIn7, mult.getProgramIn(6 + bias));
            setComboBox(multProgIn8, mult.getProgramIn(7 + bias));
            setComboBox(multProgOut1, mult.getProgramOut(0 + bias));
            setComboBox(multProgOut2, mult.getProgramOut(1 + bias));
            setComboBox(multProgOut3, mult.getProgramOut(2 + bias));
            setComboBox(multProgOut4, mult.getProgramOut(3 + bias));
            setComboBox(multProgOut5, mult.getProgramOut(4 + bias));
            setComboBox(multProgOut6, mult.getProgramOut(5 + bias));
            setComboBox(multProgOut7, mult.getProgramOut(6 + bias));
            setComboBox(multProgOut8, mult.getProgramOut(7 + bias));
            setComboBox(lhpp1, mult.getLhpp1Out());
            setComboBox(lhpp2, mult.getLhpp2Out());
            setComboBox(rhpp1, mult.getRhpp1Out());
            setComboBox(rhpp2, mult.getRhpp2Out());
        }

        private void populateDividerTab()
        {
            EniacDivider div;
            div = computer.getDivider();
            if (div.getPowered()) dividerPowerOn.Checked = true; else dividerPowerOff.Checked = true;
            if (div.getNumeratorClear(0)) divNum1C.Checked = true; else divNum10.Checked = true;
            if (div.getNumeratorClear(1)) divNum2C.Checked = true; else divNum20.Checked = true;
            if (div.getNumeratorClear(2)) divNum3C.Checked = true; else divNum30.Checked = true;
            if (div.getNumeratorClear(3)) divNum4C.Checked = true; else divNum40.Checked = true;
            if (div.getNumeratorClear(4)) divNum5C.Checked = true; else divNum50.Checked = true;
            if (div.getNumeratorClear(5)) divNum6C.Checked = true; else divNum60.Checked = true;
            if (div.getNumeratorClear(6)) divNum7C.Checked = true; else divNum70.Checked = true;
            if (div.getNumeratorClear(7)) divNum8C.Checked = true; else divNum80.Checked = true;
            divNum1.SelectedIndex = div.getNumeratorReceive(0);
            divNum2.SelectedIndex = div.getNumeratorReceive(1);
            divNum3.SelectedIndex = div.getNumeratorReceive(2);
            divNum4.SelectedIndex = div.getNumeratorReceive(3);
            divNum5.SelectedIndex = div.getNumeratorReceive(4);
            divNum6.SelectedIndex = div.getNumeratorReceive(5);
            divNum7.SelectedIndex = div.getNumeratorReceive(6);
            divNum8.SelectedIndex = div.getNumeratorReceive(7);
            if (div.getDenominatorClear(0)) divDen1C.Checked = true; else divDen10.Checked = true;
            if (div.getDenominatorClear(1)) divDen2C.Checked = true; else divDen20.Checked = true;
            if (div.getDenominatorClear(2)) divDen3C.Checked = true; else divDen30.Checked = true;
            if (div.getDenominatorClear(3)) divDen4C.Checked = true; else divDen40.Checked = true;
            if (div.getDenominatorClear(4)) divDen5C.Checked = true; else divDen50.Checked = true;
            if (div.getDenominatorClear(5)) divDen6C.Checked = true; else divDen60.Checked = true;
            if (div.getDenominatorClear(6)) divDen7C.Checked = true; else divDen70.Checked = true;
            if (div.getDenominatorClear(7)) divDen8C.Checked = true; else divDen80.Checked = true;
            divDen1.SelectedIndex = div.getDenominatorReceive(0);
            divDen2.SelectedIndex = div.getDenominatorReceive(1);
            divDen3.SelectedIndex = div.getDenominatorReceive(2);
            divDen4.SelectedIndex = div.getDenominatorReceive(3);
            divDen5.SelectedIndex = div.getDenominatorReceive(4);
            divDen6.SelectedIndex = div.getDenominatorReceive(5);
            divDen7.SelectedIndex = div.getDenominatorReceive(6);
            divDen8.SelectedIndex = div.getDenominatorReceive(7);
            if (div.getRoundoff(0)) divRoundOn1.Checked = true; else divRoundOff1.Checked = true;
            if (div.getRoundoff(1)) divRoundOn2.Checked = true; else divRoundOff2.Checked = true;
            if (div.getRoundoff(2)) divRoundOn3.Checked = true; else divRoundOff3.Checked = true;
            if (div.getRoundoff(3)) divRoundOn4.Checked = true; else divRoundOff4.Checked = true;
            if (div.getRoundoff(4)) divRoundOn5.Checked = true; else divRoundOff5.Checked = true;
            if (div.getRoundoff(5)) divRoundOn6.Checked = true; else divRoundOff6.Checked = true;
            if (div.getRoundoff(6)) divRoundOn7.Checked = true; else divRoundOff7.Checked = true;
            if (div.getRoundoff(7)) divRoundOn8.Checked = true; else divRoundOff8.Checked = true;
            divPlaces1.SelectedIndex = div.getPlaces(0);
            divPlaces2.SelectedIndex = div.getPlaces(1);
            divPlaces3.SelectedIndex = div.getPlaces(2);
            divPlaces4.SelectedIndex = div.getPlaces(3);
            divPlaces5.SelectedIndex = div.getPlaces(4);
            divPlaces6.SelectedIndex = div.getPlaces(5);
            divPlaces7.SelectedIndex = div.getPlaces(6);
            divPlaces8.SelectedIndex = div.getPlaces(7);
            if (div.getInterlockSwitch(0)) divIlOn1.Checked = true; else divIlOff1.Checked = true;
            if (div.getInterlockSwitch(1)) divIlOn2.Checked = true; else divIlOff2.Checked = true;
            if (div.getInterlockSwitch(2)) divIlOn3.Checked = true; else divIlOff3.Checked = true;
            if (div.getInterlockSwitch(3)) divIlOn4.Checked = true; else divIlOff4.Checked = true;
            if (div.getInterlockSwitch(4)) divIlOn5.Checked = true; else divIlOff5.Checked = true;
            if (div.getInterlockSwitch(5)) divIlOn6.Checked = true; else divIlOff6.Checked = true;
            if (div.getInterlockSwitch(6)) divIlOn7.Checked = true; else divIlOff7.Checked = true;
            if (div.getInterlockSwitch(7)) divIlOn8.Checked = true; else divIlOff8.Checked = true;
            divAnswer1.SelectedIndex = div.getAnswerSwitch(0);
            divAnswer2.SelectedIndex = div.getAnswerSwitch(1);
            divAnswer3.SelectedIndex = div.getAnswerSwitch(2);
            divAnswer4.SelectedIndex = div.getAnswerSwitch(3);
            divAnswer5.SelectedIndex = div.getAnswerSwitch(4);
            divAnswer6.SelectedIndex = div.getAnswerSwitch(5);
            divAnswer7.SelectedIndex = div.getAnswerSwitch(6);
            divAnswer8.SelectedIndex = div.getAnswerSwitch(7);
            setComboBox(divIlock1, div.getInterlockIn(0));
            setComboBox(divIlock2, div.getInterlockIn(1));
            setComboBox(divIlock3, div.getInterlockIn(2));
            setComboBox(divIlock4, div.getInterlockIn(3));
            setComboBox(divIlock5, div.getInterlockIn(4));
            setComboBox(divIlock6, div.getInterlockIn(5));
            setComboBox(divIlock7, div.getInterlockIn(6));
            setComboBox(divIlock8, div.getInterlockIn(7));
            setComboBox(divProgramIn1, div.getProgramIn(0));
            setComboBox(divProgramIn2, div.getProgramIn(1));
            setComboBox(divProgramIn3, div.getProgramIn(2));
            setComboBox(divProgramIn4, div.getProgramIn(3));
            setComboBox(divProgramIn5, div.getProgramIn(4));
            setComboBox(divProgramIn6, div.getProgramIn(5));
            setComboBox(divProgramIn7, div.getProgramIn(6));
            setComboBox(divProgramIn8, div.getProgramIn(7));
            setComboBox(divProgramOut1, div.getProgramOut(0));
            setComboBox(divProgramOut2, div.getProgramOut(1));
            setComboBox(divProgramOut3, div.getProgramOut(2));
            setComboBox(divProgramOut4, div.getProgramOut(3));
            setComboBox(divProgramOut5, div.getProgramOut(4));
            setComboBox(divProgramOut6, div.getProgramOut(5));
            setComboBox(divProgramOut7, div.getProgramOut(6));
            setComboBox(divProgramOut8, div.getProgramOut(7));
            setComboBox(divDigitOut, div.getDigitOut());
            quotientIC.SelectedIndex = div.getQuotientIC();
            numeratorIC.SelectedIndex = div.getNumeratorIC();
            denominatorIC.SelectedIndex = div.getDenominatorIC();
            shifterIC.SelectedIndex = div.getShifterIC();

        }

        private void populateFunc1Tab()
        {
            EniacFunctionTable func;
            func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            if (func.getPowered()) funcPowerOn.Checked = true; else funcPowerOff.Checked = true;
            switch (func.getClearMode(0))
            {
                case 0: funcClear01.Checked = true; break;
                case 1: funcClearNc1.Checked = true; break;
                case 2: funcClearC1.Checked = true; break;
            }
            switch (func.getClearMode(1))
            {
                case 0: funcClear02.Checked = true; break;
                case 1: funcClearNc2.Checked = true; break;
                case 2: funcClearC2.Checked = true; break;
            }
            switch (func.getClearMode(2))
            {
                case 0: funcClear03.Checked = true; break;
                case 1: funcClearNc3.Checked = true; break;
                case 2: funcClearC3.Checked = true; break;
            }
            switch (func.getClearMode(3))
            {
                case 0: funcClear04.Checked = true; break;
                case 1: funcClearNc4.Checked = true; break;
                case 2: funcClearC4.Checked = true; break;
            }
            switch (func.getClearMode(4))
            {
                case 0: funcClear05.Checked = true; break;
                case 1: funcClearNc5.Checked = true; break;
                case 2: funcClearC5.Checked = true; break;
            }
            switch (func.getClearMode(5))
            {
                case 0: funcClear06.Checked = true; break;
                case 1: funcClearNc6.Checked = true; break;
                case 2: funcClearC6.Checked = true; break;
            }
            switch (func.getClearMode(6))
            {
                case 0: funcClear07.Checked = true; break;
                case 1: funcClearNc7.Checked = true; break;
                case 2: funcClearC7.Checked = true; break;
            }
            switch (func.getClearMode(7))
            {
                case 0: funcClear08.Checked = true; break;
                case 1: funcClearNc8.Checked = true; break;
                case 2: funcClearC8.Checked = true; break;
            }
            switch (func.getClearMode(8))
            {
                case 0: funcClear09.Checked = true; break;
                case 1: funcClearNc9.Checked = true; break;
                case 2: funcClearC9.Checked = true; break;
            }
            switch (func.getClearMode(9))
            {
                case 0: funcClear010.Checked = true; break;
                case 1: funcClearNc10.Checked = true; break;
                case 2: funcClearC10.Checked = true; break;
            }
            switch (func.getClearMode(10))
            {
                case 0: funcClear011.Checked = true; break;
                case 1: funcClearNc11.Checked = true; break;
                case 2: funcClearC11.Checked = true; break;
            }
            funcMode1.SelectedIndex = func.getOperation(0);
            funcMode2.SelectedIndex = func.getOperation(1);
            funcMode3.SelectedIndex = func.getOperation(2);
            funcMode4.SelectedIndex = func.getOperation(3);
            funcMode5.SelectedIndex = func.getOperation(4);
            funcMode6.SelectedIndex = func.getOperation(5);
            funcMode7.SelectedIndex = func.getOperation(6);
            funcMode8.SelectedIndex = func.getOperation(7);
            funcMode9.SelectedIndex = func.getOperation(8);
            funcMode10.SelectedIndex = func.getOperation(9);
            funcMode11.SelectedIndex = func.getOperation(10);
            funcRepeat1.SelectedIndex = func.getRepeat(0);
            funcRepeat2.SelectedIndex = func.getRepeat(1);
            funcRepeat3.SelectedIndex = func.getRepeat(2);
            funcRepeat4.SelectedIndex = func.getRepeat(3);
            funcRepeat5.SelectedIndex = func.getRepeat(4);
            funcRepeat6.SelectedIndex = func.getRepeat(5);
            funcRepeat7.SelectedIndex = func.getRepeat(6);
            funcRepeat8.SelectedIndex = func.getRepeat(7);
            funcRepeat9.SelectedIndex = func.getRepeat(8);
            funcRepeat10.SelectedIndex = func.getRepeat(9);
            funcRepeat11.SelectedIndex = func.getRepeat(10);
            setComboBox(funcProgramIn1, func.getProgramIn(0));
            setComboBox(funcProgramIn2, func.getProgramIn(1));
            setComboBox(funcProgramIn3, func.getProgramIn(2));
            setComboBox(funcProgramIn4, func.getProgramIn(3));
            setComboBox(funcProgramIn5, func.getProgramIn(4));
            setComboBox(funcProgramIn6, func.getProgramIn(5));
            setComboBox(funcProgramIn7, func.getProgramIn(6));
            setComboBox(funcProgramIn8, func.getProgramIn(7));
            setComboBox(funcProgramIn9, func.getProgramIn(8));
            setComboBox(funcProgramIn10, func.getProgramIn(9));
            setComboBox(funcProgramIn11, func.getProgramIn(10));
            setComboBox(funcProgramOut1, func.getProgramOut(0));
            setComboBox(funcProgramOut2, func.getProgramOut(1));
            setComboBox(funcProgramOut3, func.getProgramOut(2));
            setComboBox(funcProgramOut4, func.getProgramOut(3));
            setComboBox(funcProgramOut5, func.getProgramOut(4));
            setComboBox(funcProgramOut6, func.getProgramOut(5));
            setComboBox(funcProgramOut7, func.getProgramOut(6));
            setComboBox(funcProgramOut8, func.getProgramOut(7));
            setComboBox(funcProgramOut9, func.getProgramOut(8));
            setComboBox(funcProgramOut10, func.getProgramOut(9));
            setComboBox(funcProgramOut11, func.getProgramOut(10));
            setComboBox(funcOutNC, func.getOutputNC());
            setComboBox(funcOutC, func.getOutputC());
            setComboBox(funcArgIn, func.getArgumentIn());

        }

        private void populateFunc2Tab()
        {
            EniacFunctionTable func;
            func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            switch (func.getMasterSign(0))
            {
                case 0: funcP1.Checked = true; break;
                case 1: funcM1.Checked = true; break;
                case 2: funcTable1.Checked = true; break;
            }
            switch (func.getMasterSign(1))
            {
                case 0: funcP2.Checked = true; break;
                case 1: funcM2.Checked = true; break;
                case 2: funcTable2.Checked = true; break;
            }
            setComboBox(funcOutputA, func.getOutputA());
            setComboBox(funcOutputB, func.getOutputB());
            if (func.getADeleteSwitch(0)) funcAD1.Checked = true; else funcA01.Checked = true;
            if (func.getADeleteSwitch(1)) funcAD2.Checked = true; else funcA02.Checked = true;
            if (func.getADeleteSwitch(2)) funcAD3.Checked = true; else funcA03.Checked = true;
            if (func.getADeleteSwitch(3)) funcAD4.Checked = true; else funcA04.Checked = true;
            funcAValue1.SelectedIndex = func.getConstantA(0);
            funcAValue2.SelectedIndex = func.getConstantA(1);
            funcAValue3.SelectedIndex = func.getConstantA(2);
            funcAValue4.SelectedIndex = func.getConstantA(3);
            if (func.getBDeleteSwitch(0)) funcBD1.Checked = true; else funcB01.Checked = true;
            if (func.getBDeleteSwitch(1)) funcBD2.Checked = true; else funcB02.Checked = true;
            if (func.getBDeleteSwitch(2)) funcBD3.Checked = true; else funcB03.Checked = true;
            if (func.getBDeleteSwitch(3)) funcBD4.Checked = true; else funcB04.Checked = true;
            funcBValue1.SelectedIndex = func.getConstantB(0);
            funcBValue2.SelectedIndex = func.getConstantB(1);
            funcBValue3.SelectedIndex = func.getConstantB(2);
            funcBValue4.SelectedIndex = func.getConstantB(3);
            if (func.getASubSwitch(0)) funcASubS1.Checked = true; else funcASub01.Checked = true;
            if (func.getASubSwitch(1)) funcASubS2.Checked = true; else funcASub02.Checked = true;
            if (func.getASubSwitch(2)) funcASubS3.Checked = true; else funcASub03.Checked = true;
            if (func.getASubSwitch(3)) funcASubS4.Checked = true; else funcASub04.Checked = true;
            if (func.getASubSwitch(4)) funcASubS5.Checked = true; else funcASub05.Checked = true;
            if (func.getASubSwitch(5)) funcASubS6.Checked = true; else funcASub06.Checked = true;
            if (func.getBSubSwitch(0)) funcBSubS1.Checked = true; else funcBSub01.Checked = true;
            if (func.getBSubSwitch(1)) funcBSubS2.Checked = true; else funcBSub02.Checked = true;
            if (func.getBSubSwitch(2)) funcBSubS3.Checked = true; else funcBSub03.Checked = true;
            if (func.getBSubSwitch(3)) funcBSubS4.Checked = true; else funcBSub04.Checked = true;
            if (func.getBSubSwitch(4)) funcBSubS5.Checked = true; else funcBSub05.Checked = true;
            if (func.getBSubSwitch(5)) funcBSubS6.Checked = true; else funcBSub06.Checked = true;

        }

        private void populatePortTab()
        {
            updatePortableTable();
        }

        private void populatePrinterTab()
        {
            EniacCardPunch punch;
            punch = computer.getPunch();
            couplerGroup1.Visible = false;
            couplerGroup2.Visible = false;
            couplerGroup3.Visible = false;
            couplerGroup4.Visible = false;
            couplerGroup5.Visible = false;
            couplerGroup6.Visible = false;
            couplerGroup7.Visible = false;
            couplerGroup8.Visible = false;
            couplerGroup9.Visible = false;
            couplerGroup10.Visible = false;
            couplerGroup11.Visible = false;
            couplerGroup12.Visible = false;
            couplerGroup13.Visible = false;
            couplerGroup14.Visible = false;
            couplerGroup15.Visible = false;
            couplerGroup16.Visible = false;
            printGroup1.Visible = false;
            printGroup2.Visible = false;
            printGroup3.Visible = false;
            printGroup4.Visible = false;
            printGroup5.Visible = false;
            printGroup6.Visible = false;
            printGroup7.Visible = false;
            printGroup8.Visible = false;
            printGroup9.Visible = false;
            printGroup10.Visible = false;
            printGroup11.Visible = false;
            printGroup12.Visible = false;
            printGroup13.Visible = false;
            printGroup14.Visible = false;
            printGroup15.Visible = false;
            printGroup16.Visible = false;
            switch (printerPanelSelect.SelectedIndex)
            {
                case 0:
                    couplerGroup1.Visible = true;
                    couplerGroup2.Visible = true;
                    couplerGroup3.Visible = true;
                    couplerGroup4.Visible = true;
                    couplerGroup5.Visible = true;
                    couplerGroup6.Visible = true;
                    couplerGroup7.Visible = true;
                    couplerGroup8.Visible = true;
                    if (punch.getCouplerSwitch(0)) coupler1C.Checked = true; else coupler10.Checked = true;
                    if (punch.getCouplerSwitch(1)) coupler2C.Checked = true; else coupler20.Checked = true;
                    if (punch.getCouplerSwitch(2)) coupler3C.Checked = true; else coupler30.Checked = true;
                    if (punch.getCouplerSwitch(3)) coupler4C.Checked = true; else coupler40.Checked = true;
                    if (punch.getCouplerSwitch(4)) coupler5C.Checked = true; else coupler50.Checked = true;
                    if (punch.getCouplerSwitch(5)) coupler6C.Checked = true; else coupler60.Checked = true;
                    if (punch.getCouplerSwitch(6)) coupler7C.Checked = true; else coupler70.Checked = true;
                    if (punch.getCouplerSwitch(7)) coupler8C.Checked = true; else coupler80.Checked = true;
                    break;
                case 1:
                    printGroup1.Visible = true;
                    printGroup2.Visible = true;
                    printGroup3.Visible = true;
                    printGroup4.Visible = true;
                    printGroup5.Visible = true;
                    printGroup6.Visible = true;
                    printGroup7.Visible = true;
                    printGroup8.Visible = true;
                    printGroup9.Visible = true;
                    printGroup10.Visible = true;
                    printGroup11.Visible = true;
                    printGroup12.Visible = true;
                    printGroup13.Visible = true;
                    printGroup14.Visible = true;
                    printGroup15.Visible = true;
                    printGroup16.Visible = true;
                    if (punch.getPrintSwitch(0)) print1On.Checked = true; else print1Off.Checked = true;
                    if (punch.getPrintSwitch(1)) print2On.Checked = true; else print2Off.Checked = true;
                    if (punch.getPrintSwitch(2)) print3On.Checked = true; else print3Off.Checked = true;
                    if (punch.getPrintSwitch(3)) print4On.Checked = true; else print4Off.Checked = true;
                    if (punch.getPrintSwitch(4)) print5On.Checked = true; else print5Off.Checked = true;
                    if (punch.getPrintSwitch(5)) print6On.Checked = true; else print6Off.Checked = true;
                    if (punch.getPrintSwitch(6)) print7On.Checked = true; else print7Off.Checked = true;
                    if (punch.getPrintSwitch(7)) print8On.Checked = true; else print8Off.Checked = true;
                    if (punch.getPrintSwitch(8)) print9On.Checked = true; else print9Off.Checked = true;
                    if (punch.getPrintSwitch(9)) print10On.Checked = true; else print10Off.Checked = true;
                    if (punch.getPrintSwitch(10)) print11On.Checked = true; else print11Off.Checked = true;
                    if (punch.getPrintSwitch(11)) print12On.Checked = true; else print12Off.Checked = true;
                    if (punch.getPrintSwitch(12)) print13On.Checked = true; else print13Off.Checked = true;
                    if (punch.getPrintSwitch(13)) print14On.Checked = true; else print14Off.Checked = true;
                    if (punch.getPrintSwitch(14)) print15On.Checked = true; else print15Off.Checked = true;
                    if (punch.getPrintSwitch(15)) print16On.Checked = true; else print16Off.Checked = true;
                    break;
                case 2:
                    couplerGroup9.Visible = true;
                    couplerGroup10.Visible = true;
                    couplerGroup11.Visible = true;
                    couplerGroup12.Visible = true;
                    couplerGroup13.Visible = true;
                    couplerGroup14.Visible = true;
                    couplerGroup15.Visible = true;
                    couplerGroup16.Visible = true;
                    if (punch.getCouplerSwitch(8))  coupler9C.Checked = true; else coupler90.Checked = true;
                    if (punch.getCouplerSwitch(9))  coupler10C.Checked = true; else coupler100.Checked = true;
                    if (punch.getCouplerSwitch(10)) coupler11C.Checked = true; else coupler110.Checked = true;
                    if (punch.getCouplerSwitch(11)) coupler12C.Checked = true; else coupler120.Checked = true;
                    if (punch.getCouplerSwitch(12)) coupler13C.Checked = true; else coupler130.Checked = true;
                    if (punch.getCouplerSwitch(13)) coupler14C.Checked = true; else coupler140.Checked = true;
                    if (punch.getCouplerSwitch(14)) coupler15C.Checked = true; else coupler150.Checked = true;
                    if (punch.getCouplerSwitch(15)) coupler16C.Checked = true; else coupler160.Checked = true;
                    break;
            }
        }

        private void populateCurrentTab()
        {
            EniacAccumulator acc = getAccumulatorForTab();
            if (tabControl1.SelectedTab == tabInit) populateInitTab();
            if (tabControl1.SelectedTab == tabConst1) populateConst1Tab();
            if (tabControl1.SelectedTab == tabConst2) populateConst2Tab();
            if (tabControl1.SelectedTab == tabPunch) populatePunchTab();
            if (tabControl1.SelectedTab == tabReader) populateReaderTab();
            if (tabControl1.SelectedTab == tabMaster) populateMasterProgrammerTab();
            if (tabControl1.SelectedTab == tabMultiplier) populateMultiplierTab();
            if (tabControl1.SelectedTab == tabDivider) populateDividerTab();
            if (tabControl1.SelectedTab == tabFunc1) populateFunc1Tab();
            if (tabControl1.SelectedTab == tabFunc2) populateFunc2Tab();
            if (tabControl1.SelectedTab == tabPort) populatePortTab();
            if (tabControl1.SelectedTab == tabFilters) populateFiltersTab();
            if (tabControl1.SelectedTab == tabPrint) populatePrinterTab();
            if (tabControl1.SelectedTab == tabAcc1)
            {
                if (acc != null)
                {
                    populateAccumulatorTab( acc);
                }
            }
        }


        private void showSelectedFilter()
        {
            EniacFilter filter;
            int[] map;
            filter = computer.findFilter(filterSelect.Text);
            if (filter != null)
            {
                filterName.Text = filter.getName();
                map = filter.getMap();
                filterPin1.SelectedIndex = map[0] + 1;
                filterPin2.SelectedIndex = map[1] + 1;
                filterPin3.SelectedIndex = map[2] + 1;
                filterPin4.SelectedIndex = map[3] + 1;
                filterPin5.SelectedIndex = map[4] + 1;
                filterPin6.SelectedIndex = map[5] + 1;
                filterPin7.SelectedIndex = map[6] + 1;
                filterPin8.SelectedIndex = map[7] + 1;
                filterPin9.SelectedIndex = map[8] + 1;
                filterPin10.SelectedIndex = map[9] + 1;
                filterPin11.SelectedIndex = map[10] + 1;
            }

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateCurrentTab();
        }

        private void PowerOff_CheckedChanged(object sender, EventArgs e)
        {
            EniacAccumulator acc = getAccumulatorForTab();
            if (acc != null)
            {
                acc.setPowered((((RadioButton)sender).Checked) ? false : true   );
            }
        }

        private void ClearSwitch_CheckedChanged(object sender, EventArgs e)
        {
            EniacAccumulator acc = getAccumulatorForTab();
            if (acc != null)
            {
                acc.setClearSwitch(Convert.ToInt32(((RadioButton)sender).Tag),(((RadioButton)sender).Checked) ? true : false);
            }
        }

        private void OperationSwitch_Changed(object sender, EventArgs e)
        {
            EniacAccumulator acc = getAccumulatorForTab();
            if (acc != null)
            {
                acc.setOperationSwitch(Convert.ToInt32(((ComboBox)sender).Tag), ((ComboBox)sender).SelectedIndex);
            }
        }

        private void RepeatSwitch_Changed(object sender, EventArgs e)
        {
            EniacAccumulator acc = getAccumulatorForTab();
            if (acc != null)
            {
                acc.setRepeatSwitch(Convert.ToInt32(((ComboBox)sender).Tag)-1, ((ComboBox)sender).SelectedIndex + 1);
            }
        }

        private void DigitInput_Changed(object sender, EventArgs e)
        {
            String item;
            EniacAccumulator acc = getAccumulatorForTab();
            if (acc != null)
            {
                item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
                acc.setDigitInput(Convert.ToInt32(((ComboBox)sender).Tag), item);
            }
        }

        private void Inputfilter_Changed(object sender, EventArgs e)
        {
            String item;
            EniacAccumulator acc = getAccumulatorForTab();
            if (acc != null)
            {
                item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
                acc.setInputFilter(Convert.ToInt32(((ComboBox)sender).Tag), item);
            }
        }

        private void DigitOutput_Changed(object sender, EventArgs e)
        {
            String item;
            EniacAccumulator acc = getAccumulatorForTab();
            if (acc != null)
            {
                item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
                acc.setDigitOutput(Convert.ToInt32(((ComboBox)sender).Tag), item);
            }
        }

        private void ProgramInput_Changed(object sender, EventArgs e)
        {
            String item;
            EniacAccumulator acc = getAccumulatorForTab();
            if (acc != null)
            {
                item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
                acc.setProgramInput(Convert.ToInt32(((ComboBox)sender).Tag), item);
            }
        }

        private void ProgramOutput_Changed(object sender, EventArgs e)
        {
            String item;
            EniacAccumulator acc = getAccumulatorForTab();
            if (acc != null)
            {
                item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
                acc.setProgramOutput(Convert.ToInt32(((ComboBox)sender).Tag), item);
            }
        }

        private void accSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacAccumulator acc = getAccumulatorForTab();
            populateAccumulatorTab( acc);
        }

        private void SigDigitsSwitch_Changed(object sender, EventArgs e)
        {
            EniacAccumulator acc = getAccumulatorForTab();
            if (acc != null)
            {
                acc.setSignificantFigures(((ComboBox)sender).SelectedIndex);
            }
        }

        private void cycle()
        {
            int i;
            String card;
            for (i = 0; i < 1000; i++)
            {
                computer.cycle();
                showCurrentAccumulator();
                showCurrentSteppers();
                showCurrentDecades();
                if (debugEnabled.Checked) debugOutput.AppendText(computer.getDebug());
                else computer.clearDebug();
                card = computer.getCard();
                if (card.Length > 2) punchStacker.AppendText(card + "\r\n");
                if (breakPoint > 0 && breakPoint == computer.getCycles())
                {
                    cycleModeAdd.Checked = true;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            cycle();
        }

        private void digit_Click(object sender, EventArgs e)
        {
            int i;
            EniacAccumulator acc;
            String s;
            i = Convert.ToInt32(((TextBox)sender).Text);
            i++;
            if (i > 10) i -= 10;
            ((TextBox)sender).Text = i.ToString();
            s = accDigit0.Text;
            s += accDigit1.Text;
            s += accDigit2.Text;
            s += accDigit3.Text;
            s += accDigit4.Text;
            s += accDigit5.Text;
            s += accDigit6.Text;
            s += accDigit7.Text;
            s += accDigit8.Text;
            s += accDigit9.Text;
            acc = getAccumulatorForTab();
            acc.setValue(s);
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            computer.goPressed();
        }

        private void initGoOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            EniacInitiatingUnit acc = computer.getInitUnit();
            if (acc != null)
            {
                item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
                acc.setGoOutput(item);
            }
 
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            computer.clearPressed();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            int i;
            StreamWriter file;
            ArrayList filters;
            saveFileDialog1.Filter = "ENIAC files (*.en)|*.en|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file = new StreamWriter(saveFileDialog1.FileName);
                computer.getInitUnit().save(file);
                for (i = 0; i < 20; i++) computer.getAccumulator(i).save(file);
                computer.getConstUnit().save(file);
                computer.getPunch().save(file);
                computer.getReader().save(file);
                computer.getMasterUnit().save(file);
                computer.getMultiplier().save(file);
                computer.getDivider().save(file);
                for (i = 0; i < 3; i++) computer.getFunctionTable(i).save(file);
                for (i = 0; i < cardInputHopper.Lines.Length; i++)
                    if (cardInputHopper.Lines[i].Length > 8) file.WriteLine("hopper " + cardInputHopper.Lines[i]);
                for (i = 0; i < cardOutputStacker.Lines.Length; i++)
                    if (cardOutputStacker.Lines[i].Length > 8) file.WriteLine("stacker " + cardOutputStacker.Lines[i]);
                filters = computer.getFilters();
                for (i = 55; i < filters.Count; i++)
                {
                    ((EniacFilter)filters[i]).save(file);
                }
                file.Close();
                this.Text = "ENIAC - " + saveFileDialog1.FileName;
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            int i;
            String line;
            StreamReader file;
            EniacFilter filter;
            openFileDialog1.Filter = "ENIAC files (*.en)|*.en|All files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file = new StreamReader(openFileDialog1.FileName);
                computer.getConstUnit().reset();
                computer.getPunch().reset();
                computer.getReader().reset();
                computer.getMasterUnit().reset();
                computer.getMultiplier().reset();
                computer.getDivider().reset();
                for (i = 0; i < 20; i++) computer.getAccumulator(i).reset();
                for (i = 0; i < 3; i++) computer.getFunctionTable(i).reset();
                while (!file.EndOfStream)
                {
                    line = file.ReadLine().Trim().ToLower();
                    if (line.StartsWith("initiating"))
                    {
                        computer.getInitUnit().load(file);
                    }
                    if (line.StartsWith("constant"))
                    {
                        computer.getConstUnit().load(file);
                    }
                    if (line.StartsWith("cardpunch"))
                    {
                        computer.getPunch().load(file);
                    }
                    if (line.StartsWith("cardreader"))
                    {
                        computer.getReader().load(file);
                    }
                    if (line.StartsWith("master"))
                    {
                        computer.getMasterUnit().load(file);
                    }
                    if (line.StartsWith("accumulator"))
                    {
                        line = line.Substring(11).Trim();
                        line = line.Substring(0, line.IndexOf(' '));
                        i = Convert.ToInt32(line);
                        computer.getAccumulator(i-1).load(file);
                    }
                    if (line.StartsWith("filter"))
                    {
                        line = line.Substring(7).Trim();
                        line = line.Substring(0, line.IndexOf(' '));
                        filter = new EniacFilter(line);
                        filter.load(file);
                        computer.getFilters().Add(filter);
                    }
                    if (line.StartsWith("multiplier"))
                    {
                        computer.getMultiplier().load(file);
                    }
                    if (line.StartsWith("divider"))
                    {
                        computer.getDivider().load(file);
                    }
                    if (line.StartsWith("function"))
                    {
                        line = line.Substring(9).Trim();
                        line = line.Substring(0, line.IndexOf(' '));
                        i = Convert.ToInt32(line);
                        computer.getFunctionTable(i - 1).load(file);
                    }
                    if (line.StartsWith("hopper")) cardInputHopper.AppendText(line.Substring(7) + "\r\n");
                    if (line.StartsWith("stacker")) cardOutputStacker.AppendText(line.Substring(8) + "\r\n");
                }
                file.Close();
                populateInitTab();
                this.Text = "ENIAC - " + openFileDialog1.FileName;
            }
        }

        private void regJ_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            constUnit.setRegJ(Convert.ToInt32(((ComboBox)sender).Tag), ((ComboBox)sender).SelectedIndex);
            populateConst2Tab();
        }

        private void regK_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            constUnit.setRegK(Convert.ToInt32(((ComboBox)sender).Tag), ((ComboBox)sender).SelectedIndex);
            populateConst2Tab();
        }

        private void jlm_CheckedChanged(object sender, EventArgs e)
        {
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            constUnit.setJLSign(jlm.Checked);
        }

        private void jrm_CheckedChanged(object sender, EventArgs e)
        {
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            constUnit.setJRSign(jrm.Checked);
        }

        private void klm_CheckedChanged(object sender, EventArgs e)
        {
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            constUnit.setKLSign(klm.Checked);
        }

        private void krm_CheckedChanged(object sender, EventArgs e)
        {
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            constUnit.setKRSign(krm.Checked);
        }

        private void constFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tag;
            int pos;
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            pos = ((ComboBox)sender).SelectedIndex;
            constUnit.setOperationSwitch(tag, pos);
        }

        private void constSelect_CheckChanged(object sender, EventArgs e)
        {
            int tag;
            Boolean pos;
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            pos = (((RadioButton)sender).Checked) ? false : true;
            constUnit.setSelectSwitch(tag, pos);
        }

        private void constDigitOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tag;
            String pos;
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            pos = (((ComboBox)sender).SelectedItem).ToString();
            constUnit.setDigitOutput(tag, pos);
        }

        private void constProgramInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tag;
            String pos;
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            pos = (((ComboBox)sender).SelectedItem).ToString();
            constUnit.setProgramInput(tag, pos);
        }

        private void constProgramOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tag;
            String pos;
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            pos = (((ComboBox)sender).SelectedItem).ToString();
            constUnit.setProgramOutput(tag, pos);
        }

        private void constPowerOff_CheckedChanged(object sender, EventArgs e)
        {
            EniacConstantTransmitter constUnit = computer.getConstUnit();
            constUnit.setPowered(constPowerOn.Checked);
        }

        private void punchPowerOff_CheckedChanged(object sender, EventArgs e)
        {
            EniacCardPunch punch = computer.getPunch();
            punch.setPowered(punchPowerOn.Checked);
        }

        private void punchProgramIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacCardPunch punch = computer.getPunch();
            punch.setProgramInput(punchProgramIn.SelectedItem.ToString());
        }

        private void punchProgramOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacCardPunch punch = computer.getPunch();
            punch.setProgramOutput(punchProgramOut.SelectedItem.ToString());
        }

        private void readerPowerOff_CheckedChanged(object sender, EventArgs e)
        {
            EniacCardReader reader = computer.getReader();
            reader.setPowered(readerPowerOn.Checked);
        }

        private void readerProgramIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacCardReader reader = computer.getReader();
            reader.setProgramInput(readerProgramIn.SelectedItem.ToString());
        }

        private void readerInterlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacCardReader reader = computer.getReader();
            reader.setInterlockIn(readerInterlock.SelectedItem.ToString());
        }

        private void readerProgramOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacCardReader reader = computer.getReader();
            reader.setProgramOutput(readerProgramOut.SelectedItem.ToString());
        }

        private void removePunchCardsButton_Click(object sender, EventArgs e)
        {
            punchStacker.Text = "";
        }

        private void readerRemoveCardsButton_Click(object sender, EventArgs e)
        {
            cardOutputStacker.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cardInputHopper.AppendText(cardOutputStacker.Text);
            cardOutputStacker.Text = "";
        }

        private void masterPowerOff_CheckedChanged(object sender, EventArgs e)
        {
            computer.getMasterUnit().setPowered((masterPowerOn.Checked) ? true : false);
        }

        private void decadeSwitch_SelectedIndexChanged(object sender, EventArgs e)
        {
            int stage;
            int decade;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            stage = tag / 100;
            decade = (tag % 100) - 1;
            decade += (masterPanelSelect.SelectedIndex == 0) ? 10 : 0;
            computer.getMasterUnit().setDecadeSwitch(decade, stage, ((ComboBox)sender).SelectedIndex);
        }

        private void masterPanelSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateCurrentTab();
        }

        private void assoc_CheckedChanged(object sender, EventArgs e)
        {
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            if (masterPanelSelect.SelectedIndex == 0) tag += 4;
            computer.getMasterUnit().setAssociationSwitch(tag,(((RadioButton)sender).Checked) ? false : true);
        }

        private void decadeDI_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            if (masterPanelSelect.SelectedIndex == 0) tag += 10;
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getMasterUnit().setDecadeDirectInput(tag, item);
        }

        private void stepperClear_SelectedIndexChanged(object sender, EventArgs e)
        {
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            if (masterPanelSelect.SelectedIndex == 1) tag += 5;
            computer.getMasterUnit().setStepperClearSwitch(tag, ((ComboBox)sender).SelectedIndex);
        }

        private void stepperDI_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            if (masterPanelSelect.SelectedIndex == 1) tag += 5;
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getMasterUnit().setStepperDirectInput(tag, item);
        }

        private void stepperI_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            if (masterPanelSelect.SelectedIndex == 1) tag += 5;
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getMasterUnit().setStepperInput(tag, item);
        }

        private void stepperDirectClear_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            if (masterPanelSelect.SelectedIndex == 1) tag += 5;
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getMasterUnit().setStepperDirectClear(tag, item);
        }

        private void stepperOutput_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            int stepper;
            int output;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            stepper = (tag / 100) - 1;
            output = (tag % 100) - 1;
            if (masterPanelSelect.SelectedIndex == 1) stepper += 5;
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getMasterUnit().setStepperOutput(stepper, output, item);
        }

        private void clearIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getInitUnit().setClearIn(tag, item);
        }

        private void clearOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getInitUnit().setClearOut(tag, item);
        }

        private void cycleMode_CheckedChanged(object sender, EventArgs e)
        {
            if (cycleModePulse.Checked) computer.getCyclingUnit().setMode(EniacCyclingUnit.MODE_PULSE);
            if (cycleModeAdd.Checked) computer.getCyclingUnit().setMode(EniacCyclingUnit.MODE_ADD);
            if (cycleModeCont.Checked) computer.getCyclingUnit().setMode(EniacCyclingUnit.MODE_CONT);
        }

        private void stepButton_Click(object sender, EventArgs e)
        {
            computer.getCyclingUnit().goPressed();
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            int i;
            computer.getInitUnit().reset();
            computer.getCyclingUnit().reset();
            computer.getMasterUnit().reset();
            computer.getReader().reset();
            computer.getPunch().reset();
            computer.getConstUnit().reset();
            computer.getMultiplier().reset();
            computer.getDivider().reset();
            for (i = 0; i < 20; i++)
                computer.getAccumulator(i).reset();
            for (i = 0; i < 3; i++)
                computer.getFunctionTable(i).reset();
            populateCurrentTab();
            this.Text = "ENIAC";
        }

        private void leftAccumulator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (populating) return;
            EniacAccumulator acc = getAccumulatorForTab();
            acc.setLeftAccumulator(leftAccumulator.SelectedIndex - 1,true);
            populateCurrentTab();
        }

        private void rightAccumulator_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (populating) return;
            EniacAccumulator acc = getAccumulatorForTab();
            acc.setRightAccumulator(rightAccumulator.SelectedIndex - 1,true);
            populateCurrentTab();
        }

        private void debugClearButton_Click(object sender, EventArgs e)
        {
            debugOutput.Text = "";
        }

        private void breakPointBox_TextChanged(object sender, EventArgs e)
        {
            int i;
            Boolean flag;
            flag = true;
            if (breakPointBox.Text.Length < 1) flag = false;
            for (i = 0; i < breakPointBox.Text.Length; i++)
                if (breakPointBox.Text[i] < '0' || breakPointBox.Text[i] > '9') flag = false;
            if (flag) breakPoint = Convert.ToInt64(breakPointBox.Text);
        }

        private void multIerOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            int bias;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            bias = multiplierPanelSelect.SelectedIndex * 8;
            computer.getMultiplier().setIerOperationSwitch(tag+bias, item);
        }

        private void multIerClearC_CheckedChanged(object sender, EventArgs e)
        {
            Boolean item;
            int tag;
            int bias;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            item = ((RadioButton)sender).Checked;
            bias = multiplierPanelSelect.SelectedIndex * 8;
            computer.getMultiplier().setIerClearSwitch(tag + bias, item);

        }

        private void multiplierPanelSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            int bias;
            populateCurrentTab();
            bias = multiplierPanelSelect.SelectedIndex * 8;
            multIer1.Text = "Ier " + (bias + 1).ToString();
            multIer2.Text = "Ier " + (bias + 2).ToString();
            multIer3.Text = "Ier " + (bias + 3).ToString();
            multIer4.Text = "Ier " + (bias + 4).ToString();
            multIer5.Text = "Ier " + (bias + 5).ToString();
            multIer6.Text = "Ier " + (bias + 6).ToString();
            multIer7.Text = "Ier " + (bias + 7).ToString();
            multIer8.Text = "Ier " + (bias + 8).ToString();
            multIcand1.Text = "Icand " + (bias + 1).ToString();
            multIcand2.Text = "Icand " + (bias + 2).ToString();
            multIcand3.Text = "Icand " + (bias + 3).ToString();
            multIcand4.Text = "Icand " + (bias + 4).ToString();
            multIcand5.Text = "Icand " + (bias + 5).ToString();
            multIcand6.Text = "Icand " + (bias + 6).ToString();
            multIcand7.Text = "Icand " + (bias + 7).ToString();
            multIcand8.Text = "Icand " + (bias + 8).ToString();
            multSigFigures1.Text = "Figures " + (bias + 1).ToString();
            multSigFigures2.Text = "Figures " + (bias + 2).ToString();
            multSigFigures3.Text = "Figures " + (bias + 3).ToString();
            multSigFigures4.Text = "Figures " + (bias + 4).ToString();
            multSigFigures5.Text = "Figures " + (bias + 5).ToString();
            multSigFigures6.Text = "Figures " + (bias + 6).ToString();
            multSigFigures7.Text = "Figures " + (bias + 7).ToString();
            multSigFigures8.Text = "Figures " + (bias + 8).ToString();
            multPlacesGroup1.Text = "Places " + (bias + 1).ToString();
            multPlacesGroup2.Text = "Places " + (bias + 2).ToString();
            multPlacesGroup3.Text = "Places " + (bias + 3).ToString();
            multPlacesGroup4.Text = "Places " + (bias + 4).ToString();
            multPlacesGroup5.Text = "Places " + (bias + 5).ToString();
            multPlacesGroup6.Text = "Places " + (bias + 6).ToString();
            multPlacesGroup7.Text = "Places " + (bias + 7).ToString();
            multPlacesGroup8.Text = "Places " + (bias + 8).ToString();
            multProductGroup1.Text = "Product " + (bias + 1).ToString();
            multProductGroup2.Text = "Product " + (bias + 2).ToString();
            multProductGroup3.Text = "Product " + (bias + 3).ToString();
            multProductGroup4.Text = "Product " + (bias + 4).ToString();
            multProductGroup5.Text = "Product " + (bias + 5).ToString();
            multProductGroup6.Text = "Product " + (bias + 6).ToString();
            multProductGroup7.Text = "Product " + (bias + 7).ToString();
            multProductGroup8.Text = "Product " + (bias + 8).ToString();
            multIO1.Text = "IO " + (bias + 1).ToString();
            multIO2.Text = "IO " + (bias + 2).ToString();
            multIO3.Text = "IO " + (bias + 3).ToString();
            multIO4.Text = "IO " + (bias + 4).ToString();
            multIO5.Text = "IO " + (bias + 5).ToString();
            multIO6.Text = "IO " + (bias + 6).ToString();
            multIO7.Text = "IO " + (bias + 7).ToString();
            multIO8.Text = "IO " + (bias + 8).ToString();
            multR1.Visible = (multiplierPanelSelect.SelectedIndex == 0) ? true : false;
            multR2.Visible = (multiplierPanelSelect.SelectedIndex == 0) ? true : false;
            multR3.Visible = (multiplierPanelSelect.SelectedIndex == 0) ? true : false;
            multR4.Visible = (multiplierPanelSelect.SelectedIndex == 0) ? true : false;
            multR5.Visible = (multiplierPanelSelect.SelectedIndex == 0) ? true : false;
            multD1.Visible = (multiplierPanelSelect.SelectedIndex == 0) ? true : false;
            multD2.Visible = (multiplierPanelSelect.SelectedIndex == 0) ? true : false;
            multD3.Visible = (multiplierPanelSelect.SelectedIndex == 0) ? true : false;
            multD4.Visible = (multiplierPanelSelect.SelectedIndex == 0) ? true : false;
            multD5.Visible = (multiplierPanelSelect.SelectedIndex == 0) ? true : false;
            multA.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            multS.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            multAS.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            multAC.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            multSC.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            multASC.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            multRS.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            multDS.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            multF.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            lhpp1Group.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            lhpp2Group.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            rhpp1Group.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
            rhpp2Group.Visible = (multiplierPanelSelect.SelectedIndex == 2) ? true : false;
        }

        private void multiplierPowerOff_CheckedChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setPowered(multiplierPowerOn.Checked);
        }

        private void multIcandOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            int bias;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            bias = multiplierPanelSelect.SelectedIndex * 8;
            computer.getMultiplier().setIcandOperationSwitch(tag + bias, item);
        }

        private void multIcandClearC_CheckedChanged(object sender, EventArgs e)
        {
            Boolean item;
            int tag;
            int bias;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            item = ((RadioButton)sender).Checked;
            bias = multiplierPanelSelect.SelectedIndex * 8;
            computer.getMultiplier().setIcandClearSwitch(tag + bias, item);

        }

        private void multFigures_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            int bias;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            bias = multiplierPanelSelect.SelectedIndex * 8;
            computer.getMultiplier().setSignificantFigures(tag + bias, item);
        }

        private void multPlaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            int bias;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            bias = multiplierPanelSelect.SelectedIndex * 8;
            computer.getMultiplier().setPlaces(tag + bias, item);
        }

        private void multProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            int bias;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            bias = multiplierPanelSelect.SelectedIndex * 8;
            computer.getMultiplier().setProduct(tag + bias, item);
        }

        private void multRAlpha_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getMultiplier().setRProgramOut(tag, item);
        }

        private void multDAlpha_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getMultiplier().setDProgramOut(tag, item);
        }

        private void multAOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setOutputA(multAOut.Text);
        }

        private void multSOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setOutputS(multSOut.Text);
        }

        private void multASOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setOutputAS(multASOut.Text);
        }

        private void multACOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setOutputAC(multACOut.Text);
        }

        private void multSCOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setOutputSC(multSCOut.Text);
        }

        private void multASCOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setOutputASC(multASCOut.Text);
        }

        private void multRSOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setOutputRS(multRSOut.Text);
        }

        private void multDSOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setOutputDS(multDSOut.Text);
        }

        private void multFOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setOutputF(multFOut.Text);
        }

        private void multProgIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            int bias;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            bias = multiplierPanelSelect.SelectedIndex * 8;
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getMultiplier().setProgramIn(tag+bias, item);
        }

        private void multProgOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            int bias;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            bias = multiplierPanelSelect.SelectedIndex * 8;
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getMultiplier().setProgramOut(tag + bias, item);
        }

        private void lhpp1_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setLhpp1Out(lhpp1.Text);
        }

        private void lhpp2_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setLhpp2Out(lhpp2.Text);
        }

        private void rhpp1_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setRhpp1Out(rhpp1.Text);
        }

        private void rhpp2_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getMultiplier().setRhpp2Out(rhpp2.Text);
        }

        private void dividerPowerOff_CheckedChanged(object sender, EventArgs e)
        {
            computer.getDivider().setPowered(dividerPowerOn.Checked);
        }

        private void divNumC_CheckedChanged(object sender, EventArgs e)
        {
            Boolean item;
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            item = ((RadioButton)sender).Checked;
            computer.getDivider().setNumeratorClear(tag, item);
        }

        private void divNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            computer.getDivider().setNumeratorReceive(tag, item);
        }

        private void divDenC_CheckedChanged(object sender, EventArgs e)
        {
            Boolean item;
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            item = ((RadioButton)sender).Checked;
            computer.getDivider().setDenominatorClear(tag, item);
        }

        private void divDen_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            computer.getDivider().setDenominatorReceive(tag, item);
        }

        private void divRoundC_CheckedChanged(object sender, EventArgs e)
        {
            Boolean item;
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            item = ((RadioButton)sender).Checked;
            computer.getDivider().setRoundoff(tag, item);
        }

        private void divPlaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            computer.getDivider().setPlaces(tag, item);
        }

        private void divInterlock_CheckedChanged(object sender, EventArgs e)
        {
            Boolean item;
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            item = ((RadioButton)sender).Checked;
            computer.getDivider().setInterlockSwitch(tag, item);
        }

        private void divAnswer_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            computer.getDivider().setAnswerSwitch(tag, item);
        }

        private void divIlockIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getDivider().setInterlockIn(tag, item);
        }

        private void divProgramIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getDivider().setProgramIn(tag, item);
        }

        private void divProgramOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            computer.getDivider().setProgramOut(tag, item);
        }

        private void divDigitOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            computer.getDivider().setDigitOut(divDigitOut.Text);
        }

        private void functionTableSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateCurrentTab();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            EniacFunctionTable func;
            func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            func.setPowered(funcPowerOn.Checked);
        }

        private void funcClear_CheckedChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            item = 0;
            switch (tag)
            {
                case 0: if (funcClear01.Checked) item = 0;
                    else if (funcClearNc1.Checked) item = 1;
                    else if (funcClearC1.Checked) item = 2;
                    break;
                case 1: if (funcClear02.Checked) item = 0;
                    else if (funcClearNc2.Checked) item = 1;
                    else if (funcClearC2.Checked) item = 2;
                    break;
                case 2: if (funcClear03.Checked) item = 0;
                    else if (funcClearNc3.Checked) item = 1;
                    else if (funcClearC3.Checked) item = 2;
                    break;
                case 3: if (funcClear04.Checked) item = 0;
                    else if (funcClearNc4.Checked) item = 1;
                    else if (funcClearC4.Checked) item = 2;
                    break;
                case 4: if (funcClear05.Checked) item = 0;
                    else if (funcClearNc5.Checked) item = 1;
                    else if (funcClearC5.Checked) item = 2;
                    break;
                case 5: if (funcClear06.Checked) item = 0;
                    else if (funcClearNc6.Checked) item = 1;
                    else if (funcClearC6.Checked) item = 2;
                    break;
                case 6: if (funcClear07.Checked) item = 0;
                    else if (funcClearNc7.Checked) item = 1;
                    else if (funcClearC7.Checked) item = 2;
                    break;
                case 7: if (funcClear08.Checked) item = 0;
                    else if (funcClearNc8.Checked) item = 1;
                    else if (funcClearC8.Checked) item = 2;
                    break;
                case 8: if (funcClear09.Checked) item = 0;
                    else if (funcClearNc9.Checked) item = 1;
                    else if (funcClearC9.Checked) item = 2;
                    break;
                case 9: if (funcClear010.Checked) item = 0;
                    else if (funcClearNc10.Checked) item = 1;
                    else if (funcClearC10.Checked) item = 2;
                    break;
                case 10: if (funcClear011.Checked) item = 0;
                    else if (funcClearNc11.Checked) item = 1;
                    else if (funcClearC11.Checked) item = 2;
                    break;
            }
            computer.getFunctionTable(functionTableSelect.SelectedIndex).setClearMode(tag, item);
        }

        private void funcMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            computer.getFunctionTable(functionTableSelect.SelectedIndex).setOperation(tag, item);
        }

        private void funcRepeat_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            computer.getFunctionTable(functionTableSelect.SelectedIndex).setRepeat(tag, item);
        }

        private void funcProgramIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            EniacFunctionTable func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            if (func != null)
            {
                item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
                func.setProgramIn(Convert.ToInt32(((ComboBox)sender).Tag), item);
            }
        }

        private void funcProgramOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            EniacFunctionTable func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            if (func != null)
            {
                item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
                func.setProgramOut(Convert.ToInt32(((ComboBox)sender).Tag), item);
            }
        }

        private void funcOutNC_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            EniacFunctionTable func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            func.setOutputNC(item);
        }

        private void funcOutC_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            EniacFunctionTable func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            func.setOutputC(item);
        }

        private void funcArgIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            EniacFunctionTable func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            func.setArgumentIn(item);
        }

        private void funcOutputA_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            EniacFunctionTable func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            func.setOutputA(item);
        }

        private void funcOutputB_SelectedIndexChanged(object sender, EventArgs e)
        {
            String item;
            EniacFunctionTable func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            item = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
            func.setOutputB(item);
        }

        private void funcP1_CheckedChanged(object sender, EventArgs e)
        {
            int item;
            item = 0;
            if (funcP1.Checked) item = 0;
            else if (funcM1.Checked) item = 1;
            else if (funcTable1.Checked) item = 2;
            computer.getFunctionTable(functionTableSelect.SelectedIndex).setMasterSign(0, item);
        }

        private void funcP2_CheckedChanged(object sender, EventArgs e)
        {
            int item;
            item = 0;
            if (funcP2.Checked) item = 0;
            else if (funcM2.Checked) item = 1;
            else if (funcTable2.Checked) item = 2;
            computer.getFunctionTable(functionTableSelect.SelectedIndex).setMasterSign(1, item);
        }

        private void funcAD_CheckedChanged(object sender, EventArgs e)
        {
            Boolean item;
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            item = ((RadioButton)sender).Checked;
            computer.getFunctionTable(functionTableSelect.SelectedIndex).setADeleteSwitch(tag, item);
        }

        private void funcAValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            computer.getFunctionTable(functionTableSelect.SelectedIndex).setConstantA(tag, item);
        }

        private void funcBD_CheckedChanged(object sender, EventArgs e)
        {
            Boolean item;
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            item = ((RadioButton)sender).Checked;
            computer.getFunctionTable(functionTableSelect.SelectedIndex).setBDeleteSwitch(tag, item);
        }

        private void funcBValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            item = ((ComboBox)sender).SelectedIndex;
            computer.getFunctionTable(functionTableSelect.SelectedIndex).setConstantB(tag, item);
        }

        private void funcSubA_CheckedChanged(object sender, EventArgs e)
        {
            Boolean item;
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            item = ((RadioButton)sender).Checked;
            computer.getFunctionTable(functionTableSelect.SelectedIndex).setASubSwitch(tag, item);
        }

        private void funcSubB_CheckedChanged(object sender, EventArgs e)
        {
            Boolean item;
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            item = ((RadioButton)sender).Checked;
            computer.getFunctionTable(functionTableSelect.SelectedIndex).setBSubSwitch(tag, item);
        }

        private void portableDigit_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            int row;
            int col;
            String value;
            int pos;
            EniacFunctionTable func;
            pos = portableScrollbar.Value - 3;
            if (pos < -2) pos = -2;
            if (pos > 82) pos = 82;
            func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            if (tag < 100) return;
            row = (tag / 100)-1;
            col = tag % 100;
            item = ((ComboBox)sender).SelectedIndex;
            if (item < 0) return;
            item += '0';
            value = func.getValue(pos+row);
            value = value.Substring(0, col) + Convert.ToChar(item).ToString() + value.Substring(col + 1);
            func.setValue(pos+row, value);
        }

        private void portableSign_SelectedIndexChanged(object sender, EventArgs e)
        {
            int item;
            int tag;
            int row;
            int col;
            String value;
            char c;
            int pos;
            EniacFunctionTable func;
            pos = portableScrollbar.Value - 3;
            if (pos < -2) pos = -2;
            if (pos > 82) pos = 82;
            func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            tag = Convert.ToInt32(((ComboBox)sender).Tag);
            row = (tag / 100)-1;
            col = tag % 100;
            item = ((ComboBox)sender).SelectedIndex;
            if (item < 0) return;
            c = (item == 0) ? '+' : '-';
            value = func.getValue(pos+row);
            if (col == 0) value = c.ToString() + value.Substring(1);
            if (col == 13) value = value.Substring(0, 13) + c.ToString();
            func.setValue(pos+row, value);
        }

        private void updateRow(String value,ComboBox ls, ComboBox d1, ComboBox d2, ComboBox d3, ComboBox d4, ComboBox d5, ComboBox d6,
                               ComboBox d7, ComboBox d8, ComboBox d9, ComboBox d10, ComboBox d11, ComboBox d12, ComboBox rs)
        {
            ls.SelectedIndex = (value[0] == '+') ? 0 : 1;
            rs.SelectedIndex = (value[13] == '+') ? 0 : 1;
            d1.SelectedIndex = value[1] - '0';
            d2.SelectedIndex = value[2] - '0';
            d3.SelectedIndex = value[3] - '0';
            d4.SelectedIndex = value[4] - '0';
            d5.SelectedIndex = value[5] - '0';
            d6.SelectedIndex = value[6] - '0';
            d7.SelectedIndex = value[7] - '0';
            d8.SelectedIndex = value[8] - '0';
            d9.SelectedIndex = value[9] - '0';
            d10.SelectedIndex = value[10] - '0';
            d11.SelectedIndex = value[11] - '0';
            d12.SelectedIndex = value[12] - '0';
        }

        private void updatePortableTable()
        {
            int pos;
            EniacFunctionTable func;
            String value;
            func = computer.getFunctionTable(functionTableSelect.SelectedIndex);
            pos = portableScrollbar.Value - 3;
            if (pos < -2) pos = -2;
            if (pos > 82) pos = 82;
            row1.Text = pos.ToString();
            row2.Text = (pos+1).ToString();
            row3.Text = (pos+2).ToString();
            row4.Text = (pos+3).ToString();
            row5.Text = (pos+4).ToString();
            row6.Text = (pos+5).ToString();
            row7.Text = (pos+6).ToString();
            row8.Text = (pos+7).ToString();
            row9.Text = (pos+8).ToString();
            row10.Text = (pos+9).ToString();
            row11.Text = (pos+10).ToString();
            row12.Text = (pos + 11).ToString();
            row13.Text = (pos + 12).ToString();
            row14.Text = (pos + 13).ToString();
            row15.Text = (pos + 14).ToString();
            row16.Text = (pos + 15).ToString();
            row17.Text = (pos + 16).ToString();
            row18.Text = (pos + 17).ToString();
            row19.Text = (pos + 18).ToString();
            row20.Text = (pos +19).ToString();
            value = func.getValue(pos + 0);
            updateRow(value, portPmL1, portR1C1, portR1C2, portR1C3, portR1C4, portR1C5, portR1C6, portR1C7, portR1C8, portR1C9, portR1C10, portR1C11, portR1C12, portPmR1);
            value = func.getValue(pos + 1);
            updateRow(value, portPmL2, portR2C1, portR2C2, portR2C3, portR2C4, portR2C5, portR2C6, portR2C7, portR2C8, portR2C9, portR2C10, portR2C11, portR2C12, portPmR2);
            value = func.getValue(pos + 2);
            updateRow(value, portPmL3, portR3C1, portR3C2, portR3C3, portR3C4, portR3C5, portR3C6, portR3C7, portR3C8, portR3C9, portR3C10, portR3C11, portR3C12, portPmR3);
            value = func.getValue(pos + 3);
            updateRow(value, portPmL4, portR4C1, portR4C2, portR4C3, portR4C4, portR4C5, portR4C6, portR4C7, portR4C8, portR4C9, portR4C10, portR4C11, portR4C12, portPmR4);
            value = func.getValue(pos + 4);
            updateRow(value, portPmL5, portR5C1, portR5C2, portR5C3, portR5C4, portR5C5, portR5C6, portR5C7, portR5C8, portR5C9, portR5C10, portR5C11, portR5C12, portPmR5);
            value = func.getValue(pos + 5);
            updateRow(value, portPmL6, portR6C1, portR6C2, portR6C3, portR6C4, portR6C5, portR6C6, portR6C7, portR6C8, portR6C9, portR6C10, portR6C11, portR6C12, portPmR6);
            value = func.getValue(pos + 6);
            updateRow(value, portPmL7, portR7C1, portR7C2, portR7C3, portR7C4, portR7C5, portR7C6, portR7C7, portR7C8, portR7C9, portR7C10, portR7C11, portR7C12, portPmR7);
            value = func.getValue(pos + 7);
            updateRow(value, portPmL8, portR8C1, portR8C2, portR8C3, portR8C4, portR8C5, portR8C6, portR8C7, portR8C8, portR8C9, portR8C10, portR8C11, portR8C12, portPmR8);
            value = func.getValue(pos + 8);
            updateRow(value, portPmL9, portR9C1, portR9C2, portR9C3, portR9C4, portR9C5, portR9C6, portR9C7, portR9C8, portR9C9, portR9C10, portR9C11, portR9C12, portPmR9);
            value = func.getValue(pos + 9);
            updateRow(value, portPmL10, portR10C1, portR10C2, portR10C3, portR10C4, portR10C5, portR10C6, portR10C7, portR10C8, portR10C9, portR10C10, portR10C11, portR10C12, portPmR10);
            value = func.getValue(pos + 10);
            updateRow(value, portPmL11, portR11C1, portR11C2, portR11C3, portR11C4, portR11C5, portR11C6, portR11C7, portR11C8, portR11C9, portR11C10, portR11C11, portR11C12, portPmR11);
            value = func.getValue(pos + 11);
            updateRow(value, portPmL12, portR12C1, portR12C2, portR12C3, portR12C4, portR12C5, portR12C6, portR12C7, portR12C8, portR12C9, portR12C10, portR12C11, portR12C12, portPmR12);
            value = func.getValue(pos + 12);
            updateRow(value, portPmL13, portR13C1, portR13C2, portR13C3, portR13C4, portR13C5, portR13C6, portR13C7, portR13C8, portR13C9, portR13C10, portR13C11, portR13C12, portPmR13);
            value = func.getValue(pos + 13);
            updateRow(value, portPmL14, portR14C1, portR14C2, portR14C3, portR14C4, portR14C5, portR14C6, portR14C7, portR14C8, portR14C9, portR14C10, portR14C11, portR14C12, portPmR14);
            value = func.getValue(pos + 14);
            updateRow(value, portPmL15, portR15C1, portR15C2, portR15C3, portR15C4, portR15C5, portR15C6, portR15C7, portR15C8, portR15C9, portR15C10, portR15C11, portR15C12, portPmR15);
            value = func.getValue(pos + 15);
            updateRow(value, portPmL16, portR16C1, portR16C2, portR16C3, portR16C4, portR16C5, portR16C6, portR16C7, portR16C8, portR16C9, portR16C10, portR16C11, portR16C12, portPmR16);
            value = func.getValue(pos + 16);
            updateRow(value, portPmL17, portR17C1, portR17C2, portR17C3, portR17C4, portR17C5, portR17C6, portR17C7, portR17C8, portR17C9, portR17C10, portR17C11, portR17C12, portPmR17);
            value = func.getValue(pos + 17);
            updateRow(value, portPmL18, portR18C1, portR18C2, portR18C3, portR18C4, portR18C5, portR18C6, portR18C7, portR18C8, portR18C9, portR18C10, portR18C11, portR18C12, portPmR18);
            value = func.getValue(pos + 18);
            updateRow(value, portPmL19, portR19C1, portR19C2, portR19C3, portR19C4, portR19C5, portR19C6, portR19C7, portR19C8, portR19C9, portR19C10, portR19C11, portR19C12, portPmR19);
            value = func.getValue(pos + 19);
            updateRow(value, portPmL20, portR20C1, portR20C2, portR20C3, portR20C4, portR20C5, portR20C6, portR20C7, portR20C8, portR20C9, portR20C10, portR20C11, portR20C12, portPmR20);
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            updatePortableTable();
        }

        private void quotientIC_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacDivider div;
            div = computer.getDivider();
            div.setQuotientIC(quotientIC.SelectedIndex);
        }

        private void numeratorIC_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacDivider div;
            div = computer.getDivider();
            div.setNumeratorIC(numeratorIC.SelectedIndex);
        }

        private void denominatorIC_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacDivider div;
            div = computer.getDivider();
            div.setDenominatorIC(denominatorIC.SelectedIndex);
        }

        private void shifterIC_SelectedIndexChanged(object sender, EventArgs e)
        {
            EniacDivider div;
            div = computer.getDivider();
            div.setShifterIC(shifterIC.SelectedIndex);
        }

        private void readerStart_Click(object sender, EventArgs e)
        {
            computer.getReader().start();
        }

        private void filterSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            showSelectedFilter();
        }

        private void filterSaveButton_Click(object sender, EventArgs e)
        {
            int m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, m11;
            m1 = filterPin1.SelectedIndex - 1;
            m2 = filterPin2.SelectedIndex - 1;
            m3 = filterPin3.SelectedIndex - 1;
            m4 = filterPin4.SelectedIndex - 1;
            m5 = filterPin5.SelectedIndex - 1;
            m6 = filterPin6.SelectedIndex - 1;
            m7 = filterPin7.SelectedIndex - 1;
            m8 = filterPin8.SelectedIndex - 1;
            m9 = filterPin9.SelectedIndex - 1;
            m10 = filterPin10.SelectedIndex - 1;
            m11 = filterPin11.SelectedIndex - 1;
            if (filterName.Text.Length > 0)
            {
                computer.addFilter(filterName.Text, m1, m2, m3, m4, m5, m6, m7, m8, m9, m10, m11);
            }

        }

        private void coupler_CheckedChanged(object sender, EventArgs e)
        {
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            computer.getPunch().setCouplerSwitch(tag,((((RadioButton)sender).Checked ) ? false : true));
        }

        private void print_CheckedChanged(object sender, EventArgs e)
        {
            int tag;
            tag = Convert.ToInt32(((RadioButton)sender).Tag);
            computer.getPunch().setPrintSwitch(tag, ((((RadioButton)sender).Checked) ? false : true));
        }

        private void printerPanelSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateCurrentTab();
        }

    }
}
