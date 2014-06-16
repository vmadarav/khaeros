using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Spells;
using System.Collections;
using Server.Commands;

namespace Server.Items
{
    public class MutilatedFairy : Item
    {
        private int m_Power;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Power
        {
            get { return m_Power; }
            set { m_Power = value; }
        }

        [Constructable]
        public MutilatedFairy()
            : base(0x2634)
        {
            Stackable = false;
            Weight = 1.0;
            Name = "A Mutilated Fairy";

        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || !(from is PlayerMobile) || from.Deleted || !from.Alive || from.Frozen || from.Paralyzed)
                return;

            PlayerMobile pm = from as PlayerMobile;


            if (from.Backpack != null && this.ParentEntity == from.Backpack)
            {

                from.Emote("*Pops a live fairy into their mouth*");

                this.Delete();
                pm.WikiConfig = "fey";

                
            }

            else
                from.SendMessage("That needs to be in your backpack for you to use it.");
        }

        public MutilatedFairy(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((int)m_Power);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Power = reader.ReadInt();
        }

      
    }
}