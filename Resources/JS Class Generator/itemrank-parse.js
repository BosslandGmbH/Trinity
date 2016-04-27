

var DiabloProgress = function() {

    var RequestProxy = function () {

        var request = function(url, callback) {            

            $.get(url, {}, function (response) {

                callback($("<div>").html(response.responseText));

            });

        }

        return {

            GreaterRiftsPage: function (callback) {
                request("http://www.diabloprogress.com/top_items/greater_rifts_1000", callback);
            },
        };

    }();
    
    var classRank = {
        Monk: {
            Class: "ActorClass.Invalid",
            SampleSize: 0, 
            PercentUsed: 0, 
            Rank: 0, 
        },
        Barbarian: {
            Class: "ActorClass.Invalid",
            SampleSize: 0,
            PercentUsed: 0,
            Rank: 0,
        },
        DemonHunter: {
            Class: "ActorClass.Invalid",
            SampleSize: 0,
            PercentUsed: 0,
            Rank: 0,
        },
        Witchdoctor: {
            Class: "ActorClass.Invalid",
            SampleSize: 0,
            PercentUsed: 0,
            Rank: 0,
        },
        Wizard: {
            Class: "ActorClass.Invalid",
            SampleSize: 0,
            PercentUsed: 0,
            Rank: 0,
        },
        Crusader: {
            Class: "ActorClass.Invalid",
            SampleSize: 0,
            PercentUsed: 0,
            Rank: 0,
        },
    }

    var StripToClassName = function (string) {
        return string.replace(/\w\S*/g, function(txt) {
             return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
        }).replace(/\W|\s/g, '');
    }

    var ProcessData = function (onDataLoadFinished) {

        var todo = Object.keys(RequestProxy).length;

        var data = {
            timestamp: (new Date()).toUTCString(),
            all: {},
            Hardcore: [],
            Softcore: [],
        }

        $.each(RequestProxy, function (index, request) {

            // closure variables to access current type/class
            var gameType = "";
            var classType = "";

            // Expects jquery collection with a single table of items.
            var GrabItems = function (table,slotName) {

                var items = [];
                var rows = $(table).find("tr");

                $.each(rows, function () {

                    var item = $(this);
                    var itemName = item.find("[class*='diablo_']").text();

                    if (itemName != "") {

                        var itemId = StripToClassName(itemName);

                        var itemPercentHtml = item.find("nobr").text();
                        var itemPercentRaw = itemPercentHtml.match(/^\d{0,2}(\.\d{1,4})? *%?/g)[0];
                        var itemPercent = itemPercentRaw.substring(0, itemPercentRaw.length - 1);

                        
                        var itemCountHtml = item.find("nobr").text();
                        var itemCountRaw = itemCountHtml.match(/\([^\d]*(\d+)[^\d]*\)/g)[0];
                        var itemCount = itemCountRaw.substring(1, itemCountRaw.length - 1);

                        var itemObj = {
                            Name: itemName,
                            Id: itemId,
                            Count: itemCount,
                            Percent: itemPercent,
                            GameType: gameType,
                            Slot: slotName,
                            Rank: rows.index(this),
                        };

                        var clone = jQuery.extend({}, itemObj);

                        if (!(itemObj.Id in data.all)) {
                            data.all[itemObj.Id] = jQuery.extend({}, itemObj);
                            data.all[itemObj.Id].HardcoreRankByClass = {};
                            data.all[itemObj.Id].SoftcoreRankByClass = {};                            
                        }

                        if (gameType == "Hardcore") {

                            data.all[itemObj.Id].HardcoreRankByClass[classType] = {
                                Class: "ActorClass." + classType,
                                SampleSize: clone.Count,
                                PercentUsed: clone.Percent,
                                Rank: clone.Rank,
                            }
                        }

                        if (gameType == "Softcore") {

                            data.all[itemObj.Id].SoftcoreRankByClass[classType] = {
                                Class: "ActorClass." + classType,
                                SampleSize: clone.Count,
                                PercentUsed: clone.Percent,
                                Rank: clone.Rank,
                            }
                        }
                        items.push(itemObj);
                        
                    }
                });

                return items;

            }

            // Expects jquery collection of table elements
            var GetItemsBySlot = function (html) {
                return {
                    Bracers: GrabItems(html.get(6),"Bracers"),
                    Helm: GrabItems(html.get(2), "Helm"),
                    Shoulders: GrabItems(html.get(1), "Shoulders"),
                    Chest: GrabItems(html.get(5), "Chest"),
                    Gloves: GrabItems(html.get(4), "Gloves"),
                    Belts: GrabItems(html.get(8), "Belts"),
                    Legs: GrabItems(html.get(11), "Legs"),
                    Feet: GrabItems(html.get(12), "Feet"),
                    Amulet: GrabItems(html.get(3), "Amulet"),
                    Rings: GrabItems(html.get(7), "Rings"),
                    Righthand: GrabItems(html.get(10), "Righthand"),
                    Lefthand: GrabItems(html.get(9), "Lefthand"),
                    //Sets: GrabItems(html.get(0), "Sets"),
                }
            }

            var GetClassItems = function (html, classId, className) {

                var classTable = html.find(classId);
                var table = classTable.find("> table");
                var allitems = table.find("[class*='diablo_']");
                var hardcore = table.find("tr table.rating:even");
                var softcore = table.find("tr table.rating:odd");

                classType = className;

                var currentData = {};

                gameType = "Hardcore";
                currentData.Hardcore = GetItemsBySlot(hardcore);

                gameType = "Softcore";
                currentData.Softcore = GetItemsBySlot(softcore)

                return currentData;
            }

            request(function(html) {



                data.Monk = GetClassItems(html, '#monk_table', "Monk");
                data.Barbarian = GetClassItems(html, '#barbarian_table', "Barbarian");
                data.DemonHunter = GetClassItems(html, '#demon_hunter_table', "DemonHunter");
                data.Witchdoctor = GetClassItems(html, '#witch_doctor_table', "Witchdoctor");
                data.WizardItems = GetClassItems(html, '#wizard_table', "Wizard");
                data.CrusaderItems = GetClassItems(html, '#crusader_table', "Crusader");
         



                //$.each(classHtml, function() {

                //    var table = $(this).find("> table");
                //    var allitems = table.find("[class*='diablo_']");
                    
                //    var hardcore = table.find("tr table.rating:even");
                //    var softcore = table.find("tr table.rating:odd");

                //    gameType = "Hardcore";
                //    data.Hardcore = GetItemsBySlot(hardcore);

                //    gameType = "Softcore";
                //    classType = 
                //    data.Softcore = GetItemsBySlot(softcore)

                //});

     
                todo--;
                if (todo == 0) {
                    onDataLoadFinished(data);
                }

            });

        });

    };

    return {
        GetData: function (delegate) {
            return ProcessData(delegate);
        },

    }

}();
