using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zorro.Core.CLI.ParsableTypes;
using Zorro.Core.Serizalization;
using Zorro.Core.SmallShadows;
using Zorro.UI.Modal;

namespace Zorro.Core.CLI
{
	// Token: 0x020003A9 RID: 937
	public class ConsoleCommands
	{
		// Token: 0x0600185E RID: 6238 RVA: 0x0007B8C4 File Offset: 0x00079AC4
		// Note: this type is marked as 'beforefieldinit'.
		static ConsoleCommands()
		{
			List<ConsoleCommand> list = new List<ConsoleCommand>();
			list.Add(new ConsoleCommand(new Action(AchievementManager.ClearAchievements).Method));
			list.Add(new ConsoleCommand(new Action<int>(AchievementManager.GiveAscentLevel).Method));
			list.Add(new ConsoleCommand(new Action<ACHIEVEMENTTYPE>(AchievementManager.Grant).Method));
			list.Add(new ConsoleCommand(new Action(Ascents.LockAll).Method));
			list.Add(new ConsoleCommand(new Action(Ascents.UnlockAll).Method));
			list.Add(new ConsoleCommand(new Action(Ascents.UnlockOne).Method));
			list.Add(new ConsoleCommand(new Action(Backpack.PrintBackpacks).Method));
			list.Add(new ConsoleCommand(new Action(Character.Die).Method));
			list.Add(new ConsoleCommand(new Action(Character.GainFullStamina).Method));
			list.Add(new ConsoleCommand(new Action(Character.InfiniteStamina).Method));
			list.Add(new ConsoleCommand(new Action(Character.LockStatuses).Method));
			list.Add(new ConsoleCommand(new Action(Character.PassOut).Method));
			list.Add(new ConsoleCommand(new Action(Character.Revive).Method));
			list.Add(new ConsoleCommand(new Action(Character.TestWarp).Method));
			list.Add(new ConsoleCommand(new Action(Character.TestWin).Method));
			list.Add(new ConsoleCommand(new Action(Character.WarpToSpawn).Method));
			list.Add(new ConsoleCommand(new Action(Character.Zombify).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.AddCold).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.AddCurse).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.AddDrowsy).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.AddHot).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.AddHunger).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.AddInjury).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.AddPoison).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.AddSpores).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.AddTinyHunger).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.ClearAll).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.ClearAllAilments).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.ClearCold).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.ClearCurse).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.ClearDrowsy).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.ClearHot).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.ClearHunger).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.ClearInjury).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.ClearPoison).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.Die).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.GetThorned).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.GetUnThorned).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.Hungry).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.Starve).Method));
			list.Add(new ConsoleCommand(new Action(CharacterAfflictions.TestExactStatus).Method));
			list.Add(new ConsoleCommand(new Action(CharacterCustomization.Randomize).Method));
			list.Add(new ConsoleCommand(new Action<Item>(ItemDatabase.Add).Method));
			list.Add(new ConsoleCommand(new Action(MapDebugUI.IncrementLevel).Method));
			list.Add(new ConsoleCommand(new Action(MapDebugUI.ToggleDebugText).Method));
			list.Add(new ConsoleCommand(new Action<Segment>(MapHandler.JumpToSegment).Method));
			list.Add(new ConsoleCommand(new Action(PassportManager.TestAllCosmetics).Method));
			list.Add(new ConsoleCommand(new Action<Player>(Player.PrintInventory).Method));
			list.Add(new ConsoleCommand(new Action<int>(ApplicationCLI.SetTargetFramerate).Method));
			list.Add(new ConsoleCommand(new Action(ConsoleSettings.Clear).Method));
			list.Add(new ConsoleCommand(new Action(ConsoleSettings.ClearMuted).Method));
			list.Add(new ConsoleCommand(new Action(ConsoleSettings.Pause).Method));
			list.Add(new ConsoleCommand(new Action<float>(ConsoleSettings.SetDPI).Method));
			list.Add(new ConsoleCommand(new Action(ConsoleSettings.Unpause).Method));
			list.Add(new ConsoleCommand(new Func<ScriptPath, Task>(Script.Execute).Method));
			list.Add(new ConsoleCommand(new Action(IBinarySerializable.EnableLog).Method));
			list.Add(new ConsoleCommand(new Action(SmallShadowHandler.DebugDisable).Method));
			list.Add(new ConsoleCommand(new Action(SmallShadowHandler.DebugEnable).Method));
			list.Add(new ConsoleCommand(new Action(Modal.TestModal).Method));
			ConsoleCommands.ConsoleCommandMethods = list;
			Dictionary<Type, CLITypeParser> dictionary = new Dictionary<Type, CLITypeParser>();
			dictionary.Add(typeof(ACHIEVEMENTTYPE), new AchievementCLIParser());
			dictionary.Add(typeof(Item), new ItemCLIParser());
			dictionary.Add(typeof(bool), new BoolCLIParser());
			dictionary.Add(typeof(byte), new ByteCLIParser());
			dictionary.Add(typeof(float), new FloatCLIParser());
			dictionary.Add(typeof(int), new IntCLIParser());
			dictionary.Add(typeof(ScriptPath), new ScriptPathCLIParser());
			dictionary.Add(typeof(ushort), new UShortCLIParser());
			ConsoleCommands.TypeParsers = dictionary;
		}

		// Token: 0x04001694 RID: 5780
		public static List<ConsoleCommand> ConsoleCommandMethods;

		// Token: 0x04001695 RID: 5781
		public static Dictionary<Type, CLITypeParser> TypeParsers;
	}
}
