using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBSEBowyer: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBSEBowyer()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( WarBow ), 53, 20, 0x27A5, 0 ) );
				Add( new GenericBuyInfo( typeof( BlowGun ), 20, 20, 0x27AA, 0 ) );
				Add( new GenericBuyInfo( typeof( Nunchaku ), 35, 20, 0x27AE, 0 ) );
				Add( new GenericBuyInfo( typeof( BlowGunDarts ), 3, 20, 0x2806, 0 ) );
				Add( new GenericBuyInfo( typeof( Bokuto ), 21, 20, 0x27A8, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( WarBow ), 26 );
				Add( typeof( BlowGun ), 10 );
				Add( typeof( Nunchaku ), 17 );
				Add( typeof( BlowGunDarts ), 1 );
				Add( typeof( Bokuto ), 10 );
			}
		}
	}
}
