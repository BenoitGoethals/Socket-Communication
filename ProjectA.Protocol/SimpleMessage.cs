using System;
using Chat.Core;

namespace ProjectA.Protocol
{
    [Serializable]
    public class SimpleMessage: MessageBase
    {
        private string m_message;

        public SimpleMessage(string message)
        {
            m_message = message;
        }

        public string Message
        {
            get { return m_message; }
        }


        ///<summary>
        ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override string ToString()
        {
            return m_message;
        }

        
    }
}
