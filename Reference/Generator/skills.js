

var Skills = function() {

    var ClassType = {
        Monk: "Monk",
        Crusader: "Crusader",
        Barbarian: "Barbarian",
        Wizard: "Wizard",
        WitchDoctor: "WitchDoctor",
        DemonHunter: "DemonHunter"
    }

    var RequestSkills = function() {

        var data = {
            Barbarian: {},
            WitchDoctor: {},
            Crusader: {},
            Wizard: {},
            Monk: {},
            DemonHunter: {},
        };

        var request = function(className, callback) {

            console.log("Requesting " + className + " Data!");

            var classToken = "";
            switch (className) {
            case ClassType.DemonHunter:
                classToken = "demon-hunter"
                break;
            case ClassType.WitchDoctor:
                classToken = "witch-doctor"
                break;
            default:
                classToken = className.toLowerCase();
            }

            $.get("http://us.battle.net/d3/en/data/calculator/" + classToken, {}, function(data) {

                // Origin bypass with YQL wraps as a HTML document
                // So we have to strip it all out again.
                var document = $("<div>").html(data.responseText);
                document.first("title").remove();
                document.first("meta").remove();

                var objectString = document.text();

                // Line Breaks
                objectString = objectString.replace(/(?:\r\n|\r|\n)/g, '');

                // Double White Spaces
                objectString = objectString.replace(/\s{2,}/g, ' ');

                if (objectString != "") {

                    // Save to we dont have to request it again
                    data[className] = $.parseJSON(objectString);

                }

                // handle callback
                callback(data[className], className);

            });

        }

        console.log("Initialized Skills Object!");

        return {
            Monk: function(callback) {

                if ($.isEmptyObject(data.Monk)) {
                    request(ClassType.Monk, callback);
                } else {
                    callback(data.Monk, ClassType.Monk);
                }
            },
            Crusader: function(callback) {

                if ($.isEmptyObject(data.Crusader)) {
                    request(ClassType.Crusader, callback);
                } else {
                    callback(data.Crusader, ClassType.Crusader);
                }
            },
            Barbarian: function(callback) {

                if ($.isEmptyObject(data.Barbarian)) {
                    request(ClassType.Barbarian, callback);
                } else {
                    callback(data.Barbarian, ClassType.Barbarian);
                }
            },
            Wizard: function(callback) {

                if ($.isEmptyObject(data.Wizard)) {
                    request(ClassType.Wizard, callback);
                } else {
                    callback(data.Wizard, ClassType.Wizard);
                }
            },
            WitchDoctor: function(callback) {

                if ($.isEmptyObject(data.WitchDoctor)) {
                    request(ClassType.WitchDoctor, callback);
                } else {
                    callback(data.WitchDoctor, ClassType.WitchDoctor);
                }
            },
            DemonHunter: function(callback) {

                if ($.isEmptyObject(data.DemonHunter)) {
                    request(ClassType.DemonHunter, callback);
                } else {
                    callback(data.DemonHunter, ClassType.DemonHunter);
                }
            },

        };

    }();

    var HandleSkillData = function(onDataLoadFinished) {

        var todo = Object.keys(RequestSkills).length;

        var data = {
            timestamp: (new Date()).toUTCString(),
            runes: [],
            categories: [],
            skills: [],
            passives: [],
            classes: [],
        }

        $.each(RequestSkills, function(classIndex, classDefinition) {

            classDefinition(function(classData, className) {

                var playerclass = {
                    name: className,
                    skills: [],
                    passives: [],
                    runes: [],
                    categories: []
                }

                // Process Skills
                $.each(classData.skills, function(skillIndex, skillData) {

                    /* SKILL
                    categoryName: "Primary"
                    categorySlug: "primary"
                    description: " Generate: 14 Spirit per attack Teleport to your target and unleash a series of extremely fast punches that deal 122% weapon damage as Lightning. Every third hit deals 183% weapon damage as Lightning split between all enemies in front of you. "
                    icon: "monk_fistsofthunder"
                    name: "Fists of Thunder"
                    orderIndex: 0
                    primary: true
                    requiredLevel: 1
                    runes: Array[5]
                    simpleDescription: " Generate: 14 Spirit per attack Teleport to your target and attack it with a series of rapid punches. "
                    slug: "fists-of-thunder"
                    tooltipParams: "skill/monk/fists-of-thunder"
                    __proto__: Object
                    */

                    //skillData.orderIndex = skillIndex + 1;
                    skillData.className = className;
                    playerclass.skills.push(skillData);
                    data.skills.push(skillData);

                    if ($.inArray(skillData.categoryName, data.categories) == -1) {
                        data.categories.push(skillData.categoryName);
                    }

                    if ($.inArray(skillData.categoryName, playerclass.categories) == -1) {
                        playerclass.categories.push(skillData.categoryName);
                    }

                    // Process Runes
                    $.each(skillData.runes, function(runeIndex, runeData) {

                        /* RUNE
                        description: " Release an electric shockwave with every punch that hits all enemies within 6 yards of your primary enemy for 95% weapon damage as Lightning and causes knockback with every third hit. "
                        name: "Thunderclap"
                        requiredLevel: 6
                        simpleDescription: " Release an electric shockwave with every punch and cause knockback every third hit. "
                        tooltipParams: "rune/fists-of-thunder/a"
                        type: "a"
                        __proto__: Object
                        */

                        runeData.orderIndex = runeIndex + 1;
                        runeData.skill = skillData.name;
                        runeData.className = className;
                        playerclass.runes.push(runeData);
                        data.runes.push(runeData);

                    });

                });

                // Process Passives
                $.each(classData.traits, function(passiveIndex, passiveData) {

                    /* TRAIT
                    description: " Damage you deal reduces enemy damage by 20% for 2.5 seconds. "
                    icon: "monk_passive_resolve"
                    name: "Resolve"
                    orderIndex: 0
                    requiredLevel: 10
                    simpleDescription: " Damage you deal reduces enemy damage by 20% for 2.5 seconds. "
                    slug: "resolve"
                    tooltipParams: "skill/monk/resolve"
                    __proto__: Object
                    */

                    //passiveData.orderIndex = passiveIndex + 1;
                    passiveData.className = className;
                    playerclass.passives.push(passiveData);
                    data.passives.push(passiveData);

                });

                data.classes.push(playerclass);

                todo--;
                if (todo == 0) {
                    onDataLoadFinished(data);
                }

            });

        });

    };

    return {
        HandleSkillData: function (delegate) {            
            return HandleSkillData(delegate);
        },

    }

}();





