using System.Collections.Generic;
using System;

[Serializable]
public class Program
{
    public string       name;
    public string       dateCreated;
    public string       timeCreated;
    public string       description;
    public bool         assembled;
    public List<string> code;
    public List<string> errors;
    public List<string> data;
}
