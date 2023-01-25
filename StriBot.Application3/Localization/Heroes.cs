﻿using StriBot.Application.Extensions;

namespace StriBot.Application.Localization
{
    public static class Heroes
    {
        private static readonly string[] ListHeroes =  {
        "Abaddon",
        "Axe",
        "Beastmaster",
        "Brewmaster",
        "Bristleback",
        "Centaur Warruner",
        "Chaos Knight",
        "Clockwerk",
        "Doom",
        "Downbreaker",
        "Dragon Knight",
        "Earth Spirit",
        "Earthshaker",
        "Elder Titan",
        "Huskar",
        "Io",
        "Kunkka",
        "Legion Commander",
        "Lifestealer",
        "Lycan",
        "Magnus",
        "Night Stalker",
        "Omniknight",
        "Phoenix",
        "Pudge",
        "Sand King",
        "Slardar",
        "Spirit Breaker",
        "Sven",
        "Tidehunter",
        "Timbersaw",
        "Tiny",
        "Treant Protector",
        "Tusk",
        "Underlord",
        "Undying",
        "Wraith King",
        "Anti-Mage",
        "Arc Warden",
        "Bloodseeker",
        "Bounty Hunter",
        "Broodmother",
        "Clinkz",
        "Drow Ranger",
        "Ember Spirit",
        "Faceless Void",
        "Gyrocopter",
        "Juggernaut",
        "Lone Druid",
        "Luna",
        "Medusa",
        "Meepo",
        "Mirana",
        "Monkey King",
        "Morphling",
        "Naga Siren",
        "Nyx Assassin",
        "Pangolier",
        "Phantom Assassin",
        "Phaton Lancer",
        "Razor",
        "Riki",
        "Shadow Fiend",
        "Slark",
        "Snapfire",
        "Sniper",
        "Spectre",
        "Templar Assassin",
        "Terrorblade",
        "Troll Warlord",
        "Ursa",
        "Vengeful Spirit",
        "Venomancer",
        "Viper",
        "Void Spirit",
        "Weaver",
        "Ancient Apparation",
        "Bane",
        "Batrider",
        "Chen",
        "Crystal Maiden",
        "Dark Seer",
        "Dark Willow",
        "Shadow Shaman",
        "Death Prophet",
        "Disruptor",
        "Enchantress",
        "Enigma",
        "Invoker",
        "Jakiro",
        "Keeper of the Light",
        "Leshrac",
        "Lich",
        "Lina",
        "Lion",
        "Nature Prophet",
        "Necrophos",
        "Ogre Magi",
        "Oracle",
        "Outworld Devourer",
        "Puck",
        "Pugna",
        "Queen of Pain",
        "Rubick",
        "Shadow Demon",
        "Silencer",
        "Skywrath Mage",
        "Storm Spirit",
        "Techies",
        "Tinker",
        "Visage",
        "Warlock",
        "Windranger",
        "Winter Wyvern",
        "Witch Doctor",
        "Zeus",
        "Hoodwink"
        };
        public static string GetRandomHero()
            => RandomHelper.GetRandomOfArray(ListHeroes);
    }
}