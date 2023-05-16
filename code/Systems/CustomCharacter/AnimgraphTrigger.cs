using System;

namespace CustomCharacter;

[HammerEntity]
public class AnimgraphTrigger : BaseTrigger
{

	[Property]
	public bool SetCrouched { get; set; }
	[Property]
	public bool SetStrafe { get; set; }

	[Property]
	public bool SetLookAtLocked { get; set; }

	public override void OnTouchStart( Entity toucher )
	{
		base.OnTouchStart( toucher );

		if ( toucher is not NpcTest npc )
			return;

		if ( SetCrouched )
			npc.SetAnimParameter( "Crouch", true );

		if ( SetStrafe )
			npc.SetAnimParameter( "Strafe", true );

		if ( SetLookAtLocked )
			npc.SetAnimParameter( "LookAtLocked", true );
	}
	public override void OnTouchEnd( Entity toucher )
	{
		base.OnTouchEnd( toucher );

		if ( toucher is not NpcTest npc )
			return;

		if ( SetCrouched )
			npc.SetAnimParameter( "Crouch", false );

		if ( SetStrafe )
			npc.SetAnimParameter( "Strafe", false );

		if ( SetLookAtLocked )
			npc.SetAnimParameter( "LookAtLocked", false );
	}
}
