using System;
using System.IO;
using System.Reflection;
using UnityEngine;

using Log = KSPPP.Logging.ConsoleLogger;

namespace KSPPP
{
	public static class String_ {
		public static void Dispose(this String s, bool f) {
			// nothing need be done
		}
	}
	public class Icon {


		private static Rect pos_icon = new Rect(0,0,0,0);
		private static Rect grid_pos;

		private static string WhereIsKSPPP_DLL () { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }

		public static Texture2D octIcons = new Texture2D(1 * 32, 177 * 32, TextureFormat.ARGB32, false);
		public static Texture2D planetIcons = new Texture2D(1 * 26, 16 * 26, TextureFormat.ARGB32, false);


		public static void loadAssets() {
			Log.Now("trying to load textures");

			try {
				var ret = Icon.LoadImageFromFile(ref octIcons);
				var ret2= Icon.LoadImageFromFile(ref planetIcons,"planeticons.png");
				if (!ret)
					Log.Now("Failed to load octIcons");
				if (!ret2)
					Log.Now("Failed to load planetIcons");
			} catch(Exception e) {
				Log.Now("Failed to load textures. File location error: {0} ",e);
			}
		}

		public static bool LoadImageFromFile(ref Texture2D tex, string name = "octicons.png", string path = "./GameData/KSPPP") {

			var location = string.Format("{0}/{1}",path, name);
			if (File.Exists(location))
				return tex.LoadImage(File.ReadAllBytes(location));
			else
				Log.Now("File.Exists() failed with location: {0}", location);
			return false;
		}

		public static void drawIconAt (int x, int y, Icons i, Color c, int size = 32 /*px*/, bool outline = false) {
			var old = GUI.color;

			// PX [0..n]
			// ORIGIN: NorthWest
			pos_icon.x			= (x - size) / 2;
			pos_icon.y			= (y - size) / 2;
			pos_icon.width	= size;
			pos_icon.height = size;

			// UV [0..1]
			// Origin: SouthWest
			grid_pos.width	= 1f;	// 32 px
			grid_pos.height = (float)(1/size);	// 1/177th
			grid_pos.x			= 0f; //(float)(1/size);	// 1/177th
			grid_pos.y			= 0f; // (float)(1/size) * 1/(177 - ((int) i));

			GUI.color = c;
			GUI.DrawTextureWithTexCoords(pos_icon, octIcons, grid_pos);
			GUI.color = old;
		}

		public static void drawIcon (Icons i, Color c, int size = 32 /*px*/, bool outline = false) {
			var old = GUI.color;

			// PX [0..n]
			// ORIGIN: NorthWest
			pos_icon.x			= 0;
			pos_icon.y			= 0;
			pos_icon.width	= size;
			pos_icon.height = size;

			// UV [0..1]
			// Origin: SouthWest
			grid_pos.width	= 1f;	// 32 px
			grid_pos.height = (float)(1/size);	// 1/177th
			grid_pos.x			= 0f; // (float)(1/size);	// 1/177th
			grid_pos.y			= 0f; // (float)(1/size) * 1/(177 - ((int) i));

			GUI.color = c;
			GUI.DrawTextureWithTexCoords(pos_icon, octIcons, grid_pos);
			GUI.color = old;
		}
	}

	// I am guessing that splitting these up like this will be nice for rendering purposes.
	public enum PlanetIcons {
		Moho,
		Eve,
		Kerbin,
		Duna,
		Dres,
		Jool,
		Eeloo,
	}
	public enum MoonIcons {
		Gilly,
		Mun,
		Minmus,
		Ike,
		Laythe,
		Vall,
		Tylo,
		Bop,
		Pol,
	}
	public enum Icons {
		Heart,				/* = 0 */
		Zap,
		LightBulb,
		Repo,
		RepoForked,
		RepoPush,
		RepoPull,
		Book,
		Octoface,
		GitPullRequest,
		MarkGithub,
		CloudDownload,
		CloudUpload,
		Keyboard,
		Gist,
		FileCode,
		FileText,
		FileMedia,
		FileZip,
		FilePdf,
		Tag,
		FileDirectory,
		FileSubmodule,
		Person,
		Jersey,
		GitCommit,
		GitBranch,
		GitMerge,
		Mirror,
		IssueOpened,
		IssueReopened,
		IssueClosed,
		Star,
		Comment,
		Question,
		Alert,
		Search,
		Gear,
		RadioTower,
		Tools,
		SignOut,
		Rocket,
		Rss,
		Clippy,
		SignIn,
		Organization,
		DeviceMobile,
		Unfold,
		Check,
		Mail,
		MailRead,
		ArrowUp,
		ArrowRight,
		ArrowDown,
		ArrowLeft,
		Pin,
		Gift,
		Graph,
		TriangleLeft,
		CreditCard,
		Clock,
		Ruby,
		Broadcast,
		Key,
		RepoForcePush,
		RepoClone,
		Diff,
		Eye,
		CommentDiscussion,
		MailReply,
		PrimitiveDot,
		PrimitiveSquare,
		DeviceCamera,
		DeviceCameraVideo,
		Pencil,
		Info,
		TriangleRight,
		TriangleDown,
		Link,
		Plus,
		ThreeBars,
		Code,
		Location,
		ListUnordered,
		ListOrdered,
		Quote,
		Versions,
		ColorMode,
		ScreenFull,
		ScreenNormal,
		Calendar,
		Beer,
		Lock,
		DiffAdded,
		DiffRemoved,
		DiffModified,
		DiffRenamed,
		HorizontalRule,
		ArrowSmallRight,
		JumpDown,
		JumpUp,
		MoveLeft,
		Milestone,
		Checklist,
		Megaphone,
		ChevronRight,
		Bookmark,
		Settings,
		Dashboard,
		History,
		LinkExternal,
		Mute,
		X,
		CircleSlash,
		Pulse,
		Sync,
		Telescope,
		Microscope,
		AlignmentAlign,
		AlignmentUnalign,
		GistSecret,
		Home,
		AlignmentAlignedTo,
		Stop,
		Bug,
		FileBinary,
		Database,
		Server,
		DiffIgnored,
		Ellipsis,
		NoNewline,
		Hubot,
		Hourglass,
		ArrowSmallUp,
		ArrowSmallDown,
		ArrowSmallLeft,
		ChevronUp,
		ChevronDown,
		ChevronLeft,
		JumpLeft,
		JumpRight,
		MoveUp,
		MoveDown,
		MoveRight,
		TriangleUp,
		GitCompare,
		Podium,
		FileSymlinkFile,
		FileSymlinkDirectory,
		Squirrel,
		Globe,
		Unmute,
		PlaybackPause,
		PlaybackRewind,
		PlaybackFastForward,
		Mention,
		PlaybackPlay,
		Puzzle,
		Package,
		Browser,
		Split,
		Steps,
		Terminal,
		Markdown,
		Dash,
		Fold,
		Inbox,
		Trashcan,
		Paintcan,
		Flame,
		Briefcase,
		Plug,
		CircuitBoard,
		MortarBoard,
		Law,
		DeviceDesktop,
		Blank						/* = 177 */
	}

}

