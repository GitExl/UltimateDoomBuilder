
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder.Config
{
	public class GameConfiguration
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Original configuration
		private Configuration cfg;
		
		// General settings
		private float defaulttexturescale;
		private float defaultflatscale;
		private string formatinterface;
		private int soundlinedefflags;
		private int singlesidedflags;
		private int doublesidedflags;
		private int impassableflags;
		private bool mixtexturesflats;
		private bool generalizedactions;
		private bool generalizedeffects;
		private int start3dmodethingtype;
		
		// Map lumps
		private IDictionary maplumpnames;
		
		// Texture/flat sources
		private IDictionary textureranges;
		private IDictionary flatranges;
		
		// Things
		private Dictionary<int, string> thingflags;
		private List<ThingCategory> thingcategories;
		private Dictionary<int, ThingTypeInfo> things;
		
		// Linedefs
		private Dictionary<int, string> linedefflags;
		private Dictionary<int, LinedefActionInfo> linedefactions;
		private List<LinedefActionInfo> sortedlinedefactions;
		private List<LinedefActionCategory> actioncategories;
		private List<LinedefActivateInfo> linedefactivates;
		private List<GeneralizedCategory> genactioncategories;
		
		// Sectors
		private Dictionary<int, SectorEffectInfo> sectoreffects;
		private List<SectorEffectInfo> sortedsectoreffects;
		private List<GeneralizedOption> geneffectoptions;

		// Universal fields
		private List<UniversalFieldInfo> linedeffields;
		private List<UniversalFieldInfo> sectorfields;
		
		#endregion

		#region ================== Properties

		// General settings
		public float DefaultTextureScale { get { return defaulttexturescale; } }
		public float DefaultFlatScale { get { return defaultflatscale; } }
		public string FormatInterface { get { return formatinterface; } }
		public int SoundLinedefFlags { get { return soundlinedefflags; } }
		public int SingleSidedFlags { get { return singlesidedflags; } }
		public int DoubleSidedFlags { get { return doublesidedflags; } }
		public int ImpassableFlags { get { return impassableflags; } }
		public bool MixTexturesFlats { get { return mixtexturesflats; } }
		public bool GeneralizedActions { get { return generalizedactions; } }
		public bool GeneralizedEffects { get { return generalizedeffects; } }
		public int Start3DModeThingType { get { return start3dmodethingtype; } }
		
		// Map lumps
		public IDictionary MapLumpNames { get { return maplumpnames; } }

		// Texture/flat sources
		public IDictionary TextureRanges { get { return textureranges; } }
		public IDictionary FlatRanges { get { return flatranges; } }

		// Things
		public IDictionary<int, string> ThingFlags { get { return thingflags; } }
		public List<ThingCategory> ThingCategories { get { return thingcategories; } }
		public ICollection<ThingTypeInfo> Things { get { return things.Values; } }
		
		// Linedefs
		public IDictionary<int, string> LinedefFlags { get { return linedefflags; } }
		public IDictionary<int, LinedefActionInfo> LinedefActions { get { return linedefactions; } }
		public List<LinedefActionInfo> SortedLinedefActions { get { return sortedlinedefactions; } }
		public List<LinedefActionCategory> ActionCategories { get { return actioncategories; } }
		public List<LinedefActivateInfo> LinedefActivates { get { return linedefactivates; } }
		public List<GeneralizedCategory> GenActionCategories { get { return genactioncategories; } }

		// Sectors
		public IDictionary<int, SectorEffectInfo> SectorEffects { get { return sectoreffects; } }
		public List<SectorEffectInfo> SortedSectorEffects { get { return sortedsectoreffects; } }
		public List<GeneralizedOption> GenEffectOptions { get { return geneffectoptions; } }

		// Universal fields
		public List<UniversalFieldInfo> LinedefFields { get { return linedeffields; } }
		public List<UniversalFieldInfo> SectorFields { get { return sectorfields; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal GameConfiguration(Configuration cfg)
		{
			// Initialize
			this.cfg = cfg;
			this.thingflags = new Dictionary<int, string>();
			this.thingcategories = new List<ThingCategory>();
			this.things = new Dictionary<int, ThingTypeInfo>();
			this.linedefflags = new Dictionary<int, string>();
			this.linedefactions = new Dictionary<int, LinedefActionInfo>();
			this.actioncategories = new List<LinedefActionCategory>();
			this.sortedlinedefactions = new List<LinedefActionInfo>();
			this.linedefactivates = new List<LinedefActivateInfo>();
			this.genactioncategories = new List<GeneralizedCategory>();
			this.sectoreffects = new Dictionary<int, SectorEffectInfo>();
			this.sortedsectoreffects = new List<SectorEffectInfo>();
			this.geneffectoptions = new List<GeneralizedOption>();
			
			// Read general settings
			defaulttexturescale = cfg.ReadSetting("defaulttexturescale", 1f);
			defaultflatscale = cfg.ReadSetting("defaultflatscale", 1f);
			formatinterface = cfg.ReadSetting("formatinterface", "");
			soundlinedefflags = cfg.ReadSetting("soundlinedefflags", 0);
			singlesidedflags = cfg.ReadSetting("singlesidedflags", 0);
			doublesidedflags = cfg.ReadSetting("doublesidedflags", 0);
			impassableflags = cfg.ReadSetting("impassableflags", 0);
			mixtexturesflats = cfg.ReadSetting("mixtexturesflats", false);
			generalizedactions = cfg.ReadSetting("generalizedlinedefs", false);
			generalizedeffects = cfg.ReadSetting("generalizedsectors", false);
			start3dmodethingtype = cfg.ReadSetting("start3dmode", 0);
			
			// Get map lumps
			maplumpnames = cfg.ReadSetting("maplumpnames", new Hashtable());

			// Get texture and flat sources
			textureranges = cfg.ReadSetting("textures", new Hashtable());
			flatranges = cfg.ReadSetting("flats", new Hashtable());
			
			// Things
			LoadThingFlags();
			LoadThingCategories();
			
			// Linedefs
			LoadLinedefFlags();
			LoadLinedefActions();
			LoadLinedefActivations();
			LoadLinedefGeneralizedActions();

			// Sectors
			LoadSectorEffects();
			LoadSectorGeneralizedEffects();
			
			// Universal fields
			linedeffields = LoadUniversalFields("linedefs");
			sectorfields = LoadUniversalFields("sectors");
		}

		// Destructor
		~GameConfiguration()
		{
			foreach(ThingCategory tc in thingcategories) tc.Dispose();
			foreach(LinedefActionCategory ac in actioncategories) ac.Dispose();
		}
		
		#endregion

		#region ================== Loading

		// This loads a universal fields list
		private List<UniversalFieldInfo> LoadUniversalFields(string elementname)
		{
			List<UniversalFieldInfo> list = new List<UniversalFieldInfo>();
			UniversalFieldInfo uf;
			IDictionary dic;
			
			// Get fields
			dic = cfg.ReadSetting("universalfields." + elementname, new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				try
				{
					// Read the field info and add to list
					uf = new UniversalFieldInfo(elementname, de.Key.ToString(), cfg);
					list.Add(uf);
				}
				catch(Exception)
				{
					General.WriteLogLine("WARNING: Unable to read universal field definition 'universalfields." + elementname + "." + de.Key + "'!");
				}
			}

			// Return result
			return list;
		}
		
		// Things and thing categories
		private void LoadThingCategories()
		{
			IDictionary dic;
			ThingCategory thingcat;
			
			// Get thing categories
			dic = cfg.ReadSetting("thingtypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Make a category
				thingcat = new ThingCategory(cfg, de.Key.ToString());

				// Add all things in category to the big list
				foreach(ThingTypeInfo t in thingcat.Things) things.Add(t.Index, t);

				// Add category to list
				thingcategories.Add(thingcat);
			}
		}
		
		// Linedef flags
		private void LoadLinedefFlags()
		{
			IDictionary dic;
			int bitflagscheck = 0;
			int bitvalue;
			
			// Get linedef flags
			dic = cfg.ReadSetting("linedefflags", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Try paring the bit value
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out bitvalue))
				{
					// Check for conflict and add to list
					if((bitvalue & bitflagscheck) == 0)
						linedefflags.Add(bitvalue, de.Value.ToString());
					else
						General.WriteLogLine("WARNING: Structure 'linedefflags' contains conflicting bit flag keys. Make sure all keys are unique integers and powers of 2!");
						
					// Update bit flags checking value
					bitflagscheck |= bitvalue;
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'linedefflags' contains invalid keys!");
				}
			}
		}

		// Linedef actions and action categories
		private void LoadLinedefActions()
		{
			Dictionary<string, LinedefActionCategory> cats = new Dictionary<string, LinedefActionCategory>();
			IDictionary dic;
			LinedefActionInfo ai;
			LinedefActionCategory ac;
			int actionnumber;
			
			// Get linedef actions
			dic = cfg.ReadSetting("linedeftypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Try paring the action number
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out actionnumber))
				{
					// Expanded type?
					if(de.Value is IDictionary)
					{
						// Let the class constructure read it
						ai = new LinedefActionInfo(actionnumber, cfg);
					}
					else
					{
						// We have all the information in one string (title/prefix only)
						ai = new LinedefActionInfo(actionnumber, de.Value.ToString());
					}

					// Make or get a category
					if(cats.ContainsKey(ai.Category))
						ac = cats[ai.Category];
					else
					{
						ac = new LinedefActionCategory(ai.Category);
						cats.Add(ai.Category, ac);
					}
					
					// Add action to category and sorted list
					sortedlinedefactions.Add(ai);
					linedefactions.Add(actionnumber, ai);
					ac.Add(ai);
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'linedeftypes' contains invalid keys!");
				}
			}

			// Sort the actions list
			sortedlinedefactions.Sort();
			
			// Copy categories to final list
			actioncategories.Clear();
			actioncategories.AddRange(cats.Values);

			// Sort the categories list
			actioncategories.Sort();
		}

		// Linedef activates
		private void LoadLinedefActivations()
		{
			IDictionary dic;
			int bitvalue;

			// Get linedef activations
			dic = cfg.ReadSetting("linedefactivations", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Try paring the bit value
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out bitvalue))
				{
					// Add to the list
					linedefactivates.Add(new LinedefActivateInfo(bitvalue, de.Value.ToString()));
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'linedefactivations' contains invalid keys!");
				}
			}

			// Sort the list
			linedefactivates.Sort();
		}

		// Linedef generalized actions
		private void LoadLinedefGeneralizedActions()
		{
			IDictionary dic;

			// Get linedef activations
			dic = cfg.ReadSetting("gen_linedeftypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check for valid structure
				if(de.Value is IDictionary)
				{
					// Add category
					genactioncategories.Add(new GeneralizedCategory("gen_linedeftypes", de.Key.ToString(), cfg));
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'gen_linedeftypes' contains invalid entries!");
				}
			}
		}

		// Sector effects
		private void LoadSectorEffects()
		{
			IDictionary dic;
			SectorEffectInfo si;
			int actionnumber;
			
			// Get sector effects
			dic = cfg.ReadSetting("sectortypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Try paring the action number
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out actionnumber))
				{
					// Make effects
					si = new SectorEffectInfo(actionnumber, de.Value.ToString());
					
					// Add action to category and sorted list
					sortedsectoreffects.Add(si);
					sectoreffects.Add(actionnumber, si);
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'sectortypes' contains invalid keys!");
				}
			}

			// Sort the actions list
			sortedsectoreffects.Sort();
		}

		// Sector generalized effects
		private void LoadSectorGeneralizedEffects()
		{
			IDictionary dic;

			// Get sector effects
			dic = cfg.ReadSetting("gen_sectortypes", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Check for valid structure
				if(de.Value is IDictionary)
				{
					// Add option
					geneffectoptions.Add(new GeneralizedOption("gen_sectortypes", "", de.Key.ToString(), de.Value as IDictionary));
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'gen_sectortypes' contains invalid entries!");
				}
			}
		}

		// Thing flags
		private void LoadThingFlags()
		{
			IDictionary dic;
			int bitflagscheck = 0;
			int bitvalue;

			// Get linedef flags
			dic = cfg.ReadSetting("thingflags", new Hashtable());
			foreach(DictionaryEntry de in dic)
			{
				// Try paring the bit value
				if(int.TryParse(de.Key.ToString(),
					NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
					CultureInfo.InvariantCulture, out bitvalue))
				{
					// Check for conflict and add to list
					if((bitvalue & bitflagscheck) == 0)
						thingflags.Add(bitvalue, de.Value.ToString());
					else
						General.WriteLogLine("WARNING: Structure 'thingflags' contains conflicting bit flag keys. Make sure all keys are unique integers and powers of 2!");

					// Update bit flags checking value
					bitflagscheck |= bitvalue;
				}
				else
				{
					General.WriteLogLine("WARNING: Structure 'thingflags' contains invalid keys!");
				}
			}
		}
		
		#endregion

		#region ================== Methods

		// ReadSetting
		public string ReadSetting(string setting, string defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public int ReadSetting(string setting, int defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public float ReadSetting(string setting, float defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public short ReadSetting(string setting, short defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public long ReadSetting(string setting, long defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public bool ReadSetting(string setting, bool defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public byte ReadSetting(string setting, byte defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		public IDictionary ReadSetting(string setting, IDictionary defaultsetting) { return cfg.ReadSetting(setting, defaultsetting); }
		
		// This gets thing information by index
		public ThingTypeInfo GetThingInfo(int thingtype)
		{
			// Index in config?
			if(things.ContainsKey(thingtype))
			{
				// Return from config
				return things[thingtype];
			}
			else
			{
				// Create unknown thing info
				return new ThingTypeInfo(thingtype);
			}
		}

		// This gets thing information by index
		// Returns null when thing type info could not be found
		public ThingTypeInfo GetThingInfoEx(int thingtype)
		{
			// Index in config?
			if(things.ContainsKey(thingtype))
			{
				// Return from config
				return things[thingtype];
			}
			else
			{
				// No such thing type known
				return null;
			}
		}
		
		// This checks if an action is generalized or predefined
		public static bool IsGeneralized(int action, List<GeneralizedCategory> categories)
		{
			// Only actions above 0
			if(action > 0)
			{
				// Go for all categories
				foreach(GeneralizedCategory ac in categories)
				{
					// Check if the action is within range of this category
					if((action >= ac.Offset) && (action < (ac.Offset + ac.Length))) return true;
				}
			}

			// Not generalized
			return false;
		}

		// This gets the generalized action category from action number
		public GeneralizedCategory GetGeneralizedActionCategory(int action)
		{
			// Only actions above 0
			if(action > 0)
			{
				// Go for all categories
				foreach(GeneralizedCategory ac in genactioncategories)
				{
					// Check if the action is within range of this category
					if((action >= ac.Offset) && (action < (ac.Offset + ac.Length))) return ac;
				}
			}

			// Not generalized
			return null;
		}
		
		// This checks if a specific edit mode class is listed
		public bool IsEditModeSpecified(string classname)
		{
			return cfg.SettingExists("editingmodes." + classname.ToString(CultureInfo.InvariantCulture));
		}
		
		#endregion
	}
}
