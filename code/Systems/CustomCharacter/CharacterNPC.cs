namespace CustomCharacter;

[Library( "npc_test", Title = "Npc Test" ), Model( Model = "models/biped_standard/biped_standard.vmdl" ), HammerEntity]
public partial class NpcTest : AnimatedEntity
{
	[ConVar.Replicated]
	public static bool nav_drawpath { get; set; }


	public Vector3 EyePosition => Position + Vector3.Up * 64;

	[ConCmd.Server( "npc_clear" )]
	public static void NpcClear()
	{
		foreach ( var npc in Entity.All.OfType<NpcTest>().ToArray() )
			npc.Delete();
	}


	float Speed;

	MovePath Path = new MovePath();
	public NavSteer Steer;

	public override void Spawn()
	{
		base.Spawn();
		SetupPhysicsFromCapsule( PhysicsMotionType.Keyframed, Capsule.FromHeightAndRadius( 72, 8 ) );
		SetMaterialGroup( Game.Random.Int( 0, 3 ) );
		SetBodyGroup( 1, 0 );

		EnableHitboxes = true;
		Speed = Game.Random.Float( 100, 300 );
	}

	public Sandbox.Debug.Draw Draw => Sandbox.Debug.Draw.Once;

	Vector3 InputVelocity;

	Vector3 LookDir;

	[GameEvent.Tick.Server]
	public void Tick()
	{
		using var _a = Sandbox.Debug.Profile.Scope( "NpcTest::Tick" );

		InputVelocity = 0;

		if ( Steer != null )
		{
			using var _b = Sandbox.Debug.Profile.Scope( "Steer" );

			Steer.Tick( Position );

			if ( !Steer.Output.Finished )
			{
				InputVelocity = Steer.Output.Direction.Normal;
				Velocity = Velocity.AddClamped( InputVelocity * Time.Delta * 500, Speed );
				if ( nav_drawpath )
				{
					Steer.DebugDrawPath();
				}
			}
			else
			{
				//Steer = null;
			}


		}

		using ( Sandbox.Debug.Profile.Scope( "Move" ) )
		{
			Move( Time.Delta );
		}

		var walkVelocity = Velocity.WithZ( 0 );
		if ( walkVelocity.Length > 0.5f )
		{
			var turnSpeed = walkVelocity.Length.LerpInverse( 0, 100, true );
			var targetRotation = Rotation.LookAt( walkVelocity.Normal, Vector3.Up );
			Rotation = Rotation.Lerp( Rotation, targetRotation, turnSpeed * Time.Delta * 20.0f );
		}

		var animHelper = new CustomHelper( this );

		LookDir = Vector3.Lerp( LookDir, InputVelocity.WithZ( 0 ) * 1000, Time.Delta * 100.0f );
		animHelper.WithLookAt( EyePosition + LookDir );
		animHelper.WithVelocity( Velocity );
		animHelper.WithWishVelocity( InputVelocity );
		animHelper.WithNavStatus( Steer );
	}

	protected virtual void Move( float timeDelta )
	{
		var bbox = BBox.FromHeightAndRadius( 64, 4 );
		//DebugOverlay.Box( Position, bbox.Mins, bbox.Maxs, Color.Green );

		MoveHelper move = new( Position, Velocity );
		move.MaxStandableAngle = 50;
		move.Trace = move.Trace.Ignore( this ).Size( bbox );

		if ( !Velocity.IsNearlyZero( 0.001f ) )
		{
			//	Sandbox.Debug.Draw.Once
			//						.WithColor( Color.Red )
			//						.IgnoreDepth()
			//						.Arrow( Position, Position + Velocity * 2, Vector3.Up, 2.0f );

			using ( Sandbox.Debug.Profile.Scope( "TryUnstuck" ) )
				move.TryUnstuck();

			using ( Sandbox.Debug.Profile.Scope( "TryMoveWithStep" ) )
				move.TryMoveWithStep( timeDelta, 30 );
		}

		using ( Sandbox.Debug.Profile.Scope( "Ground Checks" ) )
		{
			var tr = move.TraceDirection( Vector3.Down * 10.0f );

			if ( move.IsFloor( tr ) )
			{
				GroundEntity = tr.Entity;

				if ( !tr.StartedSolid )
				{
					move.Position = tr.EndPosition;
				}

				if ( InputVelocity.Length > 0 )
				{
					var movement = move.Velocity.Dot( InputVelocity.Normal );
					move.Velocity = move.Velocity - movement * InputVelocity.Normal;
					move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
					move.Velocity += movement * InputVelocity.Normal;
				}
				else
				{
					move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
				}
			}
			else
			{
				GroundEntity = null;
				move.Velocity += Vector3.Down * 900 * timeDelta;
				Sandbox.Debug.Draw.Once.WithColor( Color.Red ).Circle( Position, Vector3.Up, 10.0f );
			}
		}

		Rotation *= Rotation.FromYaw( (RootMotionAngle * Time.Delta) );
		Position += (Rotation * RootMotion) * Time.Delta;
		Position = Position.WithZ( move.Position.z );
		Velocity = move.Velocity;
	}

	public void SetExample( int i )
	{
		SetAnimParameter( "Active Example", i );
	}
}
