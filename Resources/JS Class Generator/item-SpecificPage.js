

var Items = function() {

    var ItemData = function () {

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
		};

		var currentUrl = "";
		
        var request = function(itemUrl, callback) {            

            console.log("Requesting Data!");

			
			
            $.get(itemUrl, {}, function (response) {				
			
                var raw = response.responseText.replace('&#39;', "'");

                // Origin bypass with YQL wraps as a HTML document
                // So we have to strip it all out again.
                var doc = $("<div>").html(raw);

                // Find all item records
                var items = doc.find(".item-detail-box");
                var staffofherding = false;
				
				currentUrl = doc.find("meta[name=identifier]").attr("content");

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
                    var itemDetails = itemHtml;
                    var itemType = itemDetails.find(".item-type");

                    var itemDetailsLink = itemDetails.find(".detail-text h2");
                    
                    item.Name = itemDetailsLink.text().replace('&#39;', "'");
                    if (item.Name == "")
                        return;

                    // strange double listing
                    if (item.Name == "Staff of Herding" && staffofherding) {
                        return;
                    } else {
                        staffofherding = true;
                    }

					var splitUrl = currentUrl.split("/");
                    item.RelativeUrl = "";
                    item.Slug = splitUrl[splitUrl.length - 1]					
                    item.Url = "http://us.battle.net/d3/en/item/" + item.Slug;
                    item.IsCrafted = item.Url.contains("recipe");
                    item.DataUrl = "https://us.battle.net/api/d3/data/" + ((item.IsCrafted) ? "recipe" : "item") + "/" + item.Slug;

                    if (itemLookup[item.Slug] != null) {
                        item.ActorSNO = itemLookup[item.Slug][1];
                        item.InternalName = itemLookup[item.Slug][2];
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
                
                    
                    if (itemHtml.find(".item-weapon-damage").length !== 0) {
                        item.BaseType = "Weapon"
                    } else if (item.Type == "Amulet" || item.Type == "Ring") {
                        item.BaseType = "Jewelry"
                    } else if (itemDetails.find(".item-armor-weapon.item-armor-armor").length !== 0) {
                        item.BaseType = "Armor"
                    } else {
                        item.BaseType = "None"
                    }

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

					// handle callback
					callback(item);
					
                });
            });
        }

		console.log("Initialized Skills Object!");

		return {
			GetDetailPageItem: function (url, callback) {
				request(url, callback)
			}
		};

    }();
    
    var HandleItemData = function (urls, onDataLoadFinished) {

        var todo = urls.length;	
	
        var data = {
            timestamp: (new Date()).toUTCString(),
			All: []
        }
		
		var process = function (item) {
			data.All.push(item);					

			todo--;
			if (todo == 0) {
				onDataLoadFinished(data);
			}
		};			
		
		$.each(urls, function () {
			var url = this;
			console.log("Processing URL: " + url);
			ItemData.GetDetailPageItem(url, process);
		});
		
		onDataLoadFinished(data);		
    };

    return {
        GetItemPages: function (urls, delegate) {
            return HandleItemData(urls, delegate);
        }
    }

}();
