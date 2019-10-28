                                 ENIAC
             Electronic Numerical Integrator And Computer

  Welcome to the RCS ENIAC simulation.  This simulator attempts to 
simulate the entirety of the ENIAC computer built in the 1940s.

  This simulation currently simulates:

  * - The initiating Unit
  * - The Cycling Unit
  * - The Master Programmer Unit
  * - 20 Accumulators
  * - The High Speed Multiplier Unit
  * - The Divider/Square Rooter Unit
  * - 3 Function Table Units
  * - The Constant Transmitter Unit
  * - The Card Reader
  * - The Card Punch

  Currently all units except the card reader and punch are 100%
simulated.  The card reader and punch are currently simplified
versions of these two units.  The plug boards for the card reader
and punch have not yet been implementd, mainly because I have not
totally figured them out yet.  These will be implemented in a future
version of the simulator.

  The purpose of this document is to describe the controls on each
of the simulated units.  A separate document is included in the 
simulator package that provides a tutorial on how to actually program
the ENIAC.

known Issues:
=============

1.  There are a couple known issues with this simulation, first appears
to be an issue related to the .net framework and involves the loading
of the controls on each of the panels.  Many panels the first time
they are selected can take several seconds to show up.  After a 
panel has been selected once, then all future selections of that 
panel work just fine.

2.  The Divider/Square rooter often has an incorrect value in the 
least significant digit of the answer.  I do not know if this is how
I implemented the ENIAC algorithm or if it is a limitation of the
original ENIAC algorithm.  I am continuing to research this issue
and depending on what I find, this could get fixed in a future version
of the simulator.


The Initiating Unit (Init tab):
===============================
NEW           - The button will reset the simulator to its start up
                condition.  All unit controls will be at default and 
                all cable connections will be removed.    

Load          - This buttons allows you to load a previously saved 
                setup into the simulator.

Save          - This button allows you to save the current setup to a
                file.

Clear         - This button peforms a clear of the machine.  This will
                set all accumulator values to 0, set all units to idle
                and clear any triggers

Start         - This button starts an ENIAC setup running by initiating
                a card read.  The card reader will read the first card
                in the input hopper and send an output pulse.  An 
                interlock pulse is not needed for the reader to output
                a program pulse when using this button to start.

Go            - This button will trigger a program pulse on the next
                CPP from the Cycling unit.  This pulse will be sent
                out the Initial Pulse output.

C0-C5         - These six groups of controls control the selective clear
                of the ENIAC.  The top drop-down sets the program input
                pulse and the bottom drop-down sets the program output
                pulse.  When a pulse is received at the input, a
                signal is sent out the synchronising trunk that will 
                cause all accumulator with their selective clear switch
                set to "C" to clear their values to zero.  The following
                addition time, CPP will be routed through the program 
                output port.

Reader IO     - This group of controls provides the program input (top)
                and program output (bottom) ports for the card reader.
                When a pulse is received at the input the card reader
                will read one card into the Constant Transmitter.  When
                this process is complete CPP will be routed to the 
                program out port.  Note, that the reader will not 
                provide a program out pulse without having first 
                received a pulse on the interlock port.

Reader IL     - This control is for connecting the reader interlock to
                a program tray.  The reader will not output a program
                pulse without first having received a program pulse
                on this port.

Punch IO      - This group provides the program input (top) and program
                output (bottom) for the card punch.  When a pulse is
                received on this port the card punch will punch one
                card.  Once complete the punch will output a program
                pulse from the bottom port.

Init.Pulse    - This control sets which program tray and position the
                Go button will send a CPP when pressed


The Cycling Unit (Cycle tab):
=============================
Step          - When the Cycling unit is set to any mode other than
                continuous, this button allows you to initiate the next
                pulse/addition time.

Mode          - This control specifies how the Cyclying unit emits its
                pulses.  Normal operation is "Continuous" which means
                the Cycyling unit will continually send out pulses.
                When set to "Single Add." the Cycling unit will emit
                pulses for one complete addition time.  When set to 
                "Single Pulse" the Cycling unit will only emit the 
                pulses for a single pulse time.  There are 20 pulse
                times per addition time.


The Master Programmer (Master tab):
===================================
  Note that the Master Programmer consists of two panels.  Panel 1 has
all the settings for steppers A through E and decades 20-11.  Panel 2
has all the settings for steppers F through K and decades 10-1.

Decade Disp   - The 10 decades associated with a panel can be viewed
                in the digit boxes on the top left of the panel.

Stepper stage - The stage of each of the 5 steppers associated with the
                current panel can be viewed in the 5 digit boxes on the
                upper right of the panel.

A/B           - Not all decades are hard assigned to steppers.  Four
B/C             decades on each panel can be assigned to two different
C/D             steppers.  The A/B, etc. switches select wich stepper
D/E             the non-fixed decade is assigned to.

Decade limits - There are 6 rows of 10 numerical controls on each panel
                which are used to set next-stage threshold for the
                steppers.

Stepper Clear - These switches set the stage at which each of the 
                steppers reset back to stage 1.

Decade Direct - When a program pulse is detected on the program tray
                these are connected to, the associated decade will be
                increased by 1.

Steppers      - The steppers themselves consist of the large boxes at
                the bottom of the panel labeled A-E (on panel 1) or
                F through K (on panel 2).  These contain the inputs
                and outputs for each of the 10 steppers.
       DI     - This input is the direct input.  Any time a program
                pulse is received on this terminal the stage of the 
                associated stepper will be increased by 1.
       I      - This is the stepper program input.  When a program
                pulse is received on this terminal the stepper will
                output a program pulse on one of the O1 through O6
                program outputs, depending on the current stage of the
                stepper.
       C      - A program pulse received on this input will immediately
                reset the stepper back to stage 1.
       O1-O6  - These are the stepper program outputs.  A program pulse
                is sent out the output associated with the current 
                stage of the stepper whenever a program pulse is 
                received by the stepper Input.


The Accumulators (Acc tab):
===========================
Accumulator   - This lets you select which of ENIAC's 20 accumulators 
                you want to see/alter the settings for.

decade disp   - The value currently stored in the accumulator will be
                shown in these digit displays.

alpha, beta,  - The five groups contain the digit input ports.  The top
gamma, delta,   drop-down in each group selects which digit tray the
epsilon         port is connected to.  The bottom drop-down selects a
                filter to be applied to the input before it reaches the
                input port.

A, S          - These two controls are the two digit output ports for
                the accumulator.  Any value sent out the A port will be
                sent as is, as if addition was being performed.  Any
                value sent out the S port is complimented, as if a
                subtraction operation was being performed.

Sig Digits    - This group consists of two controls, first the Selective
                Clear switch which can be set at either "C" or "0".  
                When this switch is set at "C", then if any of the Clear
                programs are activated on the Initiating Unit this 
                accumulator will clear to zero.
                The second control in this group is the significant 
                figures switch.  This switch specifies in which decade
                position the 1'P pulse is transmitted on when values
                are transmitted subtractively (out the S port).  This
                switch also affects round-off when the accumulator is
                cleared.  If this switch is set at anything but 10, 
                5 will be placed into the decade to the right of the
                number selected on this switch.

Left          - This is the left side interconnect.  When it is desired
                to connect two accumulators together to work with 20
                digit values, this control specifies which other 
                accumulator this one is connected to.  The one this
                connects to will contain the most significant 10 digits
                of the combined accumulators.

Right         - This is the opposite of the "Left" interconnect.  This
                also will connect the accumulator to another for using
                20 digit values but now selects this accumulator to be
                the most significant 10 digits.

1-12          - These are the 12 program setups for the accumulator and
                are used to specify what action is to be performed when
                the associated program is stimulated.
                There are two controls in each program, first the Clear/
                Correct switch which can be set at either "C" or "0".
                When set at "0" this switch will have no impact on the
                functioning of the accumulator.  When set at "C" it
                depends on which mode the program is set for as to what
                function this switch performs.  If the program is set
                for any of the input functions (alpha-epsilon) then when
                this switch is set at "C" the units decade will be 
                incremented by 1 at the end of each addition time where
                this program is active.  When the program is in any of
                the output functions (0, A, AS, S) then when this 
                switch is set at "C" the accumulator will be cleared at
                the conclusion of this program's execution.
                The second control in this group is the mode switch.
                This switch specifies which action is to be performed
                when this program is stimulated.  The five input modes:
                alpha, beta, gamma, delta, epsilon specify that this
                program expects to receive a number from the specified
                digit input port.  The transmit modes are 0, A, AS, and
                S.  If "0" is selected then no value is transmitted,
                The A, AS, and S modes select which digit output ports
                the accumulator will send its value to.


Rep 5-Rep 12  - These are the repeat controls for programs 5 through
                12.  These specify how many addition cycles the given
                program will repeat over.  There are no repeat controls
                for programs 1 through 4.

I1-I4         - These are the program pulse inputs for programs 1 
                through 4.  When a program pulse is received on one of
                these controls the associated program will function
                starting on the next addition cycle.

IO5-IO12      - This group of controls provide the program input (top
                drop-down) and the program output (bottom drop-down)
                for programs 5 through 12.  When a program pulse is
                received by an input the associated program will be
                triggered to run on the next addition cycle.  When
                a program has completed it will send a program pulse
                out the associated program output port.


The High Speed Multiplier (Multiplier tab):
===========================================
  Note that the controls for the High Speed Multiplier are spread across
3 panels.  The Multiplier contains 24 programs split 8 per panel.

Ier           - This group of controls provides options for the 
                multiplier accumulator (#9).  It consists of two 
                controls, the Multiplier Clear switch which can be set
                at either "C" or "0".  When this switch is set to "C"
                then the multiplier accumulator will be cleared at the
                end of the multiplication program.  The second control
                is the source switch which can be set at alpha, beta,
                gamma, delta, epsilon, or 0.  This specifies which of
                the Ralpha-REpsilon program outputs will be pulsed in
                order to stimulate the multiplier accumulator to 
                receive its argument.

Icand         - This group of controls provides options for the 
                multiplicand accumulator (#10).  It consists of two 
                controls, the Multiplicand Clear switch which can be
                set at either "C" or "0".  When this switch is set to
                "C" then the multiplicand accumulator will be cleared
                at the end of the multiplication program.  The second
                control is the source switch which can be set at alpha,
                beta, gamma, delta, epsilon, or 0.  This specifies which
                of the Dalpha-DEpsilon program outputs will be pulsed in
                order to stimulate the multiplicand accumulator to 
                receive its argument.

Figures       - This switch controls round-off at the end of the 
                multiplication program.  When this switch is set to
                "Off" then no round-off will occur in the answer.  If
                this switch is set to any of the number positions then
                round-off will occur one digit to the right of the 
                number specified by this switch.

Places        - This control specifies how many digits of the multiplier
                are used to compute the answer.  Digits are counted from
                the left in the multiplier accumulator.

Product       - This switch controls product disposal when the program
                completes.  It can bet set at A, S, AS, 0, AC, SC, or 
                ASC.  If set at "0" then no product disposal pulse will
                be generated.  All of the other settings will send a 
                pulse out the specified program output port.

Ralpha -      - The program output ports are used to stimulate argument
Repsilon        reception in the multiplier accumulator.  These program
                outputs need to be connected to properly setup programs
                in the multiplier accumulator in order to function
                properly.

Dalpha -      - The program output ports are used to stimulate argument
Depsilon        reception in the multiplicand accumulator.  These
                program outputs need to be connected to properly setup
                programs in the multiplicand accumulator in order to
                function properly.

IO            - These groups provied the program inputs (top drop-down)
                and program outputs (bottom drop-down) for each of the
                24 multiplier programs.  When a pulse is received on
                a program input, the associated program will be 
                stimulated into functioning.  When the current program
                finishes its function it will pulse the appropriate
                program output.

A,S,AS,AC,    - These program outputs are used to stimulate the product
SC,ASC          register (#13) to transmit its value.  These must be
                connected to appropriately setup programs on the product
                accumulator in order to function properly.

RS            - This program output is used for sign correction when
                either or both of the arguments are negative.  The RS
                output is connected to the multiplier accumulator on a
                program setup to send the value subtractively.  It is
                also connected to the LHPP I accumulator (#11) on a 
                program set to receive.

DS            - This program output is used for sign correction when
                either or both of the arguments are negative.  The DS
                output is connected to the multiplicand accumulator on
                a program setup to send the value subtractively.  It is
                also connected to the RHPP I accumulator (#13) on a 
                program set to receive.

F             - This program is pulsed in order to combine the two 
                partial products into the final product.  It is 
                normally connected to the LHPP register on a program
                control setup to transmit additively as well as to
                the RHPP register on a program setup to receive.

LHPP I        - This is a digit output port that transmits the left
                partial product during each step of multiplication.
                It is normally connected to a digit input port on
                LHPP I accumulator (#11).

LHPP II       - This is a digit output port that transmits the low
                10 digits of the left partial product.  It is normally
                connected to the LHPP II accumulator (#12).  This
                connection is only necessary when you need an 
                answer greater than 10 digits.

RHPP I        - This is a digit output port that transmits the right
                partial product during multiplication.  It is normally
                connected to the RHPP I accumulator (#13).

RHPP II       - This is a digit output port that transmits the low
                10 digits of the right partial product.  It is 
                normally connected to the RHPP II accumulator (#14).
                This connection is only necessary when you need an
                answer greater than 10 digits.


The Divider/Square Rooter Unit (Divider tab):
=============================================
Digit         - This digit output transmits numerical information 
                generated during operation.  It is normally connected
                to the alpha channel of the quotient accumulator (#2)
                and the gamma channel of the denominator accumulator
                (#5).

Quot IC,      - These terminals interconnect the various accumulators
Num IC,         the divider unit needs to control during operation.
Den IC,
Shift Ic

Num           - This group of controls specify options for the numerator
                accumulator (#3).  It consits of a clear switch that can
                be set at either "C" or "0".  If the clear switch is set
                to "C" then the numerator accumulator will be cleared
                at the end of the program.
                The second control is the receive mode switch and is 
                used to stimulate the numertor accumulator to receive
                its argument through either the alpha or beta channel.
                If this switch is set to "0" then the numerator 
                accumulator will not be stimulated to receive an
                argument.

Den           - This group of controls specify options for the
                denominator accumulator (#5).  It consits of a clear
                switch that can be set at either "C" or "0".  If the
                clear switch is set to "C" then the denominator
                accumulator will be cleared at the end of the program.
                The second control is the receive mode switch and is 
                used to stimulate the denominator accumulator to receive
                its argument through either the alpha or beta channel.
                If this switch is set to "0" then the denominator 
                accumulator will not be stimulated to receive an
                argument.

Places        - This group contains the Round-off Switch and the Mode/
                Places switch.  The Round-off switch can be set to 
                either "R" or "0".  If this switch is set to "R" then
                round-off will occur at the end of the program.
                The mode switch selects whether this program will 
                perform division (D options) or square-rooting (R
                options) as well as how many places to compute the 
                answer to.

Ans           - This group contains two switches, the Interlock switch
                and the answer disposal switch.  The Interlock switch
                can be set at either "I" or "0".  If this switch is
                set to "I" then a pulse must be received on an Ilock
                program input before this program is capable of sending
                a pulse out its program output.
                The answer disposal switch specifies which accumulator
                will transmit its answer and how.  The options are:
                   1: Quotient accumulator A
                   2: Quotient accumulator S
                   3: Denominator accumulator A
                   4: Denominator accumulator S
                   OFF: No answer will be transmitted

Ilock         - This program input accepts interlock pulses when the
                current program is set to require an interlock pulse
                before an output pulse can be generated.  It is 
                important to note that it does not matter which Ilock
                port a pulse comes in, the interlock will be released,
                the interlock pulse does not need to come in on the 
                Ilock port associated with the running program.

IO            - This group contains the program input (top drop-down)
                and program output (bottom drop-down) for each of the
                8 programs of this unit.  When a pulse is received
                on a program input, the associated program will begin
                to run.  When a program completes and an interlock
                pulse has been received, if required, then a program
                pulse will be sent out the program output port for the
                associated program.


Function Table (Function 1 & Function 2 tabs):
==============================================
  ENIAC contains 3 function tables, each of which has two panels and a
portable function table.

Table Select  - This control allows you to select which of ENIAC's 
                three function tables you want to work with.

Arg IN        - This is a digit input port that allows the function 
                table to retrieve the argument value.

1-11          - These groups are the 11 program controls.  They contain
                two controls, the Receive Mode Switch and the Function
                Mode Switch.  The Receive Mode Switch can be set at
                "0", "NC", or "C".  If this switch is set at "0" then
                no argument pulse will be generatated.  If this switch
                set at either NC or C then a program pulse to stimulate
                argument transmission will be sent out the
                corresponding NC or C program output port.
                The Function Mode Switch selects whether the function
                value will be returned additively or subtractively.
                This switch also selects on offset between -2 and +2
                for which function value is to be returned.

Rep 1-Rep 11  - These are the repeat controls for the programs.  The
                function table will transmit the result the number of
                times specified by this switch.

NC/C          - These program output ports are used to stimulate the
                transmission of the argument.  A pulse will be sent
                out one of these ports depending upon the setting of
                the Receive Mode Switch for the program.

IO 1-IO 11    - This group of controls provide the program inputs 
                (top drop-down) and program outputs (bottom drop-down)
                for each of the 11 programs.  A pulse received at one
                of the input ports will trigger that program to run.
                Once a program has completed it will send a pulse out
                the corresponding program output port.

PM 1,PM 2     - These switches provide sign selection.  PM 1 is 
                associated with output port A while PM 2 is 
                associated with output port B.  Each switch has three
                settings: P, M, Table.  When the switch is set at P 
                then the sign for all function values will be positive.
                When the switch is set at M then all function values 
                will be returned as negative.  When the switch is set
                at Table then the sign is dependent upon the sign switch
                on the portable function table for the specified value.

A             - This is the left side digit output port.  The left side
                of the function value will be transmitted on this port.

B             - This is the right side digit output port.  The right
                side of the function value will be transmitted on this
                port.

A1-A4         - This group provides for leading constant select for the
                left side of the function result.  It contains a 
                delete switch and a digit select.  The Delete switch 
                can be set at either "D" or "0".  If the switch is set
                at "D" then this constant value will not be sent, when
                set at "0" then this constant value is sent along with
                the function result.
                The digit select is used to set the constant for this
                position.

B1-B4         - This group provides for leading constant select for the
                right side of the function result.  It contains a 
                delete switch and a digit select.  The Delete switch 
                can be set at either "D" or "0".  If the switch is set
                at "D" then this constant value will not be sent, when
                set at "0" then this constant value is sent along with
                the function result.
                The digit select is used to set the constant for this
                position.


Sub A5-Sub A10- This group of switches selects in which position the
                1'P pulse is sent on when sending subtractively.  This
                sets the position for the value being transmitted on the
                A digit output port.  Note that it is possible to send
                the 1'P on multiple positions.

Sub B5-Sub B10- This group of switches selects in which position the
                1'P pulse is sent on when sending subtractively.  This
                sets the position for the value being transmitted on the
                B digit output port.  Note that it is possible to send
                the 1'P on multiple positions.


Portable Function Table (PortFunc tab):
=======================================
  The portable function table provides the lookup table for the 
function table it is attached to.  It contains 104 entries numbered 
from -2 to +101

Left Sign     - The Left sign is the far left switch.  This specifies
                the sign for the left side function value, that which
                is transmitted from the A output port.

Left Value    - The next 6 switches provide the left side value.  These
                combined with the A1-A4 controls on panel 2 of the
                function table provide the left side 10 digit function
                value that is transmitted out the A port.

Right Value   - The next 6 switches provide the right side value.
                These combined with the B1-B4 controls on panel 2 of
                the function table provide the right side 10 digit
                function value that is transmitted out the B port.

Right Sign    - The last switch in each row is the right side sign.


Constant Transmitter (Const 1 & Const 2 tabs):
==============================================
A             - This is the digit output port that the Constant
                Transmitter will send on.

1-30          - These groups provide the program controls for the 
                Constant Transmitter's 30 programs.  Each program has
                two controls:  the Mode Switch and the Register Select.
                The Mode switch can be set at L, LR, or R and specifies
                which half (or all) of the register is to be sent.
                The Register Select switch allows you to specify which
                of the two registers the program is attached to that
                will be transmitted.

IO1-IO30      - These provide the program inputs (top drop-down) and
                the program output (bottom drop-down) for each of the
                Constant Transmitter's 30 programs.  A pulse received
                at a program input will trigger that program to run on
                the next addition cycle.  When a program completes it
                will pulse the appropriate program output port.

JL            - This switch provides the sign for the left half of
                constant register J.  This sign is also the one used
                if register J is transmitted in LR mode.

JR            - This switch provides the sign for the right half of
                constant register J.

KL            - This switch provides the sign for the left half of
                constant register K.  This sign is also the one used
                if register K is transmitted in LR mode.

KR            - This switch provides the sign for the right half of
                constant register K.

J             - These ten switches allow you to set the 10 digit 
                constant value of register J.

K             - These ten switches allow you to set the 10 digit
                constant value of register K.


Card Reader (Reader tab):
=========================
Input         - This text box shows all cards currently in the card
                reader's input hopper.  This box can be edited to add
                cards to the hopper.  Cards are removed from the input
                hopper after they have been read.

Output        - This box will receive the cards from the input hopper
                after the reader has read them.

Restack       - When this button is pressed all the cards currently in
                the ouptut stacker will be added to the end of the list
                of cards in the input hopper.

Remove        - This button when pressed will remove and discad all
                cards in the output stacker.


Card Punch (Punch tab):
=======================
Stacker       - All cards by the punch will appear in the Card Stacker.

Remove Cards  - When this button is pressed all cards in the card
                stacker will be removed.


Sample connections for Divider/Square Rooter:
=============================================
Acc 2 (Quotient)
  alpha       D2-A

Acc 3 (Numerator)
  gamma       D2-B
  A           D2-B

Acc 5 (Denominator)
  gamma       D2-A
  A,S         D2-B

Acc 7 (Shift)
  alpha       D2-B with SHL1
  A           D2-B

Divider
  Digit       D2-A

  These provide the minimum connections for the Divider/Square Rooter
to function.  You would still need to connect the Numerater and 
Denominator accumulators to their input sources on alpha and/or beta
and connect the Quotient and Denominator A and S outputs to other
units to retrieve answers.


Sample connections for High Speed Multiplier:
=============================================
Acc 9 (Multiplier)
  S      D5-A                                    (Req)
  1      clear=0 mode=alpha    in=P5-A:1
  2      clear=0 mode=beta     in=P5-A:2
  3      clear=0 mode=gamma    in=P5-A:3
  4      clear=0 mode=delta    in=P5-A:4
  5      clear=0 mode=epsilon  in=P5-A:5  rep=1
  6      clear=0 mode=S        in=P5-B:1  rep=1  (Req)

Acc 10 (Multiplicand)
  S      D5-B                                    (Req)
  1      clear=0 mode=alpha    in=P5-A:6
  2      clear=0 mode=beta     in=P5-A:7
  3      clear=0 mode=gamma    in=P5-A:8
  4      clear=0 mode=delta    in=P5-A:9
  5      clear=0 mode=epsilon  in=P5-A:10 rep=1
  6      clear=0 mode=S        in=P5-B:2  rep=1  (Req)

Acc 11 (LHPP I)
  alpha  D5-A                                    (Req)
  A      D5-A                                    (Req)
  1      clear=0 mode=alpha    in=P5-B:1         (Req)
  2      clear=0 mode=A        in=P5-B:3         (Req)
  Right  12

Acc 12 (LHPP II)
  alpha  D5-B
  A      D5-B
  Left   11

Acc 13 (RHPP I)
  alpha  D5-C                                    (Req)
  beta   D5-B                                    (Req)
  gamma  D5-A                                    (Req)
  1      clear=0 mode=beta     in=P5-B:2         (Req)
  2      clear=0 mode=gamma    in=P5-B:3         (Req)
  5      clear=0 mode=A        in=P5-B:4  rep=1
  6      clear=0 mode=S        in=P5-B:5  rep=1
  7      clear=0 mode=AS       in=P5-B:6  rep=1
  8      clear=C mode=A        in=P5-B:7  rep=1
  9      clear=C mode=S        in=P5-B:8  rep=1
  10     clear=C mode=AS       in=P5-B:9  rep=1
  Right  14

Acc 14 (RHPP II)
  alpha  P5-D
  gamma  P5-B
  Left   13

Multiplier
  R alpha        P5-A:1
  R beta         P5-A:2
  R gamma        P5-A:3
  R delta        P5-A:4
  R epsilon      P5-A:5
  D alpha        P5-A:6
  D beta         P5-A:7
  D gamma        P5-A:8
  D delta        P5-A:9
  D epsilon      P5-A:10
  A              P5-B:4
  S              P5-B:5
  AS             P5-B:6
  AC             P5-B:7
  SC             P5-B:8
  ASC            P5-B:9
  RS             P5-B:1                          (Req)
  DS             P5-B:2                          (Req)
  F              P5-B:3                          (Req)
  LHPP I         D5-A                            (Req)
  LHPP II        D5-B
  RHPP I         D5-C                            (Req)
  RHPP II        D5-D

  All connections marked (Req) are required for proper operation of the
high speed multiplier unit.  Other connections are either related to 
argument reception, product disposal or for answers creater than 10
digits and only those connects you intend to use need to be connected.

  You will also need to connect alpha-epsilon (as needed) inputs for the
multiplier and multiplicand accumulators to their inputs as well as to
connect A and S of RHPP I to accumulators to receive the product.
