using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedClassLibrary;

public interface MessageCreated
{
    int Id { get; set; }
    string From { get; set; }
    string To { get; set; }
    string Message { get; set; }
}
