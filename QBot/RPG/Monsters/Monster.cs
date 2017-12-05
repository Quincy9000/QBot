using System.Collections.Generic;
using System.Data;
using QBot;

namespace QuincyBot.RPG.Monsters
{
	public class Monster
	{
		public static Dictionary<string, MonsterTemplate> Monsters { get; } = new Dictionary<string, MonsterTemplate>();

		static Monster()
		{
			var ds = DataAccess.FillDataSet("SELECT * FROM Enemies;", DataAccess.DataPath("Data/Enemies.sqlite")).Result;
			foreach(DataRow row in ds.Tables[0].Rows)
			{
				var mt = new MonsterTemplate
				{
					Name = row["Name"].ToString(),
					Physique = int.Parse(row["Physique"].ToString()),
					Technique = int.Parse(row["Technique"].ToString()),
					Mystique = int.Parse(row["Mystique"].ToString()),
					Level = int.Parse(row["Level"].ToString()),
					ExpDrop = int.Parse(row["ExpDrops"].ToString()),
				};
				Monsters.Add(mt.Name, mt);
			}
		}

        private string _name;

		public string Name
		{
			get
			{
				var s = "";
				if(!string.IsNullOrEmpty(Prefix))
					s += $"{Prefix} ";
				s += _name;
				if(!string.IsNullOrEmpty(Suffix))
					s += $" {Suffix}";
				return s;
			}
			set => _name = value;
		}

		public string Prefix { get; set; }

		public string Suffix { get; set; }

		public int Level { get; set; }

		public int ExpDrop { get; set; }

		public Attributes Stats { get; set; }

        private static readonly string[] Prefixes =
		{
			"present", "raspy", "capricious", "lavish", "agreeable", "early", "marked", "dry", "panicky", "merciful", "curious", "awesome", "slimy", "heavy", "bizarre", "statuesque", "bad", "unruly", "greasy", "false", "intelligent", "hissing", "nonchalant", "robust", "nifty", "frequent", "supreme", "curly", "redundant", "voiceless", "awful", "dreary", "reminiscent", "futuristic", "worthless", "swift", "telling", "sticky", "goofy", "powerful", "lame", "cool", "jaded", "dazzling", "left", "furtive", "screeching", "fast", "resolute", "drunk", "envious", "strong", "unusual", "sharp", "bouncy", "kaput", "alike", "eager", "meaty", "used", "cowardly", "recondite", "fertile", "selfish", "psychedelic", "busy", "seemly", "crooked", "numberless", "small", "knowledgeable", "steep", "shiny", "giddy", "yielding", "assorted", "tacit", "rude", "melodic", "obsequious", "sedate", "noisy", "magnificent", "salty", "arrogant", "best", "dysfunctional", "massive", "able", "changeable", "thick", "wise", "quizzical", "maniacal", "earthy", "serious", "capable"
		};

        private static readonly string[] Suffixes =
		{
			"ants", "night", "apparatus", "desire", "need", "furniture", "purpose", "animal", "bucket", "underwear", "decision", "cars", "trousers", "arm", "growth", "cakes", "kettle", "monkey", "chess", "deer", "son", "spy", "rhythm", "harbor", "ring", "produce", "veil", "canvas", "knot", "account", "lunchroom", "brass", "fang", "cellar", "language", "condition", "theory", "attack", "art", "screw", "secretary", "step", "river", "stocking", "hot", "cracker", "arch", "drop", "crowd", "income", "agreement", "oil", "dogs", "geese", "heat", "straw", "end", "detail", "yak", "cushion", "dime", "metal", "border", "downtown", "bulb", "fold", "doctor", "order", "mice", "jail", "branch", "bedroom", "ball", "water", "start", "vest", "pet", "rabbits", "year", "tongue", "root", "sneeze", "muscle", "flesh", "writer", "picture", "stamp", "rock", "fish", "flame", "woman", "maid", "adjustment", "station", "umbrella", "example", "thunder", "belief", "mitten", "line", "digestion", "market", "part", "collar", "glove", "vacation", "home", "push", "fork", "horse", "advertisement", "slave", "quiet", "waves", "knee", "dress", "partner", "back", "party", "humor", "cake", "dinosaurs", "grade", "mark", "amount", "smell", "range", "sleep", "children", "thrill", "toes", "science", "chin", "ship", "ladybug", "tray", "nut", "cabbage", "size", "amusement", "lip", "tent", "loaf", "laugh", "blow", "frame", "transport", "business", "meat", "experience", "drink", "friction", "pipe", "achiever", "pies", "teaching", "hospital", "yard", "seat", "event", "respect", "self", "competition", "judge", "leather", "company", "wax", "cheese", "care", "weather", "lake", "swing", "wheel", "creature", "office", "rake", "haircut", "babies", "guitar", "dirt", "cemetery", "fire", "system", "tiger", "support", "visitor", "bath", "rings", "rifle", "car", "design", "edge", "wrist", "reaction", "slope", "wall", "cart", "ground", "match", "hat", "juice", "burst", "finger", "spade", "plate", "afterthought", "wind", "cattle", "vessel", "hand", "nose", "property", "shop", "fireman", "apparel", "bat", "girl", "sail", "cough", "arithmetic", "rainstorm", "whistle", "death", "acoustics", "coast", "impulse", "record", "trucks", "jeans", "authority", "truck", "face", "cap", "pear", "stretch", "relation", "pancake", "existence", "twist", "rabbit", "pen", "birds", "page", "rose", "error", "men", "tin", "coach", "price", "lamp", "pizzas", "history", "tax", "request", "porter", "bait", "table", "show", "vase", "rain", "kittens", "pigs", "swim", "scent", "snow", "unit", "protest", "jump", "destruction", "crown", "marble", "afternoon", "hook", "expert", "kick", "tooth", "aunt", "giraffe", "steel", "snakes", "passenger", "meeting", "word", "top", "bell", "yam", "nest", "expansion", "position", "wine", "harmony", "town", "direction", "cloth", "hate", "creator", "ear", "dinner", "shirt", "shape", "battle", "baby", "number", "winter", "baseball", "cannon", "corn", "engine", "bird", "anger", "steam", "badge", "bikes", "angle", "snake", "beds", "food", "taste", "education", "button", "tomatoes", "pull", "grandmother", "change", "minister", "eyes", "scarecrow", "form", "rail", "eye", "industry", "month", "key", "stone", "linen", "beginner", "knowledge", "noise", "boundary", "donkey", "pocket", "statement", "songs", "health", "dolls", "current", "title", "ghost", "lunch", "society", "quill", "building", "doll", "plough", "sponge", "pin", "shoe", "basketball", "cover", "drawer", "tub", "cent", "earthquake", "circle", "measure", "frog", "song", "caption", "thumb", "ducks", "skin", "letter", "route", "plane", "shoes", "sofa", "curve", "control", "crook", "advice", "shock", "cause", "hour", "trail", "smile", "silk", "base", "soap", "snail", "nation", "bubble", "kiss", "discovery", "sheet", "whip", "robin", "sky", "group", "wing", "kitty", "aftermath", "team", "spark", "quarter", "friend", "quilt", "comparison", "channel", "cave", "blade", "oranges", "bite", "squirrel", "cub", "card", "addition", "shame", "lock", "question", "rule", "governor", "grip", "pollution", "development", "playground", "soup", "berry", "smash", "increase", "camp", "wash", "thought", "bone", "work", "voyage", "spot", "love", "bushes", "trip", "houses", "magic", "turn", "wren", "zephyr", "front", "crack", "rub", "moon", "floor", "cherries", "string", "chicken", "instrument", "legs", "snails", "stomach", "curtain", "spoon", "carpenter", "sign", "potato", "ticket", "value", "stove", "train", "store", "bear", "blood", "crime", "chickens", "horses", "dad", "desk", "silver", "sweater", "flower", "surprise", "sisters", "mountain", "camera", "trees", "friends", "skirt", "hair", "crib", "look", "color", "room", "sort", "place", "fuel", "mouth", "birth", "twig", "trains", "pickle", "week", "quince", "oven", "recess", "wave", "move", "approval", "limit", "hydrant", "van", "morning", "wool", "sheep", "bells", "uncle", "clover", "field", "fact", "summer", "railway", "sock", "sticks", "fruit", "war", "reason", "gun", "basket", "birthday", "school", "scarf", "mom", "hobbies", "leg", "wound", "act", "scene", "ocean", "government", "church", "flight", "thread", "jewel", "mint", "daughter", "appliance", "sand", "grass", "bed", "bee", "cast", "jar", "action", "toys", "loss", "cows", "sidewalk", "celery", "feeling", "airport", "fall", "liquid", "offer", "cactus", "writing", "distance", "clam", "mass", "shelf", "toe", "eggs", "territory", "side", "coal", "sun", "slip", "sack", "women", "paper", "calculator", "toad", "person", "cable", "use", "interest", "cobweb", "play", "chance", "rat", "fowl", "ink", "roll", "teeth", "stop", "island", "view", "meal", "square", "throat", "behavior", "pest", "copper", "nerve", "door", "head", "basin", "scissors", "farm", "giants", "pleasure", "country", "quartz", "texture", "attraction", "calendar", "flavor", "point", "fairies", "pig", "join", "reading", "driving", "cats", "wood", "balance", "gate", "plantation", "mist", "pot", "soda", "machine", "bead", "wrench", "scale", "bit", "sleet", "sink", "holiday", "notebook", "earth", "regret", "coat", "note", "middle", "watch", "cat", "substance", "low", "zebra", "money", "treatment", "temper", "boy", "crow", "hammer", "zinc", "receipt", "quiver", "letters", "peace", "effect", "smoke", "jam", "box", "trouble", "representative", "airplane", "selection", "mine", "hill", "bridge", "punishment", "quicksand", "rest", "honey", "road", "icicle", "neck", "wilderness", "boot", "turkey", "orange", "hands", "beef", "star", "weight", "rate", "skate", "gold", "queen", "egg", "pie", "cook", "lace", "suit", "board", "grape", "elbow", "stitch", "spring", "duck", "carriage", "tramp", "sound", "stream", "cup", "mask", "dog", "jellyfish", "powder", "fly", "servant", "stew", "touch", "brother", "verse", "wish", "club", "foot", "rice", "lettuce", "grandfather", "tail", "books", "level", "popcorn", "exchange", "needle", "locket", "thing", "brick", "street", "mind", "iron", "profit", "things", "planes", "guide", "sugar", "toothbrush", "dock", "way", "can", "shake", "butter", "girls", "plants", "jelly", "prose", "seashore", "voice", "believe", "memory", "flowers", "invention", "army", "drain", "fog", "sister", "bike", "minute", "knife", "insurance", "yoke", "sea", "payment", "observation", "chalk", "stem", "rod", "seed", "power", "trick", "north", "tendency", "lumber", "throne", "flag", "ray", "actor", "bomb", "wire", "dust", "roof", "breath", "hope", "religion", "hall", "window", "brake", "alarm", "crate", "reward", "horn", "hose", "distribution", "walk", "day", "house", "milk", "book", "bag", "poison", "plant", "pets", "mailbox", "wealth", "riddle", "crayon", "suggestion", "credit", "name", "stranger", "volcano", "toothpaste", "pump", "shade", "activity", "eggnog", "spiders", "division", "waste", "library", "cream", "man", "ice", "committee", "connection", "stage", "mother", "idea", "coil", "space", "bottle", "structure", "boat", "frogs", "plot", "grain", "flock", "test", "air", "parcel", "toy", "insect", "vein", "story", "zipper", "sense", "volleyball", "debt", "run", "cow", "vegetable", "finger", "salt", "trade", "degree", "yarn", "oatmeal", "hole", "fear", "time", "tank", "pan", "land", "class", "discussion", "motion", "worm", "pail", "stick", "pencil", "argument", "laborer", "tree", "plastic", "talk", "cherry", "zoo", "force", "glass"
		};

		public Monster(string name)
		{
			_name = name;
			var s = Prefixes[Program.R.Next(0, Prefixes.Length)];
			var c = s.ToCharArray();
			c[0] = char.ToUpper(c[0]);
			Prefix = new string(c);
			c = Suffixes[Program.R.Next(0, Suffixes.Length)].ToCharArray();
			c[0] = char.ToUpper(c[0]);
			Suffix = $"of {new string(c)}";
			Level = 0;
			Stats = new Attributes();
		}
	}
}