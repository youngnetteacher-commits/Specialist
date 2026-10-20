using System;

namespace Specialist.Assessment.Models;

public class CustomStrangeException: Exception
{
    public CustomStrangeException(string message): base(message)
    {
        
    }
}
