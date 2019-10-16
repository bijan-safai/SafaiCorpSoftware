OptimalSolarArray Array;
IMyTimerBlock Timer;
private IMyRadioAntenna Ant;

public Program()

{
    Array = new OptimalSolarArray(
        GridTerminalSystem.GetBlockGroupWithName("solar_1"),
        GridTerminalSystem.GetBlockGroupWithName("solar_3"),
        GridTerminalSystem.GetBlockGroupWithName("solar_2"),
        GridTerminalSystem.GetBlockGroupWithName("solar_4"),
        GridTerminalSystem.GetBlockWithName("solar_rotor_h") as IMyMotorAdvancedStator,
        GridTerminalSystem.GetBlockWithName("solar_rotor_l") as IMyMotorAdvancedStator,
        GridTerminalSystem.GetBlockWithName("solar_rotor_r") as IMyMotorAdvancedStator,
        GridTerminalSystem.GetBlockWithName("solar_out") as IMyTextPanel
    );

    Ant = GridTerminalSystem.GetBlockWithName("gecko_ant") as IMyRadioAntenna;
    Ant.AttachedProgrammableBlock = Me.EntityId;
     
    Timer = GridTerminalSystem.GetBlockWithName("solar_timer") as IMyTimerBlock;
}

public void Main(string argument, UpdateType updateSource)

{
    if(argument == "reset")
    {
        Array.ResetArray();
        Timer.StopCountdown();
    }
    else if(argument == "reset2")
    {
        Array.TotalReset();
    }
    else
    {
        string output = Array.Optimize();
        Timer.StartCountdown();
        IGC.SendBroadcastMessage("logs", output, TransmissionDistance.TransmissionDistanceMax);

    }
}


class OptimalSolarArray
{
    const float stdDelta = 0.1f;
    float tolerence = 0.01f;

    List<IMySolarPanel> LU;
    List<IMySolarPanel> LD;
    List<IMySolarPanel> RU;
    List<IMySolarPanel> RD;

    IMyMotorAdvancedStator HorziontalMotor;
    
    IMyMotorAdvancedStator LeftMotor;    
    IMyMotorAdvancedStator RightMotor;

    IMyTextPanel OutPanel;

    public OptimalSolarArray(
        IMyBlockGroup LUg,
        IMyBlockGroup LDg,
        IMyBlockGroup RUg,
        IMyBlockGroup RDg,
        IMyMotorAdvancedStator horMotor,
        IMyMotorAdvancedStator leftMotor,
        IMyMotorAdvancedStator rightMotor,
        IMyTextPanel outpanel
    )
    {
        LU = new List<IMySolarPanel>();
        LD = new List<IMySolarPanel>();
        RU = new List<IMySolarPanel>();
        RD = new List<IMySolarPanel>();

        LUg.GetBlocksOfType<IMySolarPanel>(LU);
        RUg.GetBlocksOfType<IMySolarPanel>(RU);
        LDg.GetBlocksOfType<IMySolarPanel>(LD);
        RDg.GetBlocksOfType<IMySolarPanel>(RD);

        HorziontalMotor = horMotor;
        LeftMotor = leftMotor;
        RightMotor = rightMotor;

        OutPanel = outpanel;
    }

    public void ResetArray()
    {
        Reset(HorziontalMotor);
        Reset(LeftMotor);
        Reset(RightMotor);
    }

    private void Reset(IMyMotorAdvancedStator motor)
    {
        if(motor.Angle > 0.0f)
        {
            motor.TargetVelocityRPM = -0.5f;
            motor.LowerLimitRad = 0.0f;
        }
        else if(motor.Angle < 0.0f)
        {
            motor.TargetVelocityRPM = 0.5f;
            motor.UpperLimitRad = 0.0f;
        }
    }

    public void TotalReset()
    {
        HorziontalMotor.UpperLimitRad = 0.0f;
        HorziontalMotor.LowerLimitRad = 0.0f;

        LeftMotor.UpperLimitRad = 0.0f;
        LeftMotor.LowerLimitRad = 0.0f;

        RightMotor.UpperLimitRad = 0.0f;
        RightMotor.LowerLimitRad = 0.0f;
    }

    private float CalculateTotalPowerGenerationOfGroup(List<IMySolarPanel> panelGroup)
    {
        float powerOutput = 0.0f;

        foreach (IMySolarPanel panel in panelGroup)
        {
            powerOutput += panel.MaxOutput;
        }
        return powerOutput;
    }

    private float CalculateTotalPowerGenerationOfHalf(List<IMySolarPanel> panelGroup1, List<IMySolarPanel> panelGroup2)
    {
        return CalculateTotalPowerGenerationOfGroup(panelGroup1) + CalculateTotalPowerGenerationOfGroup(panelGroup2);
    }

    private bool EqualsWithinTolerence(float power1, float power2)
    {
        if(power1 + tolerence <= power2 || power1 - tolerence >= power2)
        {
            return false;
        }

        return true;
    }

    public string Optimize()
    {
        string orgAngleHorizontal =  $"Horizontal start angle = {HorziontalMotor.Angle}\n";
        string orgAngleLeft =  $"Left start angle = {LeftMotor.Angle}\n";
        string orgAngleRight =  $"Motor start angle = {RightMotor.Angle}\n";

        string adjustHorLog = "";
        string adjustVertLog = "";

        OutPanel.WriteText("!---------------Starting optimization-------------------!\n", false);
        OutPanel.WriteText("Motor       |   Angle   |   Velocity\n", true);
        OutPanel.WriteText($"Horizontal | {HorziontalMotor.Angle} | {HorziontalMotor.TargetVelocityRPM}\n", true);
        OutPanel.WriteText($"Left       | {LeftMotor.Angle} | {LeftMotor.TargetVelocityRPM}\n", true);
        OutPanel.WriteText($"Right      | {RightMotor.Angle} | {RightMotor.TargetVelocityRPM}\n", true);

        OutPanel.WriteText("!---------------Optimizing Horizontal--------------------!\n", true);
        float leftPower = CalculateTotalPowerGenerationOfHalf(LU, LD);
        OutPanel.WriteText($"Left Power = {leftPower}\n", true);


        float rightPower = CalculateTotalPowerGenerationOfHalf(RU, RD);
        OutPanel.WriteText($"Right Power = {rightPower}\n", true);

        if(!EqualsWithinTolerence(leftPower, rightPower))
        {
            OutPanel.WriteText("Adjusting horizontial\n", true);
            adjustHorLog = AdjustHorizontal(leftPower, rightPower);
        }

        OutPanel.WriteText("!---------------Optimizing Vertical--------------------!\n", true);
        float upPower = CalculateTotalPowerGenerationOfHalf(LU, RU);
        OutPanel.WriteText($"Up Power = {upPower}\n", true);

        float downPower = CalculateTotalPowerGenerationOfHalf(LD, RD);
        OutPanel.WriteText($"Down Power = {downPower}\n", true);
        if(!EqualsWithinTolerence(upPower, downPower))
        {
            OutPanel.WriteText("Adjusting vertical\n", true);
            adjustVertLog = AdjustVertical(upPower, downPower);
        }
        return $"!------------Optimization iteration start----------------!\n{orgAngleHorizontal}{orgAngleLeft}{orgAngleRight}{adjustHorLog}{adjustVertLog}!--------------Optimzation iteration complete-------------!\n";
    }

    private string AdjustHorizontal(float leftPower, float rightPower)
    {

        if(leftPower > rightPower)
        {
            HorziontalMotor.TargetVelocityRPM = 0.5f;
            HorziontalMotor.UpperLimitRad += stdDelta;
            OutPanel.WriteText($"Setting horizontal to {HorziontalMotor.UpperLimitRad}\n", true);
            return $"Horizontal angle target = {HorziontalMotor.UpperLimitRad}\n";
        }
        else
        {
            HorziontalMotor.TargetVelocityRPM = -0.5f;
            HorziontalMotor.LowerLimitRad -= stdDelta;
            OutPanel.WriteText($"Setting horizontal to {HorziontalMotor.LowerLimitRad}\n", true);
            return $"Horizontal angle target = {HorziontalMotor.LowerLimitRad}\n";
        }
    }

    private string AdjustVertical(float upPower, float downPower)
    {
        if(upPower > downPower)
        {
            LeftMotor.TargetVelocityRPM = -0.5f;
            RightMotor.TargetVelocityRPM = 0.5f;
            
            LeftMotor.LowerLimitRad -= stdDelta;
            RightMotor.UpperLimitRad += stdDelta;
        }
        else
        {
            LeftMotor.TargetVelocityRPM = 0.5f;
            RightMotor.TargetVelocityRPM = -0.5f;
            
            LeftMotor.LowerLimitRad += stdDelta;
            RightMotor.UpperLimitRad -= stdDelta;
        }

         OutPanel.WriteText($"Setting left to {LeftMotor.LowerLimitRad}\n", true);
         OutPanel.WriteText($"Setting right to {RightMotor.UpperLimitRad}\n", true);

         return $"Left angle target = {LeftMotor.LowerLimitRad}\n Right angle target = {RightMotor.UpperLimitRad}\n";
    }

}