using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlEnthrallment : XmlAttachment
    {
        private TimeSpan m_Duration = TimeSpan.FromSeconds(30.0);       // default 30 sec duration
        private int m_Value = 10;       // default value of 10
        
        [CommandProperty( AccessLevel.GameMaster )]
        public int Value { get { return m_Value; } set { m_Value  = value; } }


        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
       
        // a serial constructor is REQUIRED
        public XmlEnthrallment(ASerial serial) : base(serial)
        {
        }

        [Attachable]
        public XmlEnthrallment()
        {
        }

        [Attachable]
        public XmlEnthrallment(int value)
        {
            m_Value = value;
        }
        
        [Attachable]
        public XmlEnthrallment(int value, double duration)
        {
            m_Value = value;
            m_Duration = TimeSpan.FromMinutes(duration);
        }

		public override void OnAttach()
		{
		    base.OnAttach();

			Timer.DelayCall(m_Duration, new TimerCallback(Delete));
		}
    }
}
