using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Mobiles
{
	public class InnKeeper : BaseVendor
	{
		private ArrayList m_SBInfos = new ArrayList();
		protected override ArrayList SBInfos{ get { return m_SBInfos; } }
		private bool m_ChargedLastTime;
		
		public bool ChargedLastTime
		{
			get{ return m_ChargedLastTime; }
			set{ m_ChargedLastTime = value; }
		}

		[Constructable]
		public InnKeeper() : base( "the inn keeper" )
		{
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBInnKeeper() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return Female ? VendorShoeType.ThighBoots : VendorShoeType.Boots; }
		}

		public override int GetShoeHue()
		{
			return 0;
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( Utility.RandomBool() ? (Item)new QuarterStaff() : (Item)new ClericCrook() );
		}
		
		private class GumpEntry : ContextMenuEntry
		{
			private InnKeeper m_Trainer;
			private Mobile m_From;

			public GumpEntry( InnKeeper trainer, Mobile from ) : base( 6181, 12 )
			{
				m_Trainer = trainer;
				m_From = from;
			}

			public override void OnClick()
			{
				m_Trainer.BeginClaimList( m_From );
			}
		}

		private class StableEntry : ContextMenuEntry
		{
			private InnKeeper m_Trainer;
			private Mobile m_From;

			public StableEntry( InnKeeper trainer, Mobile from ) : base( 6126, 12 )
			{
				m_Trainer = trainer;
				m_From = from;
			}

			public override void OnClick()
			{
				m_Trainer.BeginStable( m_From );
			}
		}
		
		private class ClaimListGump : Gump
		{
			private InnKeeper m_Trainer;
			private Mobile m_From;
			private List<BaseCreature> m_List;

			public ClaimListGump( InnKeeper trainer, Mobile from, List<BaseCreature> list ) : base( 50, 50 )
			{
				m_Trainer = trainer;
				m_From = from;
				m_List = list;

				from.CloseGump( typeof( ClaimListGump ) );

				AddPage( 0 );

				AddBackground( 0, 0, 325, 50 + (list.Count * 20), 9250 );
				AddAlphaRegion( 5, 5, 315, 40 + (list.Count * 20) );

				AddHtml( 15, 15, 275, 20, "<BASEFONT COLOR=#FFFFFF>Select a mercenary to retrieve from the inn:</BASEFONT>", false, false );

				for ( int i = 0; i < list.Count; ++i )
				{
					BaseCreature pet = list[i];

					if ( pet == null || pet.Deleted )
						continue;

					AddButton( 15, 39 + (i * 20), 10006, 10006, i + 1, GumpButtonType.Reply, 0 );
					AddHtml( 32, 35 + (i * 20), 275, 18, String.Format( "<BASEFONT COLOR=#C0C0EE>{0}</BASEFONT>", pet.Name ), false, false );
				}
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				int index = info.ButtonID - 1;

				if ( index >= 0 && index < m_List.Count )
					m_Trainer.EndClaimList( m_From, m_List[index], null );
			}
		}

		private class ClaimAllEntry : ContextMenuEntry
		{
			private InnKeeper m_Trainer;
			private Mobile m_From;

			public ClaimAllEntry( InnKeeper trainer, Mobile from ) : base( 6127, 12 )
			{
				m_Trainer = trainer;
				m_From = from;
			}

			public override void OnClick()
			{
				m_Trainer.Claim( m_From );
			}
		}
		
		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive )
			{
				list.Add( new StableEntry( this, from ) );

				if ( this.Stabled.Count > 0 )
				{
					foreach( Mobile mob in this.Stabled )
					{
						if( mob is BaseCreature && ((BaseCreature)mob).StabledOwner == from )
						{
							list.Add( new ClaimAllEntry( this, from ) );
							list.Add( new GumpEntry( this, from ) );
							break;
						}
					}
				}
			}

			base.AddCustomContextEntries( from, list );
		}

		private class StableTarget : Target
		{
			private InnKeeper m_Trainer;

			public StableTarget( InnKeeper trainer ) : base( 12, false, TargetFlags.None )
			{
				m_Trainer = trainer;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Mercenary )
					m_Trainer.EndStable( from, (BaseCreature)targeted );
				else
					m_Trainer.SayTo( from, "We don't accept that here." ); // You can't stable that!
			}
		}
		
		public override bool OnDragDrop( Mobile from, Item item )
		{
			if( from != null && item != null && !from.Deleted && !item.Deleted && item is StableTicket )
			{
				if( ((StableTicket)item).StabledPet == null )
					this.SayTo( from, "I am sorry. Your ticket seems to have expired." );
				
				else if( this.Stabled == null || !this.Stabled.Contains(((StableTicket)item).StabledPet) )
					this.SayTo( from, "I am sorry. I do not have any mercenaries with that name resting here." );
				
				else if( this.Stabled != null && this.Stabled.Contains(((StableTicket)item).StabledPet) && ((StableTicket)item).StabledPet is BaseCreature )
				{
					EndClaimList( from, (BaseCreature)((StableTicket)item).StabledPet, (StableTicket)item );
					return true;
				}
			}
			
			return false;
		}
		
		public void BeginClaimList( Mobile from )
		{
			if ( Deleted || !from.CheckAlive() )
				return;

			List<BaseCreature> list = new List<BaseCreature>();

			for ( int i = 0; i < this.Stabled.Count; ++i )
			{
				BaseCreature pet = this.Stabled[i] as BaseCreature;

				if ( pet == null || pet.Deleted )
				{
					pet.IsStabled = false;
					this.Stabled.RemoveAt( i );
					--i;
					continue;
				}

				if( pet.StabledOwner == from )
					list.Add( pet );
			}

			if ( list.Count > 0 )
				from.SendGump( new ClaimListGump( this, from, list ) );
			else
				SayTo( from, 502671 ); // But I have no animals stabled with me at the moment!
		}

		public void EndClaimList( Mobile from, BaseCreature pet, StableTicket ticket )
		{
			if ( pet == null || pet.Deleted || from.Map != this.Map || !from.InRange( this, 14 ) || !this.Stabled.Contains( pet ) || !from.CheckAlive() )
				return;

			if ( (from.Followers + pet.ControlSlots) <= from.FollowersMax )
			{
				pet.SetControlMaster( from );

				if ( pet.Summoned )
					pet.SummonMaster = from;

				pet.ControlTarget = from;
				pet.ControlOrder = OrderType.Follow;
				pet.StabledOwner = null;
				pet.MoveToWorld( from.Location, from.Map );
				pet.IsStabled = false;
				this.Stabled.Remove( pet );
				
				if( ticket != null && !ticket.Deleted )
					ticket.Delete();
				
				if( pet.StableTicket != null && !pet.StableTicket.Deleted )
					pet.StableTicket.Delete();
				
				if( pet is Mercenary )
					((Mercenary)pet).StartPayTimer();

				SayTo( from, 1042559 ); // Here you go... and good day to you!
			}
			else
			{
				SayTo( from, pet.Name + " remained here because you have too many followers." ); // ~1_NAME~ remained in the stables because you have too many followers.
			}
		}

		public void BeginStable( Mobile from )
		{
			if ( Deleted || !from.CheckAlive() )
				return;

			SayTo( from, "I charge one copper piece per pet every six hours [OOC: two real-life hours]. I will withdraw it from thy bank account. " +
			      "Which mercenary wouldst thou like to rest here?" );

			from.Target = new StableTarget( this );
		}

		public void EndStable( Mobile from, BaseCreature pet )
		{
			if ( Deleted || !from.CheckAlive() )
				return;

			if ( !pet.Controlled || pet.ControlMaster != from )
			{
				SayTo( from, "You do not own that." ); // You do not own that pet!
			}
			else if ( pet.IsDeadPet )
			{
				SayTo( from, "Living mercenaries only, please." ); // Living pets only, please.
			}
			else if ( pet.Summoned || pet.VanishTime != DateTime.MinValue )
			{
				SayTo( from, 502673 ); // I can not stable summoned creatures.
			}
			else if( (pet.Backpack != null && pet.Backpack.Items.Count > 0) || pet.Items.Count > 1 )
			{
				SayTo( from, "You need to remove your mercenary's belongings." ); // You need to unload your pet.
			}
			else if ( pet.Combatant != null && pet.InRange( pet.Combatant, 12 ) && pet.Map == pet.Combatant.Map )
			{
				SayTo( from, 1042564 ); // I'm sorry.  Your pet seems to be busy.
			}
			else if( pet is Mercenary )
			{
				Container bank = from.BankBox;

				if ( bank != null && bank.ConsumeTotal( typeof( Copper ), 1 ) )
				{
					pet.ControlTarget = null;
					pet.ControlOrder = OrderType.Stay;
					pet.StabledOwner = from;
					pet.Internalize();
					pet.SetControlMaster( null );
					pet.SummonMaster = null;
					pet.IsStabled = true;
					pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy
					this.Stabled.Add( pet );
					SayTo( from, "Thy mercenary has checked in. I will charge you one copper piece every six hours [OOC: two real-life hours]." );
					SayTo( from, "Here is your ticket. Hand it back to me when you wish to wake your mercenary up." );
					StableTicket ticket = new StableTicket();
					ticket.StabledPet = pet;
					ticket.Name = "Inn Ticket";
					pet.StableTicket = ticket;
					
					if( pet is Mercenary )
						((Mercenary)pet).StopPayTimer();
					
					if( from.Backpack != null )
						from.AddToBackpack( ticket );
				}
				else
				{
					SayTo( from, 502677 ); // But thou hast not the funds in thy bank account!
				}
			}
		}
		
		public override bool HandlesOnSpeech( Mobile from )
		{
			return true;
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled && e.HasKeyword( 0x0008 ) )
			{
				e.Handled = true;
				BeginStable( e.Mobile );
			}
			else if ( !e.Handled && e.HasKeyword( 0x0009 ) )
			{
				e.Handled = true;

				if ( !Insensitive.Equals( e.Speech, "claim" ) )
					BeginClaimList( e.Mobile );
				else
					Claim( e.Mobile );
			}
			else
			{
				base.OnSpeech( e );
			}
		}
		
		public void Claim( Mobile from )
		{
			if ( Deleted || !from.CheckAlive() )
				return;

			bool claimed = false;
			int stabled = 0;

			for ( int i = 0; i < this.Stabled.Count; ++i )
			{
				BaseCreature pet = this.Stabled[i] as BaseCreature;

				if ( pet == null || pet.Deleted )
				{
					pet.IsStabled = false;
					this.Stabled.RemoveAt( i );
					--i;
					continue;
				}
				
				if( pet.StabledOwner != from )
					continue;

				++stabled;

				if ( (from.Followers + pet.ControlSlots) <= from.FollowersMax )
				{
					pet.SetControlMaster( from );

					if ( pet.Summoned )
						pet.SummonMaster = from;

					pet.ControlTarget = from;
					pet.ControlOrder = OrderType.Follow;

					pet.MoveToWorld( from.Location, from.Map );

					pet.IsStabled = false;
					pet.StabledOwner = null;
					
					if( pet.StableTicket != null && !pet.StableTicket.Deleted )
						pet.StableTicket.Delete();

					if( pet is Mercenary )
						((Mercenary)pet).StartPayTimer();
					
					if ( Core.SE )
						pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy

					this.Stabled.RemoveAt( i );
					--i;

					claimed = true;
				}
				else
				{
					SayTo( from, pet.Name + " remained here because you have too many followers." ); // ~1_NAME~ remained in the stables because you have too many followers.
				}
			}

			if ( claimed )
				SayTo( from, 1042559 ); // Here you go... and good day to you!
			else if ( stabled == 0 )
				SayTo( from, "I have no mercenaries of yours resting here at the moment." ); // But I have no animals stabled with me at the moment!
		}

		public InnKeeper( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
			writer.Write( (bool) m_ChargedLastTime );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			if( version > 0 )
				m_ChargedLastTime = reader.ReadBool();
		}
	}
}
