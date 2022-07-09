var fs = require("fs");
var child_process = require("child_process");

exports.startGameServer = function(players, gameServerPort, path, map) {
    if (players != null){
        var config = fs.readFileSync("./Config/config.json");
        var configData = JSON.parse(config);
        var objToJSON = new Object();
        objToJSON.players = new Array();
        objToJSON.game = new Object();
        objToJSON.gameInfo = new Object();
        var count = 0;
        var id_player = 1; 
        var max = players.length - 1;
        while (count <= max){
            console.log("coucou");
            console.log(players[count].spell1id);
            console.log(players[count].spell2id);
            console.log(summonerSpells[players[count].spell1id]);
            console.log(summonerSpells[players[count].spell2id]);
            objToJSON.players[count] = new Object();
            objToJSON.players[count]["runes"] = new Object();
            objToJSON.players[count].rank = "DIAMOND";
            objToJSON["players"][count].playerId = id_player ;
            objToJSON["players"][count].blowfishKey = "17BLOhi6KZsTtldTsizvHg==";
            objToJSON["players"][count].name = players[count].username;
            objToJSON["players"][count].champion = dictChamps[players[count].championId];
            objToJSON["players"][count].team = dictTeams[players[count].teamId];
            objToJSON["players"][count].skin = 1;
            objToJSON["players"][count].summoner1 = summonerSpells[players[count].spell1id];
            objToJSON["players"][count].summoner2 = summonerSpells[players[count].spell2id];
            objToJSON["players"][count].ribbon = 2;
            objToJSON["players"][count].icon = 0;
            objToJSON["players"][count]["runes"] = runeeuuuh;
          //  objToJSON["players"][count]["runes"] = talents;
            count++;
            id_player++;
        }
        objToJSON["game"]["map"] = map;
        objToJSON["game"]["gameMode"] = "CLASSIC";
        objToJSON["game"]["dataPackage"] = "LeagueSandbox-Scripts";
        objToJSON["gameInfo"]["MANACOSTS_ENABLED"] = true;
        objToJSON["gameInfo"]["COOLDOWNS_ENABLED"] = true;
        objToJSON["gameInfo"]["CHEATS_ENABLED"] = true;
        objToJSON["gameInfo"]["MINION_SPAWNS_ENABLED"] = true;
        objToJSON["gameInfo"]["CONTENT_PATH"] = "../../../../../Content";
        objToJSON["gameInfo"]["IS_DAMAGE_TEXT_GLOBAL"] = false;
       // objToJSON["gameInfo"]["JUNGLE_SPAWNS_ENABLED"] = true;
        var args = [];
        args[0] = "--config"
        args[1] =  __dirname  + "/config/GameInfo.json"
        args[2] = "--port";
        args[3] = gameServerPort;
        var readyToJSON = JSON.stringify(objToJSON);
        fs.writeFile("./app/factories/config/GameInfo.json", readyToJSON, (err) => {
            if (err) throw err;
                child_process.execFile(path, args, {cwd: configData.pathToFolder, maxBuffer: 1024 * 90000}, (error) => {
                    if (error){
                        throw error;
                    }
                });
        });
    }   
}
function fillRune(){
    const runes = new Array();
    var iterate = 1
    for (let iterate = 1; iterate <= 30; iterate++){
        if (iterate < 10){
            runes[iterate.toString()] = 5245; 
        }else if (iterate < 19){
            runes[iterate.toString()] = 5317;
        }else if (iterate <= 31){
            runes[iterate.toString()] = 5289;
        }
    }
    return runes;
}
var dictTeams = {
    0: "BLUE",
    1: "PURPLE"
}
var dictChamps = {
    266: "Aatrox",
    103: "Ahri",
    84: "Akali",
    12: "Alistar",
    32: "Amumu",
    34: "Anivia",
    1: "Annie",
    22: "Ashe",
    268: "Azir",
    53: "Blitzcrank",
    63: "Brand",
    201: "Braum",
    51: "Caitlyn",
    69: "Cassiopeia",
    31: "Chogath",
    42: "Corki",
    122: "Darius",
    131: "Diana",
    36: "DrMundo",
    119: "Draven",
    60: "Elise",
    28: "Evelynn",
    81: "Ezreal",
    9: "FiddleSticks",
    114: "Fiora",
    105: "Fizz",
    3: "Galio",
    41: "Gangplank",
    86: "Garen",
    150: "Gnar",
    79: "Gragas",
    104: "Graves",
    120: "Hecarim",
    74: "Heimerdinger",
    39: "Irelia",
    40: "Janna",
    59: "JarvanIV",
    24: "Jax",
    126: "Jayce",
    222: "Jinx",
    43: "Karma",
    30: "Karthus",
    38: "Kassadin",
    55: "Katarina",
    10: "Kayle",
    85: "Kennen",
    121: "Khazix",
    96: "KogMaw",
    7: "Leblanc",
    64: "LeeSin",
    89: "Leona",
    127: "Lissandra",
    236: "Lucian",
    117: "Lulu",
    99: "Lux",
    54: "Malphite",
    11: "MasterYi",
    21: "MissFortune",
    82: "Mordekaiser",
    25: "Morgana",
    267: "Nami",
    75: "Nasus",
    111: "Nautilus",
    76: "Nidalee",
    56: "Nocturne",
    20: "Nunu",
    2: "Olaf",
    61: "Orianna",
    80: "Pantheon",
    78: "Poppy",
    133: "Quinn",
    33: "Rammus",
    58: "Renekton",
    107: "Rengar",
    92: "Riven",
    68: "Rumble",
    13: "Ryze",
    113: "Sejuani",
    35: "Shaco",
    98: "Shen",
    102: "Shyvana",
    27: "Singed",
    14: "Sion",
    15: "Sivir",
    72: "Skarner",
    37: "Sona",
    16: "Soraka",
    50: "Swain",
    134: "Syndra",
    91: "Talon",
    44: "Taric",
    17: "Teemo",
    412: "Thresh",
    18: "Tristana",
    48: "Trundle",
    23: "Tryndamere",
    4: "TwistedFate",
    29: "Twitch",
    77: "Udyr",
    6: "Urgot",
    110: "Varus",
    67: "Vayne",
    45: "Veigar",
    161: "Velkoz",
    254: "Vi",
    112: "Viktor",
    8: "Vladimir",
    106: "Volibear",
    19: "Warwick",
    62: "MonkeyKing",
    101: "Xerath",
    5: "XinZhao",
    157: "Yasuo",
    83: "Yorick",
    154: "Zac",
    238: "Zed",
    115: "Ziggs",
    26: "Zilean",
    143: "Zyra"
};
var runeeuuuh = {
    //DO NOT CHANGE THESE IF YOU DONT KNOW WHAT YOU ARE DOING.
    "1": 5245,
    "2": 5245,
    "3": 5245,
    "4": 5245,
    "5": 5245,
    "6": 5245,
    "7": 5245,
    "8": 5245,
    "9": 5245,
    "10": 5317,
    "11": 5317,
    "12": 5317,
    "13": 5317,
    "14": 5317,
    "15": 5317,
    "16": 5317,
    "17": 5317,
    "18": 5317,
    "19": 5289,
    "20": 5289,
    "21": 5289,
    "22": 5289,
    "23": 5289,
    "24": 5289,
    "25": 5289,
    "26": 5289,
    "27": 5289,
    "28": 5335,
    "29": 5335,
    "30": 5335
};
var talents = {
    //DO NOT CHANGE THESE IF YOU DONT KNOW WHAT YOU ARE DOING.
    "4111": 1,
    "4112": 3,
    "4114": 1,
    "4122": 3,
    "4124": 1,
    "4132": 1,
    "4134": 3,
    "4142": 3,
    "4151": 1,
    "4152": 3,
    "4162": 1,
    "4211": 2,
    "4213": 2,
    "4221": 1,
    "4222": 3,
    "4232": 1
};
    var summonerSpells = {
        21 : "SummonerBarrier", 
        1 :  "SummonerBoost", 
        2 : "SummonerClairvoyance", 
        14 : "SummonerDot", 
        3 :  "SummonerExhaust", 
        4 : "SummonerFlash", 
        6 :  "SummonerHastet", 
        7 : "SummonerHeal",
        13 :  "SummonerMana",
        17 : "SummonerOdinGarrison", 
        10 :  "SummonerRevive",
        11 : "SummonerSmite", 
        12 : "SummonerTeleport"
    }
