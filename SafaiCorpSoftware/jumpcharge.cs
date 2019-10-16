IMySoundBlock SoundBlock;
IMyJumpDrive IndDrive;
IMyTimerBlock LongTimer;
private IMyTextPanel OutPanel;

public Program()

{

    IndDrive = GridTerminalSystem.GetBlockWithName("M_Jump_Drive_1") as IMyJumpDrive;
    OutPanel = GridTerminalSystem.GetBlockWithName("Text panel 7") as IMyTextPanel;
    SoundBlock = GridTerminalSystem.GetBlockWithName("jump_sound") as IMySoundBlock;
    LongTimer = GridTerminalSystem.GetBlockWithName("Black_Timer") as IMyTimerBlock;
}

public void Main(string argument, UpdateType updateSource)

{   
    if(argument != "check")
    {
        if(IndDrive.CurrentStoredPower == IndDrive.MaxStoredPower)
        {
            SoundBlock.Play();
            OutPanel.WriteText("Jump drives ready.\n", false);
            Runtime.UpdateFrequency = 0;
        }
        else
        {
            int percentage = (int) (IndDrive.CurrentStoredPower / IndDrive.MaxStoredPower) * 100;
            if(percentage < 25)
            {
                OutPanel.FontColor = Color.Red;
            }
            else if (percentage < 75)
            {
                OutPanel.FontColor = Color.Orange;
            }
            else
            {
                OutPanel.FontColor = Color.Yellow;
            }

            OutPanel.FontColor = Color.Green;
            OutPanel.WriteText($"Jump dive charge: {percentage} %");
        }
    }
    else if(IndDrive.CurrentStoredPower == IndDrive.MaxStoredPower)
    {
        Runtime.UpdateFrequency = UpdateFrequency.Update100;
    }
    
}