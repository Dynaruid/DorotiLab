#nullable enable
#pragma warning disable CS0108, CS0162, CS0168, CS4014, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622
// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/keyboard_key.g.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Services;

public abstract class KeyboardKey : Diagnosticable
{
    protected KeyboardKey()
    {
    }

}

public class LogicalKeyboardKey : KeyboardKey
{
    public virtual long keyId { get; private set; } = default!;
    public const long valueMask = 4294967295L;
    public const long planeMask = 1095216660480L;
    public const long unicodePlane = 0L;
    public const long unprintablePlane = 4294967296L;
    public const long flutterPlane = 8589934592L;
    public const long startOfPlatformPlanes = 73014444032L;
    public const long androidPlane = 73014444032L;
    public const long fuchsiaPlane = 77309411328L;
    public const long iosPlane = 81604378624L;
    public const long macosPlane = 85899345920L;
    public const long gtkPlane = 90194313216L;
    public const long windowsPlane = 94489280512L;
    public const long webPlane = 98784247808L;
    public const long glfwPlane = 103079215104L;
    public static LogicalKeyboardKey space = new LogicalKeyboardKey(32L);
    public static LogicalKeyboardKey exclamation = new LogicalKeyboardKey(33L);
    public static LogicalKeyboardKey quote = new LogicalKeyboardKey(34L);
    public static LogicalKeyboardKey numberSign = new LogicalKeyboardKey(35L);
    public static LogicalKeyboardKey dollar = new LogicalKeyboardKey(36L);
    public static LogicalKeyboardKey percent = new LogicalKeyboardKey(37L);
    public static LogicalKeyboardKey ampersand = new LogicalKeyboardKey(38L);
    public static LogicalKeyboardKey quoteSingle = new LogicalKeyboardKey(39L);
    public static LogicalKeyboardKey parenthesisLeft = new LogicalKeyboardKey(40L);
    public static LogicalKeyboardKey parenthesisRight = new LogicalKeyboardKey(41L);
    public static LogicalKeyboardKey asterisk = new LogicalKeyboardKey(42L);
    public static LogicalKeyboardKey add = new LogicalKeyboardKey(43L);
    public static LogicalKeyboardKey comma = new LogicalKeyboardKey(44L);
    public static LogicalKeyboardKey minus = new LogicalKeyboardKey(45L);
    public static LogicalKeyboardKey period = new LogicalKeyboardKey(46L);
    public static LogicalKeyboardKey slash = new LogicalKeyboardKey(47L);
    public static LogicalKeyboardKey digit0 = new LogicalKeyboardKey(48L);
    public static LogicalKeyboardKey digit1 = new LogicalKeyboardKey(49L);
    public static LogicalKeyboardKey digit2 = new LogicalKeyboardKey(50L);
    public static LogicalKeyboardKey digit3 = new LogicalKeyboardKey(51L);
    public static LogicalKeyboardKey digit4 = new LogicalKeyboardKey(52L);
    public static LogicalKeyboardKey digit5 = new LogicalKeyboardKey(53L);
    public static LogicalKeyboardKey digit6 = new LogicalKeyboardKey(54L);
    public static LogicalKeyboardKey digit7 = new LogicalKeyboardKey(55L);
    public static LogicalKeyboardKey digit8 = new LogicalKeyboardKey(56L);
    public static LogicalKeyboardKey digit9 = new LogicalKeyboardKey(57L);
    public static LogicalKeyboardKey colon = new LogicalKeyboardKey(58L);
    public static LogicalKeyboardKey semicolon = new LogicalKeyboardKey(59L);
    public static LogicalKeyboardKey less = new LogicalKeyboardKey(60L);
    public static LogicalKeyboardKey equal = new LogicalKeyboardKey(61L);
    public static LogicalKeyboardKey greater = new LogicalKeyboardKey(62L);
    public static LogicalKeyboardKey question = new LogicalKeyboardKey(63L);
    public static LogicalKeyboardKey at = new LogicalKeyboardKey(64L);
    public static LogicalKeyboardKey bracketLeft = new LogicalKeyboardKey(91L);
    public static LogicalKeyboardKey backslash = new LogicalKeyboardKey(92L);
    public static LogicalKeyboardKey bracketRight = new LogicalKeyboardKey(93L);
    public static LogicalKeyboardKey caret = new LogicalKeyboardKey(94L);
    public static LogicalKeyboardKey underscore = new LogicalKeyboardKey(95L);
    public static LogicalKeyboardKey backquote = new LogicalKeyboardKey(96L);
    public static LogicalKeyboardKey keyA = new LogicalKeyboardKey(97L);
    public static LogicalKeyboardKey keyB = new LogicalKeyboardKey(98L);
    public static LogicalKeyboardKey keyC = new LogicalKeyboardKey(99L);
    public static LogicalKeyboardKey keyD = new LogicalKeyboardKey(100L);
    public static LogicalKeyboardKey keyE = new LogicalKeyboardKey(101L);
    public static LogicalKeyboardKey keyF = new LogicalKeyboardKey(102L);
    public static LogicalKeyboardKey keyG = new LogicalKeyboardKey(103L);
    public static LogicalKeyboardKey keyH = new LogicalKeyboardKey(104L);
    public static LogicalKeyboardKey keyI = new LogicalKeyboardKey(105L);
    public static LogicalKeyboardKey keyJ = new LogicalKeyboardKey(106L);
    public static LogicalKeyboardKey keyK = new LogicalKeyboardKey(107L);
    public static LogicalKeyboardKey keyL = new LogicalKeyboardKey(108L);
    public static LogicalKeyboardKey keyM = new LogicalKeyboardKey(109L);
    public static LogicalKeyboardKey keyN = new LogicalKeyboardKey(110L);
    public static LogicalKeyboardKey keyO = new LogicalKeyboardKey(111L);
    public static LogicalKeyboardKey keyP = new LogicalKeyboardKey(112L);
    public static LogicalKeyboardKey keyQ = new LogicalKeyboardKey(113L);
    public static LogicalKeyboardKey keyR = new LogicalKeyboardKey(114L);
    public static LogicalKeyboardKey keyS = new LogicalKeyboardKey(115L);
    public static LogicalKeyboardKey keyT = new LogicalKeyboardKey(116L);
    public static LogicalKeyboardKey keyU = new LogicalKeyboardKey(117L);
    public static LogicalKeyboardKey keyV = new LogicalKeyboardKey(118L);
    public static LogicalKeyboardKey keyW = new LogicalKeyboardKey(119L);
    public static LogicalKeyboardKey keyX = new LogicalKeyboardKey(120L);
    public static LogicalKeyboardKey keyY = new LogicalKeyboardKey(121L);
    public static LogicalKeyboardKey keyZ = new LogicalKeyboardKey(122L);
    public static LogicalKeyboardKey braceLeft = new LogicalKeyboardKey(123L);
    public static LogicalKeyboardKey bar = new LogicalKeyboardKey(124L);
    public static LogicalKeyboardKey braceRight = new LogicalKeyboardKey(125L);
    public static LogicalKeyboardKey tilde = new LogicalKeyboardKey(126L);
    public static LogicalKeyboardKey unidentified = new LogicalKeyboardKey(4294967297L);
    public static LogicalKeyboardKey backspace = new LogicalKeyboardKey(4294967304L);
    public static LogicalKeyboardKey tab = new LogicalKeyboardKey(4294967305L);
    public static LogicalKeyboardKey enter = new LogicalKeyboardKey(4294967309L);
    public static LogicalKeyboardKey escape = new LogicalKeyboardKey(4294967323L);
    public static LogicalKeyboardKey delete = new LogicalKeyboardKey(4294967423L);
    public static LogicalKeyboardKey accel = new LogicalKeyboardKey(4294967553L);
    public static LogicalKeyboardKey altGraph = new LogicalKeyboardKey(4294967555L);
    public static LogicalKeyboardKey capsLock = new LogicalKeyboardKey(4294967556L);
    public static LogicalKeyboardKey fn = new LogicalKeyboardKey(4294967558L);
    public static LogicalKeyboardKey fnLock = new LogicalKeyboardKey(4294967559L);
    public static LogicalKeyboardKey hyper = new LogicalKeyboardKey(4294967560L);
    public static LogicalKeyboardKey numLock = new LogicalKeyboardKey(4294967562L);
    public static LogicalKeyboardKey scrollLock = new LogicalKeyboardKey(4294967564L);
    public static LogicalKeyboardKey superKey = new LogicalKeyboardKey(4294967566L);
    public static LogicalKeyboardKey symbol = new LogicalKeyboardKey(4294967567L);
    public static LogicalKeyboardKey symbolLock = new LogicalKeyboardKey(4294967568L);
    public static LogicalKeyboardKey shiftLevel5 = new LogicalKeyboardKey(4294967569L);
    public static LogicalKeyboardKey arrowDown = new LogicalKeyboardKey(4294968065L);
    public static LogicalKeyboardKey arrowLeft = new LogicalKeyboardKey(4294968066L);
    public static LogicalKeyboardKey arrowRight = new LogicalKeyboardKey(4294968067L);
    public static LogicalKeyboardKey arrowUp = new LogicalKeyboardKey(4294968068L);
    public static LogicalKeyboardKey end = new LogicalKeyboardKey(4294968069L);
    public static LogicalKeyboardKey home = new LogicalKeyboardKey(4294968070L);
    public static LogicalKeyboardKey pageDown = new LogicalKeyboardKey(4294968071L);
    public static LogicalKeyboardKey pageUp = new LogicalKeyboardKey(4294968072L);
    public static LogicalKeyboardKey clear = new LogicalKeyboardKey(4294968321L);
    public static LogicalKeyboardKey copy = new LogicalKeyboardKey(4294968322L);
    public static LogicalKeyboardKey crSel = new LogicalKeyboardKey(4294968323L);
    public static LogicalKeyboardKey cut = new LogicalKeyboardKey(4294968324L);
    public static LogicalKeyboardKey eraseEof = new LogicalKeyboardKey(4294968325L);
    public static LogicalKeyboardKey exSel = new LogicalKeyboardKey(4294968326L);
    public static LogicalKeyboardKey insert = new LogicalKeyboardKey(4294968327L);
    public static LogicalKeyboardKey paste = new LogicalKeyboardKey(4294968328L);
    public static LogicalKeyboardKey redo = new LogicalKeyboardKey(4294968329L);
    public static LogicalKeyboardKey undo = new LogicalKeyboardKey(4294968330L);
    public static LogicalKeyboardKey accept = new LogicalKeyboardKey(4294968577L);
    public static LogicalKeyboardKey again = new LogicalKeyboardKey(4294968578L);
    public static LogicalKeyboardKey attn = new LogicalKeyboardKey(4294968579L);
    public static LogicalKeyboardKey cancel = new LogicalKeyboardKey(4294968580L);
    public static LogicalKeyboardKey contextMenu = new LogicalKeyboardKey(4294968581L);
    public static LogicalKeyboardKey execute = new LogicalKeyboardKey(4294968582L);
    public static LogicalKeyboardKey find = new LogicalKeyboardKey(4294968583L);
    public static LogicalKeyboardKey help = new LogicalKeyboardKey(4294968584L);
    public static LogicalKeyboardKey pause = new LogicalKeyboardKey(4294968585L);
    public static LogicalKeyboardKey play = new LogicalKeyboardKey(4294968586L);
    public static LogicalKeyboardKey props = new LogicalKeyboardKey(4294968587L);
    public static LogicalKeyboardKey select = new LogicalKeyboardKey(4294968588L);
    public static LogicalKeyboardKey zoomIn = new LogicalKeyboardKey(4294968589L);
    public static LogicalKeyboardKey zoomOut = new LogicalKeyboardKey(4294968590L);
    public static LogicalKeyboardKey brightnessDown = new LogicalKeyboardKey(4294968833L);
    public static LogicalKeyboardKey brightnessUp = new LogicalKeyboardKey(4294968834L);
    public static LogicalKeyboardKey camera = new LogicalKeyboardKey(4294968835L);
    public static LogicalKeyboardKey eject = new LogicalKeyboardKey(4294968836L);
    public static LogicalKeyboardKey logOff = new LogicalKeyboardKey(4294968837L);
    public static LogicalKeyboardKey power = new LogicalKeyboardKey(4294968838L);
    public static LogicalKeyboardKey powerOff = new LogicalKeyboardKey(4294968839L);
    public static LogicalKeyboardKey printScreen = new LogicalKeyboardKey(4294968840L);
    public static LogicalKeyboardKey hibernate = new LogicalKeyboardKey(4294968841L);
    public static LogicalKeyboardKey standby = new LogicalKeyboardKey(4294968842L);
    public static LogicalKeyboardKey wakeUp = new LogicalKeyboardKey(4294968843L);
    public static LogicalKeyboardKey allCandidates = new LogicalKeyboardKey(4294969089L);
    public static LogicalKeyboardKey alphanumeric = new LogicalKeyboardKey(4294969090L);
    public static LogicalKeyboardKey codeInput = new LogicalKeyboardKey(4294969091L);
    public static LogicalKeyboardKey compose = new LogicalKeyboardKey(4294969092L);
    public static LogicalKeyboardKey convert = new LogicalKeyboardKey(4294969093L);
    public static LogicalKeyboardKey finalMode = new LogicalKeyboardKey(4294969094L);
    public static LogicalKeyboardKey groupFirst = new LogicalKeyboardKey(4294969095L);
    public static LogicalKeyboardKey groupLast = new LogicalKeyboardKey(4294969096L);
    public static LogicalKeyboardKey groupNext = new LogicalKeyboardKey(4294969097L);
    public static LogicalKeyboardKey groupPrevious = new LogicalKeyboardKey(4294969098L);
    public static LogicalKeyboardKey modeChange = new LogicalKeyboardKey(4294969099L);
    public static LogicalKeyboardKey nextCandidate = new LogicalKeyboardKey(4294969100L);
    public static LogicalKeyboardKey nonConvert = new LogicalKeyboardKey(4294969101L);
    public static LogicalKeyboardKey previousCandidate = new LogicalKeyboardKey(4294969102L);
    public static LogicalKeyboardKey process = new LogicalKeyboardKey(4294969103L);
    public static LogicalKeyboardKey singleCandidate = new LogicalKeyboardKey(4294969104L);
    public static LogicalKeyboardKey hangulMode = new LogicalKeyboardKey(4294969105L);
    public static LogicalKeyboardKey hanjaMode = new LogicalKeyboardKey(4294969106L);
    public static LogicalKeyboardKey junjaMode = new LogicalKeyboardKey(4294969107L);
    public static LogicalKeyboardKey eisu = new LogicalKeyboardKey(4294969108L);
    public static LogicalKeyboardKey hankaku = new LogicalKeyboardKey(4294969109L);
    public static LogicalKeyboardKey hiragana = new LogicalKeyboardKey(4294969110L);
    public static LogicalKeyboardKey hiraganaKatakana = new LogicalKeyboardKey(4294969111L);
    public static LogicalKeyboardKey kanaMode = new LogicalKeyboardKey(4294969112L);
    public static LogicalKeyboardKey kanjiMode = new LogicalKeyboardKey(4294969113L);
    public static LogicalKeyboardKey katakana = new LogicalKeyboardKey(4294969114L);
    public static LogicalKeyboardKey romaji = new LogicalKeyboardKey(4294969115L);
    public static LogicalKeyboardKey zenkaku = new LogicalKeyboardKey(4294969116L);
    public static LogicalKeyboardKey zenkakuHankaku = new LogicalKeyboardKey(4294969117L);
    public static LogicalKeyboardKey f1 = new LogicalKeyboardKey(4294969345L);
    public static LogicalKeyboardKey f2 = new LogicalKeyboardKey(4294969346L);
    public static LogicalKeyboardKey f3 = new LogicalKeyboardKey(4294969347L);
    public static LogicalKeyboardKey f4 = new LogicalKeyboardKey(4294969348L);
    public static LogicalKeyboardKey f5 = new LogicalKeyboardKey(4294969349L);
    public static LogicalKeyboardKey f6 = new LogicalKeyboardKey(4294969350L);
    public static LogicalKeyboardKey f7 = new LogicalKeyboardKey(4294969351L);
    public static LogicalKeyboardKey f8 = new LogicalKeyboardKey(4294969352L);
    public static LogicalKeyboardKey f9 = new LogicalKeyboardKey(4294969353L);
    public static LogicalKeyboardKey f10 = new LogicalKeyboardKey(4294969354L);
    public static LogicalKeyboardKey f11 = new LogicalKeyboardKey(4294969355L);
    public static LogicalKeyboardKey f12 = new LogicalKeyboardKey(4294969356L);
    public static LogicalKeyboardKey f13 = new LogicalKeyboardKey(4294969357L);
    public static LogicalKeyboardKey f14 = new LogicalKeyboardKey(4294969358L);
    public static LogicalKeyboardKey f15 = new LogicalKeyboardKey(4294969359L);
    public static LogicalKeyboardKey f16 = new LogicalKeyboardKey(4294969360L);
    public static LogicalKeyboardKey f17 = new LogicalKeyboardKey(4294969361L);
    public static LogicalKeyboardKey f18 = new LogicalKeyboardKey(4294969362L);
    public static LogicalKeyboardKey f19 = new LogicalKeyboardKey(4294969363L);
    public static LogicalKeyboardKey f20 = new LogicalKeyboardKey(4294969364L);
    public static LogicalKeyboardKey f21 = new LogicalKeyboardKey(4294969365L);
    public static LogicalKeyboardKey f22 = new LogicalKeyboardKey(4294969366L);
    public static LogicalKeyboardKey f23 = new LogicalKeyboardKey(4294969367L);
    public static LogicalKeyboardKey f24 = new LogicalKeyboardKey(4294969368L);
    public static LogicalKeyboardKey soft1 = new LogicalKeyboardKey(4294969601L);
    public static LogicalKeyboardKey soft2 = new LogicalKeyboardKey(4294969602L);
    public static LogicalKeyboardKey soft3 = new LogicalKeyboardKey(4294969603L);
    public static LogicalKeyboardKey soft4 = new LogicalKeyboardKey(4294969604L);
    public static LogicalKeyboardKey soft5 = new LogicalKeyboardKey(4294969605L);
    public static LogicalKeyboardKey soft6 = new LogicalKeyboardKey(4294969606L);
    public static LogicalKeyboardKey soft7 = new LogicalKeyboardKey(4294969607L);
    public static LogicalKeyboardKey soft8 = new LogicalKeyboardKey(4294969608L);
    public static LogicalKeyboardKey close = new LogicalKeyboardKey(4294969857L);
    public static LogicalKeyboardKey mailForward = new LogicalKeyboardKey(4294969858L);
    public static LogicalKeyboardKey mailReply = new LogicalKeyboardKey(4294969859L);
    public static LogicalKeyboardKey mailSend = new LogicalKeyboardKey(4294969860L);
    public static LogicalKeyboardKey mediaPlayPause = new LogicalKeyboardKey(4294969861L);
    public static LogicalKeyboardKey mediaStop = new LogicalKeyboardKey(4294969863L);
    public static LogicalKeyboardKey mediaTrackNext = new LogicalKeyboardKey(4294969864L);
    public static LogicalKeyboardKey mediaTrackPrevious = new LogicalKeyboardKey(4294969865L);
    public static LogicalKeyboardKey newKey = new LogicalKeyboardKey(4294969866L);
    public static LogicalKeyboardKey open = new LogicalKeyboardKey(4294969867L);
    public static LogicalKeyboardKey print = new LogicalKeyboardKey(4294969868L);
    public static LogicalKeyboardKey save = new LogicalKeyboardKey(4294969869L);
    public static LogicalKeyboardKey spellCheck = new LogicalKeyboardKey(4294969870L);
    public static LogicalKeyboardKey audioVolumeDown = new LogicalKeyboardKey(4294969871L);
    public static LogicalKeyboardKey audioVolumeUp = new LogicalKeyboardKey(4294969872L);
    public static LogicalKeyboardKey audioVolumeMute = new LogicalKeyboardKey(4294969873L);
    public static LogicalKeyboardKey launchApplication2 = new LogicalKeyboardKey(4294970113L);
    public static LogicalKeyboardKey launchCalendar = new LogicalKeyboardKey(4294970114L);
    public static LogicalKeyboardKey launchMail = new LogicalKeyboardKey(4294970115L);
    public static LogicalKeyboardKey launchMediaPlayer = new LogicalKeyboardKey(4294970116L);
    public static LogicalKeyboardKey launchMusicPlayer = new LogicalKeyboardKey(4294970117L);
    public static LogicalKeyboardKey launchApplication1 = new LogicalKeyboardKey(4294970118L);
    public static LogicalKeyboardKey launchScreenSaver = new LogicalKeyboardKey(4294970119L);
    public static LogicalKeyboardKey launchSpreadsheet = new LogicalKeyboardKey(4294970120L);
    public static LogicalKeyboardKey launchWebBrowser = new LogicalKeyboardKey(4294970121L);
    public static LogicalKeyboardKey launchWebCam = new LogicalKeyboardKey(4294970122L);
    public static LogicalKeyboardKey launchWordProcessor = new LogicalKeyboardKey(4294970123L);
    public static LogicalKeyboardKey launchContacts = new LogicalKeyboardKey(4294970124L);
    public static LogicalKeyboardKey launchPhone = new LogicalKeyboardKey(4294970125L);
    public static LogicalKeyboardKey launchAssistant = new LogicalKeyboardKey(4294970126L);
    public static LogicalKeyboardKey launchControlPanel = new LogicalKeyboardKey(4294970127L);
    public static LogicalKeyboardKey browserBack = new LogicalKeyboardKey(4294970369L);
    public static LogicalKeyboardKey browserFavorites = new LogicalKeyboardKey(4294970370L);
    public static LogicalKeyboardKey browserForward = new LogicalKeyboardKey(4294970371L);
    public static LogicalKeyboardKey browserHome = new LogicalKeyboardKey(4294970372L);
    public static LogicalKeyboardKey browserRefresh = new LogicalKeyboardKey(4294970373L);
    public static LogicalKeyboardKey browserSearch = new LogicalKeyboardKey(4294970374L);
    public static LogicalKeyboardKey browserStop = new LogicalKeyboardKey(4294970375L);
    public static LogicalKeyboardKey audioBalanceLeft = new LogicalKeyboardKey(4294970625L);
    public static LogicalKeyboardKey audioBalanceRight = new LogicalKeyboardKey(4294970626L);
    public static LogicalKeyboardKey audioBassBoostDown = new LogicalKeyboardKey(4294970627L);
    public static LogicalKeyboardKey audioBassBoostUp = new LogicalKeyboardKey(4294970628L);
    public static LogicalKeyboardKey audioFaderFront = new LogicalKeyboardKey(4294970629L);
    public static LogicalKeyboardKey audioFaderRear = new LogicalKeyboardKey(4294970630L);
    public static LogicalKeyboardKey audioSurroundModeNext = new LogicalKeyboardKey(4294970631L);
    public static LogicalKeyboardKey avrInput = new LogicalKeyboardKey(4294970632L);
    public static LogicalKeyboardKey avrPower = new LogicalKeyboardKey(4294970633L);
    public static LogicalKeyboardKey channelDown = new LogicalKeyboardKey(4294970634L);
    public static LogicalKeyboardKey channelUp = new LogicalKeyboardKey(4294970635L);
    public static LogicalKeyboardKey colorF0Red = new LogicalKeyboardKey(4294970636L);
    public static LogicalKeyboardKey colorF1Green = new LogicalKeyboardKey(4294970637L);
    public static LogicalKeyboardKey colorF2Yellow = new LogicalKeyboardKey(4294970638L);
    public static LogicalKeyboardKey colorF3Blue = new LogicalKeyboardKey(4294970639L);
    public static LogicalKeyboardKey colorF4Grey = new LogicalKeyboardKey(4294970640L);
    public static LogicalKeyboardKey colorF5Brown = new LogicalKeyboardKey(4294970641L);
    public static LogicalKeyboardKey closedCaptionToggle = new LogicalKeyboardKey(4294970642L);
    public static LogicalKeyboardKey dimmer = new LogicalKeyboardKey(4294970643L);
    public static LogicalKeyboardKey displaySwap = new LogicalKeyboardKey(4294970644L);
    public static LogicalKeyboardKey exit = new LogicalKeyboardKey(4294970645L);
    public static LogicalKeyboardKey favoriteClear0 = new LogicalKeyboardKey(4294970646L);
    public static LogicalKeyboardKey favoriteClear1 = new LogicalKeyboardKey(4294970647L);
    public static LogicalKeyboardKey favoriteClear2 = new LogicalKeyboardKey(4294970648L);
    public static LogicalKeyboardKey favoriteClear3 = new LogicalKeyboardKey(4294970649L);
    public static LogicalKeyboardKey favoriteRecall0 = new LogicalKeyboardKey(4294970650L);
    public static LogicalKeyboardKey favoriteRecall1 = new LogicalKeyboardKey(4294970651L);
    public static LogicalKeyboardKey favoriteRecall2 = new LogicalKeyboardKey(4294970652L);
    public static LogicalKeyboardKey favoriteRecall3 = new LogicalKeyboardKey(4294970653L);
    public static LogicalKeyboardKey favoriteStore0 = new LogicalKeyboardKey(4294970654L);
    public static LogicalKeyboardKey favoriteStore1 = new LogicalKeyboardKey(4294970655L);
    public static LogicalKeyboardKey favoriteStore2 = new LogicalKeyboardKey(4294970656L);
    public static LogicalKeyboardKey favoriteStore3 = new LogicalKeyboardKey(4294970657L);
    public static LogicalKeyboardKey guide = new LogicalKeyboardKey(4294970658L);
    public static LogicalKeyboardKey guideNextDay = new LogicalKeyboardKey(4294970659L);
    public static LogicalKeyboardKey guidePreviousDay = new LogicalKeyboardKey(4294970660L);
    public static LogicalKeyboardKey info = new LogicalKeyboardKey(4294970661L);
    public static LogicalKeyboardKey instantReplay = new LogicalKeyboardKey(4294970662L);
    public static LogicalKeyboardKey link = new LogicalKeyboardKey(4294970663L);
    public static LogicalKeyboardKey listProgram = new LogicalKeyboardKey(4294970664L);
    public static LogicalKeyboardKey liveContent = new LogicalKeyboardKey(4294970665L);
    public static LogicalKeyboardKey @lock = new LogicalKeyboardKey(4294970666L);
    public static LogicalKeyboardKey mediaApps = new LogicalKeyboardKey(4294970667L);
    public static LogicalKeyboardKey mediaFastForward = new LogicalKeyboardKey(4294970668L);
    public static LogicalKeyboardKey mediaLast = new LogicalKeyboardKey(4294970669L);
    public static LogicalKeyboardKey mediaPause = new LogicalKeyboardKey(4294970670L);
    public static LogicalKeyboardKey mediaPlay = new LogicalKeyboardKey(4294970671L);
    public static LogicalKeyboardKey mediaRecord = new LogicalKeyboardKey(4294970672L);
    public static LogicalKeyboardKey mediaRewind = new LogicalKeyboardKey(4294970673L);
    public static LogicalKeyboardKey mediaSkip = new LogicalKeyboardKey(4294970674L);
    public static LogicalKeyboardKey nextFavoriteChannel = new LogicalKeyboardKey(4294970675L);
    public static LogicalKeyboardKey nextUserProfile = new LogicalKeyboardKey(4294970676L);
    public static LogicalKeyboardKey onDemand = new LogicalKeyboardKey(4294970677L);
    public static LogicalKeyboardKey pInPDown = new LogicalKeyboardKey(4294970678L);
    public static LogicalKeyboardKey pInPMove = new LogicalKeyboardKey(4294970679L);
    public static LogicalKeyboardKey pInPToggle = new LogicalKeyboardKey(4294970680L);
    public static LogicalKeyboardKey pInPUp = new LogicalKeyboardKey(4294970681L);
    public static LogicalKeyboardKey playSpeedDown = new LogicalKeyboardKey(4294970682L);
    public static LogicalKeyboardKey playSpeedReset = new LogicalKeyboardKey(4294970683L);
    public static LogicalKeyboardKey playSpeedUp = new LogicalKeyboardKey(4294970684L);
    public static LogicalKeyboardKey randomToggle = new LogicalKeyboardKey(4294970685L);
    public static LogicalKeyboardKey rcLowBattery = new LogicalKeyboardKey(4294970686L);
    public static LogicalKeyboardKey recordSpeedNext = new LogicalKeyboardKey(4294970687L);
    public static LogicalKeyboardKey rfBypass = new LogicalKeyboardKey(4294970688L);
    public static LogicalKeyboardKey scanChannelsToggle = new LogicalKeyboardKey(4294970689L);
    public static LogicalKeyboardKey screenModeNext = new LogicalKeyboardKey(4294970690L);
    public static LogicalKeyboardKey settings = new LogicalKeyboardKey(4294970691L);
    public static LogicalKeyboardKey splitScreenToggle = new LogicalKeyboardKey(4294970692L);
    public static LogicalKeyboardKey stbInput = new LogicalKeyboardKey(4294970693L);
    public static LogicalKeyboardKey stbPower = new LogicalKeyboardKey(4294970694L);
    public static LogicalKeyboardKey subtitle = new LogicalKeyboardKey(4294970695L);
    public static LogicalKeyboardKey teletext = new LogicalKeyboardKey(4294970696L);
    public static LogicalKeyboardKey tv = new LogicalKeyboardKey(4294970697L);
    public static LogicalKeyboardKey tvInput = new LogicalKeyboardKey(4294970698L);
    public static LogicalKeyboardKey tvPower = new LogicalKeyboardKey(4294970699L);
    public static LogicalKeyboardKey videoModeNext = new LogicalKeyboardKey(4294970700L);
    public static LogicalKeyboardKey wink = new LogicalKeyboardKey(4294970701L);
    public static LogicalKeyboardKey zoomToggle = new LogicalKeyboardKey(4294970702L);
    public static LogicalKeyboardKey dvr = new LogicalKeyboardKey(4294970703L);
    public static LogicalKeyboardKey mediaAudioTrack = new LogicalKeyboardKey(4294970704L);
    public static LogicalKeyboardKey mediaSkipBackward = new LogicalKeyboardKey(4294970705L);
    public static LogicalKeyboardKey mediaSkipForward = new LogicalKeyboardKey(4294970706L);
    public static LogicalKeyboardKey mediaStepBackward = new LogicalKeyboardKey(4294970707L);
    public static LogicalKeyboardKey mediaStepForward = new LogicalKeyboardKey(4294970708L);
    public static LogicalKeyboardKey mediaTopMenu = new LogicalKeyboardKey(4294970709L);
    public static LogicalKeyboardKey navigateIn = new LogicalKeyboardKey(4294970710L);
    public static LogicalKeyboardKey navigateNext = new LogicalKeyboardKey(4294970711L);
    public static LogicalKeyboardKey navigateOut = new LogicalKeyboardKey(4294970712L);
    public static LogicalKeyboardKey navigatePrevious = new LogicalKeyboardKey(4294970713L);
    public static LogicalKeyboardKey pairing = new LogicalKeyboardKey(4294970714L);
    public static LogicalKeyboardKey mediaClose = new LogicalKeyboardKey(4294970715L);
    public static LogicalKeyboardKey audioBassBoostToggle = new LogicalKeyboardKey(4294970882L);
    public static LogicalKeyboardKey audioTrebleDown = new LogicalKeyboardKey(4294970884L);
    public static LogicalKeyboardKey audioTrebleUp = new LogicalKeyboardKey(4294970885L);
    public static LogicalKeyboardKey microphoneToggle = new LogicalKeyboardKey(4294970886L);
    public static LogicalKeyboardKey microphoneVolumeDown = new LogicalKeyboardKey(4294970887L);
    public static LogicalKeyboardKey microphoneVolumeUp = new LogicalKeyboardKey(4294970888L);
    public static LogicalKeyboardKey microphoneVolumeMute = new LogicalKeyboardKey(4294970889L);
    public static LogicalKeyboardKey speechCorrectionList = new LogicalKeyboardKey(4294971137L);
    public static LogicalKeyboardKey speechInputToggle = new LogicalKeyboardKey(4294971138L);
    public static LogicalKeyboardKey appSwitch = new LogicalKeyboardKey(4294971393L);
    public static LogicalKeyboardKey call = new LogicalKeyboardKey(4294971394L);
    public static LogicalKeyboardKey cameraFocus = new LogicalKeyboardKey(4294971395L);
    public static LogicalKeyboardKey endCall = new LogicalKeyboardKey(4294971396L);
    public static LogicalKeyboardKey goBack = new LogicalKeyboardKey(4294971397L);
    public static LogicalKeyboardKey goHome = new LogicalKeyboardKey(4294971398L);
    public static LogicalKeyboardKey headsetHook = new LogicalKeyboardKey(4294971399L);
    public static LogicalKeyboardKey lastNumberRedial = new LogicalKeyboardKey(4294971400L);
    public static LogicalKeyboardKey notification = new LogicalKeyboardKey(4294971401L);
    public static LogicalKeyboardKey mannerMode = new LogicalKeyboardKey(4294971402L);
    public static LogicalKeyboardKey voiceDial = new LogicalKeyboardKey(4294971403L);
    public static LogicalKeyboardKey tv3DMode = new LogicalKeyboardKey(4294971649L);
    public static LogicalKeyboardKey tvAntennaCable = new LogicalKeyboardKey(4294971650L);
    public static LogicalKeyboardKey tvAudioDescription = new LogicalKeyboardKey(4294971651L);
    public static LogicalKeyboardKey tvAudioDescriptionMixDown = new LogicalKeyboardKey(4294971652L);
    public static LogicalKeyboardKey tvAudioDescriptionMixUp = new LogicalKeyboardKey(4294971653L);
    public static LogicalKeyboardKey tvContentsMenu = new LogicalKeyboardKey(4294971654L);
    public static LogicalKeyboardKey tvDataService = new LogicalKeyboardKey(4294971655L);
    public static LogicalKeyboardKey tvInputComponent1 = new LogicalKeyboardKey(4294971656L);
    public static LogicalKeyboardKey tvInputComponent2 = new LogicalKeyboardKey(4294971657L);
    public static LogicalKeyboardKey tvInputComposite1 = new LogicalKeyboardKey(4294971658L);
    public static LogicalKeyboardKey tvInputComposite2 = new LogicalKeyboardKey(4294971659L);
    public static LogicalKeyboardKey tvInputHDMI1 = new LogicalKeyboardKey(4294971660L);
    public static LogicalKeyboardKey tvInputHDMI2 = new LogicalKeyboardKey(4294971661L);
    public static LogicalKeyboardKey tvInputHDMI3 = new LogicalKeyboardKey(4294971662L);
    public static LogicalKeyboardKey tvInputHDMI4 = new LogicalKeyboardKey(4294971663L);
    public static LogicalKeyboardKey tvInputVGA1 = new LogicalKeyboardKey(4294971664L);
    public static LogicalKeyboardKey tvMediaContext = new LogicalKeyboardKey(4294971665L);
    public static LogicalKeyboardKey tvNetwork = new LogicalKeyboardKey(4294971666L);
    public static LogicalKeyboardKey tvNumberEntry = new LogicalKeyboardKey(4294971667L);
    public static LogicalKeyboardKey tvRadioService = new LogicalKeyboardKey(4294971668L);
    public static LogicalKeyboardKey tvSatellite = new LogicalKeyboardKey(4294971669L);
    public static LogicalKeyboardKey tvSatelliteBS = new LogicalKeyboardKey(4294971670L);
    public static LogicalKeyboardKey tvSatelliteCS = new LogicalKeyboardKey(4294971671L);
    public static LogicalKeyboardKey tvSatelliteToggle = new LogicalKeyboardKey(4294971672L);
    public static LogicalKeyboardKey tvTerrestrialAnalog = new LogicalKeyboardKey(4294971673L);
    public static LogicalKeyboardKey tvTerrestrialDigital = new LogicalKeyboardKey(4294971674L);
    public static LogicalKeyboardKey tvTimer = new LogicalKeyboardKey(4294971675L);
    public static LogicalKeyboardKey key11 = new LogicalKeyboardKey(4294971905L);
    public static LogicalKeyboardKey key12 = new LogicalKeyboardKey(4294971906L);
    public static LogicalKeyboardKey suspend = new LogicalKeyboardKey(8589934592L);
    public static LogicalKeyboardKey resume = new LogicalKeyboardKey(8589934593L);
    public static LogicalKeyboardKey sleep = new LogicalKeyboardKey(8589934594L);
    public static LogicalKeyboardKey abort = new LogicalKeyboardKey(8589934595L);
    public static LogicalKeyboardKey lang1 = new LogicalKeyboardKey(8589934608L);
    public static LogicalKeyboardKey lang2 = new LogicalKeyboardKey(8589934609L);
    public static LogicalKeyboardKey lang3 = new LogicalKeyboardKey(8589934610L);
    public static LogicalKeyboardKey lang4 = new LogicalKeyboardKey(8589934611L);
    public static LogicalKeyboardKey lang5 = new LogicalKeyboardKey(8589934612L);
    public static LogicalKeyboardKey intlBackslash = new LogicalKeyboardKey(8589934624L);
    public static LogicalKeyboardKey intlRo = new LogicalKeyboardKey(8589934625L);
    public static LogicalKeyboardKey intlYen = new LogicalKeyboardKey(8589934626L);
    public static LogicalKeyboardKey controlLeft = new LogicalKeyboardKey(8589934848L);
    public static LogicalKeyboardKey controlRight = new LogicalKeyboardKey(8589934849L);
    public static LogicalKeyboardKey shiftLeft = new LogicalKeyboardKey(8589934850L);
    public static LogicalKeyboardKey shiftRight = new LogicalKeyboardKey(8589934851L);
    public static LogicalKeyboardKey altLeft = new LogicalKeyboardKey(8589934852L);
    public static LogicalKeyboardKey altRight = new LogicalKeyboardKey(8589934853L);
    public static LogicalKeyboardKey metaLeft = new LogicalKeyboardKey(8589934854L);
    public static LogicalKeyboardKey metaRight = new LogicalKeyboardKey(8589934855L);
    public static LogicalKeyboardKey control = new LogicalKeyboardKey(8589935088L);
    public static LogicalKeyboardKey shift = new LogicalKeyboardKey(8589935090L);
    public static LogicalKeyboardKey alt = new LogicalKeyboardKey(8589935092L);
    public static LogicalKeyboardKey meta = new LogicalKeyboardKey(8589935094L);
    public static LogicalKeyboardKey numpadEnter = new LogicalKeyboardKey(8589935117L);
    public static LogicalKeyboardKey numpadParenLeft = new LogicalKeyboardKey(8589935144L);
    public static LogicalKeyboardKey numpadParenRight = new LogicalKeyboardKey(8589935145L);
    public static LogicalKeyboardKey numpadMultiply = new LogicalKeyboardKey(8589935146L);
    public static LogicalKeyboardKey numpadAdd = new LogicalKeyboardKey(8589935147L);
    public static LogicalKeyboardKey numpadComma = new LogicalKeyboardKey(8589935148L);
    public static LogicalKeyboardKey numpadSubtract = new LogicalKeyboardKey(8589935149L);
    public static LogicalKeyboardKey numpadDecimal = new LogicalKeyboardKey(8589935150L);
    public static LogicalKeyboardKey numpadDivide = new LogicalKeyboardKey(8589935151L);
    public static LogicalKeyboardKey numpad0 = new LogicalKeyboardKey(8589935152L);
    public static LogicalKeyboardKey numpad1 = new LogicalKeyboardKey(8589935153L);
    public static LogicalKeyboardKey numpad2 = new LogicalKeyboardKey(8589935154L);
    public static LogicalKeyboardKey numpad3 = new LogicalKeyboardKey(8589935155L);
    public static LogicalKeyboardKey numpad4 = new LogicalKeyboardKey(8589935156L);
    public static LogicalKeyboardKey numpad5 = new LogicalKeyboardKey(8589935157L);
    public static LogicalKeyboardKey numpad6 = new LogicalKeyboardKey(8589935158L);
    public static LogicalKeyboardKey numpad7 = new LogicalKeyboardKey(8589935159L);
    public static LogicalKeyboardKey numpad8 = new LogicalKeyboardKey(8589935160L);
    public static LogicalKeyboardKey numpad9 = new LogicalKeyboardKey(8589935161L);
    public static LogicalKeyboardKey numpadEqual = new LogicalKeyboardKey(8589935165L);
    public static LogicalKeyboardKey gameButton1 = new LogicalKeyboardKey(8589935361L);
    public static LogicalKeyboardKey gameButton2 = new LogicalKeyboardKey(8589935362L);
    public static LogicalKeyboardKey gameButton3 = new LogicalKeyboardKey(8589935363L);
    public static LogicalKeyboardKey gameButton4 = new LogicalKeyboardKey(8589935364L);
    public static LogicalKeyboardKey gameButton5 = new LogicalKeyboardKey(8589935365L);
    public static LogicalKeyboardKey gameButton6 = new LogicalKeyboardKey(8589935366L);
    public static LogicalKeyboardKey gameButton7 = new LogicalKeyboardKey(8589935367L);
    public static LogicalKeyboardKey gameButton8 = new LogicalKeyboardKey(8589935368L);
    public static LogicalKeyboardKey gameButton9 = new LogicalKeyboardKey(8589935369L);
    public static LogicalKeyboardKey gameButton10 = new LogicalKeyboardKey(8589935370L);
    public static LogicalKeyboardKey gameButton11 = new LogicalKeyboardKey(8589935371L);
    public static LogicalKeyboardKey gameButton12 = new LogicalKeyboardKey(8589935372L);
    public static LogicalKeyboardKey gameButton13 = new LogicalKeyboardKey(8589935373L);
    public static LogicalKeyboardKey gameButton14 = new LogicalKeyboardKey(8589935374L);
    public static LogicalKeyboardKey gameButton15 = new LogicalKeyboardKey(8589935375L);
    public static LogicalKeyboardKey gameButton16 = new LogicalKeyboardKey(8589935376L);
    public static LogicalKeyboardKey gameButtonA = new LogicalKeyboardKey(8589935377L);
    public static LogicalKeyboardKey gameButtonB = new LogicalKeyboardKey(8589935378L);
    public static LogicalKeyboardKey gameButtonC = new LogicalKeyboardKey(8589935379L);
    public static LogicalKeyboardKey gameButtonLeft1 = new LogicalKeyboardKey(8589935380L);
    public static LogicalKeyboardKey gameButtonLeft2 = new LogicalKeyboardKey(8589935381L);
    public static LogicalKeyboardKey gameButtonMode = new LogicalKeyboardKey(8589935382L);
    public static LogicalKeyboardKey gameButtonRight1 = new LogicalKeyboardKey(8589935383L);
    public static LogicalKeyboardKey gameButtonRight2 = new LogicalKeyboardKey(8589935384L);
    public static LogicalKeyboardKey gameButtonSelect = new LogicalKeyboardKey(8589935385L);
    public static LogicalKeyboardKey gameButtonStart = new LogicalKeyboardKey(8589935386L);
    public static LogicalKeyboardKey gameButtonThumbLeft = new LogicalKeyboardKey(8589935387L);
    public static LogicalKeyboardKey gameButtonThumbRight = new LogicalKeyboardKey(8589935388L);
    public static LogicalKeyboardKey gameButtonX = new LogicalKeyboardKey(8589935389L);
    public static LogicalKeyboardKey gameButtonY = new LogicalKeyboardKey(8589935390L);
    public static LogicalKeyboardKey gameButtonZ = new LogicalKeyboardKey(8589935391L);
    internal static DartMap<long, LogicalKeyboardKey> _knownLogicalKeys = new DartMap<long, LogicalKeyboardKey> { [32L] = space, [33L] = exclamation, [34L] = quote, [35L] = numberSign, [36L] = dollar, [37L] = percent, [38L] = ampersand, [39L] = quoteSingle, [40L] = parenthesisLeft, [41L] = parenthesisRight, [42L] = asterisk, [43L] = add, [44L] = comma, [45L] = minus, [46L] = period, [47L] = slash, [48L] = digit0, [49L] = digit1, [50L] = digit2, [51L] = digit3, [52L] = digit4, [53L] = digit5, [54L] = digit6, [55L] = digit7, [56L] = digit8, [57L] = digit9, [58L] = colon, [59L] = semicolon, [60L] = less, [61L] = equal, [62L] = greater, [63L] = question, [64L] = at, [91L] = bracketLeft, [92L] = backslash, [93L] = bracketRight, [94L] = caret, [95L] = underscore, [96L] = backquote, [97L] = keyA, [98L] = keyB, [99L] = keyC, [100L] = keyD, [101L] = keyE, [102L] = keyF, [103L] = keyG, [104L] = keyH, [105L] = keyI, [106L] = keyJ, [107L] = keyK, [108L] = keyL, [109L] = keyM, [110L] = keyN, [111L] = keyO, [112L] = keyP, [113L] = keyQ, [114L] = keyR, [115L] = keyS, [116L] = keyT, [117L] = keyU, [118L] = keyV, [119L] = keyW, [120L] = keyX, [121L] = keyY, [122L] = keyZ, [123L] = braceLeft, [124L] = bar, [125L] = braceRight, [126L] = tilde, [4294967297L] = unidentified, [4294967304L] = backspace, [4294967305L] = tab, [4294967309L] = enter, [4294967323L] = escape, [4294967423L] = delete, [4294967553L] = accel, [4294967555L] = altGraph, [4294967556L] = capsLock, [4294967558L] = fn, [4294967559L] = fnLock, [4294967560L] = hyper, [4294967562L] = numLock, [4294967564L] = scrollLock, [4294967566L] = superKey, [4294967567L] = symbol, [4294967568L] = symbolLock, [4294967569L] = shiftLevel5, [4294968065L] = arrowDown, [4294968066L] = arrowLeft, [4294968067L] = arrowRight, [4294968068L] = arrowUp, [4294968069L] = end, [4294968070L] = home, [4294968071L] = pageDown, [4294968072L] = pageUp, [4294968321L] = clear, [4294968322L] = copy, [4294968323L] = crSel, [4294968324L] = cut, [4294968325L] = eraseEof, [4294968326L] = exSel, [4294968327L] = insert, [4294968328L] = paste, [4294968329L] = redo, [4294968330L] = undo, [4294968577L] = accept, [4294968578L] = again, [4294968579L] = attn, [4294968580L] = cancel, [4294968581L] = contextMenu, [4294968582L] = execute, [4294968583L] = find, [4294968584L] = help, [4294968585L] = pause, [4294968586L] = play, [4294968587L] = props, [4294968588L] = select, [4294968589L] = zoomIn, [4294968590L] = zoomOut, [4294968833L] = brightnessDown, [4294968834L] = brightnessUp, [4294968835L] = camera, [4294968836L] = eject, [4294968837L] = logOff, [4294968838L] = power, [4294968839L] = powerOff, [4294968840L] = printScreen, [4294968841L] = hibernate, [4294968842L] = standby, [4294968843L] = wakeUp, [4294969089L] = allCandidates, [4294969090L] = alphanumeric, [4294969091L] = codeInput, [4294969092L] = compose, [4294969093L] = convert, [4294969094L] = finalMode, [4294969095L] = groupFirst, [4294969096L] = groupLast, [4294969097L] = groupNext, [4294969098L] = groupPrevious, [4294969099L] = modeChange, [4294969100L] = nextCandidate, [4294969101L] = nonConvert, [4294969102L] = previousCandidate, [4294969103L] = process, [4294969104L] = singleCandidate, [4294969105L] = hangulMode, [4294969106L] = hanjaMode, [4294969107L] = junjaMode, [4294969108L] = eisu, [4294969109L] = hankaku, [4294969110L] = hiragana, [4294969111L] = hiraganaKatakana, [4294969112L] = kanaMode, [4294969113L] = kanjiMode, [4294969114L] = katakana, [4294969115L] = romaji, [4294969116L] = zenkaku, [4294969117L] = zenkakuHankaku, [4294969345L] = f1, [4294969346L] = f2, [4294969347L] = f3, [4294969348L] = f4, [4294969349L] = f5, [4294969350L] = f6, [4294969351L] = f7, [4294969352L] = f8, [4294969353L] = f9, [4294969354L] = f10, [4294969355L] = f11, [4294969356L] = f12, [4294969357L] = f13, [4294969358L] = f14, [4294969359L] = f15, [4294969360L] = f16, [4294969361L] = f17, [4294969362L] = f18, [4294969363L] = f19, [4294969364L] = f20, [4294969365L] = f21, [4294969366L] = f22, [4294969367L] = f23, [4294969368L] = f24, [4294969601L] = soft1, [4294969602L] = soft2, [4294969603L] = soft3, [4294969604L] = soft4, [4294969605L] = soft5, [4294969606L] = soft6, [4294969607L] = soft7, [4294969608L] = soft8, [4294969857L] = close, [4294969858L] = mailForward, [4294969859L] = mailReply, [4294969860L] = mailSend, [4294969861L] = mediaPlayPause, [4294969863L] = mediaStop, [4294969864L] = mediaTrackNext, [4294969865L] = mediaTrackPrevious, [4294969866L] = newKey, [4294969867L] = open, [4294969868L] = print, [4294969869L] = save, [4294969870L] = spellCheck, [4294969871L] = audioVolumeDown, [4294969872L] = audioVolumeUp, [4294969873L] = audioVolumeMute, [4294970113L] = launchApplication2, [4294970114L] = launchCalendar, [4294970115L] = launchMail, [4294970116L] = launchMediaPlayer, [4294970117L] = launchMusicPlayer, [4294970118L] = launchApplication1, [4294970119L] = launchScreenSaver, [4294970120L] = launchSpreadsheet, [4294970121L] = launchWebBrowser, [4294970122L] = launchWebCam, [4294970123L] = launchWordProcessor, [4294970124L] = launchContacts, [4294970125L] = launchPhone, [4294970126L] = launchAssistant, [4294970127L] = launchControlPanel, [4294970369L] = browserBack, [4294970370L] = browserFavorites, [4294970371L] = browserForward, [4294970372L] = browserHome, [4294970373L] = browserRefresh, [4294970374L] = browserSearch, [4294970375L] = browserStop, [4294970625L] = audioBalanceLeft, [4294970626L] = audioBalanceRight, [4294970627L] = audioBassBoostDown, [4294970628L] = audioBassBoostUp, [4294970629L] = audioFaderFront, [4294970630L] = audioFaderRear, [4294970631L] = audioSurroundModeNext, [4294970632L] = avrInput, [4294970633L] = avrPower, [4294970634L] = channelDown, [4294970635L] = channelUp, [4294970636L] = colorF0Red, [4294970637L] = colorF1Green, [4294970638L] = colorF2Yellow, [4294970639L] = colorF3Blue, [4294970640L] = colorF4Grey, [4294970641L] = colorF5Brown, [4294970642L] = closedCaptionToggle, [4294970643L] = dimmer, [4294970644L] = displaySwap, [4294970645L] = exit, [4294970646L] = favoriteClear0, [4294970647L] = favoriteClear1, [4294970648L] = favoriteClear2, [4294970649L] = favoriteClear3, [4294970650L] = favoriteRecall0, [4294970651L] = favoriteRecall1, [4294970652L] = favoriteRecall2, [4294970653L] = favoriteRecall3, [4294970654L] = favoriteStore0, [4294970655L] = favoriteStore1, [4294970656L] = favoriteStore2, [4294970657L] = favoriteStore3, [4294970658L] = guide, [4294970659L] = guideNextDay, [4294970660L] = guidePreviousDay, [4294970661L] = info, [4294970662L] = instantReplay, [4294970663L] = link, [4294970664L] = listProgram, [4294970665L] = liveContent, [4294970666L] = @lock, [4294970667L] = mediaApps, [4294970668L] = mediaFastForward, [4294970669L] = mediaLast, [4294970670L] = mediaPause, [4294970671L] = mediaPlay, [4294970672L] = mediaRecord, [4294970673L] = mediaRewind, [4294970674L] = mediaSkip, [4294970675L] = nextFavoriteChannel, [4294970676L] = nextUserProfile, [4294970677L] = onDemand, [4294970678L] = pInPDown, [4294970679L] = pInPMove, [4294970680L] = pInPToggle, [4294970681L] = pInPUp, [4294970682L] = playSpeedDown, [4294970683L] = playSpeedReset, [4294970684L] = playSpeedUp, [4294970685L] = randomToggle, [4294970686L] = rcLowBattery, [4294970687L] = recordSpeedNext, [4294970688L] = rfBypass, [4294970689L] = scanChannelsToggle, [4294970690L] = screenModeNext, [4294970691L] = settings, [4294970692L] = splitScreenToggle, [4294970693L] = stbInput, [4294970694L] = stbPower, [4294970695L] = subtitle, [4294970696L] = teletext, [4294970697L] = tv, [4294970698L] = tvInput, [4294970699L] = tvPower, [4294970700L] = videoModeNext, [4294970701L] = wink, [4294970702L] = zoomToggle, [4294970703L] = dvr, [4294970704L] = mediaAudioTrack, [4294970705L] = mediaSkipBackward, [4294970706L] = mediaSkipForward, [4294970707L] = mediaStepBackward, [4294970708L] = mediaStepForward, [4294970709L] = mediaTopMenu, [4294970710L] = navigateIn, [4294970711L] = navigateNext, [4294970712L] = navigateOut, [4294970713L] = navigatePrevious, [4294970714L] = pairing, [4294970715L] = mediaClose, [4294970882L] = audioBassBoostToggle, [4294970884L] = audioTrebleDown, [4294970885L] = audioTrebleUp, [4294970886L] = microphoneToggle, [4294970887L] = microphoneVolumeDown, [4294970888L] = microphoneVolumeUp, [4294970889L] = microphoneVolumeMute, [4294971137L] = speechCorrectionList, [4294971138L] = speechInputToggle, [4294971393L] = appSwitch, [4294971394L] = call, [4294971395L] = cameraFocus, [4294971396L] = endCall, [4294971397L] = goBack, [4294971398L] = goHome, [4294971399L] = headsetHook, [4294971400L] = lastNumberRedial, [4294971401L] = notification, [4294971402L] = mannerMode, [4294971403L] = voiceDial, [4294971649L] = tv3DMode, [4294971650L] = tvAntennaCable, [4294971651L] = tvAudioDescription, [4294971652L] = tvAudioDescriptionMixDown, [4294971653L] = tvAudioDescriptionMixUp, [4294971654L] = tvContentsMenu, [4294971655L] = tvDataService, [4294971656L] = tvInputComponent1, [4294971657L] = tvInputComponent2, [4294971658L] = tvInputComposite1, [4294971659L] = tvInputComposite2, [4294971660L] = tvInputHDMI1, [4294971661L] = tvInputHDMI2, [4294971662L] = tvInputHDMI3, [4294971663L] = tvInputHDMI4, [4294971664L] = tvInputVGA1, [4294971665L] = tvMediaContext, [4294971666L] = tvNetwork, [4294971667L] = tvNumberEntry, [4294971668L] = tvRadioService, [4294971669L] = tvSatellite, [4294971670L] = tvSatelliteBS, [4294971671L] = tvSatelliteCS, [4294971672L] = tvSatelliteToggle, [4294971673L] = tvTerrestrialAnalog, [4294971674L] = tvTerrestrialDigital, [4294971675L] = tvTimer, [4294971905L] = key11, [4294971906L] = key12, [8589934592L] = suspend, [8589934593L] = resume, [8589934594L] = sleep, [8589934595L] = abort, [8589934608L] = lang1, [8589934609L] = lang2, [8589934610L] = lang3, [8589934611L] = lang4, [8589934612L] = lang5, [8589934624L] = intlBackslash, [8589934625L] = intlRo, [8589934626L] = intlYen, [8589934848L] = controlLeft, [8589934849L] = controlRight, [8589934850L] = shiftLeft, [8589934851L] = shiftRight, [8589934852L] = altLeft, [8589934853L] = altRight, [8589934854L] = metaLeft, [8589934855L] = metaRight, [8589935088L] = control, [8589935090L] = shift, [8589935092L] = alt, [8589935094L] = meta, [8589935117L] = numpadEnter, [8589935144L] = numpadParenLeft, [8589935145L] = numpadParenRight, [8589935146L] = numpadMultiply, [8589935147L] = numpadAdd, [8589935148L] = numpadComma, [8589935149L] = numpadSubtract, [8589935150L] = numpadDecimal, [8589935151L] = numpadDivide, [8589935152L] = numpad0, [8589935153L] = numpad1, [8589935154L] = numpad2, [8589935155L] = numpad3, [8589935156L] = numpad4, [8589935157L] = numpad5, [8589935158L] = numpad6, [8589935159L] = numpad7, [8589935160L] = numpad8, [8589935161L] = numpad9, [8589935165L] = numpadEqual, [8589935361L] = gameButton1, [8589935362L] = gameButton2, [8589935363L] = gameButton3, [8589935364L] = gameButton4, [8589935365L] = gameButton5, [8589935366L] = gameButton6, [8589935367L] = gameButton7, [8589935368L] = gameButton8, [8589935369L] = gameButton9, [8589935370L] = gameButton10, [8589935371L] = gameButton11, [8589935372L] = gameButton12, [8589935373L] = gameButton13, [8589935374L] = gameButton14, [8589935375L] = gameButton15, [8589935376L] = gameButton16, [8589935377L] = gameButtonA, [8589935378L] = gameButtonB, [8589935379L] = gameButtonC, [8589935380L] = gameButtonLeft1, [8589935381L] = gameButtonLeft2, [8589935382L] = gameButtonMode, [8589935383L] = gameButtonRight1, [8589935384L] = gameButtonRight2, [8589935385L] = gameButtonSelect, [8589935386L] = gameButtonStart, [8589935387L] = gameButtonThumbLeft, [8589935388L] = gameButtonThumbRight, [8589935389L] = gameButtonX, [8589935390L] = gameButtonY, [8589935391L] = gameButtonZ };
    internal static DartMap<LogicalKeyboardKey, HashSet<LogicalKeyboardKey>> _synonyms = new DartMap<LogicalKeyboardKey, HashSet<LogicalKeyboardKey>> { [shiftLeft] = new HashSet<LogicalKeyboardKey> { shift }, [shiftRight] = new HashSet<LogicalKeyboardKey> { shift }, [metaLeft] = new HashSet<LogicalKeyboardKey> { meta }, [metaRight] = new HashSet<LogicalKeyboardKey> { meta }, [altLeft] = new HashSet<LogicalKeyboardKey> { alt }, [altRight] = new HashSet<LogicalKeyboardKey> { alt }, [controlLeft] = new HashSet<LogicalKeyboardKey> { control }, [controlRight] = new HashSet<LogicalKeyboardKey> { control } };
    internal static DartMap<LogicalKeyboardKey, HashSet<LogicalKeyboardKey>> _reverseSynonyms = new DartMap<LogicalKeyboardKey, HashSet<LogicalKeyboardKey>> { [shift] = new HashSet<LogicalKeyboardKey> { shiftLeft, shiftRight }, [meta] = new HashSet<LogicalKeyboardKey> { metaLeft, metaRight }, [alt] = new HashSet<LogicalKeyboardKey> { altLeft, altRight }, [control] = new HashSet<LogicalKeyboardKey> { controlLeft, controlRight } };
    internal static DartMap<long, string> _keyLabels = new DartMap<long, string> { [32L] = "Space", [33L] = "Exclamation", [34L] = "Quote", [35L] = "Number Sign", [36L] = "Dollar", [37L] = "Percent", [38L] = "Ampersand", [39L] = "Quote Single", [40L] = "Parenthesis Left", [41L] = "Parenthesis Right", [42L] = "Asterisk", [43L] = "Add", [44L] = "Comma", [45L] = "Minus", [46L] = "Period", [47L] = "Slash", [48L] = "Digit 0", [49L] = "Digit 1", [50L] = "Digit 2", [51L] = "Digit 3", [52L] = "Digit 4", [53L] = "Digit 5", [54L] = "Digit 6", [55L] = "Digit 7", [56L] = "Digit 8", [57L] = "Digit 9", [58L] = "Colon", [59L] = "Semicolon", [60L] = "Less", [61L] = "Equal", [62L] = "Greater", [63L] = "Question", [64L] = "At", [91L] = "Bracket Left", [92L] = "Backslash", [93L] = "Bracket Right", [94L] = "Caret", [95L] = "Underscore", [96L] = "Backquote", [97L] = "Key A", [98L] = "Key B", [99L] = "Key C", [100L] = "Key D", [101L] = "Key E", [102L] = "Key F", [103L] = "Key G", [104L] = "Key H", [105L] = "Key I", [106L] = "Key J", [107L] = "Key K", [108L] = "Key L", [109L] = "Key M", [110L] = "Key N", [111L] = "Key O", [112L] = "Key P", [113L] = "Key Q", [114L] = "Key R", [115L] = "Key S", [116L] = "Key T", [117L] = "Key U", [118L] = "Key V", [119L] = "Key W", [120L] = "Key X", [121L] = "Key Y", [122L] = "Key Z", [123L] = "Brace Left", [124L] = "Bar", [125L] = "Brace Right", [126L] = "Tilde", [4294967297L] = "Unidentified", [4294967304L] = "Backspace", [4294967305L] = "Tab", [4294967309L] = "Enter", [4294967323L] = "Escape", [4294967423L] = "Delete", [4294967553L] = "Accel", [4294967555L] = "Alt Graph", [4294967556L] = "Caps Lock", [4294967558L] = "Fn", [4294967559L] = "Fn Lock", [4294967560L] = "Hyper", [4294967562L] = "Num Lock", [4294967564L] = "Scroll Lock", [4294967566L] = "Super", [4294967567L] = "Symbol", [4294967568L] = "Symbol Lock", [4294967569L] = "Shift Level 5", [4294968065L] = "Arrow Down", [4294968066L] = "Arrow Left", [4294968067L] = "Arrow Right", [4294968068L] = "Arrow Up", [4294968069L] = "End", [4294968070L] = "Home", [4294968071L] = "Page Down", [4294968072L] = "Page Up", [4294968321L] = "Clear", [4294968322L] = "Copy", [4294968323L] = "Cr Sel", [4294968324L] = "Cut", [4294968325L] = "Erase Eof", [4294968326L] = "Ex Sel", [4294968327L] = "Insert", [4294968328L] = "Paste", [4294968329L] = "Redo", [4294968330L] = "Undo", [4294968577L] = "Accept", [4294968578L] = "Again", [4294968579L] = "Attn", [4294968580L] = "Cancel", [4294968581L] = "Context Menu", [4294968582L] = "Execute", [4294968583L] = "Find", [4294968584L] = "Help", [4294968585L] = "Pause", [4294968586L] = "Play", [4294968587L] = "Props", [4294968588L] = "Select", [4294968589L] = "Zoom In", [4294968590L] = "Zoom Out", [4294968833L] = "Brightness Down", [4294968834L] = "Brightness Up", [4294968835L] = "Camera", [4294968836L] = "Eject", [4294968837L] = "Log Off", [4294968838L] = "Power", [4294968839L] = "Power Off", [4294968840L] = "Print Screen", [4294968841L] = "Hibernate", [4294968842L] = "Standby", [4294968843L] = "Wake Up", [4294969089L] = "All Candidates", [4294969090L] = "Alphanumeric", [4294969091L] = "Code Input", [4294969092L] = "Compose", [4294969093L] = "Convert", [4294969094L] = "Final Mode", [4294969095L] = "Group First", [4294969096L] = "Group Last", [4294969097L] = "Group Next", [4294969098L] = "Group Previous", [4294969099L] = "Mode Change", [4294969100L] = "Next Candidate", [4294969101L] = "Non Convert", [4294969102L] = "Previous Candidate", [4294969103L] = "Process", [4294969104L] = "Single Candidate", [4294969105L] = "Hangul Mode", [4294969106L] = "Hanja Mode", [4294969107L] = "Junja Mode", [4294969108L] = "Eisu", [4294969109L] = "Hankaku", [4294969110L] = "Hiragana", [4294969111L] = "Hiragana Katakana", [4294969112L] = "Kana Mode", [4294969113L] = "Kanji Mode", [4294969114L] = "Katakana", [4294969115L] = "Romaji", [4294969116L] = "Zenkaku", [4294969117L] = "Zenkaku Hankaku", [4294969345L] = "F1", [4294969346L] = "F2", [4294969347L] = "F3", [4294969348L] = "F4", [4294969349L] = "F5", [4294969350L] = "F6", [4294969351L] = "F7", [4294969352L] = "F8", [4294969353L] = "F9", [4294969354L] = "F10", [4294969355L] = "F11", [4294969356L] = "F12", [4294969357L] = "F13", [4294969358L] = "F14", [4294969359L] = "F15", [4294969360L] = "F16", [4294969361L] = "F17", [4294969362L] = "F18", [4294969363L] = "F19", [4294969364L] = "F20", [4294969365L] = "F21", [4294969366L] = "F22", [4294969367L] = "F23", [4294969368L] = "F24", [4294969601L] = "Soft 1", [4294969602L] = "Soft 2", [4294969603L] = "Soft 3", [4294969604L] = "Soft 4", [4294969605L] = "Soft 5", [4294969606L] = "Soft 6", [4294969607L] = "Soft 7", [4294969608L] = "Soft 8", [4294969857L] = "Close", [4294969858L] = "Mail Forward", [4294969859L] = "Mail Reply", [4294969860L] = "Mail Send", [4294969861L] = "Media Play Pause", [4294969863L] = "Media Stop", [4294969864L] = "Media Track Next", [4294969865L] = "Media Track Previous", [4294969866L] = "New", [4294969867L] = "Open", [4294969868L] = "Print", [4294969869L] = "Save", [4294969870L] = "Spell Check", [4294969871L] = "Audio Volume Down", [4294969872L] = "Audio Volume Up", [4294969873L] = "Audio Volume Mute", [4294970113L] = "Launch Application 2", [4294970114L] = "Launch Calendar", [4294970115L] = "Launch Mail", [4294970116L] = "Launch Media Player", [4294970117L] = "Launch Music Player", [4294970118L] = "Launch Application 1", [4294970119L] = "Launch Screen Saver", [4294970120L] = "Launch Spreadsheet", [4294970121L] = "Launch Web Browser", [4294970122L] = "Launch Web Cam", [4294970123L] = "Launch Word Processor", [4294970124L] = "Launch Contacts", [4294970125L] = "Launch Phone", [4294970126L] = "Launch Assistant", [4294970127L] = "Launch Control Panel", [4294970369L] = "Browser Back", [4294970370L] = "Browser Favorites", [4294970371L] = "Browser Forward", [4294970372L] = "Browser Home", [4294970373L] = "Browser Refresh", [4294970374L] = "Browser Search", [4294970375L] = "Browser Stop", [4294970625L] = "Audio Balance Left", [4294970626L] = "Audio Balance Right", [4294970627L] = "Audio Bass Boost Down", [4294970628L] = "Audio Bass Boost Up", [4294970629L] = "Audio Fader Front", [4294970630L] = "Audio Fader Rear", [4294970631L] = "Audio Surround Mode Next", [4294970632L] = "AVR Input", [4294970633L] = "AVR Power", [4294970634L] = "Channel Down", [4294970635L] = "Channel Up", [4294970636L] = "Color F0 Red", [4294970637L] = "Color F1 Green", [4294970638L] = "Color F2 Yellow", [4294970639L] = "Color F3 Blue", [4294970640L] = "Color F4 Grey", [4294970641L] = "Color F5 Brown", [4294970642L] = "Closed Caption Toggle", [4294970643L] = "Dimmer", [4294970644L] = "Display Swap", [4294970645L] = "Exit", [4294970646L] = "Favorite Clear 0", [4294970647L] = "Favorite Clear 1", [4294970648L] = "Favorite Clear 2", [4294970649L] = "Favorite Clear 3", [4294970650L] = "Favorite Recall 0", [4294970651L] = "Favorite Recall 1", [4294970652L] = "Favorite Recall 2", [4294970653L] = "Favorite Recall 3", [4294970654L] = "Favorite Store 0", [4294970655L] = "Favorite Store 1", [4294970656L] = "Favorite Store 2", [4294970657L] = "Favorite Store 3", [4294970658L] = "Guide", [4294970659L] = "Guide Next Day", [4294970660L] = "Guide Previous Day", [4294970661L] = "Info", [4294970662L] = "Instant Replay", [4294970663L] = "Link", [4294970664L] = "List Program", [4294970665L] = "Live Content", [4294970666L] = "Lock", [4294970667L] = "Media Apps", [4294970668L] = "Media Fast Forward", [4294970669L] = "Media Last", [4294970670L] = "Media Pause", [4294970671L] = "Media Play", [4294970672L] = "Media Record", [4294970673L] = "Media Rewind", [4294970674L] = "Media Skip", [4294970675L] = "Next Favorite Channel", [4294970676L] = "Next User Profile", [4294970677L] = "On Demand", [4294970678L] = "P In P Down", [4294970679L] = "P In P Move", [4294970680L] = "P In P Toggle", [4294970681L] = "P In P Up", [4294970682L] = "Play Speed Down", [4294970683L] = "Play Speed Reset", [4294970684L] = "Play Speed Up", [4294970685L] = "Random Toggle", [4294970686L] = "Rc Low Battery", [4294970687L] = "Record Speed Next", [4294970688L] = "Rf Bypass", [4294970689L] = "Scan Channels Toggle", [4294970690L] = "Screen Mode Next", [4294970691L] = "Settings", [4294970692L] = "Split Screen Toggle", [4294970693L] = "STB Input", [4294970694L] = "STB Power", [4294970695L] = "Subtitle", [4294970696L] = "Teletext", [4294970697L] = "TV", [4294970698L] = "TV Input", [4294970699L] = "TV Power", [4294970700L] = "Video Mode Next", [4294970701L] = "Wink", [4294970702L] = "Zoom Toggle", [4294970703L] = "DVR", [4294970704L] = "Media Audio Track", [4294970705L] = "Media Skip Backward", [4294970706L] = "Media Skip Forward", [4294970707L] = "Media Step Backward", [4294970708L] = "Media Step Forward", [4294970709L] = "Media Top Menu", [4294970710L] = "Navigate In", [4294970711L] = "Navigate Next", [4294970712L] = "Navigate Out", [4294970713L] = "Navigate Previous", [4294970714L] = "Pairing", [4294970715L] = "Media Close", [4294970882L] = "Audio Bass Boost Toggle", [4294970884L] = "Audio Treble Down", [4294970885L] = "Audio Treble Up", [4294970886L] = "Microphone Toggle", [4294970887L] = "Microphone Volume Down", [4294970888L] = "Microphone Volume Up", [4294970889L] = "Microphone Volume Mute", [4294971137L] = "Speech Correction List", [4294971138L] = "Speech Input Toggle", [4294971393L] = "App Switch", [4294971394L] = "Call", [4294971395L] = "Camera Focus", [4294971396L] = "End Call", [4294971397L] = "Go Back", [4294971398L] = "Go Home", [4294971399L] = "Headset Hook", [4294971400L] = "Last Number Redial", [4294971401L] = "Notification", [4294971402L] = "Manner Mode", [4294971403L] = "Voice Dial", [4294971649L] = "TV 3 D Mode", [4294971650L] = "TV Antenna Cable", [4294971651L] = "TV Audio Description", [4294971652L] = "TV Audio Description Mix Down", [4294971653L] = "TV Audio Description Mix Up", [4294971654L] = "TV Contents Menu", [4294971655L] = "TV Data Service", [4294971656L] = "TV Input Component 1", [4294971657L] = "TV Input Component 2", [4294971658L] = "TV Input Composite 1", [4294971659L] = "TV Input Composite 2", [4294971660L] = "TV Input HDMI 1", [4294971661L] = "TV Input HDMI 2", [4294971662L] = "TV Input HDMI 3", [4294971663L] = "TV Input HDMI 4", [4294971664L] = "TV Input VGA 1", [4294971665L] = "TV Media Context", [4294971666L] = "TV Network", [4294971667L] = "TV Number Entry", [4294971668L] = "TV Radio Service", [4294971669L] = "TV Satellite", [4294971670L] = "TV Satellite BS", [4294971671L] = "TV Satellite CS", [4294971672L] = "TV Satellite Toggle", [4294971673L] = "TV Terrestrial Analog", [4294971674L] = "TV Terrestrial Digital", [4294971675L] = "TV Timer", [4294971905L] = "Key 11", [4294971906L] = "Key 12", [8589934592L] = "Suspend", [8589934593L] = "Resume", [8589934594L] = "Sleep", [8589934595L] = "Abort", [8589934608L] = "Lang 1", [8589934609L] = "Lang 2", [8589934610L] = "Lang 3", [8589934611L] = "Lang 4", [8589934612L] = "Lang 5", [8589934624L] = "Intl Backslash", [8589934625L] = "Intl Ro", [8589934626L] = "Intl Yen", [8589934848L] = "Control Left", [8589934849L] = "Control Right", [8589934850L] = "Shift Left", [8589934851L] = "Shift Right", [8589934852L] = "Alt Left", [8589934853L] = "Alt Right", [8589934854L] = "Meta Left", [8589934855L] = "Meta Right", [8589935088L] = "Control", [8589935090L] = "Shift", [8589935092L] = "Alt", [8589935094L] = "Meta", [8589935117L] = "Numpad Enter", [8589935144L] = "Numpad Paren Left", [8589935145L] = "Numpad Paren Right", [8589935146L] = "Numpad Multiply", [8589935147L] = "Numpad Add", [8589935148L] = "Numpad Comma", [8589935149L] = "Numpad Subtract", [8589935150L] = "Numpad Decimal", [8589935151L] = "Numpad Divide", [8589935152L] = "Numpad 0", [8589935153L] = "Numpad 1", [8589935154L] = "Numpad 2", [8589935155L] = "Numpad 3", [8589935156L] = "Numpad 4", [8589935157L] = "Numpad 5", [8589935158L] = "Numpad 6", [8589935159L] = "Numpad 7", [8589935160L] = "Numpad 8", [8589935161L] = "Numpad 9", [8589935165L] = "Numpad Equal", [8589935361L] = "Game Button 1", [8589935362L] = "Game Button 2", [8589935363L] = "Game Button 3", [8589935364L] = "Game Button 4", [8589935365L] = "Game Button 5", [8589935366L] = "Game Button 6", [8589935367L] = "Game Button 7", [8589935368L] = "Game Button 8", [8589935369L] = "Game Button 9", [8589935370L] = "Game Button 10", [8589935371L] = "Game Button 11", [8589935372L] = "Game Button 12", [8589935373L] = "Game Button 13", [8589935374L] = "Game Button 14", [8589935375L] = "Game Button 15", [8589935376L] = "Game Button 16", [8589935377L] = "Game Button A", [8589935378L] = "Game Button B", [8589935379L] = "Game Button C", [8589935380L] = "Game Button Left 1", [8589935381L] = "Game Button Left 2", [8589935382L] = "Game Button Mode", [8589935383L] = "Game Button Right 1", [8589935384L] = "Game Button Right 2", [8589935385L] = "Game Button Select", [8589935386L] = "Game Button Start", [8589935387L] = "Game Button Thumb Left", [8589935388L] = "Game Button Thumb Right", [8589935389L] = "Game Button X", [8589935390L] = "Game Button Y", [8589935391L] = "Game Button Z" };

    public LogicalKeyboardKey(long keyId)
    {
        this.keyId = keyId;
    }

    internal static long _nonValueBits(long n)
    {
        long divisorForValueMask = (valueMask + 1L);
        var valueMaskWidth = 32L;
        var firstDivisorWidth = 28L;
        DartRuntimePrimitives.Assert(() => (divisorForValueMask == (((1L << (int)(firstDivisorWidth))) * ((1L << (int)(((valueMaskWidth - firstDivisorWidth))))))));
        var maxSafeIntegerWidth = 52L;
        long nonValueMask = (((1L << (int)(((maxSafeIntegerWidth - valueMaskWidth))))) - 1L);
        if (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kIsWeb)
        {
            return (((n / divisorForValueMask)).floor() & nonValueMask);
        }
        else
        {
            return (((n >> (int)(valueMaskWidth))) & nonValueMask);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static string? _unicodeKeyLabel(long keyId)
    {
        if ((_nonValueBits(keyId) == 0L))
        {
            return char.ConvertFromUtf32(checked((int)keyId)).toUpperCase();
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string keyLabel
    {
        get
        {
            return ((_unicodeKeyLabel(keyId) ?? _keyLabels.GetValueOrDefault(keyId)) ?? "");
        }
    }
    public virtual string? debugName
    {
        get
        {
            string? result = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    result = _keyLabels.GetValueOrDefault(keyId);
                    if ((result is null))
                    {
                        string? unicodeKeyLabel__6559 = _unicodeKeyLabel(keyId);
                        if ((unicodeKeyLabel__6559 is not null))
                        {
                            result = $"Key {unicodeKeyLabel__6559}";
                        }
                        else
                        {
                            result = $"Key with ID 0x{keyId.toRadixString(16L).padLeft(11L, "0")}";
                        }
                    }
                    return true;
                });
            return result;
        }
    }
    public override int GetHashCode() => keyId.GetHashCode();
    public override bool Equals(object? other)
    {
        var __other = other as LogicalKeyboardKey;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return ((__other is LogicalKeyboardKey) && (((LogicalKeyboardKey)__other).keyId == keyId));
    }

    public static LogicalKeyboardKey? findKeyByKeyId(long keyId) => _knownLogicalKeys.GetValueOrDefault(keyId);
    public static bool isControlCharacter(string label)
    {
        if ((label.Length != 1L))
        {
            return false;
        }
        long codeUnit = label.codeUnitAt(0L);
        return ((((codeUnit <= 31L) && (codeUnit >= 0L))) || (((codeUnit >= 127L) && (codeUnit <= 159L))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isAutogenerated => (((keyId & planeMask)) >= startOfPlatformPlanes);
    public virtual HashSet<LogicalKeyboardKey> synonyms => (_synonyms.GetValueOrDefault(this) ?? new HashSet<LogicalKeyboardKey>());
    public static HashSet<LogicalKeyboardKey> collapseSynonyms(HashSet<LogicalKeyboardKey> input)
    {
        return input.expand(((element) =>
        {
            return (_synonyms.GetValueOrDefault(element) ?? new HashSet<LogicalKeyboardKey> { element });
        })).toSet();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static HashSet<LogicalKeyboardKey> expandSynonyms(HashSet<LogicalKeyboardKey> input)
    {
        return input.expand(((element) =>
        {
            return (_reverseSynonyms.GetValueOrDefault(element) ?? new HashSet<LogicalKeyboardKey> { element });
        })).toSet();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new StringProperty("keyId", $"0x{keyId.toRadixString(16L).padLeft(8L, "0")}"));
        properties.Add(new StringProperty("keyLabel", keyLabel));
        properties.Add(new StringProperty("debugName", debugName, defaultValue: null));
    }

    public static IEnumerable<LogicalKeyboardKey> knownLogicalKeys => _knownLogicalKeys.Values;
}

public class PhysicalKeyboardKey : KeyboardKey
{
    public virtual long usbHidUsage { get; private set; } = default!;
    public static PhysicalKeyboardKey hyper = new PhysicalKeyboardKey(16L);
    public static PhysicalKeyboardKey superKey = new PhysicalKeyboardKey(17L);
    public static PhysicalKeyboardKey fn = new PhysicalKeyboardKey(18L);
    public static PhysicalKeyboardKey fnLock = new PhysicalKeyboardKey(19L);
    public static PhysicalKeyboardKey suspend = new PhysicalKeyboardKey(20L);
    public static PhysicalKeyboardKey resume = new PhysicalKeyboardKey(21L);
    public static PhysicalKeyboardKey turbo = new PhysicalKeyboardKey(22L);
    public static PhysicalKeyboardKey privacyScreenToggle = new PhysicalKeyboardKey(23L);
    public static PhysicalKeyboardKey microphoneMuteToggle = new PhysicalKeyboardKey(24L);
    public static PhysicalKeyboardKey sleep = new PhysicalKeyboardKey(65666L);
    public static PhysicalKeyboardKey wakeUp = new PhysicalKeyboardKey(65667L);
    public static PhysicalKeyboardKey displayToggleIntExt = new PhysicalKeyboardKey(65717L);
    public static PhysicalKeyboardKey gameButton1 = new PhysicalKeyboardKey(392961L);
    public static PhysicalKeyboardKey gameButton2 = new PhysicalKeyboardKey(392962L);
    public static PhysicalKeyboardKey gameButton3 = new PhysicalKeyboardKey(392963L);
    public static PhysicalKeyboardKey gameButton4 = new PhysicalKeyboardKey(392964L);
    public static PhysicalKeyboardKey gameButton5 = new PhysicalKeyboardKey(392965L);
    public static PhysicalKeyboardKey gameButton6 = new PhysicalKeyboardKey(392966L);
    public static PhysicalKeyboardKey gameButton7 = new PhysicalKeyboardKey(392967L);
    public static PhysicalKeyboardKey gameButton8 = new PhysicalKeyboardKey(392968L);
    public static PhysicalKeyboardKey gameButton9 = new PhysicalKeyboardKey(392969L);
    public static PhysicalKeyboardKey gameButton10 = new PhysicalKeyboardKey(392970L);
    public static PhysicalKeyboardKey gameButton11 = new PhysicalKeyboardKey(392971L);
    public static PhysicalKeyboardKey gameButton12 = new PhysicalKeyboardKey(392972L);
    public static PhysicalKeyboardKey gameButton13 = new PhysicalKeyboardKey(392973L);
    public static PhysicalKeyboardKey gameButton14 = new PhysicalKeyboardKey(392974L);
    public static PhysicalKeyboardKey gameButton15 = new PhysicalKeyboardKey(392975L);
    public static PhysicalKeyboardKey gameButton16 = new PhysicalKeyboardKey(392976L);
    public static PhysicalKeyboardKey gameButtonA = new PhysicalKeyboardKey(392977L);
    public static PhysicalKeyboardKey gameButtonB = new PhysicalKeyboardKey(392978L);
    public static PhysicalKeyboardKey gameButtonC = new PhysicalKeyboardKey(392979L);
    public static PhysicalKeyboardKey gameButtonLeft1 = new PhysicalKeyboardKey(392980L);
    public static PhysicalKeyboardKey gameButtonLeft2 = new PhysicalKeyboardKey(392981L);
    public static PhysicalKeyboardKey gameButtonMode = new PhysicalKeyboardKey(392982L);
    public static PhysicalKeyboardKey gameButtonRight1 = new PhysicalKeyboardKey(392983L);
    public static PhysicalKeyboardKey gameButtonRight2 = new PhysicalKeyboardKey(392984L);
    public static PhysicalKeyboardKey gameButtonSelect = new PhysicalKeyboardKey(392985L);
    public static PhysicalKeyboardKey gameButtonStart = new PhysicalKeyboardKey(392986L);
    public static PhysicalKeyboardKey gameButtonThumbLeft = new PhysicalKeyboardKey(392987L);
    public static PhysicalKeyboardKey gameButtonThumbRight = new PhysicalKeyboardKey(392988L);
    public static PhysicalKeyboardKey gameButtonX = new PhysicalKeyboardKey(392989L);
    public static PhysicalKeyboardKey gameButtonY = new PhysicalKeyboardKey(392990L);
    public static PhysicalKeyboardKey gameButtonZ = new PhysicalKeyboardKey(392991L);
    public static PhysicalKeyboardKey usbReserved = new PhysicalKeyboardKey(458752L);
    public static PhysicalKeyboardKey usbErrorRollOver = new PhysicalKeyboardKey(458753L);
    public static PhysicalKeyboardKey usbPostFail = new PhysicalKeyboardKey(458754L);
    public static PhysicalKeyboardKey usbErrorUndefined = new PhysicalKeyboardKey(458755L);
    public static PhysicalKeyboardKey keyA = new PhysicalKeyboardKey(458756L);
    public static PhysicalKeyboardKey keyB = new PhysicalKeyboardKey(458757L);
    public static PhysicalKeyboardKey keyC = new PhysicalKeyboardKey(458758L);
    public static PhysicalKeyboardKey keyD = new PhysicalKeyboardKey(458759L);
    public static PhysicalKeyboardKey keyE = new PhysicalKeyboardKey(458760L);
    public static PhysicalKeyboardKey keyF = new PhysicalKeyboardKey(458761L);
    public static PhysicalKeyboardKey keyG = new PhysicalKeyboardKey(458762L);
    public static PhysicalKeyboardKey keyH = new PhysicalKeyboardKey(458763L);
    public static PhysicalKeyboardKey keyI = new PhysicalKeyboardKey(458764L);
    public static PhysicalKeyboardKey keyJ = new PhysicalKeyboardKey(458765L);
    public static PhysicalKeyboardKey keyK = new PhysicalKeyboardKey(458766L);
    public static PhysicalKeyboardKey keyL = new PhysicalKeyboardKey(458767L);
    public static PhysicalKeyboardKey keyM = new PhysicalKeyboardKey(458768L);
    public static PhysicalKeyboardKey keyN = new PhysicalKeyboardKey(458769L);
    public static PhysicalKeyboardKey keyO = new PhysicalKeyboardKey(458770L);
    public static PhysicalKeyboardKey keyP = new PhysicalKeyboardKey(458771L);
    public static PhysicalKeyboardKey keyQ = new PhysicalKeyboardKey(458772L);
    public static PhysicalKeyboardKey keyR = new PhysicalKeyboardKey(458773L);
    public static PhysicalKeyboardKey keyS = new PhysicalKeyboardKey(458774L);
    public static PhysicalKeyboardKey keyT = new PhysicalKeyboardKey(458775L);
    public static PhysicalKeyboardKey keyU = new PhysicalKeyboardKey(458776L);
    public static PhysicalKeyboardKey keyV = new PhysicalKeyboardKey(458777L);
    public static PhysicalKeyboardKey keyW = new PhysicalKeyboardKey(458778L);
    public static PhysicalKeyboardKey keyX = new PhysicalKeyboardKey(458779L);
    public static PhysicalKeyboardKey keyY = new PhysicalKeyboardKey(458780L);
    public static PhysicalKeyboardKey keyZ = new PhysicalKeyboardKey(458781L);
    public static PhysicalKeyboardKey digit1 = new PhysicalKeyboardKey(458782L);
    public static PhysicalKeyboardKey digit2 = new PhysicalKeyboardKey(458783L);
    public static PhysicalKeyboardKey digit3 = new PhysicalKeyboardKey(458784L);
    public static PhysicalKeyboardKey digit4 = new PhysicalKeyboardKey(458785L);
    public static PhysicalKeyboardKey digit5 = new PhysicalKeyboardKey(458786L);
    public static PhysicalKeyboardKey digit6 = new PhysicalKeyboardKey(458787L);
    public static PhysicalKeyboardKey digit7 = new PhysicalKeyboardKey(458788L);
    public static PhysicalKeyboardKey digit8 = new PhysicalKeyboardKey(458789L);
    public static PhysicalKeyboardKey digit9 = new PhysicalKeyboardKey(458790L);
    public static PhysicalKeyboardKey digit0 = new PhysicalKeyboardKey(458791L);
    public static PhysicalKeyboardKey enter = new PhysicalKeyboardKey(458792L);
    public static PhysicalKeyboardKey escape = new PhysicalKeyboardKey(458793L);
    public static PhysicalKeyboardKey backspace = new PhysicalKeyboardKey(458794L);
    public static PhysicalKeyboardKey tab = new PhysicalKeyboardKey(458795L);
    public static PhysicalKeyboardKey space = new PhysicalKeyboardKey(458796L);
    public static PhysicalKeyboardKey minus = new PhysicalKeyboardKey(458797L);
    public static PhysicalKeyboardKey equal = new PhysicalKeyboardKey(458798L);
    public static PhysicalKeyboardKey bracketLeft = new PhysicalKeyboardKey(458799L);
    public static PhysicalKeyboardKey bracketRight = new PhysicalKeyboardKey(458800L);
    public static PhysicalKeyboardKey backslash = new PhysicalKeyboardKey(458801L);
    public static PhysicalKeyboardKey semicolon = new PhysicalKeyboardKey(458803L);
    public static PhysicalKeyboardKey quote = new PhysicalKeyboardKey(458804L);
    public static PhysicalKeyboardKey backquote = new PhysicalKeyboardKey(458805L);
    public static PhysicalKeyboardKey comma = new PhysicalKeyboardKey(458806L);
    public static PhysicalKeyboardKey period = new PhysicalKeyboardKey(458807L);
    public static PhysicalKeyboardKey slash = new PhysicalKeyboardKey(458808L);
    public static PhysicalKeyboardKey capsLock = new PhysicalKeyboardKey(458809L);
    public static PhysicalKeyboardKey f1 = new PhysicalKeyboardKey(458810L);
    public static PhysicalKeyboardKey f2 = new PhysicalKeyboardKey(458811L);
    public static PhysicalKeyboardKey f3 = new PhysicalKeyboardKey(458812L);
    public static PhysicalKeyboardKey f4 = new PhysicalKeyboardKey(458813L);
    public static PhysicalKeyboardKey f5 = new PhysicalKeyboardKey(458814L);
    public static PhysicalKeyboardKey f6 = new PhysicalKeyboardKey(458815L);
    public static PhysicalKeyboardKey f7 = new PhysicalKeyboardKey(458816L);
    public static PhysicalKeyboardKey f8 = new PhysicalKeyboardKey(458817L);
    public static PhysicalKeyboardKey f9 = new PhysicalKeyboardKey(458818L);
    public static PhysicalKeyboardKey f10 = new PhysicalKeyboardKey(458819L);
    public static PhysicalKeyboardKey f11 = new PhysicalKeyboardKey(458820L);
    public static PhysicalKeyboardKey f12 = new PhysicalKeyboardKey(458821L);
    public static PhysicalKeyboardKey printScreen = new PhysicalKeyboardKey(458822L);
    public static PhysicalKeyboardKey scrollLock = new PhysicalKeyboardKey(458823L);
    public static PhysicalKeyboardKey pause = new PhysicalKeyboardKey(458824L);
    public static PhysicalKeyboardKey insert = new PhysicalKeyboardKey(458825L);
    public static PhysicalKeyboardKey home = new PhysicalKeyboardKey(458826L);
    public static PhysicalKeyboardKey pageUp = new PhysicalKeyboardKey(458827L);
    public static PhysicalKeyboardKey delete = new PhysicalKeyboardKey(458828L);
    public static PhysicalKeyboardKey end = new PhysicalKeyboardKey(458829L);
    public static PhysicalKeyboardKey pageDown = new PhysicalKeyboardKey(458830L);
    public static PhysicalKeyboardKey arrowRight = new PhysicalKeyboardKey(458831L);
    public static PhysicalKeyboardKey arrowLeft = new PhysicalKeyboardKey(458832L);
    public static PhysicalKeyboardKey arrowDown = new PhysicalKeyboardKey(458833L);
    public static PhysicalKeyboardKey arrowUp = new PhysicalKeyboardKey(458834L);
    public static PhysicalKeyboardKey numLock = new PhysicalKeyboardKey(458835L);
    public static PhysicalKeyboardKey numpadDivide = new PhysicalKeyboardKey(458836L);
    public static PhysicalKeyboardKey numpadMultiply = new PhysicalKeyboardKey(458837L);
    public static PhysicalKeyboardKey numpadSubtract = new PhysicalKeyboardKey(458838L);
    public static PhysicalKeyboardKey numpadAdd = new PhysicalKeyboardKey(458839L);
    public static PhysicalKeyboardKey numpadEnter = new PhysicalKeyboardKey(458840L);
    public static PhysicalKeyboardKey numpad1 = new PhysicalKeyboardKey(458841L);
    public static PhysicalKeyboardKey numpad2 = new PhysicalKeyboardKey(458842L);
    public static PhysicalKeyboardKey numpad3 = new PhysicalKeyboardKey(458843L);
    public static PhysicalKeyboardKey numpad4 = new PhysicalKeyboardKey(458844L);
    public static PhysicalKeyboardKey numpad5 = new PhysicalKeyboardKey(458845L);
    public static PhysicalKeyboardKey numpad6 = new PhysicalKeyboardKey(458846L);
    public static PhysicalKeyboardKey numpad7 = new PhysicalKeyboardKey(458847L);
    public static PhysicalKeyboardKey numpad8 = new PhysicalKeyboardKey(458848L);
    public static PhysicalKeyboardKey numpad9 = new PhysicalKeyboardKey(458849L);
    public static PhysicalKeyboardKey numpad0 = new PhysicalKeyboardKey(458850L);
    public static PhysicalKeyboardKey numpadDecimal = new PhysicalKeyboardKey(458851L);
    public static PhysicalKeyboardKey intlBackslash = new PhysicalKeyboardKey(458852L);
    public static PhysicalKeyboardKey contextMenu = new PhysicalKeyboardKey(458853L);
    public static PhysicalKeyboardKey power = new PhysicalKeyboardKey(458854L);
    public static PhysicalKeyboardKey numpadEqual = new PhysicalKeyboardKey(458855L);
    public static PhysicalKeyboardKey f13 = new PhysicalKeyboardKey(458856L);
    public static PhysicalKeyboardKey f14 = new PhysicalKeyboardKey(458857L);
    public static PhysicalKeyboardKey f15 = new PhysicalKeyboardKey(458858L);
    public static PhysicalKeyboardKey f16 = new PhysicalKeyboardKey(458859L);
    public static PhysicalKeyboardKey f17 = new PhysicalKeyboardKey(458860L);
    public static PhysicalKeyboardKey f18 = new PhysicalKeyboardKey(458861L);
    public static PhysicalKeyboardKey f19 = new PhysicalKeyboardKey(458862L);
    public static PhysicalKeyboardKey f20 = new PhysicalKeyboardKey(458863L);
    public static PhysicalKeyboardKey f21 = new PhysicalKeyboardKey(458864L);
    public static PhysicalKeyboardKey f22 = new PhysicalKeyboardKey(458865L);
    public static PhysicalKeyboardKey f23 = new PhysicalKeyboardKey(458866L);
    public static PhysicalKeyboardKey f24 = new PhysicalKeyboardKey(458867L);
    public static PhysicalKeyboardKey open = new PhysicalKeyboardKey(458868L);
    public static PhysicalKeyboardKey help = new PhysicalKeyboardKey(458869L);
    public static PhysicalKeyboardKey select = new PhysicalKeyboardKey(458871L);
    public static PhysicalKeyboardKey again = new PhysicalKeyboardKey(458873L);
    public static PhysicalKeyboardKey undo = new PhysicalKeyboardKey(458874L);
    public static PhysicalKeyboardKey cut = new PhysicalKeyboardKey(458875L);
    public static PhysicalKeyboardKey copy = new PhysicalKeyboardKey(458876L);
    public static PhysicalKeyboardKey paste = new PhysicalKeyboardKey(458877L);
    public static PhysicalKeyboardKey find = new PhysicalKeyboardKey(458878L);
    public static PhysicalKeyboardKey audioVolumeMute = new PhysicalKeyboardKey(458879L);
    public static PhysicalKeyboardKey audioVolumeUp = new PhysicalKeyboardKey(458880L);
    public static PhysicalKeyboardKey audioVolumeDown = new PhysicalKeyboardKey(458881L);
    public static PhysicalKeyboardKey numpadComma = new PhysicalKeyboardKey(458885L);
    public static PhysicalKeyboardKey intlRo = new PhysicalKeyboardKey(458887L);
    public static PhysicalKeyboardKey kanaMode = new PhysicalKeyboardKey(458888L);
    public static PhysicalKeyboardKey intlYen = new PhysicalKeyboardKey(458889L);
    public static PhysicalKeyboardKey convert = new PhysicalKeyboardKey(458890L);
    public static PhysicalKeyboardKey nonConvert = new PhysicalKeyboardKey(458891L);
    public static PhysicalKeyboardKey lang1 = new PhysicalKeyboardKey(458896L);
    public static PhysicalKeyboardKey lang2 = new PhysicalKeyboardKey(458897L);
    public static PhysicalKeyboardKey lang3 = new PhysicalKeyboardKey(458898L);
    public static PhysicalKeyboardKey lang4 = new PhysicalKeyboardKey(458899L);
    public static PhysicalKeyboardKey lang5 = new PhysicalKeyboardKey(458900L);
    public static PhysicalKeyboardKey abort = new PhysicalKeyboardKey(458907L);
    public static PhysicalKeyboardKey props = new PhysicalKeyboardKey(458915L);
    public static PhysicalKeyboardKey numpadParenLeft = new PhysicalKeyboardKey(458934L);
    public static PhysicalKeyboardKey numpadParenRight = new PhysicalKeyboardKey(458935L);
    public static PhysicalKeyboardKey numpadBackspace = new PhysicalKeyboardKey(458939L);
    public static PhysicalKeyboardKey numpadMemoryStore = new PhysicalKeyboardKey(458960L);
    public static PhysicalKeyboardKey numpadMemoryRecall = new PhysicalKeyboardKey(458961L);
    public static PhysicalKeyboardKey numpadMemoryClear = new PhysicalKeyboardKey(458962L);
    public static PhysicalKeyboardKey numpadMemoryAdd = new PhysicalKeyboardKey(458963L);
    public static PhysicalKeyboardKey numpadMemorySubtract = new PhysicalKeyboardKey(458964L);
    public static PhysicalKeyboardKey numpadSignChange = new PhysicalKeyboardKey(458967L);
    public static PhysicalKeyboardKey numpadClear = new PhysicalKeyboardKey(458968L);
    public static PhysicalKeyboardKey numpadClearEntry = new PhysicalKeyboardKey(458969L);
    public static PhysicalKeyboardKey controlLeft = new PhysicalKeyboardKey(458976L);
    public static PhysicalKeyboardKey shiftLeft = new PhysicalKeyboardKey(458977L);
    public static PhysicalKeyboardKey altLeft = new PhysicalKeyboardKey(458978L);
    public static PhysicalKeyboardKey metaLeft = new PhysicalKeyboardKey(458979L);
    public static PhysicalKeyboardKey controlRight = new PhysicalKeyboardKey(458980L);
    public static PhysicalKeyboardKey shiftRight = new PhysicalKeyboardKey(458981L);
    public static PhysicalKeyboardKey altRight = new PhysicalKeyboardKey(458982L);
    public static PhysicalKeyboardKey metaRight = new PhysicalKeyboardKey(458983L);
    public static PhysicalKeyboardKey info = new PhysicalKeyboardKey(786528L);
    public static PhysicalKeyboardKey closedCaptionToggle = new PhysicalKeyboardKey(786529L);
    public static PhysicalKeyboardKey brightnessUp = new PhysicalKeyboardKey(786543L);
    public static PhysicalKeyboardKey brightnessDown = new PhysicalKeyboardKey(786544L);
    public static PhysicalKeyboardKey brightnessToggle = new PhysicalKeyboardKey(786546L);
    public static PhysicalKeyboardKey brightnessMinimum = new PhysicalKeyboardKey(786547L);
    public static PhysicalKeyboardKey brightnessMaximum = new PhysicalKeyboardKey(786548L);
    public static PhysicalKeyboardKey brightnessAuto = new PhysicalKeyboardKey(786549L);
    public static PhysicalKeyboardKey kbdIllumUp = new PhysicalKeyboardKey(786553L);
    public static PhysicalKeyboardKey kbdIllumDown = new PhysicalKeyboardKey(786554L);
    public static PhysicalKeyboardKey mediaLast = new PhysicalKeyboardKey(786563L);
    public static PhysicalKeyboardKey launchPhone = new PhysicalKeyboardKey(786572L);
    public static PhysicalKeyboardKey programGuide = new PhysicalKeyboardKey(786573L);
    public static PhysicalKeyboardKey exit = new PhysicalKeyboardKey(786580L);
    public static PhysicalKeyboardKey channelUp = new PhysicalKeyboardKey(786588L);
    public static PhysicalKeyboardKey channelDown = new PhysicalKeyboardKey(786589L);
    public static PhysicalKeyboardKey mediaPlay = new PhysicalKeyboardKey(786608L);
    public static PhysicalKeyboardKey mediaPause = new PhysicalKeyboardKey(786609L);
    public static PhysicalKeyboardKey mediaRecord = new PhysicalKeyboardKey(786610L);
    public static PhysicalKeyboardKey mediaFastForward = new PhysicalKeyboardKey(786611L);
    public static PhysicalKeyboardKey mediaRewind = new PhysicalKeyboardKey(786612L);
    public static PhysicalKeyboardKey mediaTrackNext = new PhysicalKeyboardKey(786613L);
    public static PhysicalKeyboardKey mediaTrackPrevious = new PhysicalKeyboardKey(786614L);
    public static PhysicalKeyboardKey mediaStop = new PhysicalKeyboardKey(786615L);
    public static PhysicalKeyboardKey eject = new PhysicalKeyboardKey(786616L);
    public static PhysicalKeyboardKey mediaPlayPause = new PhysicalKeyboardKey(786637L);
    public static PhysicalKeyboardKey speechInputToggle = new PhysicalKeyboardKey(786639L);
    public static PhysicalKeyboardKey bassBoost = new PhysicalKeyboardKey(786661L);
    public static PhysicalKeyboardKey mediaSelect = new PhysicalKeyboardKey(786819L);
    public static PhysicalKeyboardKey launchWordProcessor = new PhysicalKeyboardKey(786820L);
    public static PhysicalKeyboardKey launchSpreadsheet = new PhysicalKeyboardKey(786822L);
    public static PhysicalKeyboardKey launchMail = new PhysicalKeyboardKey(786826L);
    public static PhysicalKeyboardKey launchContacts = new PhysicalKeyboardKey(786829L);
    public static PhysicalKeyboardKey launchCalendar = new PhysicalKeyboardKey(786830L);
    public static PhysicalKeyboardKey launchApp2 = new PhysicalKeyboardKey(786834L);
    public static PhysicalKeyboardKey launchApp1 = new PhysicalKeyboardKey(786836L);
    public static PhysicalKeyboardKey launchInternetBrowser = new PhysicalKeyboardKey(786838L);
    public static PhysicalKeyboardKey logOff = new PhysicalKeyboardKey(786844L);
    public static PhysicalKeyboardKey lockScreen = new PhysicalKeyboardKey(786846L);
    public static PhysicalKeyboardKey launchControlPanel = new PhysicalKeyboardKey(786847L);
    public static PhysicalKeyboardKey selectTask = new PhysicalKeyboardKey(786850L);
    public static PhysicalKeyboardKey launchDocuments = new PhysicalKeyboardKey(786855L);
    public static PhysicalKeyboardKey spellCheck = new PhysicalKeyboardKey(786859L);
    public static PhysicalKeyboardKey launchKeyboardLayout = new PhysicalKeyboardKey(786862L);
    public static PhysicalKeyboardKey launchScreenSaver = new PhysicalKeyboardKey(786865L);
    public static PhysicalKeyboardKey launchAudioBrowser = new PhysicalKeyboardKey(786871L);
    public static PhysicalKeyboardKey launchAssistant = new PhysicalKeyboardKey(786891L);
    public static PhysicalKeyboardKey newKey = new PhysicalKeyboardKey(786945L);
    public static PhysicalKeyboardKey close = new PhysicalKeyboardKey(786947L);
    public static PhysicalKeyboardKey save = new PhysicalKeyboardKey(786951L);
    public static PhysicalKeyboardKey print = new PhysicalKeyboardKey(786952L);
    public static PhysicalKeyboardKey browserSearch = new PhysicalKeyboardKey(786977L);
    public static PhysicalKeyboardKey browserHome = new PhysicalKeyboardKey(786979L);
    public static PhysicalKeyboardKey browserBack = new PhysicalKeyboardKey(786980L);
    public static PhysicalKeyboardKey browserForward = new PhysicalKeyboardKey(786981L);
    public static PhysicalKeyboardKey browserStop = new PhysicalKeyboardKey(786982L);
    public static PhysicalKeyboardKey browserRefresh = new PhysicalKeyboardKey(786983L);
    public static PhysicalKeyboardKey browserFavorites = new PhysicalKeyboardKey(786986L);
    public static PhysicalKeyboardKey zoomIn = new PhysicalKeyboardKey(786989L);
    public static PhysicalKeyboardKey zoomOut = new PhysicalKeyboardKey(786990L);
    public static PhysicalKeyboardKey zoomToggle = new PhysicalKeyboardKey(786994L);
    public static PhysicalKeyboardKey redo = new PhysicalKeyboardKey(787065L);
    public static PhysicalKeyboardKey mailReply = new PhysicalKeyboardKey(787081L);
    public static PhysicalKeyboardKey mailForward = new PhysicalKeyboardKey(787083L);
    public static PhysicalKeyboardKey mailSend = new PhysicalKeyboardKey(787084L);
    public static PhysicalKeyboardKey keyboardLayoutSelect = new PhysicalKeyboardKey(787101L);
    public static PhysicalKeyboardKey showAllWindows = new PhysicalKeyboardKey(787103L);
    internal static DartMap<long, PhysicalKeyboardKey> _knownPhysicalKeys = new DartMap<long, PhysicalKeyboardKey> { [16L] = hyper, [17L] = superKey, [18L] = fn, [19L] = fnLock, [20L] = suspend, [21L] = resume, [22L] = turbo, [23L] = privacyScreenToggle, [24L] = microphoneMuteToggle, [65666L] = sleep, [65667L] = wakeUp, [65717L] = displayToggleIntExt, [392961L] = gameButton1, [392962L] = gameButton2, [392963L] = gameButton3, [392964L] = gameButton4, [392965L] = gameButton5, [392966L] = gameButton6, [392967L] = gameButton7, [392968L] = gameButton8, [392969L] = gameButton9, [392970L] = gameButton10, [392971L] = gameButton11, [392972L] = gameButton12, [392973L] = gameButton13, [392974L] = gameButton14, [392975L] = gameButton15, [392976L] = gameButton16, [392977L] = gameButtonA, [392978L] = gameButtonB, [392979L] = gameButtonC, [392980L] = gameButtonLeft1, [392981L] = gameButtonLeft2, [392982L] = gameButtonMode, [392983L] = gameButtonRight1, [392984L] = gameButtonRight2, [392985L] = gameButtonSelect, [392986L] = gameButtonStart, [392987L] = gameButtonThumbLeft, [392988L] = gameButtonThumbRight, [392989L] = gameButtonX, [392990L] = gameButtonY, [392991L] = gameButtonZ, [458752L] = usbReserved, [458753L] = usbErrorRollOver, [458754L] = usbPostFail, [458755L] = usbErrorUndefined, [458756L] = keyA, [458757L] = keyB, [458758L] = keyC, [458759L] = keyD, [458760L] = keyE, [458761L] = keyF, [458762L] = keyG, [458763L] = keyH, [458764L] = keyI, [458765L] = keyJ, [458766L] = keyK, [458767L] = keyL, [458768L] = keyM, [458769L] = keyN, [458770L] = keyO, [458771L] = keyP, [458772L] = keyQ, [458773L] = keyR, [458774L] = keyS, [458775L] = keyT, [458776L] = keyU, [458777L] = keyV, [458778L] = keyW, [458779L] = keyX, [458780L] = keyY, [458781L] = keyZ, [458782L] = digit1, [458783L] = digit2, [458784L] = digit3, [458785L] = digit4, [458786L] = digit5, [458787L] = digit6, [458788L] = digit7, [458789L] = digit8, [458790L] = digit9, [458791L] = digit0, [458792L] = enter, [458793L] = escape, [458794L] = backspace, [458795L] = tab, [458796L] = space, [458797L] = minus, [458798L] = equal, [458799L] = bracketLeft, [458800L] = bracketRight, [458801L] = backslash, [458803L] = semicolon, [458804L] = quote, [458805L] = backquote, [458806L] = comma, [458807L] = period, [458808L] = slash, [458809L] = capsLock, [458810L] = f1, [458811L] = f2, [458812L] = f3, [458813L] = f4, [458814L] = f5, [458815L] = f6, [458816L] = f7, [458817L] = f8, [458818L] = f9, [458819L] = f10, [458820L] = f11, [458821L] = f12, [458822L] = printScreen, [458823L] = scrollLock, [458824L] = pause, [458825L] = insert, [458826L] = home, [458827L] = pageUp, [458828L] = delete, [458829L] = end, [458830L] = pageDown, [458831L] = arrowRight, [458832L] = arrowLeft, [458833L] = arrowDown, [458834L] = arrowUp, [458835L] = numLock, [458836L] = numpadDivide, [458837L] = numpadMultiply, [458838L] = numpadSubtract, [458839L] = numpadAdd, [458840L] = numpadEnter, [458841L] = numpad1, [458842L] = numpad2, [458843L] = numpad3, [458844L] = numpad4, [458845L] = numpad5, [458846L] = numpad6, [458847L] = numpad7, [458848L] = numpad8, [458849L] = numpad9, [458850L] = numpad0, [458851L] = numpadDecimal, [458852L] = intlBackslash, [458853L] = contextMenu, [458854L] = power, [458855L] = numpadEqual, [458856L] = f13, [458857L] = f14, [458858L] = f15, [458859L] = f16, [458860L] = f17, [458861L] = f18, [458862L] = f19, [458863L] = f20, [458864L] = f21, [458865L] = f22, [458866L] = f23, [458867L] = f24, [458868L] = open, [458869L] = help, [458871L] = select, [458873L] = again, [458874L] = undo, [458875L] = cut, [458876L] = copy, [458877L] = paste, [458878L] = find, [458879L] = audioVolumeMute, [458880L] = audioVolumeUp, [458881L] = audioVolumeDown, [458885L] = numpadComma, [458887L] = intlRo, [458888L] = kanaMode, [458889L] = intlYen, [458890L] = convert, [458891L] = nonConvert, [458896L] = lang1, [458897L] = lang2, [458898L] = lang3, [458899L] = lang4, [458900L] = lang5, [458907L] = abort, [458915L] = props, [458934L] = numpadParenLeft, [458935L] = numpadParenRight, [458939L] = numpadBackspace, [458960L] = numpadMemoryStore, [458961L] = numpadMemoryRecall, [458962L] = numpadMemoryClear, [458963L] = numpadMemoryAdd, [458964L] = numpadMemorySubtract, [458967L] = numpadSignChange, [458968L] = numpadClear, [458969L] = numpadClearEntry, [458976L] = controlLeft, [458977L] = shiftLeft, [458978L] = altLeft, [458979L] = metaLeft, [458980L] = controlRight, [458981L] = shiftRight, [458982L] = altRight, [458983L] = metaRight, [786528L] = info, [786529L] = closedCaptionToggle, [786543L] = brightnessUp, [786544L] = brightnessDown, [786546L] = brightnessToggle, [786547L] = brightnessMinimum, [786548L] = brightnessMaximum, [786549L] = brightnessAuto, [786553L] = kbdIllumUp, [786554L] = kbdIllumDown, [786563L] = mediaLast, [786572L] = launchPhone, [786573L] = programGuide, [786580L] = exit, [786588L] = channelUp, [786589L] = channelDown, [786608L] = mediaPlay, [786609L] = mediaPause, [786610L] = mediaRecord, [786611L] = mediaFastForward, [786612L] = mediaRewind, [786613L] = mediaTrackNext, [786614L] = mediaTrackPrevious, [786615L] = mediaStop, [786616L] = eject, [786637L] = mediaPlayPause, [786639L] = speechInputToggle, [786661L] = bassBoost, [786819L] = mediaSelect, [786820L] = launchWordProcessor, [786822L] = launchSpreadsheet, [786826L] = launchMail, [786829L] = launchContacts, [786830L] = launchCalendar, [786834L] = launchApp2, [786836L] = launchApp1, [786838L] = launchInternetBrowser, [786844L] = logOff, [786846L] = lockScreen, [786847L] = launchControlPanel, [786850L] = selectTask, [786855L] = launchDocuments, [786859L] = spellCheck, [786862L] = launchKeyboardLayout, [786865L] = launchScreenSaver, [786871L] = launchAudioBrowser, [786891L] = launchAssistant, [786945L] = newKey, [786947L] = close, [786951L] = save, [786952L] = print, [786977L] = browserSearch, [786979L] = browserHome, [786980L] = browserBack, [786981L] = browserForward, [786982L] = browserStop, [786983L] = browserRefresh, [786986L] = browserFavorites, [786989L] = zoomIn, [786990L] = zoomOut, [786994L] = zoomToggle, [787065L] = redo, [787081L] = mailReply, [787083L] = mailForward, [787084L] = mailSend, [787101L] = keyboardLayoutSelect, [787103L] = showAllWindows };
    internal static DartMap<long, string> _debugNames = (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kReleaseMode ? new DartMap<long, string>() : new DartMap<long, string> { [16L] = "Hyper", [17L] = "Super Key", [18L] = "Fn", [19L] = "Fn Lock", [20L] = "Suspend", [21L] = "Resume", [22L] = "Turbo", [23L] = "Privacy Screen Toggle", [24L] = "Microphone Mute Toggle", [65666L] = "Sleep", [65667L] = "Wake Up", [65717L] = "Display Toggle Int Ext", [392961L] = "Game Button 1", [392962L] = "Game Button 2", [392963L] = "Game Button 3", [392964L] = "Game Button 4", [392965L] = "Game Button 5", [392966L] = "Game Button 6", [392967L] = "Game Button 7", [392968L] = "Game Button 8", [392969L] = "Game Button 9", [392970L] = "Game Button 10", [392971L] = "Game Button 11", [392972L] = "Game Button 12", [392973L] = "Game Button 13", [392974L] = "Game Button 14", [392975L] = "Game Button 15", [392976L] = "Game Button 16", [392977L] = "Game Button A", [392978L] = "Game Button B", [392979L] = "Game Button C", [392980L] = "Game Button Left 1", [392981L] = "Game Button Left 2", [392982L] = "Game Button Mode", [392983L] = "Game Button Right 1", [392984L] = "Game Button Right 2", [392985L] = "Game Button Select", [392986L] = "Game Button Start", [392987L] = "Game Button Thumb Left", [392988L] = "Game Button Thumb Right", [392989L] = "Game Button X", [392990L] = "Game Button Y", [392991L] = "Game Button Z", [458752L] = "Usb Reserved", [458753L] = "Usb Error Roll Over", [458754L] = "Usb Post Fail", [458755L] = "Usb Error Undefined", [458756L] = "Key A", [458757L] = "Key B", [458758L] = "Key C", [458759L] = "Key D", [458760L] = "Key E", [458761L] = "Key F", [458762L] = "Key G", [458763L] = "Key H", [458764L] = "Key I", [458765L] = "Key J", [458766L] = "Key K", [458767L] = "Key L", [458768L] = "Key M", [458769L] = "Key N", [458770L] = "Key O", [458771L] = "Key P", [458772L] = "Key Q", [458773L] = "Key R", [458774L] = "Key S", [458775L] = "Key T", [458776L] = "Key U", [458777L] = "Key V", [458778L] = "Key W", [458779L] = "Key X", [458780L] = "Key Y", [458781L] = "Key Z", [458782L] = "Digit 1", [458783L] = "Digit 2", [458784L] = "Digit 3", [458785L] = "Digit 4", [458786L] = "Digit 5", [458787L] = "Digit 6", [458788L] = "Digit 7", [458789L] = "Digit 8", [458790L] = "Digit 9", [458791L] = "Digit 0", [458792L] = "Enter", [458793L] = "Escape", [458794L] = "Backspace", [458795L] = "Tab", [458796L] = "Space", [458797L] = "Minus", [458798L] = "Equal", [458799L] = "Bracket Left", [458800L] = "Bracket Right", [458801L] = "Backslash", [458803L] = "Semicolon", [458804L] = "Quote", [458805L] = "Backquote", [458806L] = "Comma", [458807L] = "Period", [458808L] = "Slash", [458809L] = "Caps Lock", [458810L] = "F1", [458811L] = "F2", [458812L] = "F3", [458813L] = "F4", [458814L] = "F5", [458815L] = "F6", [458816L] = "F7", [458817L] = "F8", [458818L] = "F9", [458819L] = "F10", [458820L] = "F11", [458821L] = "F12", [458822L] = "Print Screen", [458823L] = "Scroll Lock", [458824L] = "Pause", [458825L] = "Insert", [458826L] = "Home", [458827L] = "Page Up", [458828L] = "Delete", [458829L] = "End", [458830L] = "Page Down", [458831L] = "Arrow Right", [458832L] = "Arrow Left", [458833L] = "Arrow Down", [458834L] = "Arrow Up", [458835L] = "Num Lock", [458836L] = "Numpad Divide", [458837L] = "Numpad Multiply", [458838L] = "Numpad Subtract", [458839L] = "Numpad Add", [458840L] = "Numpad Enter", [458841L] = "Numpad 1", [458842L] = "Numpad 2", [458843L] = "Numpad 3", [458844L] = "Numpad 4", [458845L] = "Numpad 5", [458846L] = "Numpad 6", [458847L] = "Numpad 7", [458848L] = "Numpad 8", [458849L] = "Numpad 9", [458850L] = "Numpad 0", [458851L] = "Numpad Decimal", [458852L] = "Intl Backslash", [458853L] = "Context Menu", [458854L] = "Power", [458855L] = "Numpad Equal", [458856L] = "F13", [458857L] = "F14", [458858L] = "F15", [458859L] = "F16", [458860L] = "F17", [458861L] = "F18", [458862L] = "F19", [458863L] = "F20", [458864L] = "F21", [458865L] = "F22", [458866L] = "F23", [458867L] = "F24", [458868L] = "Open", [458869L] = "Help", [458871L] = "Select", [458873L] = "Again", [458874L] = "Undo", [458875L] = "Cut", [458876L] = "Copy", [458877L] = "Paste", [458878L] = "Find", [458879L] = "Audio Volume Mute", [458880L] = "Audio Volume Up", [458881L] = "Audio Volume Down", [458885L] = "Numpad Comma", [458887L] = "Intl Ro", [458888L] = "Kana Mode", [458889L] = "Intl Yen", [458890L] = "Convert", [458891L] = "Non Convert", [458896L] = "Lang 1", [458897L] = "Lang 2", [458898L] = "Lang 3", [458899L] = "Lang 4", [458900L] = "Lang 5", [458907L] = "Abort", [458915L] = "Props", [458934L] = "Numpad Paren Left", [458935L] = "Numpad Paren Right", [458939L] = "Numpad Backspace", [458960L] = "Numpad Memory Store", [458961L] = "Numpad Memory Recall", [458962L] = "Numpad Memory Clear", [458963L] = "Numpad Memory Add", [458964L] = "Numpad Memory Subtract", [458967L] = "Numpad Sign Change", [458968L] = "Numpad Clear", [458969L] = "Numpad Clear Entry", [458976L] = "Control Left", [458977L] = "Shift Left", [458978L] = "Alt Left", [458979L] = "Meta Left", [458980L] = "Control Right", [458981L] = "Shift Right", [458982L] = "Alt Right", [458983L] = "Meta Right", [786528L] = "Info", [786529L] = "Closed Caption Toggle", [786543L] = "Brightness Up", [786544L] = "Brightness Down", [786546L] = "Brightness Toggle", [786547L] = "Brightness Minimum", [786548L] = "Brightness Maximum", [786549L] = "Brightness Auto", [786553L] = "Kbd Illum Up", [786554L] = "Kbd Illum Down", [786563L] = "Media Last", [786572L] = "Launch Phone", [786573L] = "Program Guide", [786580L] = "Exit", [786588L] = "Channel Up", [786589L] = "Channel Down", [786608L] = "Media Play", [786609L] = "Media Pause", [786610L] = "Media Record", [786611L] = "Media Fast Forward", [786612L] = "Media Rewind", [786613L] = "Media Track Next", [786614L] = "Media Track Previous", [786615L] = "Media Stop", [786616L] = "Eject", [786637L] = "Media Play Pause", [786639L] = "Speech Input Toggle", [786661L] = "Bass Boost", [786819L] = "Media Select", [786820L] = "Launch Word Processor", [786822L] = "Launch Spreadsheet", [786826L] = "Launch Mail", [786829L] = "Launch Contacts", [786830L] = "Launch Calendar", [786834L] = "Launch App2", [786836L] = "Launch App1", [786838L] = "Launch Internet Browser", [786844L] = "Log Off", [786846L] = "Lock Screen", [786847L] = "Launch Control Panel", [786850L] = "Select Task", [786855L] = "Launch Documents", [786859L] = "Spell Check", [786862L] = "Launch Keyboard Layout", [786865L] = "Launch Screen Saver", [786871L] = "Launch Audio Browser", [786891L] = "Launch Assistant", [786945L] = "New Key", [786947L] = "Close", [786951L] = "Save", [786952L] = "Print", [786977L] = "Browser Search", [786979L] = "Browser Home", [786980L] = "Browser Back", [786981L] = "Browser Forward", [786982L] = "Browser Stop", [786983L] = "Browser Refresh", [786986L] = "Browser Favorites", [786989L] = "Zoom In", [786990L] = "Zoom Out", [786994L] = "Zoom Toggle", [787065L] = "Redo", [787081L] = "Mail Reply", [787083L] = "Mail Forward", [787084L] = "Mail Send", [787101L] = "Keyboard Layout Select", [787103L] = "Show All Windows" });

    public PhysicalKeyboardKey(long usbHidUsage)
    {
        this.usbHidUsage = usbHidUsage;
    }

    public virtual string? debugName
    {
        get
        {
            string? result = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    result = (_debugNames.GetValueOrDefault(usbHidUsage) ?? $"Key with ID 0x{usbHidUsage.toRadixString(16L).padLeft(8L, "0")}");
                    return true;
                });
            return result;
        }
    }
    public override int GetHashCode() => usbHidUsage.GetHashCode();
    public override bool Equals(object? other)
    {
        var __other = other as PhysicalKeyboardKey;
        if (__other is null) return false;
        if (ReferenceEquals(this, __other))
        {
            return true;
        }
        if ((!object.Equals(__other.GetType(), this.GetType())))
        {
            return false;
        }
        return ((__other is PhysicalKeyboardKey) && (((PhysicalKeyboardKey)__other).usbHidUsage == usbHidUsage));
    }

    public static PhysicalKeyboardKey? findKeyByCode(long usageCode) => _knownPhysicalKeys.GetValueOrDefault(usageCode);
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.Add(new StringProperty("usbHidUsage", $"0x{usbHidUsage.toRadixString(16L).padLeft(8L, "0")}"));
        properties.Add(new StringProperty("debugName", debugName, defaultValue: null));
    }

    public static IEnumerable<PhysicalKeyboardKey> knownPhysicalKeys => _knownPhysicalKeys.Values;
}

