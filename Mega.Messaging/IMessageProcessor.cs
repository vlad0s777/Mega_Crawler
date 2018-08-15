using System;
using System.Collections.Generic;
using System.Text;

namespace Mega.Messaging
{
    public interface IMessageProcessor
    {
        bool Run();
    }
}
