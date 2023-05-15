using System;

namespace CustomCharacter;

public struct CustomHelper
{
	AnimatedEntity Owner;

	public CustomHelper( AnimatedEntity entity )
	{
		Owner = entity;
	}

	public void WithLookAt( Vector3 look )
	{
		var eyepos = Owner.Position + Vector3.Up * 64;
		Owner.SetAnimLookAt( "Look Heading", eyepos, look );
		//Owner.SetAnimLookAt( "Look Target", eyepos, look );
		Owner.SetAnimLookAt( "Look at Target", eyepos, look );
		//Owner.SetAnimLookAt( "aim_head", eyepos, look );
		//Owner.SetAnimLookAt( "aim_body", eyepos, look );
		Owner.SetAnimParameter( "Look Heading", look.Angle( Owner.Velocity ) );
		//Owner.SetAnimParameter( "Look at Target", eyepos + look );
	}

	public void WithVelocity( Vector3 Velocity )
	{
		var dir = Velocity;
		var forward = Owner.Rotation.Forward.Dot( dir );
		var sideward = Owner.Rotation.Right.Dot( dir );

		var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

		Owner.SetAnimParameter( "move_direction", angle );
		Owner.SetAnimParameter( "move_speed", Velocity.Length );
		Owner.SetAnimParameter( "move_groundspeed", Velocity.WithZ( 0 ).Length );
		Owner.SetAnimParameter( "move_y", sideward );
		Owner.SetAnimParameter( "move_x", forward );
	}

	public void WithWishVelocity( Vector3 Velocity )
	{
		var dir = Velocity;
		var forward = Owner.Rotation.Forward.Dot( dir );
		var sideward = Owner.Rotation.Right.Dot( dir );

		var angle = MathF.Atan2( sideward, forward ).RadianToDegree().NormalizeDegrees();

		Owner.SetAnimParameter( "wish_direction", angle );
		Owner.SetAnimParameter( "wish_speed", Velocity.Length );
		Owner.SetAnimParameter( "wish_groundspeed", Velocity.WithZ( 0 ).Length );
		Owner.SetAnimParameter( "wish_y", sideward );
		Owner.SetAnimParameter( "wish_x", forward );
	}

	public void WithNavStatus( NavSteer steer )
	{
		bool hasPath = steer != null && !steer.Output.Finished && (steer.Target.Distance( Owner.Position ) > 10);
		Owner.SetAnimParameter( "HasAPath", hasPath );
		Owner.SetAnimParameter( "Has a Path", hasPath );
	}
}

