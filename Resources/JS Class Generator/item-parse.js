

var Items = function() {

    var ItemType = {

        Helm: "helm",
        SpiritStone: "spirit-stone",
        VoodooMask: "voodoo-mask",
        WizardHat: "wizard-hat",
        Shoulders: "pauldrons",
        Chest: "chest-armor",
        Cloak: "cloak",
        Bracers: "bracers",
        Gloves: "gloves",
        Belts: "belt",
        MightyBelt: "mighty-belt",
        Legs: "pants",
        Feet: "boots",
        Amulet: "amulet",
        Rings: "ring",
        Shields: "shield",
        CrusaderShield: "crusader-shield",
        Mojos: "mojo",
        Orbs: "orb",
        Quivers: "quiver",
        Focus: "enchantress-focus",
        Token: "scoundrel-token",
        Relic: "templar-relic",
        Axe1h: "axe-1h",
        Dagger: "dagger",
        Mace1h: "mace-1h",
        Spears: "spear",
        Swords: "sword-1h",
        Knives: "ceremonial-knife",
        FistWeapon: "fist-weapon",
        Flail1h: "flail-1h",
        Mighty1h: "mighty-weapon-1h",
        Axe2h: "axe-2h",
        Mace: "mace-2h",
        Polearm: "polearm",
        Staves: "staff",
        Sword2h: "sword-2h",
        Diabo: "daibo",
        Flail2h: "flail-2h",
        Mighty2h: "mighty-weapon-2h",
        Bows: "bow",
        Crossbow: "crossbow",
        HandXBow: "hand-crossbow",
        Wand: "wand",

        GetKey: function (value) {
            var keys = Object.getOwnPropertyNames(ItemType);
            for (i = 0; i < keys.length; i++) {
                if (ItemType[keys[i]] == value) return keys[i];
            }
        }
    }

    var ItemData = function () {

        var data = {

            Helm: [],
            SpiritStone: [],
            VoodooMask: [],
            WizardHat: [],
            Shoulders: [],
            Chest: [],
            Cloak: [],
            Bracers: [],
            Gloves: [],
            Belts: [],
            MightyBelt: [],
            Legs: [],
            Feet: [],
            Amulet: [],
            Rings: [],
            Shields: [],
            CrusaderShield: [],
            Mojos: [],
            Orbs: [],
            Quivers: [],
            Focus: [],
            Token: [],
            Relic: [],
            Axe1h: [],
            Dagger: [],
            Mace1h: [],
            Spears: [],
            Swords: [],
            Knives: [],
            FistWeapon: [],
            Flail1h: [],
            Mighty1h: [],
            Axe2h: [],
            Mace: [],
            Polearm: [],
            Staves: [],
            Sword2h: [],
            Diabo: [],
            Flail2h: [],
            Mighty2h: [],
            Bows: [],
            Crossbow: [],
            HandXBow: [],
            Wand: []

        };

        var Item = {
            Quality: "",
            Type: "",
            ActorSNO: 0,
            Slug: "",
            Id: "",
            InternalName: "",
            Slot: "",
            ZetaType: "",
            Name: "",
            DataUrl: "",
            Url: "",
            IconUrl: "",
            RelativeUrl: "",
            IsCrafted: false,
            LegendaryText: "",
            IsSet: false,
            IsTwoHanded: false,
            TrinityItemType: "Unknown"
    }

        var request = function(itemTypeSlug, callback) {            

            var ItemTypeKey = ItemType.GetKey(itemTypeSlug);

            console.log("Requesting " + ItemTypeKey + " Data!");

            $.get("http://us.battle.net/d3/en/item/" + itemTypeSlug + "/", {}, function (response) {

                var raw = response.responseText.replace('&#39;', "'");

                // Origin bypass with YQL wraps as a HTML document
                // So we have to strip it all out again.
                var doc = $("<div>").html(raw);

                // Find all item records
                var items = doc.find(".table-items table tr");
                var staffofherding = false;

                // Find Regex Match Group Function
                function getMatches(string, regex, index) {
                    index || (index = 1);
                    var matches = [];
                    var match;
                    while (match = regex.exec(string)) {
                        matches.push(match[index]);
                    }
                    return matches;
                }

                var urlRegex = /\burl\s*\(\s*["']?([^"'\r\n\)\(]+)?\s*/g;

                // Process each item
                $.each(items, function () {

                    var item = Object.create(Item);

                    var itemHtml = $(this);
                    var itemDetails = itemHtml.find(".item-details");
                    var itemType = itemDetails.find(".item-type");

                    var itemDetailsLink = itemDetails.find(".item-details-text a");
                    
                    item.Name = itemDetailsLink.text().replace('&#39;', "'");
                    if (item.Name == "")
                        return;

                    // strange double listing
                    if (item.Name == "Staff of Herding" && staffofherding) {
                        return;
                    } else {
                        staffofherding = true;
                    }

                    item.RelativeUrl = itemDetailsLink.attr("href");
                    item.Url = "https://us.battle.net" + item.RelativeUrl;

                    var splitUrl = item.Url.split("/");

                    item.IsCrafted = item.Url.contains("recipe");

                    if (item.IsCrafted)
                        return;

                    item.Slug = splitUrl[splitUrl.length - 1]
                    item.DataUrl = "https://us.battle.net/api/d3/data/" + ((item.IsCrafted) ? "recipe" : "item") + "/" + item.Slug;

                    if (itemLookup[item.Slug] != null) {
                        item.ActorSNO = itemLookup[item.Slug][1];
                        item.InternalName = itemLookup[item.Slug][2];
                    }
                    
                    if(item.ActorSNO == 0)
                    {
                        var strippedName = item.Name.replace(/[^a-zA-Z ]/g, "").trim().toLowerCase();
                        var refByName = itemLookup[strippedName];
                        if (refByName != null) {
                            item.ActorSNO = refByName[1];
                        }
                    }

                    // Quality
                    var qualityClassName = itemType.find("span").attr("class");
                    switch (qualityClassName) {
                        case "d3-color-green": item.Quality = "Legendary"; break;
                        case "d3-color-default": item.Quality = "Normal"; break;
                        case "d3-color-orange": item.Quality = "Legendary"; break;
                        case "d3-color-yellow": item.Quality = "Rare"; break;
                        case "d3-color-blue": item.Quality = "Magic"; break;
                    }


                    var imageElement = itemDetails.find("span.icon-item-inner");
                    item.IconUrl = getMatches(imageElement.css('backgroundImage'), urlRegex, 1)[0];
                        //getMatches(imageElement.css('Background'), urlRegex, 1);
                        
                    // Hellfire Variants
                    if (item.Slug.contains("hellfire")) {
                        var hellfireSlugParts = item.Slug.split("-");
                        item.Name = item.Name + " " + toTitleCase(hellfireSlugParts[hellfireSlugParts.length - 1]);
                    }

                    // Only record legendary/set items
                    if (item.Quality != "Legendary")
                        return;

                    item.Type = itemType.text();                                  

                    var legText = itemHtml.find(".d3-color-orange.d3-item-property-default");
                    if (legText.length !== 0) {
                        item.LegendaryText = legText.text().trim().replace('&#39;', "'");
                    }

                    var setNameHtml = itemHtml.find(".item-itemset-name");
                    if (setNameHtml.length !== 0) {
                        item.SetName = setNameHtml.text().trim().replace('&#39;', "'");
                        item.IsSet = true;
                    }
                                        
                    var itemTypeName = itemType.text();

                    // Zeta Type
                    if(itemTypeName.contains("Bracers"))
                        item.ZetaType = "Bracer";
                    else if(itemTypeName.contains("Chest"))
                        item.ZetaType = "Chest";
                    else if (itemTypeName.contains("Cloak"))
                        item.ZetaType = "Chest";
                    else if(itemTypeName.contains("Shoulders"))
                        item.ZetaType = "Shoulder";
                    else if (itemTypeName.contains("Helm"))
                        item.ZetaType = "Helm";
                    else if(itemTypeName.contains("Spirit"))
                        item.ZetaType = "SpiritStone";
                    else if (itemTypeName.contains("Voodoo"))
                        item.ZetaType = "VoodooMask";
                    else if (itemTypeName.contains("Wizard Hat"))
                        item.ZetaType = "WizardHat";
                    else if (itemTypeName.contains("Gloves"))
                        item.ZetaType = "Gloves";
                    else if (itemTypeName.contains("Belt"))
                        item.ZetaType = "Belt";
                    else if (itemTypeName.contains("Mighty Belt"))
                        item.ZetaType = "MightyBelt";
                    else if (itemTypeName.contains("Pants"))
                        item.ZetaType = "Legs";
                    else if (itemTypeName.contains("Boots"))
                        item.ZetaType = "Boots";
                    else if (itemTypeName.contains("Amulet"))
                        item.ZetaType = "Amulet";
                    else if (itemTypeName.contains("Ring"))
                        item.ZetaType = "Ring";
                    else if (itemTypeName.contains("Shield"))
                        item.ZetaType = "Shield";
                    else if (itemTypeName.contains("Crusader"))
                        item.ZetaType = "CrusaderShield";
                    else if (itemTypeName.contains("Mojo"))
                        item.ZetaType = "Mojo";
                    else if (itemTypeName.contains("Source"))
                        item.ZetaType = "Orb";
                    else if (itemTypeName.contains("Quiver"))
                        item.ZetaType = "Quiver";
                    else if (itemTypeName.contains("Focus"))
                        item.ZetaType = "FollowerSpecial";
                    else if (itemTypeName.contains("Token"))
                        item.ZetaType = "FollowerSpecial";
                    else if (itemTypeName.contains("Relic"))
                        item.ZetaType = "FollowerSpecial";
                    else if (itemTypeName.contains("Polearm"))
                        item.ZetaType = "Polearm";
                    else if (itemTypeName.contains("Staff"))
                        item.ZetaType = "Staff";
                    else if (itemTypeName.contains("Daibo"))
                        item.ZetaType = "Daibo";
                    else if (itemTypeName.contains("Axe"))
                        item.ZetaType = "Axe";
                    else if (itemTypeName.contains("Dagger"))
                        item.ZetaType = "Dagger";
                    else if (itemTypeName.contains("Mace"))
                        item.ZetaType = "Mace";
                    else if (itemTypeName.contains("Spear"))
                        item.ZetaType = "Spear";
                    else if (itemTypeName.contains("Sword"))
                        item.ZetaType = "Sword";
                    else if (itemTypeName.contains("Knife"))
                        item.ZetaType = "CeremonialDagger";
                    else if (itemTypeName.contains("Fist"))
                        item.ZetaType = "FistWeapon";
                    else if (itemTypeName.contains("Flail"))
                        item.ZetaType = "Flail";
                    else if (itemTypeName.contains("Mighty Weapon"))
                        item.ZetaType = "MightyWeapon";
                    else if (itemTypeName.contains("Hand Crossbow"))
                        item.ZetaType = "HandCrossbow";
                    else if (itemTypeName.contains("Crossbow"))
                        item.ZetaType = "Crossbow";
                    else if (itemTypeName.contains("Bow"))
                        item.ZetaType = "Bow";
                    else if (itemTypeName.contains("Wand"))
                        item.ZetaType = "Wand";
                    else
                        item.ZetaType = "Unknown";


                    if (itemTypeName.contains("Bracers"))
                        item.TrinityItemType = "Bracer";
                    else if (itemTypeName.contains("Chest"))
                        item.TrinityItemType = "Chest";
                    else if (itemTypeName.contains("Cloak"))
                        item.TrinityItemType = "Chest";
                    else if (itemTypeName.contains("Shoulders"))
                        item.TrinityItemType = "Shoulder";
                    else if (itemTypeName.contains("Helm"))
                        item.TrinityItemType = "Helm";
                    else if (itemTypeName.contains("Spirit"))
                        item.TrinityItemType = "SpiritStone";
                    else if (itemTypeName.contains("Voodoo"))
                        item.TrinityItemType = "VoodooMask";
                    else if (itemTypeName.contains("Wizard Hat"))
                        item.TrinityItemType = "WizardHat";
                    else if (itemTypeName.contains("Gloves"))
                        item.TrinityItemType = "Gloves";
                    else if (itemTypeName.contains("Belt"))
                        item.TrinityItemType = "Belt";
                    else if (itemTypeName.contains("Mighty Belt"))
                        item.TrinityItemType = "MightyBelt";
                    else if (itemTypeName.contains("Pants"))
                        item.TrinityItemType = "Legs";
                    else if (itemTypeName.contains("Boots"))
                        item.TrinityItemType = "Boots";
                    else if (itemTypeName.contains("Amulet"))
                        item.TrinityItemType = "Amulet";
                    else if (itemTypeName.contains("Ring"))
                        item.TrinityItemType = "Ring";
                    else if (itemTypeName.contains("Shield"))
                        item.TrinityItemType = "Shield";
                    else if (itemTypeName.contains("Crusader"))
                        item.TrinityItemType = "CrusaderShield";
                    else if (itemTypeName.contains("Mojo"))
                        item.TrinityItemType = "Mojo";
                    else if (itemTypeName.contains("Source"))
                        item.TrinityItemType = "Orb";
                    else if (itemTypeName.contains("Quiver"))
                        item.TrinityItemType = "Quiver";
                    else if (itemTypeName.contains("Focus"))
                        item.TrinityItemType = "FollowerEnchantress";
                    else if (itemTypeName.contains("Token"))
                        item.TrinityItemType = "FollowerScoundrel";
                    else if (itemTypeName.contains("Relic"))
                        item.TrinityItemType = "FollowerTemplar";
                    else if (itemTypeName.contains("Two-Handed Flail"))
                        item.TrinityItemType = "TwoHandFlail";
                    else if (itemTypeName.contains("Two-Handed Mace"))
                        item.TrinityItemType = "TwoHandMace";
                    else if (itemTypeName.contains("Two-Handed Sword"))
                        item.TrinityItemType = "TwoHandSword";
                    else if (itemTypeName.contains("Two-Handed Axe"))
                        item.TrinityItemType = "TwoHandAxe";
                    else if (itemTypeName.contains("Two-Handed Mighty"))
                        item.TrinityItemType = "TwoHandMighty";
                    else if (itemTypeName.contains("Axe"))
                        item.TrinityItemType = "Axe";
                    else if (itemTypeName.contains("Dagger"))
                        item.TrinityItemType = "Dagger";
                    else if (itemTypeName.contains("Mace"))
                        item.TrinityItemType = "Mace";
                    else if (itemTypeName.contains("Spear"))
                        item.TrinityItemType = "Spear";
                    else if (itemTypeName.contains("Sword"))
                        item.TrinityItemType = "Sword";
                    else if (itemTypeName.contains("Knife"))
                        item.TrinityItemType = "CeremonialKnife";
                    else if (itemTypeName.contains("Fist"))
                        item.TrinityItemType = "FistWeapon";
                    else if (itemTypeName.contains("Flail"))
                        item.TrinityItemType = "Flail";
                    else if (itemTypeName.contains("Mighty Weapon"))
                        item.TrinityItemType = "MightyWeapon";
                    else if (itemTypeName.contains("Hand Crossbow"))
                        item.TrinityItemType = "HandCrossbow";
                    else if (itemTypeName.contains("Wand"))
                        item.TrinityItemType = "Wand";
                    else if (itemTypeName.contains("Crossbow"))
                        item.TrinityItemType = "TwoHandCrossbow";
                    else if (itemTypeName.contains("Bow"))
                        item.TrinityItemType = "TwoHandBow";
                    else if (itemTypeName.contains("Polearm"))
                        item.TrinityItemType = "TwoHandPolearm";
                    else if (itemTypeName.contains("Staff"))
                        item.TrinityItemType = "TwoHandStaff";
                    else if (itemTypeName.contains("Daibo"))
                        item.TrinityItemType = "TwoHandDaibo";
                    else
                        item.TrinityItemType = "Unknown";

                    item.IsTwoHanded = item.TrinityItemType.contains("TwoHand");

                    if (itemHtml.find(".item-weapon-damage").length !== 0) {
                        item.BaseType = "Weapon"
                    } else if (item.TrinityItemType == "Amulet" || item.TrinityItemType == "Ring") {
                        item.BaseType = "Jewelry"
                    } else if (itemDetails.find(".item-armor-weapon.item-armor-armor").length !== 0) {
                        item.BaseType = "Armor"
                    } else {
                        item.BaseType = "None"
                    }					

                    data[ItemTypeKey].push(item);
                });
	
                // handle callback
                callback(data[ItemTypeKey], itemTypeSlug);

            });

        }

        console.log("Initialized Skills Object!");

        return {
            Helm: function (callback) {
                $.isEmptyObject(data.Helm) ? request(ItemType.Helm, callback) : callback(data.Helm, ItemType.Helm);
            },
            SpiritStone: function (callback) {
                $.isEmptyObject(data.SpiritStone) ? request(ItemType.SpiritStone, callback) : callback(data.SpiritStone, ItemType.SpiritStone);
            },
            VoodooMask: function (callback) {
                $.isEmptyObject(data.VoodooMask) ? request(ItemType.VoodooMask, callback) : callback(data.VoodooMask, ItemType.VoodooMask);
            },
            WizardHat: function (callback) {
                $.isEmptyObject(data.WizardHat) ? request(ItemType.WizardHat, callback) : callback(data.WizardHat, ItemType.WizardHat);
            },
            Shoulders: function (callback) {
                $.isEmptyObject(data.Shoulders) ? request(ItemType.Shoulders, callback) : callback(data.Shoulders, ItemType.Shoulders);
            },
            Chest: function (callback) {
                $.isEmptyObject(data.Chest) ? request(ItemType.Chest, callback) : callback(data.Chest, ItemType.Chest);
            },
            Cloak: function (callback) {
                $.isEmptyObject(data.Cloak) ? request(ItemType.Cloak, callback) : callback(data.Cloak, ItemType.Cloak);
            },
            Bracers: function (callback) {
                $.isEmptyObject(data.Bracers) ? request(ItemType.Bracers, callback) : callback(data.Bracers, ItemType.Bracers);
            },
            Gloves: function (callback) {
                $.isEmptyObject(data.Gloves) ? request(ItemType.Gloves, callback) : callback(data.Gloves, ItemType.Gloves);
            },
            Belts: function (callback) {
                $.isEmptyObject(data.Belts) ? request(ItemType.Belts, callback) : callback(data.Belts, ItemType.Belts);
            },
            MightyBelt: function (callback) {
                $.isEmptyObject(data.MightyBelt) ? request(ItemType.MightyBelt, callback) : callback(data.MightyBelt, ItemType.MightyBelt);
            },
            Legs: function (callback) {
                $.isEmptyObject(data.Legs) ? request(ItemType.Legs, callback) : callback(data.Legs, ItemType.Legs);
            },
            Feet: function (callback) {
                $.isEmptyObject(data.Feet) ? request(ItemType.Feet, callback) : callback(data.Feet, ItemType.Feet);
            },
            Amulet: function (callback) {
                $.isEmptyObject(data.Amulet) ? request(ItemType.Amulet, callback) : callback(data.Amulet, ItemType.Amulet);
            },
            Rings: function (callback) {
                $.isEmptyObject(data.Rings) ? request(ItemType.Rings, callback) : callback(data.Rings, ItemType.Rings);
            },
            Shields: function (callback) {
                $.isEmptyObject(data.Shields) ? request(ItemType.Shields, callback) : callback(data.Shields, ItemType.Shields);
            },
            CrusaderShield: function (callback) {
                $.isEmptyObject(data.CrusaderShield) ? request(ItemType.CrusaderShield, callback) : callback(data.CrusaderShield, ItemType.CrusaderShield);
            },
            Mojos: function (callback) {
                $.isEmptyObject(data.Mojos) ? request(ItemType.Mojos, callback) : callback(data.Mojos, ItemType.Mojos);
            },
            Orbs: function (callback) {
                $.isEmptyObject(data.Orbs) ? request(ItemType.Orbs, callback) : callback(data.Orbs, ItemType.Orbs);
            },
            Quivers: function (callback) {
                $.isEmptyObject(data.Quivers) ? request(ItemType.Quivers, callback) : callback(data.Quivers, ItemType.Quivers);
            },
            //Focus: function (callback) {
            //    $.isEmptyObject(data.Focus) ? request(ItemType.Focus, callback) : callback(data.Focus, ItemType.Focus);
            //},
            //Token: function (callback) {
            //    $.isEmptyObject(data.Token) ? request(ItemType.Token, callback) : callback(data.Token, ItemType.Token);
            //},
            //Relic: function (callback) {
            //    $.isEmptyObject(data.Relic) ? request(ItemType.Relic, callback) : callback(data.Relic, ItemType.Relic);
            //},
            Axe1h: function (callback) {
                $.isEmptyObject(data.Axe1h) ? request(ItemType.Axe1h, callback) : callback(data.Axe1h, ItemType.Axe1h);
            },
            Dagger: function (callback) {
                $.isEmptyObject(data.Dagger) ? request(ItemType.Dagger, callback) : callback(data.Dagger, ItemType.Dagger);
            },
            Mace1h: function (callback) {
                $.isEmptyObject(data.Mace1h) ? request(ItemType.Mace1h, callback) : callback(data.Mace1h, ItemType.Mace1h);
            },
            Spears: function (callback) {
                $.isEmptyObject(data.Spears) ? request(ItemType.Spears, callback) : callback(data.Spears, ItemType.Spears);
            },
            Swords: function (callback) {
                $.isEmptyObject(data.Swords) ? request(ItemType.Swords, callback) : callback(data.Swords, ItemType.Swords);
            },
            Knives: function (callback) {
                $.isEmptyObject(data.Knives) ? request(ItemType.Knives, callback) : callback(data.Knives, ItemType.Knives);
            },
            FistWeapon: function (callback) {
                $.isEmptyObject(data.FistWeapon) ? request(ItemType.FistWeapon, callback) : callback(data.FistWeapon, ItemType.FistWeapon);
            },
            Flail1h: function (callback) {
                $.isEmptyObject(data.Flail1h) ? request(ItemType.Flail1h, callback) : callback(data.Flail1h, ItemType.Flail1h);
            },
            Mighty1h: function (callback) {
                $.isEmptyObject(data.Mighty1h) ? request(ItemType.Mighty1h, callback) : callback(data.Mighty1h, ItemType.Mighty1h);
            },
            Axe2h: function (callback) {
                $.isEmptyObject(data.Axe2h) ? request(ItemType.Axe2h, callback) : callback(data.Axe2h, ItemType.Axe2h);
            },
            Mace: function (callback) {
                $.isEmptyObject(data.Mace) ? request(ItemType.Mace, callback) : callback(data.Mace, ItemType.Mace);
            },
            Polearm: function (callback) {
                $.isEmptyObject(data.Polearm) ? request(ItemType.Polearm, callback) : callback(data.Polearm, ItemType.Polearm);
            },
            Staves: function (callback) {
                $.isEmptyObject(data.Staves) ? request(ItemType.Staves, callback) : callback(data.Staves, ItemType.Staves);
            },
            Sword2h: function (callback) {
                $.isEmptyObject(data.Sword2h) ? request(ItemType.Sword2h, callback) : callback(data.Sword2h, ItemType.Sword2h);
            },
            Diabo: function (callback) {
                $.isEmptyObject(data.Diabo) ? request(ItemType.Diabo, callback) : callback(data.Diabo, ItemType.Diabo);
            },
            Flail2h: function (callback) {
                $.isEmptyObject(data.Flail2h) ? request(ItemType.Flail2h, callback) : callback(data.Flail2h, ItemType.Flail2h);
            },
            Mighty2h: function (callback) {
                $.isEmptyObject(data.Mighty2h) ? request(ItemType.Mighty2h, callback) : callback(data.Mighty2h, ItemType.Mighty2h);
            },
            Bows: function (callback) {
                $.isEmptyObject(data.Bows) ? request(ItemType.Bows, callback) : callback(data.Bows, ItemType.Bows);
            },
            Crossbow: function (callback) {
                $.isEmptyObject(data.Crossbow) ? request(ItemType.Crossbow, callback) : callback(data.Crossbow, ItemType.Crossbow);
            },
            HandXBow: function (callback) {
                $.isEmptyObject(data.HandXBow) ? request(ItemType.HandXBow, callback) : callback(data.HandXBow, ItemType.HandXBow);
            },
            Wand: function (callback) {
                $.isEmptyObject(data.Wand) ? request(ItemType.Wand, callback) : callback(data.Wand, ItemType.Wand);
            },
        };

    }();
    
    var HandleItemData = function (onDataLoadFinished) {

        var todo = Object.keys(ItemData).length;

        var data = {
            timestamp: (new Date()).toUTCString(),
            All: [],
            Helm: [],
            SpiritStone: [],
            VoodooMask: [],
            WizardHat: [],
            Shoulders: [],
            Chest: [],
            Cloak: [],
            Bracers: [],
            Gloves: [],
            Helm: [],
            SpiritStone: [],
            VoodooMask: [],
            WizardHat: [],
            Shoulders: [],
            Chest: [],
            Cloak: [],
            Bracers: [],
            Gloves: [],
            Belts: [],
            MightyBelt: [],
            Legs: [],
            Feet: [],
            Amulet: [],
            Rings: [],
            Shields: [],
            CrusaderShield: [],
            Mojos: [],
            Orbs: [],
            Quivers: [],
            Focus: [],
            Token: [],
            Relic: [],
            Axe1h: [],
            Dagger: [],
            Mace1h: [],
            Spears: [],
            Swords: [],
            Knives: [],
            FistWeapon: [],
            Flail1h: [],
            Mighty1h: [],
            Axe2h: [],
            Mace: [],
            Polearm: [],
            Staves: [],
            Sword2h: [],
            Diabo: [],
            Flail2h: [],
            Mighty2h: [],
            Bows: [],
            Crossbow: [],
            HandXBow: [],
            Wand: []
        }

        // Gets all data (request or cached) and then passes it back to onDataLoadFinished 
        $.each(ItemData, function(index, process) {

            process(function (itemData, itemTypeName) {

                $.each(ItemType, function() {
                    if (this == itemTypeName)
                        data[itemTypeName] = itemData;
                });

                $.each(itemData, function() {
                    data.All.push(this);
                });

                todo--;
                if (todo == 0) {
                    onDataLoadFinished(data);
                }

            });

        });

    };



    return {
        HandleItemData: function (delegate) {
            return HandleItemData(delegate);
        },

    }

}();

