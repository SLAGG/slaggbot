using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.GameServers.Valve
{
	/// <remarks>
	/// http://developer.valvesoftware.com/wiki/Steam_Application_IDs
	/// Just source games right now.
	/// </remarks>
	public enum SteamApplication
		: short
	{
		HalfLifeBlueShift = 130,
		HalfLife2 = 220,
		CounterStrikeSource = 240,
		HalfLifeSource = 280,
		DayOfDefeatSource = 300,
		
		TeamFortress2Dedicated = 310,
		TeamFortress2LinuxDedicated = 311,

		/// <summary>
		/// Half Life 2 Deathmatch
		/// </summary>
		HalfLife2DM = 320,
		HalfLife2LostCoast = 340,
		
		/// <summary>
		/// Half Life Deathmatch Source
		/// </summary>
		HalfLifeDMSource = 360,

		HalfLife2EpOne = 380,
		HalfLife2EpTwo = 420,

		Portal = 400,
		TeamFortress2 = 440
	}
}
