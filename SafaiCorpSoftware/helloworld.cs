private const string XPistonBlockName = "X_Pistons";
private const string YPistonBlockName = "Y_Pistons";
private const string ZPistonBlockName = "Z_Pistons";

private const int PistonLength = 10;

private IMyTextPanel OutPanel;
private MyCommandLine cmd;
private List<IMyMotorSuspension> Wheels;
private List<IMyMotorAdvancedStator> Rotors;
private PistonGrid pistonGrid;

public Program()

{
    OutPanel = GridTerminalSystem.GetBlockWithName("progout") as IMyTextPanel;
    Wheels = new List<IMyMotorSuspension>();
    Rotors = new List<IMyMotorAdvancedStator>();
    cmd = new MyCommandLine();
    pistonGrid = new PistonGrid(
        GridTerminalSystem.GetBlockGroupWithName(XPistonBlockName),
        GridTerminalSystem.GetBlockGroupWithName(YPistonBlockName),
        GridTerminalSystem.GetBlockGroupWithName(ZPistonBlockName)
    );
    ScanForAssets();
    ConfigureSystem();
}



public void Save()

{

    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means. 
    // 
    // This method is optional and can be removed if not
    // needed.

}



public void Main(string argument, UpdateType updateSource)
{
    if(cmd.Switch("timer"))
    {

    }
    else // with no timer switch we call this a configure call
    {
        ScanForAssets();
        ConfigureSystem();
    }
}

public void ScanForAssets()
{
    OutPanel.WriteText("Scanning for wheels...\n", true);
    GridTerminalSystem.GetBlocksOfType<IMyMotorSuspension>(Wheels);
    OutPanel.WriteText($"Found {Wheels.Count()} wheels\n", true);

    OutPanel.WriteText("Scanning for motors... \n", true);
    GridTerminalSystem.GetBlocksOfType<IMyMotorAdvancedStator>(Rotors);
    OutPanel.WriteText($"Found {Rotors.Count()} motors\n", true);
}

public void ConfigureSystem()
{
    ConfigureWheels();
    ConfigureRotors();
}

public void ConfigureWheels()
{
    OutPanel.WriteText("Configuring wheels...\n", true);
    foreach (IMyMotorSuspension wheel in Wheels)
    {
        wheel.Strength = 50.0f;
    }
    OutPanel.WriteText("Done.\n", true);
}

public void ConfigureRotors()
{
    OutPanel.WriteText("Configuriong mortors...\n", true);
    foreach (IMyMotorAdvancedStator rotor in Rotors)
    {
        rotor.LowerLimitDeg = -90.0f;
        rotor.UpperLimitDeg = 0.0f;
        rotor.TargetVelocityRPM  = -1;
    }
    OutPanel.WriteText("Done.\n", true);
}

public class StorageManager
{
    private List<IMyCargoContainer> ContainerPool;
    private int LastMass;

    public bool IsFull()
    {
        foreach(IMyCargoContainer container in ContainerPool)
        {
            if(!container.GetInventory().IsFull)
            {
                return false;
            }
            return true;
        }
    }

    public bool HasIncreasedSinceLastTick()
    {
        int currentMass = 0;
        foreach(IMyCargoContainer container in ContainerPool)
        {
            currentMass += container.GetInventory().currentMass;
        }
        if(LastMass >= currentMass)
        {
            LastMass = currentMass;
            return false;
        }
        else
        {
            LastMass = currentMass;
            return true;
        }
    }
}

public class PistonGrid
{
    private List<IMyPistonBase> Xpistons;
    private List<IMyPistonBase> Ypistons;
    private List<IMyPistonBase> Zpistons;
    private List<IMyMotorAdvancedStator> Rotors

    private int RotorVelocity;
    private int ZExtension;

    public PistonGrid(
        IMyBlockGroup xPistons,
        IMyBlockGroup yPistons,
        IMyBlockGroup zPistons,
        List<IMyMotorAdvancedStator> rotors
        int rotorVelocity
    )
    {   
        Xpistons = new List<IMyPistonBase>();
        Ypistons = new List<IMyPistonBase>();
        Zpistons = new List<IMyPistonBase>();

        xPistons.GetBlocksOfType<IMyPistonBase>(Xpistons);
        yPistons.GetBlocksOfType<IMyPistonBase>(Ypistons);
        zPistons.GetBlocksOfType<IMyPistonBase>(Zpistons);
        
        Rotors = rotors; 
        rotorVelocity = RotorVelocity;
    }

    public void ReverseZ()
    {
        foreach(IMyPistonBase zpiston in Zpistons)
        {
            zpiston.Extend();
        }
    }

    public void RetractZ()
    {
        foreach(IMyPistonBase zpiston in Zpistons)
        {
            zpiston.Retract();
        }
    }

    public void ExtendYFully()
    {
        if(!RotorsAreAlignedY())
        {
            AlignRotors(1);
        }
        ExtendArmFully(this.Ypistons);
    }

    public bool DecrementY()
    {
        if(!RotorsAreAlignedY())
        {
            AlignRotors(1);
        }
        return DecrementArm(this.Ypistons);
    }

    public bool DecrementX()
    {
        if(!RotorsAreAlignedX())
        {
            AlignRotors(-1);
        }
        return DecrementArm(this.Xpistons);
    }

    public void ExtendXFully()
    {
        if(!RotorsAreAlignedX())
        {
            AlignRotors(-1);
        }
        ExtendArmFully(this.Xpistons);
    }

    public bool RotorsAreAlignedX()
    {
        return RotorVelocity > 0;
    }

    public bool RotorsAreAlignedY()
    {
        return RotorVelocity < 0;
    }

    private void AlignRotors(int rotorVelocity)
    {
        foreach(IMyMotorAdvancedStator rotor in Rotors)
        {
            rotor.Velocity = rotorVelocity;
        }
        RotorVelocity = rotorVelocity;
    }

    private void ExtendArmFully(List<IMyPistonBase> armPistons)
    {
        foreach(IMyPistonBase piston in armPistons)
        {
            piston.Extend();
        }
    }

    private bool DecrementArm(List<IMyPistonBase> armPistons)
    {
        int i = 0;
        while(i != armPistons.Count())
        {
            if(armPistons[i].Velocity > 0)
            {
                armPistons[i].Retract();
                return true;
            }
        }
        return false;
    }
}