using CustomCharacter.UI;

namespace CustomCharacter;

public partial class Hud : HudEntity<RootPanel>
{
	public Hud()
	{
		if ( !Game.IsClient )
			return;

		RootPanel.StyleSheet.Load( "/UI/Hud.scss" );
		RootPanel.AddChild<Chat>();
		RootPanel.AddChild<Info>();
		RootPanel.AddChild<Players>();
		RootPanel.AddChild<Crosshair>();
	}
}
