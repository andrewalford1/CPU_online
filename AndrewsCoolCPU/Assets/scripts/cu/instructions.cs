using System;

[Serializable]
public class Instruction
{
    public int ID;
    public string description;
    public string command;
}

[Serializable]
public class InstructionSet
{
    public Instruction[] instructions;
}
