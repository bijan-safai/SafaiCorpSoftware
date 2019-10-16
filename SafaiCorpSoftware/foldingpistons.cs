int RotorCount;
int CurrentInd;
int Delta;
string prefix = "Advanced_Rotor_";


public Program()

{

    List<IMyMotorStator> rotors = new List<IMyMotorStator>();

    GridTerminalSystem.GetBlocksOfType<IMyMotorStator>(rotors);

    foreach(IMyMotorStator rotor in rotors)
    {
        rotor.UpperLimitDeg = 0.0f;
        rotor.LowerLimitDeg = -180.0f;
    }
    RotorCount = rotors.Count();
    CurrentInd = 0;
    Delta = -1;
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

    if(CurrentInd == 0 || CurrentInd == RotorCount + 1)
    {
        Echo("Max... reversing");
        Delta *= -1;
        CurrentInd += Delta;
    }
    else
    {
            
        if(CurrentInd == 1)
        {
            GetAndReverse(CurrentInd);
            CurrentInd += Delta;
        }
        else
        {
            GetAndReverse(CurrentInd);
            CurrentInd += Delta;
            GetAndReverse(CurrentInd);
            CurrentInd += Delta;
        }

    }

}


public void GetAndReverse(int index)
{
    IMyMotorStator rotor = GridTerminalSystem.GetBlockWithName($"{prefix}{index}") as IMyMotorStator;
    rotor.TargetVelocityRPM *= -1;
}