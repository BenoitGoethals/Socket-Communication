using System;
using Chat.Core;


namespace ProjectA.Protocol
{
    [Serializable]
    public class SendingTimeMessage: MessageBase
    {
        private DateTime m_time;
        private string m_message;

        public SendingTimeMessage(string message)
        {
            m_time = DateTime.Now;
            m_message = message;
        }

        public DateTime Time
        {
            get { return m_time; }
        }

        public string Message
        {
            get { return m_message; }
        }

        public TimeSpan CalcSpan()
        {
            return DateTime.Now - m_time;
        }

      
    }
}
