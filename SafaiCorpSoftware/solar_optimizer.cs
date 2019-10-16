    private IMyPowerProducer SolarIndicator;
    private IMyMotorAdvancedStator ArrayRotator;
    private const string SolarBlockName = "Smart_Solar";
    private const string MotorSuffix = "_Rotor";
    private int MotorRotationVelocity = 20;
    private IMyTextPanel OutPanel;
    private IMyTimerBlock Timer;

    private float currPow;

    public Program()
    {
        Timer = GridTerminalSystem.GetBlockWithName("solar_timer") as IMyTimerBlock;
        OutPanel = GridTerminalSystem.GetBlockWithName("progout") as IMyTextPanel;
        SolarIndicator = GridTerminalSystem.GetBlockWithName(SolarBlockName) as IMyPowerProducer;
        ArrayRotator = GridTerminalSystem.GetBlockWithName(SolarBlockName + MotorSuffix) as IMyMotorAdvancedStator;
        currPow = SolarIndicator.CurrentOutput;
        OutPanel.WriteText($"Init pow {currPow}");
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
    }

    public void Main(string argument, UpdateType updateSource)
    {
        if(true)
        {
            OutPanel.WriteText($"Current output {SolarIndicator.CurrentOutput}\n", false);
            OutPanel.WriteText($"Last output {currPow}\n", true);
            if(currPow > SolarIndicator.CurrentOutput)
            {
                ArrayRotator.TargetVelocityRPM *= -1;
                currPow = SolarIndicator.CurrentOutput;
            }
            else
            {
                currPow = SolarIndicator.CurrentOutput;
            }
            //Timer.StartCountdown();
        }
    }

    public void StartRotation()
    {

    } 

    public void RotatePanel()
    {
        OutPanel.WriteText($"Max output {SolarIndicator.MaxOutput}\n", false);
        OutPanel.WriteText($"Current output {SolarIndicator.CurrentOutput}\n", true);
        currPow = SolarIndicator.CurrentOutput;
        if(true)
        {
            ArrayRotator.TargetVelocityRPM = MotorRotationVelocity;
            while(SolarIndicator.CurrentOutput > currPow)
            {
                currPow = SolarIndicator.CurrentOutput;
            }
            ArrayRotator.TargetVelocityRPM = 0;
            OutPanel.WriteText($"New output {SolarIndicator.CurrentOutput}\n", true);
        }
    }