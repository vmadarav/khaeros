using System;
using Server;
using System.IO;
using System.Text;
using Server.Network;
using Server.Mobiles;

namespace Server.FeatInfo
{
	public class MindI : BaseFeat
	{
		public override string Name{ get{ return "Mind I"; } }
		public override FeatList ListName{ get{ return Mobiles.FeatList.MindI; } }
		public override FeatCost CostLevel{ get{ return FeatCost.High; } }
		
		public override SkillName[] AssociatedSkills{ get{ return new SkillName[]{ }; } }
		public override FeatList[] AssociatedFeats{ get{ return new FeatList[]{ }; } }
		
		public override FeatList[] Requires{ get{ return new FeatList[]{ FeatList.Magery }; } }
		public override FeatList[] Allows{ get{ return new FeatList[]{ FeatList.MindII }; } }
		
		public override string FirstDescription{ get{ return "This skill will allow you to cast mind-based spells from scrolls you may " +
					"find. It will also increase your mana regeneration rate."; } }
		public override string SecondDescription{ get{ return "Improved effect."; } }
		public override string ThirdDescription{ get{ return "Improved effect."; } }
		
		public override string FirstCommand{ get{ return "None"; } }
		public override string SecondCommand{ get{ return "None"; } }
		public override string ThirdCommand{ get{ return "None"; } }
		
		public override string FullDescription{ get{ return GetFullDescription(this); } }
		
		public static void Initialize(){ WriteWebpage(new MindI()); }
		
		public override bool MeetsOurRequirements( PlayerMobile m )
		{
	             if (m.Feats.GetFeatLevel(FeatList.MatterI) > 0 || m.Feats.GetFeatLevel(FeatList.DeathI) > 0 || m.Feats.GetFeatLevel(FeatList.ForcesI) > 0)
                	 return false;


			
			return base.MeetsOurRequirements( m );
		}
		
		public MindI() {}
	}
}
