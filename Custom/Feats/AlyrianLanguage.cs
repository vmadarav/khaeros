using System;
using Server;
using System.IO;
using System.Text;
using Server.Network;
using Server.Mobiles;

namespace Server.FeatInfo
{
	public class SouthernLanguage : BaseFeat
	{
		public override string Name{ get{ return "Southern Language"; } }
		public override FeatList ListName{ get{ return Mobiles.FeatList.SouthernLanguage; } }
		public override FeatCost CostLevel{ get{ return FeatCost.High; } }
		
		public override SkillName[] AssociatedSkills{ get{ return new SkillName[]{ }; } }
		public override FeatList[] AssociatedFeats{ get{ return new FeatList[]{ }; } }
		
		public override FeatList[] Requires{ get{ return new FeatList[]{ FeatList.Linguistics }; } }
		public override FeatList[] Allows{ get{ return new FeatList[]{ }; } }
		
		public override string FirstDescription{ get{ return "This skill will give you some knowledge in the Language of the Free City of the Lurieth, which will " +
					"give you a small chance, according to your skill, to understand things spoken in Lurethian and, at max level, to speak the " +
					"language itself. [25% chance]"; } }
		public override string SecondDescription{ get{ return "[60% chance]"; } }
		public override string ThirdDescription{ get{ return "[100% chance]"; } }

		public override string FirstCommand{ get{ return "None"; } }
		public override string SecondCommand{ get{ return "None"; } }
		public override string ThirdCommand{ get{ return "None"; } }
		
		public override string FullDescription{ get{ return GetFullDescription(this); } }
		
		public static void Initialize(){ WriteWebpage(new SouthernLanguage()); }
		
		public override bool CanBeRemovedFrom( PlayerMobile m )
		{
			if( m.Nation == Nation.Southern )
			{
				m.SendMessage( "You cannot unlearn your first language." );
				return false;
			}
			
			return true;
		}

        public override bool IgnoreThisFeatWhenRemovingParent( PlayerMobile m )
        {
            return m.Nation == Nation.Southern;
        }
		
		public SouthernLanguage() {}
	}
}
