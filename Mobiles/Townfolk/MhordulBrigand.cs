using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Mobiles
{
	public class MhordulBrigand : BaseKhaerosMobile, IBrigand
	{
		[Constructable]
		public MhordulBrigand() : base( Nation.Mhordul ) 
		{
			SetStr( 150 );
			SetStr( 150 );
			SetDex( 150 );
			SetInt( 35 );

			SetDamage( 20, 30 );
			
			SetHits( 150, 200 );
			
			SetDamageType( ResistanceType.Blunt, 100 );

			SetSkill( SkillName.Anatomy, 80.0 );
			SetSkill( SkillName.Archery, 80.0 );
			SetSkill( SkillName.Fencing, 80.0 );
			SetSkill( SkillName.Macing, 80.0 );
			SetSkill( SkillName.Swords, 80.0 );
			SetSkill( SkillName.Tactics, 80.0 );
			SetSkill( SkillName.Polearms, 80.0 );
			SetSkill( SkillName.ExoticWeaponry, 80.0 );
			SetSkill( SkillName.Axemanship, 80.0 );
			SetResistance( ResistanceType.Blunt, 30, 40 );
			SetResistance( ResistanceType.Piercing, 50, 50 );
			SetResistance( ResistanceType.Slashing, 45, 55 );
			SetResistance( ResistanceType.Fire, 20, 25 );
			SetResistance( ResistanceType.Cold, 75 );
			SetResistance( ResistanceType.Poison, 60 );
			SetResistance( ResistanceType.Energy, 30 );
			
			this.Fame = 8000;
			
			this.VirtualArmor = 45;
			this.FightMode = FightMode.Closest;
			int hue = Utility.RandomNeutralHue();
			
			if( this.Female )
			{
				EquipItem( new RaggedBra( hue ) );
				EquipItem( new SmallRaggedSkirt( hue ) );
				EquipItem( new BoneBow() );
				PackItem( new Arrow( Utility.RandomMinMax( 10, 20 ) ) );
				AI = AIType.AI_Archer;
			}
			
			else
			{
				int chance = Utility.RandomMinMax( 0, 2 );
				
				switch( chance )
				{
					case 0: 
					{
						EquipItem( new MhordulBladedBoneStaff() );
						break;
					}
						
					case 1: 
					{
						EquipItem( new BarbarianHeavyAxe() );
						break;
					}
						
					case 2: 
					{
						EquipItem( new BarbarianMace() );
						EquipItem( new WoodenShield() );
						break;
					}
				}
				EquipItem( new WaistCloth() );
			}
			
			ExecutionerMask mask = new ExecutionerMask();
			//mask.Hue = hue;
			EquipItem( mask );
			EquipItem( new Sandals() );
		}

		public MhordulBrigand(Serial serial) : base(serial)
		{
		}


		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}
